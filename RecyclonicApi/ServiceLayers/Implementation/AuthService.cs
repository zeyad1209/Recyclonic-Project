using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RecyclonicApi.Data;
using RecyclonicApi.HelperServices;
using RecyclonicApi.Models;
using RecyclonicApi.Models.DTOs;
using RecyclonicApi.Repository.Interfaces;
using RecyclonicApi.ServiceLayers.Interfaces;
using System.Net;

namespace RecyclonicApi.ServiceLayers.Implementation
{
    public class AuthService : IAuthService
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly IMemoryCache _Cache;
        readonly EmailService _emailService;
        readonly IMapper _mapper;
        readonly  JWTService _jwtService;
        readonly IRefreshTokenService _refreshTokenService;
        readonly IHelperService _helperService;
        readonly IRefreshTokenRepo _refreshTokenRepo;
        readonly ICartRepo _cartRepo;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IMemoryCache Cache,
            EmailService emailService,
            IMapper mapper,
            JWTService jwtservice,
            IRefreshTokenService refreshTokenService,
            IHelperService helperService,
            IRefreshTokenRepo refreshTokenRepo,
            ICartRepo cartRepo
            )
        {
            _userManager = userManager;
            _Cache = Cache;
            _emailService = emailService;
            _mapper = mapper;
            _jwtService = jwtservice;
            _refreshTokenService = refreshTokenService;
            _helperService = helperService;
            _refreshTokenRepo = refreshTokenRepo;
            _cartRepo = cartRepo;
        }

        public async Task<ServiceResult> Register(RegisterDto dtouser)
        {
            var exuser = await _userManager.FindByEmailAsync(dtouser.Email);
            if (exuser != null)
                return ServiceResult.Fail("This email is executed from other user", HttpStatusCode.Conflict);

            var Code = new Random().Next(100000, 999999).ToString();
            //_Cache.Set(dtouser.Email, new { user = dtouser, Code = Code },
            //    TimeSpan.FromMinutes(5));
            _Cache.Set(dtouser.Email, new CachedUserData
            {
                User = dtouser,
                Code = Code
            }, TimeSpan.FromMinutes(5));


            await _emailService.SendEmailAsync(dtouser.Email, "Verification Code",
                $"<h3>Your verification code is: <b>{Code}</b></h3>");

            return ServiceResult.Ok(null, "Verification code sent. Please check your email.", HttpStatusCode.OK);
        }

        public async Task<ServiceResult> VerifyUser(VerifyUserDto dto)
        {
            try
            {
                if (!_Cache.TryGetValue(dto.Email, out CachedUserData data))
                    return ServiceResult.Fail("Code invalid or expired", HttpStatusCode.BadRequest);
                if (dto.Code != data.Code)
                    return ServiceResult.Fail("Code invalid or expired", HttpStatusCode.BadRequest);

                var user = data.User;

                var newuser = _mapper.Map<ApplicationUser>(user);
                newuser.UserName = user.Email;
                newuser.EmailConfirmed = true;
                newuser.IsfromRecyclonic = false;

                var result = await _userManager.CreateAsync(newuser, user.Password);
                if (!result.Succeeded)
                    return ServiceResult.Fail("Failed to Create The User. The Password can't be less than 10 Characters" +
                        ", must be include Upper and lower Case , must be include digit and must be " +
                        "include Non_Alphanumeric Character", HttpStatusCode.BadRequest);

                var role = "User";
                await _userManager.AddToRoleAsync(newuser, role);
                var accesstoken = await _jwtService.GenerateJwtToken(newuser);
                var refreshtoken = await _jwtService.GenerateRefreshtoken();

                while (!await _refreshTokenService.Add_RefreshTokenNow(refreshtoken, newuser.Id))
                    refreshtoken = await _jwtService.GenerateRefreshtoken();

                var cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    TotalAmount = 0,
                    DeliveryFees = 0,
                    UserId = newuser.Id,
                    User = newuser
                };

                await _cartRepo.CreateAsync(cart);
                await _cartRepo.Save();

                return ServiceResult.Ok(new
                {
                    Accesstoken = accesstoken,
                    RefreshToken = refreshtoken,
                    role = role,
                }, "User Created Successfully"
                , HttpStatusCode.Created);
            }
            catch(Exception ex)
            {
                return ServiceResult.Fail(ex.ToString(), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult.Fail("No user with this email", HttpStatusCode.NotFound);

            if (await _userManager.IsLockedOutAsync(user))
                return ServiceResult.Fail("This account is locked", (HttpStatusCode)423);

            var passcheck = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passcheck)
                return ServiceResult.Fail("Invalid credentials", HttpStatusCode.BadRequest);

            var accesstoken = await _jwtService.GenerateJwtToken(user);
            var refreshtoken = await _jwtService.GenerateRefreshtoken();
            while (!await _refreshTokenService.Add_RefreshTokenNow(refreshtoken, user.Id))
            {
                refreshtoken = await _jwtService.GenerateRefreshtoken();
            }
            var highestrole = await _helperService.GetHighestRole(user);

            return ServiceResult.Ok(new
            {
                accesstoken = accesstoken,
                refreshtoken = refreshtoken,
                role = highestrole,
            }, "Logged in Succesfully", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> Refresh(string oldrefreshtoken)
        {
            if (string.IsNullOrWhiteSpace(oldrefreshtoken))
                return ServiceResult.Fail("Refresh Token is Required");

            var user = await _refreshTokenRepo.GetUserfromRefreshToken(oldrefreshtoken);
            var token = await _refreshTokenRepo.GetTokenfromtokenstring(oldrefreshtoken);

            if (user == null || token.ExpiresAt < DateTime.Now || token.IsRevoked == true)
                return ServiceResult.Fail("Invalid or expired refresh token", HttpStatusCode.Unauthorized);

            var newaccesstoken = await _jwtService.GenerateJwtToken(user);
            var newrefreshtoken = await _jwtService.GenerateRefreshtoken();
            while (!await _refreshTokenService.Add_RefreshTokenNow(newrefreshtoken, user.Id))
            {
                newrefreshtoken = await _jwtService.GenerateRefreshtoken();
            }
            token.IsRevoked = true;
            await _refreshTokenRepo.Save();

            return ServiceResult.Ok(new RefreshTokenDto
            {
                AccessToken = newaccesstoken,
                RefreshToken = newrefreshtoken,
            }, "Refreshed Success", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> Log_out(string oldRefreshToken)
        {
            if (string.IsNullOrWhiteSpace(oldRefreshToken))
                return ServiceResult.Fail("Refresh Token is required");

            var user = await _refreshTokenRepo.GetUserfromRefreshToken(oldRefreshToken);
            var token = await _refreshTokenRepo.GetTokenfromtokenstring(oldRefreshToken);

            if (user == null)
                return ServiceResult.Fail("Invalid Refresh Token");     
            token.IsRevoked = true;
            await _refreshTokenRepo.Save();

            return ServiceResult.Ok(null, "Logout successfully", HttpStatusCode.OK);
        }

        public async Task<ServiceResult> Log_out_All(string UserId)
        {
            if (string.IsNullOrWhiteSpace(UserId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);
            var user = await _userManager.Users.Include(u => u.refreshTokens)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(UserId));
            if (user == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            await _userManager.UpdateAsync(user);
            var RefreshTokens = user.refreshTokens;
            _refreshTokenRepo.RemoveAllRefreshTokensForUser(RefreshTokens);
            await _refreshTokenRepo.Save();

            return ServiceResult.Ok(null, "You are log out from all devices succesfully", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> Change_Password(Change_Password_Dto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var error = string.Join(" , ", result.Errors.Select(e => e.Description));
                return ServiceResult.Fail(error, HttpStatusCode.BadRequest);
            }

            return ServiceResult.Ok(null, "The Password changed succesfully", HttpStatusCode.OK);
        }

        public async Task<ServiceResult> Forget_Password(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return ServiceResult.Fail("Email is required", HttpStatusCode.BadRequest);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ServiceResult.Fail("No account found with this email", HttpStatusCode.NotFound);

            var code = new Random().Next(100000, 999999).ToString();
            string cacheKey = $"FORGET_PASSWORD_{user.Email}";
            _Cache.Set(cacheKey, new CachedUserresetpasswordData { UserId = user.Id, Code = code }, TimeSpan.FromMinutes(7));

            await _emailService.SendEmailAsync(
                user.Email,
                "Password Reset Verification",
                $"<h3>Your verification code to reset your password is: <b>{code}</b></h3>"
            );

            return ServiceResult.Ok(null, "Verification code sent successfully. Please Check you Email", HttpStatusCode.OK);
        }

        public async Task<ServiceResult> Reset_Password(Reset_Password_Dto dto)
        {
            string cacheKey = $"FORGET_PASSWORD_{dto.Email}";
            if (!_Cache.TryGetValue(cacheKey, out CachedUserresetpasswordData resultCache))
                return ServiceResult.Fail("Code expired or invalid", HttpStatusCode.BadRequest);
            if (resultCache.Code != dto.Code)
                return ServiceResult.Fail("Wrong Verification Code", HttpStatusCode.BadRequest);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ServiceResult.Fail(errors, HttpStatusCode.BadRequest);
            }

            return ServiceResult.Ok(null, "The Password Changed Succesfully", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> Editorcompletedata(string UserId , editorcompleteprofiledata dto)
        {
            if (string.IsNullOrWhiteSpace(UserId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            if (string.IsNullOrWhiteSpace(dto.Address) && string.IsNullOrWhiteSpace(user.Address))
                return ServiceResult.Fail("Address is required", HttpStatusCode.BadRequest);
            if (string.IsNullOrWhiteSpace(dto.Phonenumber) && string.IsNullOrWhiteSpace(user.PhoneNumber))
                return ServiceResult.Fail("Phone Number is required", HttpStatusCode.BadRequest);

            user.Address = dto.Address ?? user.Address;
            user.PhoneNumber = dto.Phonenumber ?? user.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return ServiceResult.Fail("Failed to Update User.", HttpStatusCode.InternalServerError);

            return ServiceResult.Ok(null, "The Profile completed succesfully", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> Getmyprofile(string UserId)
        {
            if (string.IsNullOrWhiteSpace(UserId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            var profileDto = _mapper.Map<UserProfileDto>(user);

            return ServiceResult.Ok(profileDto , "Your Profile" , HttpStatusCode.OK);
        }
    }
}


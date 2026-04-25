using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Data;
using RecyclonicApi.HelperServices;
using RecyclonicApi.Models;
using RecyclonicApi.Models.DTOs;
using RecyclonicApi.Repository.Interfaces;
using RecyclonicApi.ServiceLayers.Interfaces;
using System.Net;
using System.Net.NetworkInformation;
using static System.Net.Mime.MediaTypeNames;

namespace RecyclonicApi.ServiceLayers.Implementation
{
    public class EwasteService : IEwasteService
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly IGenericRepo<EwasteItem> _ewasteRepo;
        readonly IRecycleRequestRepo _recycleRepo;
        readonly IImageRepo _imagesRepo;
        readonly IMapper _mapper;
        readonly AppDbContext _context;
        readonly IHelperService _helperService;
        readonly EmailService _emailService;

        public EwasteService(
            UserManager<ApplicationUser> userManager,
            IGenericRepo<EwasteItem> ewasteRepo,
            IRecycleRequestRepo recycleRepo,
            IImageRepo imagesRepo,
            IMapper mapper,
            AppDbContext context,
            IHelperService helperService,
            EmailService emailService)
        {
            _userManager = userManager;
            _ewasteRepo = ewasteRepo;
            _recycleRepo = recycleRepo;
            _imagesRepo = imagesRepo;
            _mapper = mapper;
            _context = context;
            _helperService = helperService;
            _emailService = emailService;
        }

        public async Task<ServiceResult> RecycleEwaste(string userId, RecycleEwastePostDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult.Fail("Unauthorized", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult.Fail("User not found", HttpStatusCode.Unauthorized);

            var item = _mapper.Map<EwasteItem>(dto);
            item.SubmissionDate = DateTime.Now;

            await _ewasteRepo.CreateAsync(item);
            await _ewasteRepo.Save();

            if (dto.Images != null && dto.Images.Any())
            {
                var urls = await _helperService.UploadImagesAsync(dto.Images , "E_Waste");

                foreach (var url in urls)
                {
                    var image = new Images
                    {
                        Id = Guid.NewGuid(),
                        Url = url,
                        EwasteitemId = item.Id
                    };
                    await _imagesRepo.CreateAsync(image);
                }
                await _imagesRepo.Save();
                item.ImagesUrl = await _imagesRepo.GetImagesByEwasteIdAsync(item.Id);
                await _ewasteRepo.Save();
            }

            var recycleRequest = new RecycleRequest
            {
                Status = "Pending",
                CreatedAt = DateTime.Now,
                Address = dto.Address,
                EwasteItemId = item.Id,
                ewasteItem = item,
                UserId = user.Id,
                user = user,
            };
            await _recycleRepo.CreateAsync(recycleRequest);
            await _recycleRepo.Save();
            return ServiceResult.Ok(null, "Ewaste recycle request submitted successfully", HttpStatusCode.Created);
        }

        public async Task<ServiceResult> GetRecycleWastestorespondit()
        {
            var requests = await _recycleRepo.GetAllrequeststhatnotresponding();
            if (requests == null || !requests.Any())
                return ServiceResult.Ok(null, "No requests available", HttpStatusCode.NotFound);

            var requestsdto = _mapper.Map<IEnumerable<RecycleEwasteGetDto>>(requests);
            var count = requestsdto.Count();

            return ServiceResult.Ok(new
            {
                Number_of_Requests = count,
                Requests = requestsdto
            }, "All Requests that need to respond it", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> RespondRecycleRequest(string EmployeeId, string status, Guid RequestId, decimal? price)
        {
            if (string.IsNullOrWhiteSpace(EmployeeId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var employee = await _userManager.FindByIdAsync(EmployeeId);
            if (employee == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            var request = await _recycleRepo.GetRecycleRequestById(RequestId);
            if (request == null)
                return ServiceResult.Fail("No Request", HttpStatusCode.NotFound);
            if (employee == request.user)
                return ServiceResult.Fail("You don't have an access to Respond your Recycle Request", HttpStatusCode.Forbidden);

            request.Status = status;
            request.EmployeeId = employee.Id;
            price = price ?? 0;
            if (status == "Accepted" && (price == null || price <= 0))
                return ServiceResult.Fail("You must offer a valid price for accepted request", HttpStatusCode.BadRequest);
            request.OfferedPrice = price;
            _recycleRepo.UpdateAsync(request);
            await _recycleRepo.Save();

            if (status == "Accepted")
            {
                var emailSubject = "Recycle Request Accepted";
                var emailBody = $"Dear {request.user.FirstName + " " + request.user.LastName},\n\n" +
                                $"Your recycle request for the ewaste item has been accepted.\n" +
                                $"Offered Price: {request.OfferedPrice:C}\n\n" +
                                $"By Employee {request.Employee.FirstName + " " + request.Employee.LastName},\n\n" +
                                "Thank you for using our services.\n\n" +
                                "Best regards,\n" +
                                "Recyclonic Team";
                await _emailService.SendEmailAsync(request.user.Email, emailSubject, emailBody);
            }
            else
            {
                var emailSubject = "Recycle Request Rejected";
                var emailBody = $"Dear {request.user.FirstName + " " + request.user.LastName},\n\n" +
                                $"Your recycle request for the ewaste item has been rejected.\n" +
                                $"By Employee {request.Employee.FirstName + " " + request.Employee.LastName},\n\n" +
                                "Thank you for using our services.\n\n" +
                                "Best regards,\n" +
                                "Recyclonic Team";
                await _emailService.SendEmailAsync(request.user.Email, emailSubject, emailBody);
            }

            return ServiceResult.Ok(null, "The Request respond successfully", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> Userresponsetherequest(string UserId, Guid RequestId, bool accept)
        {
            if (string.IsNullOrWhiteSpace(UserId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            var request = await _recycleRepo.GetRecycleRequestById(RequestId);
            if (request == null)
                return ServiceResult.Fail("No Request", HttpStatusCode.NotFound);
            if (request.Status != "Accepted")
                return ServiceResult.Fail("You don't have an access to active this request", HttpStatusCode.Forbidden);

            request.UserResponse = accept;
            if (!accept)
            {
                _recycleRepo.UpdateAsync(request);
                await _recycleRepo.Save();
                return ServiceResult.Ok(null, "Better luck next time", HttpStatusCode.OK);
            }
            if (user.PhoneNumber == null)
                return ServiceResult.Fail("You have to complete your informations first", HttpStatusCode.BadRequest);

            Delivery newdelivery = new Delivery()
            {
                Id = Guid.NewGuid(),
                PickUpAddress = request.Address,
                //PickUpAddressUrl = re
                RecycleRequestId = request.Id,
                statusTraking = new List<StatusTraking>()
            };
            await _context.Deliveries.AddAsync(newdelivery);
            await _context.SaveChangesAsync();

            var newstatus = new StatusTraking()
            {
                Id = Guid.NewGuid(),
                Status = "Pending",
                Dateofstatus = DateTime.Now,
                DeliveryId = newdelivery.Id
            };
            await _context.StatusTrackings.AddAsync(newstatus);
            newdelivery.statusTraking.Add(newstatus);
            await _context.SaveChangesAsync();

            return ServiceResult.Ok(null, "Your Request will accepted succesfully. Go To Details Page to see all details ", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> Getallrequestforuser(string UserId)
        {
            if (string.IsNullOrWhiteSpace(UserId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            var requests = await _recycleRepo.GetRecycleRequestsforuser(user.Id);

            if (requests == null || !requests.Any())
                return ServiceResult.Ok(null ,"No Requests with you.", HttpStatusCode.NotFound);

            var mappedRequests = _mapper.Map<IEnumerable<RecycleEwasteGetDtotouser>>(requests);

            return ServiceResult.Ok(mappedRequests, "Requests retrieved successfully", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> Cancel_Request(string UserId , Guid RequestId)
        {
            if (string.IsNullOrWhiteSpace(UserId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var User = await _userManager.FindByIdAsync(UserId);
            if (User == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            var request = await _recycleRepo.GetRecycleRequestById(RequestId);
            if (request == null)
                return ServiceResult.Fail("No Request", HttpStatusCode.NotFound);

            request.Status = "Cancelled";
            request.UserResponse = false;
            _recycleRepo.UpdateAsync(request);
            await _recycleRepo.Save();

            return ServiceResult.Ok(null, "The Request cancelled successfully", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> Getallrequestforadmin(string adminId)
        {
            if (string.IsNullOrWhiteSpace(adminId))
                return ServiceResult.Fail("", HttpStatusCode.Unauthorized);

            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin == null)
                return ServiceResult.Fail("You must make Log_in", HttpStatusCode.Unauthorized);

            var requests = await _recycleRepo.GetRecycleRequestsforadmin(admin.Id);

            if (requests == null || !requests.Any())
                return ServiceResult.Ok(null, "No Requests.", HttpStatusCode.NotFound);

            var mappedRequests = _mapper.Map<IEnumerable<RecycleEwasteGetDtotouser>>(requests);

            return ServiceResult.Ok(mappedRequests, "Requests retrieved successfully", HttpStatusCode.OK);
        }
    }
}


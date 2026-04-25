using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Data;
using RecyclonicApi.HelperServices;
using RecyclonicApi.Models;
using RecyclonicApi.Models.DTOs;
using RecyclonicApi.Repository.Implementation;
using RecyclonicApi.Repository.Interfaces;
using RecyclonicApi.ServiceLayers.Interfaces;
using System.Net;

namespace RecyclonicApi.ServiceLayers.Implementation
{
    public class MarketPlaceService : IMarketPlaceService
    {
        //readonly AppDbContext _context;
        readonly UserManager<ApplicationUser> _userManager;
        readonly IHelperService _helperService;
        private readonly IImageRepo _imagesRepo;
        readonly IMarketPlaceRepo _marketplaceRepo;
        readonly ICartRepo _cartRepo;
        readonly IMapper _mapper;

        public MarketPlaceService(
            UserManager<ApplicationUser> userManager,
            IHelperService helperService,
            IImageRepo imageRepo,
            IMarketPlaceRepo marketplaceRepo,
            ICartRepo cartRepo,
            IMapper mapper)
        {
            _userManager = userManager;
            _helperService = helperService;
            _imagesRepo = imageRepo;
            _marketplaceRepo = marketplaceRepo;
            _mapper = mapper;
            _cartRepo = cartRepo;
        }

        public async Task<ServiceResult> Add_Product(string userId, AddMarketplaceItemDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult.Fail("Unauthorized", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult.Fail("User not found", HttpStatusCode.Unauthorized);

            List<Images> imagesList = new();

            if (dto.Images != null && dto.Images.Any())
            {
                var urls = await _helperService.UploadImagesAsync(dto.Images, "MarketPlace");

                imagesList = urls.Select(url => new Images
                {
                    Id = Guid.NewGuid(),
                    Url = url
                }).ToList();
            }

            var product = _mapper.Map<MarketplaceItem>(dto);

            product.Id = Guid.NewGuid();
            product.ImagesUrl = imagesList;
            product.SellerId = user.Id;
            product.isfromrecyclonic = user.IsfromRecyclonic;

            foreach (var img in imagesList)
            {
                img.MarketPlaceItemId = product.Id;
            }

            await _marketplaceRepo.CreateAsync(product);
            await _marketplaceRepo.Save();

            return ServiceResult.Ok(null, "Product added successfully", HttpStatusCode.Created);
        }

        //public async Task<ServiceResult> Update_Product(string userId,Guid productId,UpdateMarketplaceItemDto dto)
        //{
        //    if (string.IsNullOrWhiteSpace(userId))
        //        return ServiceResult.Fail("Unauthorized", HttpStatusCode.Unauthorized);

        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        return ServiceResult.Fail("User not found", HttpStatusCode.Unauthorized);

        //    var product = await _marketplaceRepo
        //        .GetByIdWithImagesAsync(productId);

        //    if (product == null)
        //        return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);

        //    // ✅ صاحب المنتج أو Admin فقط
        //    if (product.SellerId != user.Id && !await _userManager.IsInRoleAsync(user, "Admin"))
        //        return ServiceResult.Fail("Forbidden", HttpStatusCode.Forbidden);

        //    // 🔄 update basic fields
        //    product.Name = dto.Name;
        //    product.Price = dto.Price;
        //    product.Description = dto.Description;
        //    product.Stock = dto.Stock;

        //    // 🖼️ update images (لو اتبعت صور جديدة)
        //    if (dto.Images != null && dto.Images.Any())
        //    {
        //        // احذف الصور القديمة من DB (واختياري من storage)
        //        _imageRepo.DeleteRange(product.ImagesUrl);

        //        var urls = await _helperService.UploadImagesAsync(dto.Images, "MarketPlace");

        //        product.ImagesUrl = urls.Select(url => new Images
        //        {
        //            Id = Guid.NewGuid(),
        //            Url = url,
        //            MarketPlaceItemId = product.Id
        //        }).ToList();
        //    }

        //    await _marketplaceRepo.Save();

        //    return ServiceResult.Ok(null, "Product updated successfully", HttpStatusCode.OK);
        //}


        public async Task<ServiceResult> GetAllProducts(string? userId)
        {
            ApplicationUser? user = null;
            bool auth = false;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                user = await _marketplaceRepo.userwithfavitems(userId); 
                if (user != null)
                    auth = true;
            }

            var products = await _marketplaceRepo.GetAllinMarketPlace();
            if (products == null || !products.Any())
                return ServiceResult.Ok(null, "No Items in MarketPlace", HttpStatusCode.NotFound);

            var items = _mapper.Map<List<MarketplaceItemDto>>(products);

            if (!auth || user?.FavouriteItems == null)
                return ServiceResult.Ok(items, "All Items in MarketPlace", HttpStatusCode.OK);

            var favIds = user!.FavouriteItems.Select(f => f.Id).ToHashSet();
            foreach (var item in items)
            {
                item.isfavourite = favIds.Contains(item.Id);
            }

            return ServiceResult.Ok(items, "All Items in MarketPlace", HttpStatusCode.OK);
        }
        public async Task<ServiceResult> Press_Favourite(string userId, Guid ItemId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult.Fail("Unauthorized", HttpStatusCode.Unauthorized);

            var user = await _marketplaceRepo.userwithfavitems(userId);
            if (user == null)
                return ServiceResult.Fail("User not found", HttpStatusCode.Unauthorized);

            var item = await _marketplaceRepo.GetByIdAsync(ItemId);
            if (item == null)
                return ServiceResult.Fail("Item not found", HttpStatusCode.NotFound);

            var favItem = user.FavouriteItems
                .FirstOrDefault(f => f.Id == ItemId);

            if (favItem != null)
                user.FavouriteItems.Remove(favItem);
            else
                user.FavouriteItems.Add(item);

            await _marketplaceRepo.Save();
            return ServiceResult.Ok(null, "The Item Changed Successfully", HttpStatusCode.OK);
        }

        public async Task<ServiceResult> AddToCart(string userId, Guid itemId, int quantity = 1)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult.Fail("Unauthorized", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult.Fail("User not found", HttpStatusCode.Unauthorized);

            if (quantity <= 0)
                return ServiceResult.Fail("Invalid quantity", HttpStatusCode.BadRequest);

            var item = await _marketplaceRepo.GetByIdAsync(itemId);
            if (item == null)
                return ServiceResult.Fail("Item not found", HttpStatusCode.NotFound);

            if (!item.isaviable || item.Stock < quantity)
                return ServiceResult.Fail("Not enough stock or item not available", HttpStatusCode.BadRequest);

            var cart = await _cartRepo.GetCartwithCartItems(user.Id);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id,
                    CartItems = new List<CartItem>(),
                    TotalAmount = 0,
                    DeliveryFees = 0
                };
                await _cartRepo.CreateAsync(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.MarketplaceItemId == itemId);
            if (cartItem != null)
            {
                if (cartItem.Quantity + quantity > item.Stock)
                    return ServiceResult.Fail("Not enough stock", HttpStatusCode.BadRequest);

                cartItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    MarketplaceItemId = item.Id,
                    Quantity = quantity,
                    PriceAtAdd = item.Price
                });
            }

            cart.TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.PriceAtAdd);

            await _cartRepo.Save();

            return ServiceResult.Ok(null, "Item added to cart", HttpStatusCode.OK);
        }

        public async Task<ServiceResult> GetMyCart(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult.Fail("Unauthorized", HttpStatusCode.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult.Fail("User not found", HttpStatusCode.Unauthorized);

            var cart = await _cartRepo.GetCartwithCartItems(user.Id);
            var cartDto = _mapper.Map<GetCartdto>(cart);

            return ServiceResult.Ok(cartDto, "Your Cart", HttpStatusCode.OK);
        }
    }
}

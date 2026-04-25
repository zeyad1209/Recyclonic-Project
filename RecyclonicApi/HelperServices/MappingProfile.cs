using AutoMapper;
using RecyclonicApi.Models;
using RecyclonicApi.Models.Domain;
using RecyclonicApi.Models.DTOs;

namespace RecyclonicApi.HelperServices
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserProfileDto>();
            CreateMap<RecycleEwastePostDto, EwasteItem>()
                .ForMember(dest => dest.ImagesUrl, opt => opt.Ignore());

            CreateMap<RecycleRequest, RecycleEwasteGetDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.ewasteItem.ImagesUrl.Select(i => i.Url).ToList()))
                .ForMember(dest => dest.Condition, opt => opt.MapFrom(src => src.ewasteItem.Condition))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ewasteItem.Description))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.ewasteItem.weight))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
                //.ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.ewasteItem.SubmissionDate));


            CreateMap<RecycleRequest, RecycleEwasteGetDtotouser>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src =>
                        src.ewasteItem.ImagesUrl != null
                            ? src.ewasteItem.ImagesUrl.Select(i => i.Url).ToList()
                            : new List<string>()))
                .ForMember(dest => dest.Condition,
                    opt => opt.MapFrom(src => src.ewasteItem.Condition))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.ewasteItem.Description))
                .ForMember(dest => dest.Weight,
                    opt => opt.MapFrom(src => src.ewasteItem.weight))
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src => src.Address))
                
                .ForMember(dest => dest.EmployeeName,
                    opt => opt.MapFrom(src =>
                        src.Employee != null ? src.Employee.FirstName + " " + src.Employee.LastName : null))
            .ForMember(dest => dest.delivery,
                opt => opt.MapFrom(src => src.delivery));

            CreateMap<Delivery, Deliverygetinrequestsdto>()
                .ForMember(dest => dest.DeliveryEmployeeName,
                    opt => opt.MapFrom(src =>
                        src.DeliveryUser != null ? src.DeliveryUser.FirstName + " " + src.DeliveryUser.LastName : null))
                .ForMember(dest => dest.StatusTrakingdtos,
                    opt => opt.MapFrom(src => src.statusTraking));

            CreateMap<StatusTraking, StatusTrakingdto>();

            //CreateMap<employmentDto, Invitation>()
            //   .ForMember(dest => dest.AdminId, opt => opt.Ignore())
            //   .ForMember(dest => dest.CreateddDate, opt => opt.MapFrom(src => DateTime.Now))
            //   .ForMember(dest => dest.Expired, opt => opt.MapFrom(src => false));

            CreateMap<Invitation, InvitationResponseDto>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreateddDate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.status));

            CreateMap<AddMarketplaceItemDto, MarketplaceItem>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.ImagesUrl, opt => opt.Ignore())
               .ForMember(dest => dest.isaviable,
                   opt => opt.MapFrom(src => src.Stock > 0))
               .ForMember(dest => dest.CreatedAt,
                   opt => opt.MapFrom(_ => DateTime.Now))
               .ForMember(dest => dest.isfromrecyclonic, opt => opt.MapFrom(src => true))
               .ForMember(dest => dest.SellerId, opt => opt.Ignore());

            CreateMap<MarketplaceItem, MarketplaceItemDto>()
                .ForMember(
                    dest => dest.IsAvailable,
                    opt => opt.MapFrom(src => src.isaviable))
                .ForMember(
                    dest => dest.ImagesUrls,
                    opt => opt.MapFrom(src =>
                        src.ImagesUrl != null
                            ? src.ImagesUrl.Select(img => img.Url).ToList()
                            : new List<string>()
                    ))
                .ForMember(dest => dest.isfavourite,
                    opt => opt.Ignore());

            CreateMap<Cart, GetCartdto>()
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.CartItems))
                .ForMember(dest => dest.TotalAmount,
                    opt => opt.MapFrom(src =>
                        src.CartItems != null
                            ? src.CartItems.Sum(i => i.PriceAtAdd * i.Quantity)
                            : 0))
                .ForMember(dest => dest.DeliveryFees,
                    opt => opt.MapFrom(src => src.DeliveryFees))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<CartItem, CartItemdto>()
                .ForMember(dest => dest.marketplaceItem,
                    opt => opt.MapFrom(src => src.marketplaceItem))
                .ForMember(dest => dest.Quantity,
                    opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.PriceAtAdd,
                    opt => opt.MapFrom(src => src.PriceAtAdd))
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id));
        }
    }
}

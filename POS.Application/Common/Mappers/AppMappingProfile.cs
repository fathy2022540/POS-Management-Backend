using AutoMapper;
using POS.Application.Common.DTOs;
using POS.Application.Common.DTOs.POS;
using POS.Application.Features.Products.Commands;
using POS.Application.Common.Helper;
using POS.Application.Features.LookupServices.Commands;
using POS.Application.Features.ParamterConfigs.Commands;
using POS.Application.Features.PermissionServices.Commands;
using POS.Application.Features.Products.Commands;
using POS.Application.Features.RolesServices.Commands;
using POS.Application.Features.UserAccount.Commands;

using POS.Domain.Entities;
using System.Security.Cryptography;


namespace POS.Application.Mappers
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            #region users
            CreateMap<Roles, RoleDto>().ReverseMap();
            CreateMap<Users, UserModel>().ReverseMap();
            CreateMap<UserManagementCommand, Users>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom((src, dest, destMember, context) =>
                ((IPasswordHasher)context.Items["PasswordHasher"]).HashPassword(src.Password)))
            .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.MobileNumber))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false))
            // Handle name splitting cleanly inside mapping execution
            .AfterMap((src, dest) =>
            {
                var (firstName, lastName) = src.FullName.SplitFullName();
                dest.FirstName = firstName;
                dest.LastName = lastName;
                dest.ActivationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            });

            CreateMap<CreateRoleCommand, Roles>();
            CreateMap<UpdateRoleCommand, Roles>();
            CreateMap<CreatePermissionCommand, Permissions>();
            CreateMap<UpdatePermissionCommand, Permissions>();
            #endregion

            #region Lookup
            CreateMap<Lookup, LookupDto>().ReverseMap();

            CreateMap<LookupItemDto, LookupItems>();
            CreateMap<LookupItems, LookupItemDto>()
               .ForMember(dest => dest.LookupName, act => act.MapFrom(src => src.Lookup.Name))
               .ForMember(dest => dest.LookupNameAr, act => act.MapFrom(src => src.Lookup.NameAR));

            CreateMap<CreateLookupCommand, Lookup>();
            CreateMap<UpdateLookupCommand, Lookup>();
            CreateMap<CreateLookupItemsCommand, LookupItems>();
            CreateMap<UpdateLookupCommand, LookupItems>();
            #endregion

            #region Orders
            CreateMap<OrderItemDto, OrderItem>();
            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<Payment, PaymentResultDto>()
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            #endregion

            #region ProductItems

            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CreateProductCommand, CreateProductDto>();
            CreateMap<UpdateProductCommand, UpdateProductDto>();


            #endregion








            #region ParamtersConfiguration
            CreateMap<CreateParamterConfigCommand, ParamteresConfiguration>();

            CreateMap<UpdateParamterConfigCommand, ParamteresConfiguration>();

            CreateMap<ParamteresConfiguration, ParamterConfigListRespone>();

            CreateMap<ParamteresConfiguration, paramterConfigRespone>();
            #endregion





        }
    }
}





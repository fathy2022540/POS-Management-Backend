using AutoMapper;
using Pos.Application.Common.DTOs;
using Pos.Application.Features.Orders.Commands;
using Pos.Application.Features.Products.Commands;
using POS.Application.Common.DTOs;
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
            CreateMap<OrderDto, OrderItem>().ReverseMap();

            // Map the main Command to the Order Entity
            CreateMap<CreateOrderCommand, Order>()
                // Map the incoming Items list to the OrderItems navigation property
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items))
                // Ignore business logic fields that we will set dynamically in the handler
                .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

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





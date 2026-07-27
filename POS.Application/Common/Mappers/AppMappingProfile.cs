using AutoMapper;
using System.Security.Cryptography;
using JRM.Application.Common.DTOs;
using JRM.Application.Common.DTOs.ParametersConfiguration;
using JRM.Application.Common.DTOs.VendorDocuments;
using JRM.Application.Common.Helper;
using JRM.Application.Features.ContactFollowUp.Commands;
using JRM.Application.Features.LookupServices.Commands;
using JRM.Application.Features.ParamterConfigs.Commands;
using JRM.Application.Features.PermissionServices.Commands;
using JRM.Application.Features.ProductServices.Commands;
using JRM.Application.Features.RolesServices.Commands;
using JRM.Application.Features.UserAccount.Commands;
using JRM.Application.Features.VendorProductPrices.Commands;
using JRM.Application.Features.VendorProductServices.Commands;
using JRM.Application.Features.VendorServices.Commands;
using JRM.Domain.Entities;


namespace JRM.Application.Mappers
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            #region users
            CreateMap<Roles, RolesModel>().ReverseMap();
            CreateMap<Users, UserModel>().ReverseMap();
            CreateMap<UserManagementCommand, Users>()
            .ForMember(dest => dest.Vendor, opt => opt.MapFrom((src, dest, destMember, context) =>
                (Vendors)context.Items["Vendor"]))
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

            #region Vendor
            CreateMap<CreateVendorCommand, Vendors>()
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.MobileNumber))
                .ForMember(dest => dest.VendorNameAR, opt => opt.MapFrom(src => src.VendorNameAR))
                .ForMember(dest => dest.CommercialRegistrationNumber, opt => opt.MapFrom(src => src.CommercialRegistration));

            CreateMap<Vendors, VendorResponse>();

            CreateMap<UpdateVendorCommand, Vendors>()
                .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.VendorName))
                .ForMember(dest => dest.VendorNameAR, opt => opt.MapFrom(src => src.VendorNameAR))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.MobileNumber))
                .ForMember(dest => dest.CommercialRegistrationNumber, opt => opt.MapFrom(src => src.CommercialRegistration));


            CreateMap<Vendors, VendorListResponse>()
                   .ForMember(dest => dest.VendorStatusNameEn, opt => opt.MapFrom(src =>
                    src.VendorStatus != null ? (src.VendorStatus.NameEn) : null))
                .ForMember(dest => dest.VendorStatusNameAR, opt => opt.MapFrom(src =>
                    src.VendorCategory != null ? (src.VendorCategory.NameAR) : null))

                   .ForMember(dest => dest.VendorCategoryNameEn, opt => opt.MapFrom(src =>
                    src.VendorCategory != null ? (src.VendorCategory.NameEn) : null))
                      .ForMember(dest => dest.VendorCategoryNameAR, opt => opt.MapFrom(src =>
                    src.VendorCategory != null ? (src.VendorCategory.NameAR) : null));

            CreateMap<VendorProduct, VendorProductResponse>()
                    .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src =>
                 src.Product.ItemCode != null ? (src.Product.ItemCode) : null))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                 src.Product.Name != null ? (src.Product.Name) : null))
             .ForMember(dest => dest.NameAR, opt => opt.MapFrom(src =>
                 src.Product.NameAR != null ? (src.Product.NameAR) : null))

                .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src =>
                 src.Product.ProductStatus != null ? (src.Product.ProductStatus.NameAR) : null))
                   .ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src =>
                 src.Product.ProductCategory != null ? (src.Product.ProductCategory.NameAR ?? src.Product.ProductCategory.NameEn) : null));

            CreateMap<VendorDocuments, VendorDocumentsRespone>()
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src =>
                src.DocumentType != null ? (src.DocumentType.NameAR ?? src.DocumentType.NameEn) : null));

            CreateMap<Vendors, JourneyDto>()
                .ForMember(dest => dest.VendorNameAR, opt => opt.MapFrom(src => src.VendorNameAR))
                .ForMember(dest => dest.VendorStatusName, opt => opt.MapFrom(src =>
                    src.VendorStatus != null ? (src.VendorStatus.NameAR ?? src.VendorStatus.NameEn) : null))
                .ForMember(dest => dest.VendorCategoryName, opt => opt.MapFrom(src =>
                    src.VendorCategory != null ? (src.VendorCategory.NameAR ?? src.VendorCategory.NameEn) : null))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents));

            CreateMap<CreateVendorProductCommand, VendorProduct>();
            CreateMap<CreateVendorProductPriceCommand, VendorProductPriceHistory>();
            #endregion

            #region ProductItems

            CreateMap<ProductItems, ProductitemsResponse>()
                         .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => src.ProductStatus != null ? (src.ProductStatus.NameAR ?? src.ProductStatus.NameEn) : null))
            .ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.ProductCategory != null ? (src.ProductCategory.NameAR ?? src.ProductCategory.NameEn) : null));
            CreateMap<CreateProductItemsCommand, ProductItems>();
            CreateMap<UpdateProductCommand, ProductItems>();


            #endregion


            #region VendorProductPrice

            //CreateMap<VendorProductPriceHistory, VendorProductPriceListResponse>().
            //    ForMember(dest => dest.ProductName, act => act.MapFrom(x => x.Product.Name))
            //    .ForMember(dest => dest.VendorName, act => act.MapFrom(x => x.Vendor.VendorName))
            //    .ForMember(dest => dest.CurrencyName, act => act.MapFrom(x => x.Currency.NameAR));

            //CreateMap<VendorProductPriceHistory, VendorPriceHistoryResponse>()
            //.ForMember(d => d.VendorName, o => o.MapFrom(s => s.Vendor.VendorName))
            //.ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
            //.ForMember(d => d.CurrencyName, o => o.MapFrom(s => s.Currency != null ? (s.Currency.NameAR ?? s.Currency.NameEn) : null));

            #endregion

            #region SystemData
            CreateMap<CreateContactCommand, Contact>()
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => false));
            #endregion


            #region MyRegion
            CreateMap<CreateVendorDocumentsCommand, VendorDocuments>();
            CreateMap<UpdateVendorDocumentCommand, VendorDocuments>();

            CreateMap<VendorDocuments, VendorDocumentsListRespone>().ForMember(x => x.VendorName, opt => opt.MapFrom(s => s.Vendor.VendorName))
                .ForMember(x => x.DocumentType, opt => opt.MapFrom(s =>
                s.DocumentType != null ? (s.DocumentType.NameAR ?? s.DocumentType.NameEn) : null
                ));
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





using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using JRM.Application.Common.DTOs;
using JRM.Application.Common.Helper;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.UserAccount.Commands
{
    public record UserManagementCommand(
        string FullName,
        string Username,
        string Email,
        string MobileNumber,
        string Password,
        long RoleId,
        long? StatusId,
        long? UserTypeId,
        string? VendorName,
        string? VendorNameAR,
        string? CrNumber,
        string? TaxNumber,
        bool IsActive) : IRequest<ApiResponses<bool>>;

    public class UserManagementCommandHandle(IUnitOfWork<JRMDBContext> unitOfWork,
      IPasswordHasher passwordHasher) : IRequestHandler<UserManagementCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UserManagementCommand request, CancellationToken cancellationToken)
        {
            var normalizedRequest = NormalizeRequest(request);
            var VendorsRepo = unitOfWork.GetRepository<Vendors>();
            var UsersRepo = unitOfWork.GetRepository<Users>();
            Vendors? newVendor = null;
            if (request.UserTypeId == (long)UserType.Vendor)
            {
                var vendorExists = await BusinessValidator.FindConflictAsync(VendorsRepo, v => v.VendorName == request.VendorName
                  || v.CommercialRegistrationNumber == request.CrNumber || v.TaxNumber == request.TaxNumber);

                if (vendorExists != null)
                    return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "A company with this Commercial Registration Number or Tax Number is already registered.");

                var lastVendor = await BusinessValidator.GetLastByPropertyAsync(VendorsRepo, v => v.VendorCode,
                  predicate: v => !string.IsNullOrEmpty(v.VendorCode)
              );

               var vendorcode = lastVendor == null ? "VND001": VendorCodeGenerator.GenerateNextCode(lastVendor.VendorCode);
                newVendor = new Vendors
                {
                    VendorCode = vendorcode,
                    VendorName = normalizedRequest.VendorName,
                    VendorNameAR=normalizedRequest.VendorNameAR,
                    CommercialRegistrationNumber = normalizedRequest.CrNumber,
                    TaxNumber = normalizedRequest.TaxNumber,
                    Email = normalizedRequest.Email,
                    Mobile = normalizedRequest.MobileNumber,
                    CreatedDate = DateTime.UtcNow
                };
                await VendorsRepo.Insert(newVendor);

            }
            // Send all conditions into a single predicate expression using OR (||)
            var userExists = await BusinessValidator.FindConflictAsync(UsersRepo,
                u => u.UserName == normalizedRequest.Username || u.Email == normalizedRequest.Email
            );

            if (userExists != null)
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "Username or Email address is already registered.");

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
  
                var newUser = AppMapper.Mapper.Map<Users>(normalizedRequest, opt =>
                {
                    opt.Items["Vendor"] = newVendor;
                    opt.Items["PasswordHasher"] = passwordHasher;
                });

                await UsersRepo.Insert(newUser);
                await unitOfWork.DoWork();
                // 6. Trigger external notification dispatcher pipelines
                // await emailService.SendActivationEmailAsync(newUser.Email, newUser.UserName, activationToken);

                // Commit atomic modifications securely to disk
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return ApiResponses<bool>.Success(true, "User created successfully");
            }
            catch (Exception ex)
            {
                // Something went down! Revert modifications cleanly from the server environment
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApiResponses<bool>.Failure(StatusResult.InvalidRequest, $"Registration workflow crashed: {ex.Message}");

            }
        }

        private static UserManagementCommand NormalizeRequest(UserManagementCommand request)
        {
            return request with
            {
                FullName = request.FullName?.Trim() ?? string.Empty,
                Username = request.Username?.Trim() ?? string.Empty,
                Email = request.Email?.Trim() ?? string.Empty,
                MobileNumber = request.MobileNumber?.Trim() ?? string.Empty,
                VendorName = request.VendorName?.Trim(),
                VendorNameAR= request.VendorNameAR?.Trim(),
                CrNumber = request.CrNumber?.Trim(),
                TaxNumber = request.TaxNumber?.Trim(),
                IsActive = request.IsActive,
                RoleId = request.RoleId,
            };
        }
    }
}
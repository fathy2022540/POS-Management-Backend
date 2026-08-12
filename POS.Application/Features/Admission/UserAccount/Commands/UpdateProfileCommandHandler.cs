using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.UserAccount.Commands
{
    public record UpdateProfileCommand(int UserId, string Email, string MobileNumber, string fullname, string FirstName, string LastName) : IRequest<ApiResponses<bool>>;
    public class UpdateProfileCommandHandler(IUnitOfWork<POSDBContext> unitofwork) : IRequestHandler<UpdateProfileCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var UsersRepo = unitofwork.GetRepository<Users>();
            var user = await UsersRepo.GetFirstOrDefault<Users>(null, c => c.Id == request.UserId, null, null, true);
            if (user == null)
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "User is Not Found");


            AppMapper.Mapper.Map(request, user);

            await UsersRepo.Update(user);
            await unitofwork.DoWork();

            return ApiResponses<bool>.Success(true, "User updated successfully");
        }
    }
}

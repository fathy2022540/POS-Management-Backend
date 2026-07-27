using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.ContactFollowUp.Commands
{
    public record CreateContactCommand(string FullName, string Email, string PhoneNumber, string Subject, string Message) : IRequest<ApiResponses<bool>>;

    public class CreateContactCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
    : IRequestHandler<CreateContactCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            var repo = unitOfWork.GetRepository<Contact>();

            var newContact = AppMapper.Mapper.Map<Contact>(request);
            newContact.IsRead = false;
            await repo.Insert(newContact);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true);
        }
    }
}

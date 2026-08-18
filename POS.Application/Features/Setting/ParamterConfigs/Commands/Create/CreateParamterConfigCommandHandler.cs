using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.ParamterConfigs.Commands
{
    public record CreateParamterConfigCommand(string Keyword,
        string? Parent,
        string? DescriptionEn,
        string? DescriptionAr,
        string? ContentEn,
        string? ContentAr, string? URL) : IRequest<ApiResponses<bool>>;

    public class CreateParamterConfigCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<CreateParamterConfigCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(CreateParamterConfigCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<ParamteresConfiguration>();

            var parameterConfig = AppMapper.Mapper.Map<ParamteresConfiguration>(request);

            await repository.Insert(parameterConfig);

            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Parameter configuration created successfully.");




        }
    }


}

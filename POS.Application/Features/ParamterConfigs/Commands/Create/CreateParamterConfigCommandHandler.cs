using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Application.Common.DTOs.ParametersConfiguration;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.ParamterConfigs.Commands
{
    public record CreateParamterConfigCommand(string Keyword,
        string? Parent,
        string? DescriptionEn,
        string? DescriptionAr,
        string? ContentEn,
        string? ContentAr,string? URL) : IRequest<ApiResponses<bool>>;

    public class CreateParamterConfigCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork) : IRequestHandler<CreateParamterConfigCommand, ApiResponses<bool>>
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

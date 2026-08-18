using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using POS.Application.Common.Helper.Authentication;
using POS.Application.Common.Interfaces;
using POS.Application.Common.Services;
using POS.Application.Common.Validators;
using POS.Domain.Repositories;
using POS.Infrastructure.Persistence.POSRepositories;
using POS.Infrastructure.Services;
using POS.Infrastructure.Services.DocumentAnalysis;
using System.Reflection;

namespace POS.Application.Helper
{
    public static class DependenciesConfigration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DependenciesConfigration).Assembly);
            services.AddValidatorsFromAssembly(typeof(DependenciesConfigration).Assembly);
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddScoped<IPosUnitOfWork, PosUnitOfWork>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddTransient<IEmailNotificationService, EmailNotificationService>();
            services.AddScoped<IDocumentAnalysisService, DocumentAnalysisService>();
        }
    }
}




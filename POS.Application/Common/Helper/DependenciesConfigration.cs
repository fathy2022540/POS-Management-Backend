using JRM.Application.Common.Helper.Authentication;
using JRM.Application.Common.Validators;
using JRM.Infrastructure.Services;
using JRM.Infrastructure.Services.DocumentAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace JRM.Application.Helper
{
    public static class DependenciesConfigration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            //services.AddScoped<IQoutationService, QoutationServices>();

            services.AddAutoMapper(typeof(DependenciesConfigration).Assembly);
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                // Register the validation behavior into the MediatR pipeline
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddTransient<IEmailNotificationService, EmailNotificationService>();
            services.AddScoped<IDocumentAnalysisService, DocumentAnalysisService>();
        }

    }
}




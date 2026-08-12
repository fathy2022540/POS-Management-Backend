using Microsoft.Extensions.DependencyInjection;
using POS.Application.Common.Helper.Authentication;
using POS.Application.Common.Validators;
using POS.Infrastructure.Services;
using POS.Infrastructure.Services.DocumentAnalysis;
using System.Reflection;

namespace POS.Application.Helper
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
                config.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddTransient<IEmailNotificationService, EmailNotificationService>();
            services.AddScoped<IDocumentAnalysisService, DocumentAnalysisService>();
        }

    }
}




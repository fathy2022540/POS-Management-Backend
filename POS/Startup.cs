using JRM.API.Middleware;
using JRM.Application.Helper;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Cryptography;

namespace JRM.API
{
    public class Startup
    {
        public static string privateKey = @"
MIICXAIBAAKBgQCqGKukO1De7zhZj6+H0qtjTkVxwTCpvKe4eCZ0FPqri0cb2JZfXJ/DgYSF6vUp
wmJG8wVQZKjeGcjDOL5UlsuusFncCzWBQ7RKNUSesmQRMSGkVb1/3j+skZ6UtW+5u09lHNsj6tQ5
1s1SPrCBkedbNf0Tp0GbMJDyR4e9T04ZZwIDAQABAoGAFijko56+qGyN8M0RVyaRAXz++xTqHBLh
3tx4VgMtrQ+WEgCjhoTwo23KMBAuJGSYnRmoBZM3lMfTKevIkAidPExvYCdm5dYq3XToLkkLv5L2
pIIVOFMDG+KESnAFV7l2c+cnzRMW0+b6f8mR1CJzZuxVLL6Q02fvLi55/mbSYxECQQDeAw6fiIQX
GukBI4eMZZt4nscy2o12KyYner3VpoeE+Np2q+Z3pvAMd/aNzQ/W9WaI+NRfcxUJrmfPwIGm63il
AkEAxCL5HQb2bQr4ByorcMWm/hEP2MZzROV73yF41hPsRC9m66KrheO9HPTJuo3/9s5p+sqGxOlF
L0NDt4SkosjgGwJAFklyR1uZ/wPJjj611cdBcztlPdqoxssQGnh85BzCj/u3WqBpE2vjvyyvyI5k
X6zk7S0ljKtt2jny2+00VsBerQJBAJGC1Mg5Oydo5NwD6BiROrPxGo2bpTbu/fhrT8ebHkTz2epl
U9VQQSQzY1oZMVX8i1m5WUTLPz2yLJIBQVdXqhMCQBGoiuSoSjafUhV7i1cEGpb88h5NBYZzWXGZ
37sJ5QsW+sJyoNde3xH8vdXhzU7eT82D6X/scw9RZz+/6rCJ4p0=";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            #region *****Get System Setting*****
            var systemSettings = new SystemSettingsConfiguration();
            Configuration.Bind(key: nameof(systemSettings), systemSettings);
            services.AddSingleton(systemSettings);
            #endregion
            #region *****DbContext*****
            services.AddDbContext<JRMDBContext>(m => m.UseSqlServer(systemSettings.AppSettingsConfiguration.ConnectionStrings, b => b.MigrationsAssembly(typeof(JRMDBContext).Assembly.FullName)), ServiceLifetime.Singleton);

            services.AddScoped<DbContext, JRMDBContext>();
            #endregion

            #region *****Unit Of Work*****
            services.AddScoped<IUnitOfWork<JRMDBContext>, UnitOfWork<JRMDBContext>>();
            #endregion

            services.AddApplicationServices();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());


            });


            #region *****Authentication*****

            //Configuration.Bind(nameof(systemSettings), systemSettings);
            //services.AddSingleton(systemSettings);

            //// Load the RSA private key
            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

            //// Extract public key from private key
            var rsaParameters = rsa.ExportParameters(false);
            var publicKey = new RsaSecurityKey(rsaParameters);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = systemSettings.JWTConfiguration.ValidIssuer,
                    ValidAudience = systemSettings.JWTConfiguration.ValidAudience,
                    IssuerSigningKey = publicKey, // Use the public key for validation
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            #endregion


            #region  *********** Sawager******
            if (systemSettings.AppSettingsConfiguration.enableSwagger)
            {
                services.AddSwaggerGen((swagger) =>
                {
                    //This is to generate the Default UI of Swagger Documentation
                    swagger.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "External_Portal Api",
                        Description = "Authentication and Authorization Portal Api with JWT and Swagger",
                    });
                    // To Enable authorization using Swagger (JWT)
                    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter �Bearer� [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                    });
                    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement{
                  {
                      new OpenApiSecurityScheme
                      {
                          Reference = new OpenApiReference
                          {
                              Type = ReferenceType.SecurityScheme,
                              Id = "Bearer"
                          }
                      },
                          new string[] {}
                      }
              });
                });
            }

            #endregion

            // Auto Mapper Configurations
            #region ***** Mapper *****


            services.AddCors(options =>
                 {
                     options.AddPolicy("AllowSpecificOrigins",
                     builder =>
                     {
                         builder.WithOrigins(systemSettings.CorsUrls)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("Content-Disposition");

                     });
                 });

            #endregion
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // app.ConfigureCustomExceptionMiddleware();
            }
            else
            {
                //  app.ConfigureCustomExceptionMiddleware();
                app.UseHsts();
            }


            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<JRMDBContext>();
                db.Database.Migrate();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("AllowSpecificOrigins");
            app.UseAuthentication();
            app.UseAuthorization();



            //app.UseMiddleware<MiddlewareRequest>();
            app.UseEndpoints(endpoints =>
      {
          endpoints.MapControllers();
      });
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TND.API v1"));

        }
    }
}

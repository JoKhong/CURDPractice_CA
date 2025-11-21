using CURD_Practice.Filters.ActionFilters;
using CURDPractice_CA.Core.Domain.IdentityEntities;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Internal;

namespace CURD_Practice
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment enviroment)
        {
            services.AddTransient<ResponseHeaderActionFilter>();
            
            services.AddControllersWithViews(options => {

                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                options.Filters.Add(new ResponseHeaderActionFilter(logger)
                {
                    Key = "X-Global-Key",
                    Value = "X-Global-Value",
                    Order = 0
                }); // Add in Global Filter

            });

            //Add Auto, ASP.NET covers parameters when add as service
            services.AddScoped<ICountriesRepository, CountriesRepositories>();
            services.AddScoped<IPersonsRepository, PersonsRepositories>();

            //Countries Services
            services.AddScoped<ICountryAdderService, CountryAdderService>();
            services.AddScoped<ICountryGetterServices, CountryGetterServices>();
            services.AddScoped<ICountryUploadFromExcelService, CountryUploadFromExcelService>();

            //Persons Serivces
            services.AddScoped<IPersonAdderService, PersonAdderService>();
            services.AddScoped<IPersonGetterServices, PersonGetterServices>();
            services.AddScoped<IPersonSortPersonsService, PersonSortPersonsService>();
            services.AddScoped<IPersonUpdatePersonService, PersonUpdatePersonService>();
            services.AddScoped<IPersonDeletePersonService, PersonDeletePersonService>();

            //Additional Services
            services.AddScoped<IPersonsToCSVService, PersonToCSVService>();
            services.AddScoped<IPersonsToExcelService, PersonToExcelService>();

            #region Manual Add Service
            //Add Manual, Useful when constructor has other parameters or want control. 
            //BUT NOT RECOMMENDED

            /*
            builder.Services.AddScoped<ICountriesService>(
            provider =>
            {
                PersonsDbContext? dbContext = provider.GetService<PersonsDbContext>();
                return new CountryServices(dbContext);
            });
            builder.Services.AddScoped<IPersonsServices>(
            provider =>
            {
                PersonsDbContext? dbContext = provider.GetService<PersonsDbContext>();
                CountryServices countryServices = provider.GetService<CountryServices>();

                return new PersonServices(dbContext, countryServices);
            });
            */
            #endregion

            services.AddHttpClient();

            if (enviroment.IsEnvironment("Test") == false)
            {
                services.AddDbContext<ApplicationDbContext>
                    (options =>
                    {
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    });
            }

            //Enable Identity in Project
            services.AddIdentity<ApplicationUser, ApplicationRole>(option => {
                option.Password.RequiredLength = 5;
                option.Password.RequireNonAlphanumeric = true;
                option.Password.RequireUppercase = true;
                option.Password.RequireLowercase = true;
                option.Password.RequireDigit = true;
                option.Password.RequiredUniqueChars = 3;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            //Authorization Check
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });
            services.ConfigureApplicationCookie(options => {

                options.LoginPath = "/Account/Login";

            });

            services.AddHttpLogging(options =>
            {
                options.LoggingFields =
                Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
                | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            return services;

        }
    }
}

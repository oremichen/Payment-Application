using ANQ.Notification;
using ContentServiceManagementAPI.Configuration;
using ContentServiceManagementAPI.ContentServiceLogger;
using ContentServiceManagementAPI.Data;
using ContentServiceManagementAPI.Events.Publish.ServiceEvents;
using ContentServiceManagementAPI.HttpClientService;
using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Services;
using ContentServiceManagementAPI.Services.ClientAppService;
using ContentServiceManagementAPI.Services.ContentProviderService;
using ContentServiceManagementAPI.Services.NotificationService;
using ContentServiceManagementAPI.Services.ServiceAppService;
using ContentServiceManagementAPI.Services.ServiceContentMapService;
using ContentServiceManagementAPI.Services.ServiceProviderService;
using ContentServiceManagementAPI.Services.TenantUpdateService;
using ContentServiceManagementAPI.Services.VasProviderAppService;
using ContentServiceManagementAPI.Services.VasSystemService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace ContentServiceManagementAPI
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();
            Configuration = builder.Build();


            var elasticUri = Configuration["ElasticConfiguration:Uri"];


            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    CustomFormatter = new ElasticsearchJsonFormatter(),
                    AutoRegisterTemplate = true,
                })
            .CreateLogger();
        }

        //  public IConfiguration Configuration { get; }
        public IConfiguration Configuration { get; } = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
             .Build();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSingleton(p => Configuration);

            services.Configure<WalletAppSettings>(Configuration.GetSection("WalletService"));

            // Configure CORS for angular2 UI
            services.AddCors(options =>
            {
                options.AddPolicy(_defaultCorsPolicyName, p =>
                {
                    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

           services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // added this line to make settings available globally
            services.Configure<ServiceSettings>(Configuration.GetSection("ServiceSettings"));

            services.AddAnqNotification();
            services.Configure<NotificationSettings>(Configuration.GetSection("NotificationSettings"));
            services.Configure<AppSettings>(Configuration.GetSection("ContentProcessingService"));

            services.AddMvc();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMap());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Register the Swagger services
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Content Service Management",
                    Description = "API for Content Service Management",
                    TermsOfService = new Uri("https://www.coure-tech.com"),
                    Contact = new OpenApiContact
                    {
                        Name = "Application Support",
                        Email = "support@coure-tech.com",
                        Url = new Uri("https://www.coure-tech.com"),
                    }
                });
            });

            // added this line to make settings available globally
            services.Configure<ScheduledTasksSettings>(Configuration.GetSection("ScheduledTasksSettings"));
            services.Configure<VasHostingPlatformSettings>(Configuration.GetSection("VasHostingPlatformSettings"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));

            services.AddDbContext<ANQContentServiceManageDb>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("ANQContentServiceManageDbConnection")));

            //services.AddSingleton<IHostedService, MessagingBackgroundService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IServiceProviderAppService, ServiceProviderAppService>();
            services.AddTransient<IClientAppService, ClientAppService>();
            services.AddTransient<IMapServiceToClientAppService, MapServiceToClientAppService>();
            services.AddTransient<IServiceAppService, ServiceAppService>();
            services.AddTransient<IContentProviderAppService, ContentProviderAppService>();
            services.AddTransient<IContentAppService, ContentAppService>();
            services.AddTransient<IContentServiceEvent, ContentServiceEvent>();
            services.AddTransient<IAnqClientService, AnqClientService>();
            services.AddTransient<IServiceContentMapAppService, ServiceContentMappAppService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IVasProviderAppService, VasProviderAppService>();
            services.AddTransient<IMapContentToServiceProviderAppService, MapContentToServiceProviderAppService>();
            services.AddTransient<IContentServiceEvent, ContentServiceEvent>();
            services.AddTransient<ICServiceLogger, CServiceLogger>();
            services.AddTransient<IVasSystemServiceAppService, VasSystemServiceAppService>();

            services.AddTransient<IUpdateTenantsService, UpdateTenantsService>();

            services.AddRabbitMqMassTransitServices(Configuration);

            services.AddHttpClient("ContentProcessingService", c =>
            {
                c.BaseAddress = new Uri(Configuration["ContentProcessingService:BaseUrl"]);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("CoureDeliverySmsClient", c =>
            {
                c.BaseAddress = new Uri(Configuration["CoureDeliverySmsService:BaseUrl"]);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("BillingContentService", c =>
            {
                c.BaseAddress = new Uri(Configuration["BillingContentService:BaseUrl"]);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env , ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
           
         

           // app.UseCors(_defaultCorsPolicyName);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseCors(builder =>
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            var log = new LoggerConfiguration()
               .MinimumLevel.Information()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
               .Enrich.FromLogContext()
               .ReadFrom.Configuration(Configuration)
               .CreateLogger();

            
            app.UseHttpsRedirection();
         
            loggerFactory.AddSerilog(log);
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Coure Service");
                //c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

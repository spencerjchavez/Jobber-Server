using Jobber_Server.DBContext;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Jobber_Server.Services.Contractors;
using Jobber_Server.Services.Contractors.Sectors;

namespace Jobber_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class Startup(IConfiguration configuration)
    {

        private readonly IConfiguration _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddExceptionHandler<ExceptionHandler>();
            services.AddDbContext<JobberDbContext>(options =>
                options.UseMySql(
                    _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version()),
                    mysqlOptions => mysqlOptions.EnableRetryOnFailure()
                )
            );

            services.AddScoped<IContractorService, ContractorService>();
            services.AddScoped<ISectorServiceInternal, SectorServiceInternal>();
            
            services.AddControllers(); // Adds services required for API controllers

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Jobber Server API", 
                    Version = "v1",
                    Description = "" 
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jobber Server API V1");
            });
            
            app.UseRouting();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "assets")),
                RequestPath = "/assets"
            });
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
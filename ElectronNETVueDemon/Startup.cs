using System.IO;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using ElectronNETVueDemon.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ElectronNETVueDemon
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ElectronNETVueDemon", Version = "v1" });
            });

            ConfigureCors(services);
            services.UseJwtBearerAuthentication();
            // Open the Electron-Window here
            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions() {
                Resizable = false,
                Maximizable = false
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElectronNETVueDemon v1"));
            }

            app.UseCors(DefaultCorsPolicyName);
            
            #region ??????????????????

            app.UseStaticFiles();

            string filesDirectory = Configuration.GetSection("UploadFileDirectory").Value;
            if (string.IsNullOrWhiteSpace(filesDirectory))
            {
                filesDirectory = Directory.GetCurrentDirectory();
            }
            string publiclyFilesPath = Path.Combine(filesDirectory, @"files");
            if (!Directory.Exists(publiclyFilesPath))
            {
                Directory.CreateDirectory(publiclyFilesPath);
            }

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(publiclyFilesPath),
                RequestPath = new PathString("/files"),
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
                }
            });
            
            #endregion ??????????????????
            
            app.UseFileServer();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot";
            });
        }
        
        private const string DefaultCorsPolicyName = "Default";

        private void ConfigureCors(IServiceCollection services)
        {
            IWebHostEnvironment env = services.BuildServiceProvider().GetService<IWebHostEnvironment>();
            if (env.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(DefaultCorsPolicyName, builder =>
                    {
                        builder.AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin();
                    });
                });
            }
        }
    }
}

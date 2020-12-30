using ProjectManager.API.Extensions;
using ProjectManager.API.Infrastructure.AutofacModules;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ProjectManager.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddOptions()
                .AddCustomMvc()
                .AddHealthChecks(Configuration)
                .AddCustomSwagger(Configuration)
                .AddCustomAuthentication(Configuration)
                .AddDefaultPagination(c =>
                {
                    c.MaxPageSizeAllowed = 100;
                    c.DefaultPageSize = 20;
                })
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var container = new ContainerBuilder();
            container.Populate(services);

            container.RegisterModule(new MediatorModule());

            container.RegisterModule(new ApplicationModule(Configuration));

            return new AutofacServiceProvider(container.Build());
        }

        public void Configure(IApplicationBuilder app,
            IHostApplicationLifetime applicationLifetime)
        {
            app.UseCors("CorsPolicy");

            app.UseCustomSwagger(Configuration);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}
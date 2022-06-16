using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Extensions.Configuration;

namespace OcelotApiGateway
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot();
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 200;
                    string homeData = HomePageData(env);

                    await context.Response.WriteAsync(homeData);
                });
            });

            await app.UseOcelot();
        }

        private string HomePageData(IWebHostEnvironment env)
        {
            string ocelotJsonName = $"ocelot.{env.EnvironmentName}.json";
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(ocelotJsonName);

            IConfigurationRoot Configuration = builder.Build();

            var routes = Configuration.GetSection("Routes");
            var globalConfiguration = Configuration.GetSection("GlobalConfiguration");
            var logging = Configuration.GetSection("Logging");

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                Message = "API Gateway ready",
                GlobalConfiguration = globalConfiguration,
                Routes = routes
            });
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            services.AddOcelot();
        //public void ConfigureServices(IServiceCollection services)
        //{
        //  services.AddControllers();
        //}

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
            app.UseOcelot().Wait();

        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //  if (env.IsDevelopment())
        //  {
        //    app.UseDeveloperExceptionPage();
        //  }
        //  app.UseHttpsRedirection();
        //  app.UseRouting();
        //  app.UseAuthorization();
        //  app.UseEndpoints(endpoints =>
        //  {
        //    endpoints.MapControllers();
        //  });
        //}
    }
}

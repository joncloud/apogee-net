using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Apogee
{
    public static class ApogeeApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApogee(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;

            services.GetRequiredService<ApogeeBuilder>().Initialize(services);

            var appLifetime = services.GetRequiredService<IApplicationLifetime>();
            appLifetime.ApplicationStopping.Register(() =>
            {
                services.GetRequiredService<IApogeeFlusher>().Flush();
            });
            return app;
        }
    }
}

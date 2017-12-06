using Microsoft.Extensions.DependencyInjection;
using System;

namespace Apogee
{
    public static class ApogeeServiceCollectionExtensions
    {
        public static IServiceCollection AddApogee(this IServiceCollection services, Action<IApogeeBuilder> builder)
        {
            var options = new ApogeeBuilder(services);
            builder(options);
            return services;
        }
    }
}

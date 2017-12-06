using System;
using Microsoft.Extensions.DependencyInjection;

namespace Apogee
{
    class ApogeeBuilder : IApogeeBuilder
    {
        readonly IServiceCollection _services;
        TimeSpan _interval = TimeSpan.FromSeconds(1);
        int? _count;
        Action<IServiceProvider> _initialize = _ => { };

        public ApogeeBuilder(IServiceCollection services)
        {
            _services = services;
            _services.AddSingleton(this);
            _services.AddSingleton<IApogeeFlusher, ApogeeFlusher>();
            _services.Add(ServiceDescriptor.Singleton(typeof(IBatchQueue<>), typeof(BatchQueue<>)));
            _services.Add(ServiceDescriptor.Singleton(typeof(IBatchService<>), typeof(BatchService<>)));
            _services.Add(ServiceDescriptor.Singleton(typeof(IBatchNotifier<>), typeof(BatchNotifier<>)));
            _services.Configure<ApogeeOptions>(ConfigureOptions);
        }

        void ConfigureOptions(ApogeeOptions options)
        {
            options.FlushInterval = _interval;
            options.MaximumQueueCountBeforeAutoFlush = _count;
        }

        public void Initialize(IServiceProvider services) => _initialize(services);

        IApogeeBuilder AddShared<T>()
        {
            _services.AddSingleton<BatchTimer<T>>();

            Action<IServiceProvider> fn = svc => svc.GetRequiredService<BatchTimer<T>>();
            _initialize = (Action<IServiceProvider>)Delegate.Combine(_initialize, fn);
            return this;
        }

        public IApogeeBuilder AddProcessorScoped<T, TProc>() where TProc : class, IBatchProcessor<T>
        {
            _services.AddScoped<IBatchProcessor<T>, TProc>();
            return AddShared<T>();
        }

        public IApogeeBuilder AddProcessorSingleton<T, TProc>() where TProc : class, IBatchProcessor<T>
        {
            _services.AddSingleton<IBatchProcessor<T>, TProc>();
            return AddShared<T>();
        }

        public IApogeeBuilder AddProcessorSingleton<T, TProc>(TProc processor) where TProc : class, IBatchProcessor<T>
        {
            _services.AddSingleton<IBatchProcessor<T>>(processor);
            return AddShared<T>();
        }

        public IApogeeBuilder AddProcessorTransient<T, TProc>() where TProc : class, IBatchProcessor<T>
        {
            _services.AddTransient<IBatchProcessor<T>, TProc>();
            return AddShared<T>();
        }

        public IApogeeBuilder FlushInterval(TimeSpan interval)
        {
            _interval = interval;
            return this;
        }

        public IApogeeBuilder MaximumQueueCountBeforeAutoFlush(int count)
        {
            _count = count;
            return this;
        }
    }
}

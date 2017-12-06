using System;

namespace Apogee
{
    public interface IApogeeBuilder
    {
        IApogeeBuilder FlushInterval(TimeSpan interval);
        IApogeeBuilder MaximumQueueCountBeforeAutoFlush(int count);
        IApogeeBuilder AddProcessorScoped<T, TProc>() where TProc : class, IBatchProcessor<T>;
        IApogeeBuilder AddProcessorSingleton<T, TProc>() where TProc : class, IBatchProcessor<T>;
        IApogeeBuilder AddProcessorSingleton<T, TProc>(TProc processor) where TProc : class, IBatchProcessor<T>;
        IApogeeBuilder AddProcessorTransient<T, TProc>() where TProc : class, IBatchProcessor<T>;
    }
}

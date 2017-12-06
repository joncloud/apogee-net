using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Apogee.Tests
{
    public class UnitTest1
    {
        class IntProcessor : IBatchProcessor<int>
        {
            public ManualResetEvent Event { get; } = new ManualResetEvent(false);
            public ConcurrentBag<Batch<int>> Batches { get; } = new ConcurrentBag<Batch<int>>();

            public void Process(Batch<int> batch)
            {
                Batches.Add(batch);
                Event.Set();
            }
        }
        void WaitOrThrow(ManualResetEvent manualResetEvent, int millisecondsTimeout) =>
            Assert.True(manualResetEvent.WaitOne(millisecondsTimeout));

        async Task WhenAllSequential(IEnumerable<Func<Task>> fns)
        {
            foreach (var fn in fns)
            {
                await fn();
            }
        }

        [Fact]
        public async Task ShouldBatchAllItemsTogether()
        {
            var processor = new IntProcessor();
            await TestAsync(options => options
                .MaximumQueueCountBeforeAutoFlush(5)
                .AddProcessorSingleton<int, IntProcessor>(processor),
            async http =>
            {
                var ids = Enumerable.Range(0, 5);

                await Task.WhenAll(ids.Select(id => http.GetAsync($"/{id}")));
                WaitOrThrow(processor.Event, 500);

                var batch = Assert.Single(processor.Batches);

                Assert.Equal(ids, batch.Items.OrderBy(x => x));
            });
        }

        protected async Task TestAsync(Action<IApogeeBuilder> configure, Func<HttpClient, Task> fn)
        {
            var webHostBuilder = CreateWebHostBuilder(configure);
            using (var server = new TestServer(webHostBuilder))
            using (var client = server.CreateClient())
            {
                await fn(client);
            }
        }

        IWebHostBuilder CreateWebHostBuilder(Action<IApogeeBuilder> configure)
            => new WebHostBuilder()
               .ConfigureServices(services =>
               {
                   services.AddApogee(configure);
               })
               .Configure(app =>
               {
                   app.UseApogee().Run(async ctx =>
                   {
                       string param = ctx.Request.Path.Value.Substring(1);
                       if (int.TryParse(param, out var i))
                           ctx.RequestServices.GetRequiredService<IBatchService<int>>().Add(i);
                       await ctx.Response.WriteAsync("Hello World!");
                   });
               });
    }
}

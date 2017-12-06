# Apogee.NET
[![Travis](https://img.shields.io/travis/joncloud/apogee-net.svg)](https://travis-ci.org/joncloud/apogee-net/)
[![NuGet](https://img.shields.io/nuget/v/Apogee.svg)](https://www.nuget.org/packages/Apogee/)

## Description
Apogee.NET allows activity to be batched together in the background.

## Licensing
Released under the MIT License.  See the [LICENSE][] file for further details.

[license]: LICENSE.md

## Installation
In the Package Manager Console execute

```powershell
Install-Package Apogee
```

Or update `*.csproj` to include a dependency on

```xml
<ItemGroup>
  <PackageReference Include="Apogee" Version="0.1.0-*" />
</ItemGroup>
```

## Usage

Use the `AddApogee` configuration method to define all batch processors. Follow up with the `UseApogee` method to ensure that the system is started when the application starts.

See the following example for tracking visitors to the web site. It takes any activity that occurs, and defers it to the background in a batch to prevent the activity from taking valuable time from the intent of the user.

```csharp
public void ConfigureServices(IServiceCollection services) => 
  services.AddApogee(options => options.AddProcessorScoped<Visitor, VisitorProcessor>());
  
public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
  app.UseApogee().UseMvc();
  
class VisitorProcessor : IBatchProcessor<Visitor> {
  readonly VisitorContext _context;
  public VisitorProcessor(VisitorContext context) =>
    _context = context;

  public void Process(Batch<Visitor> batch) {
    _context.Visitors.AddRange(batch.Items);
    _context.SaveChanges();
  }
}

class VisitorMiddleware {
  readonly RequestDelegate _next;
  readonly IBatchService<Visitor> _batchService;
  public VisitorMiddleware(RequestDelegate next, IBatchService<Visitor> batchService) {
    _next = next;
    _batchService = batchService;
  }
  
  public Task InvokeAsync(HttpContext context) {
    var visitor = new Visitor(context);
    _batchService.Add(visitor);
    return _next(context);
  }
}
```

For additional usage see [Tests][].

[Tests]: tests/Apogee.Tests

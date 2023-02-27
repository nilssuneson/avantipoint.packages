using AvantiPoint.Packages;
using AvantiPoint.Packages.Core;
using AvantiPoint.Packages.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddNuGetPackageApi(options =>
{
    options.AddFileStorage();
    //.AddUpstreamSource("NuGet.org", "https://api.nuget.org/v3/index.json")

    switch (options.Options.Database.Type)
    {
        case "SqlServer":
            options.AddSqlServerDatabase("SqlServer");
            break;
        case "MariaDb":
            options.AddMariaDb("MariaDb");
            break;
        case "MySql":
            options.AddMySqlDatabase("MySql");
            break;
        default:
            options.AddSqliteDatabase("Sqlite");
            break;
    }
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
#if DEBUG
    using var scope = app.Services.CreateScope();
    using var db = scope.ServiceProvider.GetRequiredService<IContext>();
    db.Database.EnsureCreated();
#endif
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseOperationCancelledMiddleware();

app.UseEndpoints(endpoints =>
{
    endpoints.MapNuGetApiRoutes();
});
await app.RunAsync();

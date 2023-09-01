using CommonModels;
using GameServerClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Set the base address for the HttpClient
#if DEBUG
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(Settings.LocalHost) });
#else
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(Settings.RemoteHost) });
#endif

builder.Services.AddMudServices();

await builder.Build().RunAsync();
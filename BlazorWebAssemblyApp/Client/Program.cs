using BlazorWebAssemblyApp;
using BlazorWebAssemblyApp.Client;
using GameComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp =>
    {
        var navigationManager = sp.GetRequiredService<NavigationManager>();
        return new HubConnectionBuilder()
          .WithUrl(navigationManager.ToAbsoluteUri("/hostguesthub"))
          .WithAutomaticReconnect();
    });

builder.Services.AddScoped<IHostGuestBridge, HostGuestWebAssemblyAppBridge>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<OthelloSettings>();
builder.Services.AddScoped<OthelloService>();

await builder.Build().RunAsync();

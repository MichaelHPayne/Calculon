using Calculon.ConsoleApp;
using Calculon.Core.Interfaces;
using Calculon.Core.Models;
using Calculon.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Configure services  
builder.Services.AddStringCalculator(new CalculatorOptions());
builder.Services.AddSingleton<IConsoleIO, ConsoleIO>();
builder.Services.AddTransient<Application>();

// Build the host  
using var host = builder.Build();

// Run the application and check if it should exit immediately
if (host.Services.GetRequiredService<Application>().Run())
{
    // Exit immediately without running the host
    return;
}

// If we reach here, it means the application didn't request an immediate exit
await host.RunAsync();
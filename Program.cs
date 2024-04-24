using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel((options) =>
{
    // Do not add the Server HTTP header.
    options.AddServerHeader = false;
    // https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/
    options.ListenAnyIP(5104);
});

builder.Services.AddControllers();

builder.Services.AddOpenTelemetry()
    .WithMetrics(builder =>
    {
        builder
        // prometheus-exporter
        .AddPrometheusExporter()
        .AddMeter("Microsoft.AspNetCore.Hosting",
                         "Microsoft.AspNetCore.Server.Kestrel");
    })
    .WithTracing(traceBuilder => 
        traceBuilder
        // open-telemetry collector
        .AddSource("vodka-source")
        .ConfigureResource(resource => resource
                .AddService("vodka-service"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://172.16.1.8:4317");
            options.Protocol = OtlpExportProtocol.Grpc;
        })
    );

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();

app.UseAuthorization();

app.MapControllers();

app.Run();

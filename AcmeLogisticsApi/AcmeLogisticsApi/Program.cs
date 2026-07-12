using AcmeLogisticsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string eventHubConnectionString = builder.Configuration["EventHub:ConnectionString"];
string eventHubName = builder.Configuration["EventHub:Name"];
string blobConnectionString = builder.Configuration["EventHub:BlobStorage"];
var sustainedHighReadingsThreshold = builder.Configuration.GetValue<int>("TelemetryIngest:SustainedHighReadingsThreshold", 3);
var monitoredDeviceId = builder.Configuration["TelemetryIngest:MonitoredDeviceId"]
    ?? throw new InvalidOperationException("TelemetryIngest:MonitoredDeviceId is required in configuration.");
var defaultTemperatureThresholdC = builder.Configuration.GetValue<double>("TelemetryIngest:DefaultTemperatureThresholdC", 8.0);
var alertMessage = builder.Configuration["TelemetryIngest:AlertMessage"]
    ?? "Alert sustained: temperature exceeds";

builder.Services.AddSingleton<ITwinRepository, RegistryTwinRepository>();
builder.Services.AddSingleton<DeviceService>();
builder.Services.AddSingleton<TelemetryService>();

builder.Services.AddSingleton<TelemetryIngestService>(sp =>
{
    var deviceService = sp.GetRequiredService<DeviceService>();
    return new TelemetryIngestService(
        eventHubConnectionString,
        eventHubName,
        blobConnectionString,
        sustainedHighReadingsThreshold,
        monitoredDeviceId,
        defaultTemperatureThresholdC,
        alertMessage,
        deviceService);
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("AllowAngularClient");
app.UseAuthorization();

//Start the services as soon the service runs.
var ingestService = app.Services.GetRequiredService<TelemetryIngestService>();
await ingestService.StartAsync();

app.MapControllers();

app.Run();

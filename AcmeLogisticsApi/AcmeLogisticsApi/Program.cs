using AcmeLogisticsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string eventHubConnectionString = builder.Configuration["EventHub:ConnectionString"];
string eventHubName = builder.Configuration["EventHub:Name"];
string blobConnectionString = builder.Configuration["EventHub:BlobStorage"];

builder.Services.AddSingleton(new TelemetryIngestService(eventHubConnectionString, eventHubName, blobConnectionString));


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

//Start the services as soon the service runs.
var ingestService = app.Services.GetRequiredService<TelemetryIngestService>();
await ingestService.StartAsync();

app.MapControllers();

app.Run();

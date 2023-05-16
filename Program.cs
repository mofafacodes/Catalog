using Catalog.Repositories;
using Catalog.Settings;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
//Mongo Db registration
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
//pulling out mongodb settings to reduce them later on
var mongoDBSettings = builder.Configuration.GetSection(nameof(MongoDBSettings)).Get<MongoDBSettings>();


builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
//mongo db settings used here as mongodb client singleton
    return new MongoClient(connectionString: mongoDBSettings?.ConnectionString);
}
);
builder.Services.AddSingleton<IItemRepository, MongoDBItemsRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
}); 
builder.Services.AddHealthChecks()
    //using mongodb settings to specify the connection string that should be used to connect to the databse
    //hence health check will be based on is database reachable or not
    //add a timeout period of 3 seconds after which health check will be considered failed if not able to connect to database
    .AddMongoDb(
    mongodbConnectionString: mongoDBSettings.ConnectionString, 
    name: "mongodb", 
    timeout: TimeSpan.FromSeconds(3),
    //adding tags to help group health checks
    tags: new[] { "ready" }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{ 
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI( c => c.SwaggerEndpoint("swagger/v1/swagger.json", "Catalog"));
}

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    //ready will make sure out api is ready to receive requests
    endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        //specify predicate to filter out which health checks you want to include in endpoint
        Predicate = (check) => check.Tags.Contains("ready"),
        //customizing message to recieve when making health checks
        ResponseWriter = async(context, report) =>
        {
            var result = JsonSerializer.Serialize(
                new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        execption = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                        duration = entry.Value.Duration.ToString()
                    })
                });
            //format output
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
        }
    });
    //ready will make sure out api is alive or up and running
    endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        //specify predicate to filter out which health checks you want to include in endpoint
        Predicate = (check) => check.Tags.Contains("live"),
        ResponseWriter = async (context, report) =>
         {
             var result = JsonSerializer.Serialize(
                 new
                 {
                     status = report.Status.ToString(),
                     checks = report.Entries.Select(entry => new
                     {
                         name = entry.Key,
                         status = entry.Value.Status.ToString(),
                         execption = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                         duration = entry.Value.Duration.ToString()
                     })
                 });
             //format output
             context.Response.ContentType = MediaTypeNames.Application.Json;
             await context.Response.WriteAsync(result);
         }
    });
});

app.Run();
 
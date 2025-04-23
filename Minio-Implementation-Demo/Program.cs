using Microsoft.Extensions.Options;
using Minio;
using Minio_Implementation_Demo.Models;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// MinIO Options Binding
builder.Services.AddSingleton<MinioService>();
//builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));

// Register MinioClient as Singleton
//builder.Services.AddSingleton(sp =>
//{
//    var options = sp.GetRequiredService<IOptions<MinioOptions>>().Value;
//    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
//    return new MinioClient()
//                .WithEndpoint(options.Endpoint)
//                .WithCredentials(options.AccessKey, options.SecretKey)
//                .WithSSL(false) // if MinIO runs without https
//                .Build();
//});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

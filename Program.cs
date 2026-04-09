using DigitalDepositAccountService.Applicaton.Behaviours;
using DigitalSalaryService;
using DigitalSalaryService.Application.Services.Abstract;
using DigitalSalaryService.Application.Services.Concrete;
using DigitalSalaryService.Applicaton.Behaviours;
using DigitalSalaryService.Middlewares;
using DigitalSalaryService.Models.Configurations;
using DigitalSalaryService.Persistence;
using DigitalSalaryService.Persistence.Repositories.Abstract;
using DigitalSalaryService.Persistence.Repositories.Concrete;
using ElasticApmSerilogIntegration;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Serilog;
using StackExchange.Redis;
using System;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(
                     options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Content-Disposition");
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => SwaggerHelper.ConfigureSwaggerGen(c));



#region Configurations

builder.Services.Configure<SimaConfig>(builder.Configuration.GetSection("SimaConfig"));
builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("DigitalSalaryDbContext"));
builder.Services.Configure<MinioClientConfig>(builder.Configuration.GetSection("MinioClientConfig"));
builder.Services.Configure<SimaSignerClientConfig>(builder.Configuration.GetSection("SimaSignerClientConfig"));
builder.Services.Configure<T24Config>(builder.Configuration.GetSection("T24Config"));
builder.Services.Configure<JasperConfig>(builder.Configuration.GetSection("JasperConfig"));
builder.Services.Configure<KycConfig>(builder.Configuration.GetSection("KycConfig"));
builder.Services.Configure<FatcaConfig>(builder.Configuration.GetSection("FatcaConfig"));



#endregion

var dbConfig = builder.Configuration.GetSection("DigitalSalaryDbContext").Get<DatabaseConfig>();
builder.Services.AddDbContext<DigitalSalaryDbContext>(options =>
{
    options.UseSqlServer(dbConfig!.GetConnectionString());
});

var redisConnection = builder.Configuration.GetValue("RedisConnection", string.Empty);

var multiplexer = ConnectionMultiplexer.Connect(redisConnection);

builder.Services
               .AddStackExchangeRedisCache(options =>
               {
                   options.Configuration = redisConnection;
               })
               ;

builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

builder.Services.AddSingleton<RedLockFactory>(provider =>
{
    return RedLockFactory.Create(new List<RedLockMultiplexer> { new RedLockMultiplexer(multiplexer) });
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddMediatR(m =>
{
    m.RegisterServicesFromAssembly(typeof(Program).Assembly);
    m.AddOpenBehavior(typeof(LoggingPipelineBehaviour<,>));
    m.AddOpenBehavior(typeof(ValidationiPipelineBehaviour<,>));
});


#region Services
var simaConfig = builder.Configuration.GetSection("SimaConfig").Get<SimaConfig>();
var simaSignerConfig = builder.Configuration.GetSection("SimaSignerClientConfig").Get<SimaSignerClientConfig>();
var kycConfig = builder.Configuration.GetSection("KycConfig").Get<KycConfig>();
var fatcaConfig = builder.Configuration.GetSection("FatcaConfig").Get<FatcaConfig>();


builder.Services.AddHttpClient<ISimaClient, SimaClient>(client =>
{
    client.BaseAddress = new Uri(simaConfig!.BaseUrl);
});

builder.Services.AddHttpClient<ISignerClient, SimaSignerClient>(client =>
{
    client.BaseAddress = new Uri(simaSignerConfig!.BaseUrl);
});
builder.Services.AddHttpClient<ISignerClient, SimaSignerClient>(client =>
{
    client.BaseAddress = new Uri(simaSignerConfig!.BaseUrl);
});
builder.Services.AddHttpClient<IKycClient, KycClient>(client =>
{
    client.BaseAddress = new Uri(kycConfig!.BaseUrl);
    client.DefaultRequestHeaders.Add("ApiKey", kycConfig.ApiKey);
});
builder.Services.AddHttpClient<IFatcaClient, FatcaClient>(client =>
{
    client.BaseAddress = new Uri(fatcaConfig!.BaseUrl);
    client.DefaultRequestHeaders.Add("ApiKey", fatcaConfig.ApiKey);
});

builder.Services.AddSingleton<IT24Client, T24Client>();
builder.Services.AddSingleton<IJasperClient, JasperClient>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<ICacheService, RedisService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IDocumentSigningService, DocumentSigningService>();
builder.Services.AddHostedService<BankSignDocumentBackgroundService>();


#endregion

#region Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseEFRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOperationRepository, OperationRepository>();

#endregion

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var projectName = Assembly.GetExecutingAssembly().GetName().Name!.ToLower();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .Build();

var elkEnvironmentName = configuration.GetValue("ElasticConfiguration:Env", string.Empty);
ApmElasticsearchLoggerInitializer.Initialize(configuration, elkEnvironmentName!, projectName);

Log.Information("DigitalSalary service started to work");

var app = builder.Build();

app.UseApm(builder.Configuration);

app.UseCors();

app.UseSwagger(c => SwaggerHelper.ConfigureSwagger(c));
app.UseSwaggerUI(c => SwaggerHelper.ConfigureSwaggerUI(c));

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

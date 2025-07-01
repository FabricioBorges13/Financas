using System.Text.Json.Serialization;
using Financas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
var redisHost = builder.Configuration.GetConnectionString("RedisConnection");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisHost));

builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<IRegistrarClienteUseCase, RegistrarClienteUseCase>();
builder.Services.AddScoped<IRegistrarContaUseCase, RegistrarContaUseCase>();
builder.Services.AddScoped<IBuscarClientesUseCase, BuscarClientesUseCase>();
builder.Services.AddScoped<IBuscarClienteUseCase, BuscarClienteUseCase>();
builder.Services.AddScoped<IBuscarContasUseCase, BuscarContasUseCase>();
builder.Services.AddScoped<IBuscarContaUseCase, BuscarContaUseCase>();
builder.Services.AddScoped<IRegistrarVendaCreditoAVistaUseCase, RegistrarVendaCreditoAVistaUseCase>();
builder.Services.AddScoped<IAdicionarSaldoUseCase, AdicionarSaldoUseCase>();
builder.Services.AddScoped<IRegistrarVendaCreditoParceladoUseCase, RegistrarVendaCreditoParceladoUseCase>();
builder.Services.AddScoped<IRegistrarVendaDebitoUseCase, RegistrarVendaDebitoUseCase>();
builder.Services.AddScoped<IRegistrarEstornoUseCase, RegistrarEstornoUseCase>();
builder.Services.AddScoped<IRegistrarTransferenciaUseCase, RegistrarTransferenciaUseCase>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
builder.Services.AddScoped<IRedisRepository, RedisRepository>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IResilienceService, ResilienceService>();
builder.Services.AddScoped<IBuscarTodasAsTransacoesUseCase, BuscarTodasAsTransacoesUseCase>();
builder.Services.AddScoped<IBuscarTodasAuditoriasUseCase, BuscarTodasAuditoriasUseCase>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Host.UseSerilog();    

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter()) // Ideal pra Docker + Loki
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Information("Iniciando a aplicação");
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DbInitializer.SeedAsync(context);
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();

app.MapControllers();
app.Run();

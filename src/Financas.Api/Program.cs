using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:80");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=localhost,1433;Database=FinancasDb;User Id=sa;Password=P@ssword1;TrustServerCertificate=True"));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379"));

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


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.MapControllers();
app.Run();

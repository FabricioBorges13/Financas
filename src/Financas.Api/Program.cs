using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=financas.db"));

builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<IRegistrarVendaUseCase, RegistrarVendaUseCase>();
builder.Services.AddScoped<IRegistrarClienteUseCase, RegistrarClienteUseCase>();
builder.Services.AddScoped<IRegistrarContaUseCase, RegistrarContaUseCase>();
builder.Services.AddScoped<IBuscarClientesUseCase, BuscarClientesUseCase>();
builder.Services.AddScoped<IBuscarClienteUseCase, BuscarClienteUseCase>();
builder.Services.AddScoped<IBuscarContasUseCase, BuscarContasUseCase>();
builder.Services.AddScoped<IBuscarContaUseCase, BuscarContaUseCase>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();

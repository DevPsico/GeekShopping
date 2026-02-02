using GeekShopping.ProductAPI.Config;
using GeekShopping.ProductAPI.Model.Context;
using GeekShopping.ProductAPI.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configura o DbContext com MySQL
builder.Services.AddDbContext<MySQLContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySQLConnection"),
        new MySqlServerVersion(new Version(9, 4, 0)) // ajuste conforme sua versão do MySQL
    )
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 🔧 Registra o repositório de produtos para injeção de dependência
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// 🔧 Configura o AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 🔧 Adiciona os controllers
builder.Services.AddControllers();

// 🔧 Adiciona suporte ao Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🌐 Ativa o Swagger em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔐 Redireciona para HTTPS
app.UseHttpsRedirection();

// 🔐 Middleware de autorização (pode ser útil com autenticação futuramente)
app.UseAuthorization();

// 🔧 Mapeia os endpoints dos controllers
app.MapControllers();

// 🚀 Inicia a aplicação
app.Run();
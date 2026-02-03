using GeekShopping.ProductAPI.Config;
using GeekShopping.ProductAPI.Model.Context;
using GeekShopping.ProductAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configura o DbContext com MySQL
builder.Services.AddDbContext<MySQLContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySQLConnection"),
        new MySqlServerVersion(new Version(9, 4, 0)) // ajuste conforme sua versão do MySQL
    )
);

// 🔧 Adiciona os controllers com configuração JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 🔧 Registra o repositório de produtos para injeção de dependência
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// 🔧 Configura o AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ============================
// 🔐 CONFIGURAÇÃO DE AUTENTICAÇÃO (IdentityServer)
// ============================
// Configura a API para validar tokens JWT emitidos pelo IdentityServer.
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        // URL do IdentityServer (emissor dos tokens)
        options.Authority = builder.Configuration["IdentityServer:Authority"];
        
        // Validação do token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false // Em produção, valide o audience!
        };
    });

// ============================
// 🛡️ CONFIGURAÇÃO DE AUTORIZAÇÃO
// ============================
// Define políticas de acesso baseadas nos scopes do token.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "geek_shopping");
    });
});

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

// 🔐 Middleware de autenticação (DEVE vir ANTES de Authorization)
app.UseAuthentication();

// 🔐 Middleware de autorização
app.UseAuthorization();

// 🔧 Mapeia os endpoints dos controllers
app.MapControllers();

// 🚀 Inicia a aplicação
app.Run();
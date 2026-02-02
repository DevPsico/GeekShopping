using Duende.IdentityServer.Services;
using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Initializer;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using GeekShopping.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================
// 🔧 CONFIGURAÇÃO DO BANCO DE DADOS
// ============================
// Configura o DbContext com MySQL para armazenar usuários do Identity.
var connection = builder.Configuration.GetConnectionString("MySQLConnection");

builder.Services.AddDbContext<MySQLContext>(options =>
    options.UseMySql(
        connection,
        new MySqlServerVersion(new Version(9, 4, 0)) // Ajuste conforme sua versão do MySQL
    )
);

// ============================
// 🔐 CONFIGURAÇÃO DO ASP.NET IDENTITY
// ============================
// Configura o sistema de autenticação/autorização de usuários.
// ApplicationUser é a classe customizada que herda de IdentityUser.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Configurações de exigências de senha
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;

        // Configurações de bloqueio de conta
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 5;

        // Configurações de confirmação de email
        options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";

    })
    .AddEntityFrameworkStores<MySQLContext>()  // Usa o MySQLContext para persistir usuários
    .AddDefaultTokenProviders();               // Adiciona providers para tokens (reset senha, confirmação email, etc.)

// ============================
// 🎫 CONFIGURAÇÃO DO DUENDE IDENTITYSERVER
// ============================
// Configura o servidor de identidade OAuth2/OpenID Connect.
builder.Services.AddIdentityServer(options =>
    {
        // Eventos para logging/debugging
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;

        // Permite acesso via HTTP em desenvolvimento (HTTPS em produção!)
        options.EmitStaticAudienceClaim = true;
    })
    // Recursos de identidade (openid, profile, email)
    .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
    // Escopos de API (geek_shopping, read, write, delete)
    .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
    // Clientes autorizados (geekshopping_web, client)
    .AddInMemoryClients(IdentityConfiguration.Clients)
    // Integração com ASP.NET Identity (usa os usuários do Identity)
    .AddAspNetIdentity<ApplicationUser>()
    // Chave de desenvolvimento (em produção, use certificado real!)
    .AddDeveloperSigningCredential();

// ============================
// 🧩 SERVIÇOS ADICIONAIS
// ============================
// Registra o inicializador de dados (seed de usuários)
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// Serviço customizado para adicionar claims extras ao token (opcional)
builder.Services.AddScoped<IProfileService, ProfileService>();

// Adiciona suporte a Controllers + Views (Razor)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ============================
// 🌱 SEED DE DADOS (INICIALIZAÇÃO)
// ============================
// Executa a criação de roles e usuário admin inicial.
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize();
}

// ============================
// 🛠️ PIPELINE DE REQUISIÇÃO HTTP
// ============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔑 Middleware do IdentityServer (DEVE vir antes do UseAuthorization)
app.UseIdentityServer();

// 🔐 Middleware de autorização
app.UseAuthorization();

// 🗺️ Mapeia as rotas dos controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapeia as Razor Pages (usadas pelo IdentityServer para login, logout, consent, etc.)
app.MapRazorPages();

// 🚀 Inicia a aplicação
app.Run();

using GeekShopping.Web.Services;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ============================
// ?? CONFIGURAÇÃO DE AUTENTICAÇÃO (OpenID Connect)
// ============================
// Configura o frontend para autenticar usuários via IdentityServer.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    // URL do IdentityServer
    options.Authority = builder.Configuration["IdentityServer:Authority"];
    
    // Credenciais do cliente (definidas no IdentityConfiguration.cs)
    options.ClientId = "geekshopping_web";
    options.ClientSecret = "geekshopping_web_secret";
    
    // Tipo de resposta (Authorization Code Flow)
    options.ResponseType = "code";
    
    // Salva os tokens (access_token, refresh_token) nos cookies
    options.SaveTokens = true;
    
    // Obtém claims adicionais do UserInfo endpoint
    options.GetClaimsFromUserInfoEndpoint = true;
    
    // Scopes solicitados ao IdentityServer
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("geek_shopping");
});

// Adiciona serviços ao contêiner
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Serializa enums como números (int) para compatibilidade com a API
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true));
    });

// Configura o ProductService com HttpClient
builder.Services.AddHttpClient<IProductService, ProductService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]);
});

// Adiciona suporte a sessões
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configura o pipeline de requisição HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ordem importante: Authentication ANTES de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
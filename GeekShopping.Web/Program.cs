using GeekShopping.Web.Services;
using GeekShopping.Web.Services.IServices;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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

// Se estiver usando sessões ou autenticação, adicione:
builder.Services.AddSession();
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

app.UseAuthentication(); // Se estiver usando autenticação
app.UseAuthorization();

app.UseSession(); // Se estiver usando sessões

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
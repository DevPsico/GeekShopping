using Duende.IdentityModel;
using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GeekShopping.IdentityServer.Initializer
{
    /// <summary>
    /// Inicializador do banco de dados.
    /// Cria roles (Admin, Client) e um usuário admin inicial.
    /// </summary>
    public class DbInitializer : IDbInitializer
    {
        private readonly MySQLContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(
            MySQLContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            // Verifica se a role Admin já existe (evita duplicação)
            if (_roleManager.FindByNameAsync(IdentityConfiguration.Admin).Result != null)
                return;

            // ============================
            // 🎭 CRIAÇÃO DAS ROLES
            // ============================
            _roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

            // ============================
            // 👤 CRIAÇÃO DO USUÁRIO ADMIN
            // ============================
            ApplicationUser admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@geekshopping.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (11) 99999-9999",
                FirstName = "Admin",
                LastName = "GeekShopping"
            };

            // Cria o usuário com senha
            _userManager.CreateAsync(admin, "Admin@123").GetAwaiter().GetResult();

            // Associa o usuário à role Admin
            _userManager.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();

            // Adiciona claims ao usuário (serão incluídas no token JWT)
            var adminClaims = _userManager.AddClaimsAsync(admin, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
                new Claim(JwtClaimTypes.GivenName, admin.FirstName),
                new Claim(JwtClaimTypes.FamilyName, admin.LastName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin)
            }).Result;

            // ============================
            // 👤 CRIAÇÃO DO USUÁRIO CLIENT (opcional)
            // ============================
            ApplicationUser client = new ApplicationUser
            {
                UserName = "client",
                Email = "client@geekshopping.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (11) 88888-8888",
                FirstName = "Cliente",
                LastName = "Teste"
            };

            _userManager.CreateAsync(client, "Client@123").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(client, IdentityConfiguration.Client).GetAwaiter().GetResult();

            var clientClaims = _userManager.AddClaimsAsync(client, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
                new Claim(JwtClaimTypes.GivenName, client.FirstName),
                new Claim(JwtClaimTypes.FamilyName, client.LastName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client)
            }).Result;
        }
    }
}
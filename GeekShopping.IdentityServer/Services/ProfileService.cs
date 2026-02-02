using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using GeekShopping.IdentityServer.Model;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GeekShopping.IdentityServer.Services
{
    /// <summary>
    /// Serviço customizado para adicionar claims extras ao token.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;

        public ProfileService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // Obtém o ID do usuário do token
            string id = context.Subject.GetSubjectId();
            
            // Busca o usuário no banco
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            
            // Gera as claims do usuário
            ClaimsPrincipal claimsPrincipal = await _claimsFactory.CreateAsync(user);

            // Adiciona as claims ao contexto (serão incluídas no token)
            List<Claim> claims = claimsPrincipal.Claims.ToList();
            
            // Filtra apenas as claims solicitadas pelo cliente
            context.IssuedClaims = claims
                .Where(c => context.RequestedClaimTypes.Contains(c.Type))
                .ToList();

            // Adiciona claims extras manualmente se necessário
            context.IssuedClaims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
            context.IssuedClaims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));

            // Adiciona as roles do usuário como claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                context.IssuedClaims.Add(new Claim(JwtClaimTypes.Role, role));
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string id = context.Subject.GetSubjectId();
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            context.IsActive = user != null;
        }
    }
}
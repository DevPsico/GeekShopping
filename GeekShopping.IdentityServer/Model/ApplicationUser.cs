using Microsoft.AspNetCore.Identity;

namespace GeekShopping.IdentityServer.Model
{
    /// <summary>
    /// Classe customizada de usuário que estende o IdentityUser.
    /// Adiciona campos extras como FirstName e LastName.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Propriedades devem ser PUBLIC para funcionar com o Identity/EF Core
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}

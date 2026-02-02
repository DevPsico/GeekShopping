using Duende.IdentityServer.Models;

namespace GeekShopping.IdentityServer.Configuration
{
    /// <summary>
    /// Configuração central do IdentityServer.
    /// Define recursos de identidade, escopos de API e clientes autorizados.
    /// </summary>
    public static class IdentityConfiguration
    {
        // ============================
        // 🔐 ROLES (Papéis de usuário)
        // ============================
        // Constantes usadas para definir os papéis/perfis de acesso no sistema.
        // Serão usadas no registro de usuários e nas claims do token.

        public const string Admin = "Admin";   // Papel de administrador (acesso total)
        public const string Client = "Client"; // Papel de cliente comum (acesso limitado)

        // ============================
        // 🆔 IDENTITY RESOURCES
        // ============================
        // Definem quais dados do usuário podem ser solicitados pelos clientes.
        // São os "scopes" de identidade padrão do OpenID Connect.
        // Quando um cliente pede "openid", "profile" ou "email", 
        // o IdentityServer sabe quais claims incluir no token.

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                // openid: Obrigatório para OpenID Connect. 
                // Retorna o "sub" (subject/id do usuário) no token.
                new IdentityResources.OpenId(),

                // email: Permite que o cliente acesse o e-mail do usuário.
                // Claims: email, email_verified
                new IdentityResources.Email(),

                // profile: Permite acesso aos dados básicos do perfil.
                // Claims: name, family_name, given_name, middle_name, nickname,
                //         preferred_username, profile, picture, website, gender,
                //         birthdate, zoneinfo, locale, updated_at
                new IdentityResources.Profile()
            };

        // ============================
        // 🎯 API SCOPES
        // ============================
        // Definem quais APIs/recursos o cliente pode acessar.
        // Cada scope representa uma permissão ou conjunto de permissões.
        // O cliente solicita esses scopes ao pedir um token,
        // e a API valida se o token contém o scope necessário.

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                // Scope principal da aplicação GeekShopping.
                // Dá acesso geral às APIs do sistema.
                new ApiScope("geek_shopping", "GeekShopping Server"),

                // Scopes granulares para controle fino de permissões:
                new ApiScope(name: "read", displayName: "Read data."),     // Permite leitura de dados
                new ApiScope(name: "write", displayName: "Write data."),   // Permite escrita/criação de dados
                new ApiScope(name: "delete", displayName: "Delete data."), // Permite exclusão de dados
            };

        // ============================
        // 🖥️ CLIENTS (Aplicações autorizadas)
        // ============================
        // Define quais aplicações podem solicitar tokens ao IdentityServer.
        // Cada client tem um ID, secret (senha), tipo de autenticação permitido
        // e quais scopes pode acessar.

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // ------------------------------
                // Cliente 1: Machine-to-Machine (M2M)
                // ------------------------------
                // Usado para comunicação entre serviços (backend-to-backend).
                // Não envolve usuário, apenas o próprio serviço se autenticando.
                new Client
                {
                    ClientId = "client",
                    
                    // Secret usado para autenticar o cliente.
                    // Em produção, use um secret forte e armazene de forma segura!
                    ClientSecrets = { new Secret("my_super_secret".Sha256()) },
                    
                    // ClientCredentials: Fluxo OAuth2 para serviços/APIs.
                    // O cliente se autentica com ID + Secret, sem usuário envolvido.
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    
                    // Scopes que este cliente pode solicitar.
                    AllowedScopes = { "read", "write", "profile" }
                },

                // ------------------------------
                // Cliente 2: GeekShopping.Web (Frontend)
                // ------------------------------
                // Usado pela aplicação web para autenticar usuários.
                // Usa o fluxo Authorization Code com PKCE (mais seguro).
                new Client
                {
                    ClientId = "geekshopping_web",
                    ClientName = "GeekShopping Web App",
                    
                    // Secret do cliente web.
                    ClientSecrets = { new Secret("geekshopping_web_secret".Sha256()) },
                    
                    // Code: Fluxo Authorization Code (recomendado para apps web).
                    // Usuário faz login, recebe um code, troca por token.
                    AllowedGrantTypes = GrantTypes.Code,
                    
                    // URLs de redirecionamento após login/logout.
                    // Ajuste as portas conforme seu launchSettings.json!
                    RedirectUris = { "https://localhost:5005/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5005/signout-callback-oidc" },
                    
                    // Scopes que o frontend pode solicitar.
                    AllowedScopes =
                    {
                        // Scopes de identidade (dados do usuário):
                        "openid",
                        "profile",
                        "email",
                        
                        // Scopes de API (acesso às APIs):
                        "geek_shopping"
                    },
                    
                    // Permite que o cliente peça refresh tokens.
                    // Útil para manter o usuário logado por mais tempo.
                    AllowOfflineAccess = true,
                    
                    // Tempo de vida do token de acesso (em segundos).
                    // 3000 = 50 minutos. Ajuste conforme necessidade.
                    AccessTokenLifetime = 3000
                }
            };
    }
}
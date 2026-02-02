namespace GeekShopping.IdentityServer.Initializer
{
    /// <summary>
    /// Interface para inicialização do banco de dados.
    /// Responsável por criar roles e usuário admin inicial.
    /// </summary>
    public interface IDbInitializer
    {
        void Initialize();
    }
}
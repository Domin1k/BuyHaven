namespace BuyHaven.Identity.Services.Identity
{
    using BuyHaven.Identity.Data;

    public interface ITokenGeneratorService
    {
        string GenerateToken(User user, IEnumerable<string>? roles = null);
    }
}

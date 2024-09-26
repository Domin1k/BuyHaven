namespace BuyHaven.Common.Services.Identity
{
    public interface ICurrentTokenService
    {
        string Get();

        void Set(string token);
    }

    public class CurrentTokenService : ICurrentTokenService
    {
        private string _currentToken;

        public string Get() => _currentToken;

        public void Set(string token) => _currentToken = token;
    }
}

namespace BuyHaven.Identity.Services.Identity.Models
{
    public class UserOutputModel
    {
        public UserOutputModel(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}

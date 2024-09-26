namespace BuyHaven.Common.Infrastructure
{
    using BuyHaven.Common.Services.Identity;

    public class JwtHeaderAuthenticationMiddleware : IMiddleware
    {
        private readonly ICurrentTokenService _currentToken;

        public JwtHeaderAuthenticationMiddleware(ICurrentTokenService currentToken)
        {
            _currentToken = currentToken;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Headers[InfrastructureConstants.AuthConstants.AuthorizationHeaderName].ToString();

            if (!string.IsNullOrWhiteSpace(token))
            {
                _currentToken.Set(token.Split().Last());
            }

            await next.Invoke(context);
        }
    }
}

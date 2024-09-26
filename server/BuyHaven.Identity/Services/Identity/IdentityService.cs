namespace BuyHaven.Identity.Services.Identity
{
    using BuyHaven.Common.Services;
    using BuyHaven.Identity.Data;
    using BuyHaven.Identity.Services.Identity.Models;
    using Microsoft.AspNetCore.Identity;

    public class IdentityService : IIdentityService
    {
        private const string InvalidErrorMessage = "Invalid credentials.";

        private readonly UserManager<User> _userManager;
        private readonly ITokenGeneratorService _jwtTokenGenerator;

        public IdentityService(
            UserManager<User> userManager,
            ITokenGeneratorService jwtTokenGenerator)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<User>> Register(UserInputModel userInput)
        {
            var user = new User
            {
                Email = userInput.Email,
                UserName = userInput.Email
            };

            var identityResult = await _userManager.CreateAsync(user, userInput.Password);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result<User>.SuccessWith(user)
                : Result<User>.Failure(errors);
        }

        public async Task<Result<UserOutputModel>> Login(UserInputModel userInput)
        {
            var user = await _userManager.FindByEmailAsync(userInput.Email);
            if (user == null)
            {
                return Result<UserOutputModel>.Failure(new[] { InvalidErrorMessage });
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, userInput.Password);
            if (!passwordValid)
            {
                return Result<UserOutputModel>.Failure(new[] { InvalidErrorMessage });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            return Result<UserOutputModel>.SuccessWith(new UserOutputModel(token));
        }

        public async Task<Result> ChangePassword(string userId, ChangePasswordInputModel changePasswordInput)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return InvalidErrorMessage;
            }

            var identityResult = await _userManager.ChangePasswordAsync(
                user,
                changePasswordInput.CurrentPassword,
                changePasswordInput.NewPassword);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);
        }
    }
}

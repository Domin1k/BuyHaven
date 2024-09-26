using BuyHaven.Common.Services;
using BuyHaven.Identity.Data;
using BuyHaven.Identity.Services.Identity.Models;

namespace BuyHaven.Identity.Services.Identity
{
    public interface IIdentityService
    {
        Task<Result<User>> Register(UserInputModel userInput);

        Task<Result<UserOutputModel>> Login(UserInputModel userInput);

        Task<Result> ChangePassword(string userId, ChangePasswordInputModel changePasswordInput);
    }
}

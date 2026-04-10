using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Models
{
    public sealed class LoginResult
    {
        public bool Success { get; private set; }

        public bool RequiresPasswordChange { get; private set; }

        public string ErrorMessage { get; private set; }

        public UserIdentity Identity { get; private set; }

        public DatabaseProfile DatabaseProfile { get; private set; }

        public static LoginResult Successful(UserIdentity identity, DatabaseProfile profile, bool requiresPasswordChange)
        {
            return new LoginResult
            {
                Success = !requiresPasswordChange,
                RequiresPasswordChange = requiresPasswordChange,
                Identity = identity,
                DatabaseProfile = profile,
            };
        }

        public static LoginResult Fail(string message)
        {
            return new LoginResult { ErrorMessage = message };
        }
    }
}

using System;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class AuthenticationController
    {
        private readonly AuthenticationService _authenticationService;

        public AuthenticationController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public LoginResult Login(AppConfiguration configuration, string profileId, string userName, string password)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return _authenticationService.Authenticate(configuration, profileId, userName, password);
        }

        public PasswordChangeResult ChangePassword(AppConfiguration configuration, DatabaseProfile profile, string userName, string newPassword)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return _authenticationService.ChangePassword(configuration, profile, userName, newPassword);
        }
    }
}

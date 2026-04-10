using System.Collections.Generic;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IAuthenticationGateway
    {
        UserAccount FindUser(DatabaseProfile profile, string userName, ConnectionResilienceSettings settings);

        IReadOnlyCollection<string> LoadPermissionKeys(DatabaseProfile profile, string userType, ConnectionResilienceSettings settings);

        void UpdatePassword(DatabaseProfile profile, string userName, string passwordHash, string salt, ConnectionResilienceSettings settings);
    }
}

using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IAdministrationGateway
    {
        IReadOnlyCollection<UserSummary> LoadUsers(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<string> LoadUserTypeNames(DatabaseProfile profile, ConnectionResilienceSettings settings);

        void CreateUser(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveUserRequest request);

        void UpdateUser(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveUserRequest request);

        void InactivateUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName);

        IReadOnlyCollection<UserTypeSummary> LoadUserTypes(DatabaseProfile profile, ConnectionResilienceSettings settings, int totalPermissionCount);

        UserTypeDetail LoadUserType(DatabaseProfile profile, ConnectionResilienceSettings settings, string typeName);

        int CountActiveUsersForType(DatabaseProfile profile, ConnectionResilienceSettings settings, string typeName);

        UserTypeSaveResult SaveUserType(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveUserTypeRequest request, string permissionsSerialized);

        void DeleteUserType(DatabaseProfile profile, ConnectionResilienceSettings settings, string typeName);

        IReadOnlyCollection<UserSummary> LoadUsersByType(DatabaseProfile profile, ConnectionResilienceSettings settings, string typeName);

        IReadOnlyCollection<AccessRequest> LoadPendingAccessRequests(DatabaseProfile profile, ConnectionResilienceSettings settings);

        AccessRequest LoadAccessRequest(DatabaseProfile profile, ConnectionResilienceSettings settings, string requestId);

        void ApproveAccessRequest(DatabaseProfile profile, ConnectionResilienceSettings settings, string requestId, string actorUserName, string respondedAt);

        void CancelAccessRequest(DatabaseProfile profile, ConnectionResilienceSettings settings, string requestId, string actorUserName, string respondedAt);
    }
}

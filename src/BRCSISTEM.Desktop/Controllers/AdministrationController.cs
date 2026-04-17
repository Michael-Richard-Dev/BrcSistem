using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class AdministrationController
    {
        private readonly AdministrationService _administrationService;

        public AdministrationController(AdministrationService administrationService)
        {
            _administrationService = administrationService;
            //teste
        }

        public PermissionCategory[] LoadPermissionCategories()
        {
            return _administrationService.GetPermissionCategories();
        }

        public UserSummary[] LoadUsers(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _administrationService.LoadUsers(configuration, profile);
        }

        public string[] LoadUserTypeNames(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _administrationService.LoadUserTypeNames(configuration, profile);
        }

        public void CreateUser(AppConfiguration configuration, DatabaseProfile profile, SaveUserRequest request)
        {
            _administrationService.CreateUser(configuration, profile, request);
        }

        public void UpdateUser(AppConfiguration configuration, DatabaseProfile profile, SaveUserRequest request)
        {
            _administrationService.UpdateUser(configuration, profile, request);
        }

        public void InactivateUser(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string userName)
        {
            _administrationService.InactivateUser(configuration, profile, actorUserName, userName);
        }

        public UserTypeSummary[] LoadUserTypes(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _administrationService.LoadUserTypes(configuration, profile);
        }

        public UserTypeDetail LoadUserType(AppConfiguration configuration, DatabaseProfile profile, string typeName)
        {
            return _administrationService.LoadUserType(configuration, profile, typeName);
        }

        public int CountActiveUsersForType(AppConfiguration configuration, DatabaseProfile profile, string typeName)
        {
            return _administrationService.CountActiveUsersForType(configuration, profile, typeName);
        }

        public UserTypeSaveResult SaveUserType(AppConfiguration configuration, DatabaseProfile profile, SaveUserTypeRequest request)
        {
            return _administrationService.SaveUserType(configuration, profile, request);
        }

        public void DeleteUserType(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string typeName)
        {
            _administrationService.DeleteUserType(configuration, profile, actorUserName, typeName);
        }

        public UserSummary[] LoadUsersByType(AppConfiguration configuration, DatabaseProfile profile, string typeName)
        {
            return _administrationService.LoadUsersByType(configuration, profile, typeName);
        }

        public AccessRequest[] LoadPendingAccessRequests(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _administrationService.LoadPendingAccessRequests(configuration, profile);
        }

        public AccessRequest LoadAccessRequest(AppConfiguration configuration, DatabaseProfile profile, string requestId)
        {
            return _administrationService.LoadAccessRequest(configuration, profile, requestId);
        }

        public void LogAccessManagementOpened(AppConfiguration configuration, DatabaseProfile profile, string actorUserName)
        {
            _administrationService.LogAccessManagementOpened(configuration, profile, actorUserName);
        }

        public void ApproveAccessRequest(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string requestId)
        {
            _administrationService.ApproveAccessRequest(configuration, profile, actorUserName, requestId);
        }

        public void CancelAccessRequest(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string requestId)
        {
            _administrationService.CancelAccessRequest(configuration, profile, actorUserName, requestId);
        }
    }
}

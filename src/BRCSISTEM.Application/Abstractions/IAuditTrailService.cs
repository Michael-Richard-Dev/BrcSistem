using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IAuditTrailService
    {
        void Write(DatabaseProfile profile, string userName, string action, string details, ConnectionResilienceSettings settings);
    }
}

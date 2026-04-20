using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IMainSidebarGateway
    {
        MainSidebarSnapshot LoadSnapshot(DatabaseProfile profile, ConnectionResilienceSettings settings);
    }
}

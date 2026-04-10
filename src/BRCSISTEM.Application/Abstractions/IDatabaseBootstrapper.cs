using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IDatabaseBootstrapper
    {
        void EnsureCoreSchema(DatabaseProfile profile, FirstUserSeed firstUser, ConnectionResilienceSettings settings);
    }
}

using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IAppConfigurationStore
    {
        AppConfiguration Load();

        void Save(AppConfiguration configuration);
    }
}

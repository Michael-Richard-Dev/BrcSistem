using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface ISessionStateStore
    {
        SessionState Load(string userName);

        void Save(SessionState state);

        void Clear(string userName);
    }
}

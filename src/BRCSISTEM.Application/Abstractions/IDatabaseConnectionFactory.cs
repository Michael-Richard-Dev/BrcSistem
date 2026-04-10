using System.Data.Common;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IDatabaseConnectionFactory
    {
        DbConnection Open(DatabaseProfile profile, ConnectionResilienceSettings settings);

        ConnectionTestResult TestConnection(DatabaseProfile profile, ConnectionResilienceSettings settings);
    }
}

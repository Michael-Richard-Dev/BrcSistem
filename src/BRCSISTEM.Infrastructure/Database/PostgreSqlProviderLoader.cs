using System;
using System.Data.Common;
using System.Reflection;

namespace BRCSISTEM.Infrastructure.Database
{
    internal static class PostgreSqlProviderLoader
    {
        public static DbProviderFactory LoadFactory()
        {
            var factoryType = Type.GetType("Npgsql.NpgsqlFactory, Npgsql", false);
            if (factoryType != null)
            {
                var field = factoryType.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
                if (field != null)
                {
                    var instance = field.GetValue(null) as DbProviderFactory;
                    if (instance != null)
                    {
                        return instance;
                    }
                }
            }

            try
            {
                return DbProviderFactories.GetFactory("Npgsql");
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    "Nao foi possivel localizar o provider Npgsql. Adicione a biblioteca Npgsql ao projeto antes de conectar no PostgreSQL.",
                    exception);
            }
        }
    }
}

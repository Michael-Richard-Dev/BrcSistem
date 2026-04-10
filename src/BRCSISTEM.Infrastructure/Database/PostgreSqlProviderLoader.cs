using System;
using System.IO;
using System.Data.Common;
using System.Reflection;

namespace BRCSISTEM.Infrastructure.Database
{
    internal static class PostgreSqlProviderLoader
    {
        public static DbProviderFactory LoadFactory()
        {
            var factoryType = Type.GetType("Npgsql.NpgsqlFactory, Npgsql", false);
            if (factoryType == null)
            {
                factoryType = TryLoadFactoryFromApplicationDirectory();
            }

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
                    "Nao foi possivel localizar o provider Npgsql. Instale o pacote NuGet Npgsql ou garanta que o arquivo Npgsql.dll esteja ao lado do executavel.",
                    exception);
            }
        }

        private static Type TryLoadFactoryFromApplicationDirectory()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                return null;
            }

            var assemblyPath = Path.Combine(baseDirectory, "Npgsql.dll");
            if (!File.Exists(assemblyPath))
            {
                return null;
            }

            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.GetType("Npgsql.NpgsqlFactory", false);
            }
            catch
            {
                return null;
            }
        }
    }
}

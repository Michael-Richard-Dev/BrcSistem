namespace BRCSISTEM.Domain.Models
{
    public sealed class ConnectionResilienceSettings
    {
        public int ConnectTimeoutSeconds { get; set; } = 5;

        public int Retries { get; set; } = 3;

        public double RetryWaitSeconds { get; set; } = 1.0;

        public double RetryBackoff { get; set; } = 1.5;

        public double MaxRetryWaitSeconds { get; set; } = 5.0;

        public double ReconnectWaitSeconds { get; set; } = 2.0;

        public int KeepAlives { get; set; } = 1;

        public int KeepAlivesIdle { get; set; } = 30;

        public int KeepAlivesInterval { get; set; } = 10;

        public int KeepAlivesCount { get; set; } = 5;

        public string ApplicationName { get; set; } = "BRCSISTEM";

        public static ConnectionResilienceSettings CreateDefault()
        {
            return new ConnectionResilienceSettings();
        }
    }
}

namespace BRCSISTEM.Application.Models
{
    public sealed class ConnectionTestResult
    {
        public bool Success { get; private set; }

        public string Message { get; private set; }

        public static ConnectionTestResult Ok(string message)
        {
            return new ConnectionTestResult { Success = true, Message = message };
        }

        public static ConnectionTestResult Fail(string message)
        {
            return new ConnectionTestResult { Success = false, Message = message };
        }
    }
}

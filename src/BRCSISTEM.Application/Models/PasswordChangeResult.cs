namespace BRCSISTEM.Application.Models
{
    public sealed class PasswordChangeResult
    {
        public bool Success { get; private set; }

        public string Message { get; private set; }

        public static PasswordChangeResult Ok(string message)
        {
            return new PasswordChangeResult { Success = true, Message = message };
        }

        public static PasswordChangeResult Fail(string message)
        {
            return new PasswordChangeResult { Success = false, Message = message };
        }
    }
}

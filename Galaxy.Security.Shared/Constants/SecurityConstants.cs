
namespace Galaxy.Security.Shared.Constants
{
    public static class SecurityConstants
    {
        public const int MinLengthPassword = 6;
        public const int MaxFailedAccessAttempts = 5;
        public const int LockoutTimeSpan = 15; // in minutes
    }
}

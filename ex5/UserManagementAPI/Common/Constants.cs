namespace UserManagementAPI.Common
{
    /// <summary>
    /// Application-wide constants for configuration and magic strings
    /// </summary>
    public static class Constants
    {
        public const string JsonPlaceholderApiUrl = "https://jsonplaceholder.typicode.com/users";
        public const int HttpRequestTimeoutSeconds = 10;
        public const int MaxRetryAttempts = 3;
        public const string CorsPolicy = "AllowAngularApp";
        public const string AngularAppUrl = "http://localhost:4200";
    }
}

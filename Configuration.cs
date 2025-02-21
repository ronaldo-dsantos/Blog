namespace Blog
{
    public static class Configuration
    {
        public static string JwtKey = "NiObng1EJnw46scPHaNeZA1p5kjxsZN01LQdJBYTTFo=";
        public static string ApiKeyName = "api_key";
        public static string ApiKey = "curso_api_xEuey0jQkM0WNCmshPrkmA==";
        public static SmtpConfiguration Smtp = new();

        public class SmtpConfiguration
        {
            public string? Host { get; set; }
            public int Port { get; set; } = 25;
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }
    }
}

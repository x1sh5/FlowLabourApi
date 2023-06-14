namespace FlowLabourApi.Options
{
    public class JWTOptions
    {
        public static string Issuer { get; set; } = "default";

        public static string SecretKey { get; set; } = "We are our choices.";

        public static string Audience { get; set; } = "/authuser";
    }
}

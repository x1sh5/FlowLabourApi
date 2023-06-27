using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace FlowLabourApi.Options
{
    public class JwtOptions
    {
        public const string Name = "Jwt";
        public readonly static Encoding DefaultEncoding = Encoding.UTF8;
        public readonly static double DefaultExpiresMinutes = 30d;

        /// <summary>
        /// 
        /// </summary>
        public string Audience { get; set; } = "Audience";

        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; } = "Issuer";

        /// <summary>
        /// 
        /// </summary>
        public string SecretKeyString { get; set; } = "We are our choices.";

        /// <summary>
        /// 
        /// </summary>
        public double ExpiresMinutes { get; set; } = DefaultExpiresMinutes;

        public Encoding Encoding { get; set; } = DefaultEncoding;

        public SymmetricSecurityKey SecurityKey => new (DefaultEncoding.GetBytes(SecretKeyString));
    }
    //public class JWTOptions
    //{
    //    public static string Issuer { get; set; } = "default";

    //    public static string SecretKey { get; set; } = "We are our choices.";

    //    public static string Audience { get; set; } = "/authuser";
    //}
}

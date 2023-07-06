using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FlowLabourApi.Options
{
    public class JwtOptions
    {
        public const string Name = "Jwt";
        public readonly static Encoding DefaultEncoding = Encoding.UTF8;
        public readonly static TimeSpan DefaultExpires = TimeSpan.FromHours(12);

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
        public TimeSpan AccessTokenExpires { get; set; } = DefaultExpires;

        public Encoding Encoding { get; set; } = DefaultEncoding;

        public SymmetricSecurityKey SecurityKey => new(DefaultEncoding.GetBytes(SecretKeyString));

        public TimeSpan RefreshTokenExpires { get; set; } = TimeSpan.FromDays(7);

        internal SecurityToken SignatureValidator(string token, TokenValidationParameters validationParameters)
        {
            throw new NotImplementedException();
        }
    }
    //public class JWTOptions
    //{
    //    public static string Issuer { get; set; } = "default";

    //    public static string SecretKey { get; set; } = "We are our choices.";

    //    public static string Audience { get; set; } = "/authuser";
    //}
}

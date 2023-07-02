using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowLabourApi.Models
{
    public class UserToken : IdentityUserToken<int>
    {
        /// <summary>
        /// 
        /// </summary>
        public override string LoginProvider { get; set; } = "miniapp";

        /// <summary>
        /// 
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 过去时间
        /// </summary>
        public DateTime Expires { get; internal set; }

        public DateTime Modify { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public override int UserId { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;

        public bool IsActive => !IsExpired;

        public virtual AuthUser User { get; set; }
    }

    //[Table("usertoken")]
    //public class RefreshToken
    //{
    //    public int Id { get; set; }
    //    public string Token { get; set; }
    //    public DateTime Expires { get; set; }
    //    public DateTime Created { get; set; }
    //    public string CreatedByIp { get; set; }
    //    public DateTime? Revoked { get; set; }
    //    public string RevokedByIp { get; set; }
    //    public string ReplacedByToken { get; set; }
    //    public string ReasonRevoked { get; set; }
    //    public bool IsExpired => DateTime.UtcNow >= Expires;
    //    public bool IsRevoked => Revoked != null;
    //    public bool IsActive => !IsRevoked && !IsExpired;
    //}
}

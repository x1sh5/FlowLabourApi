namespace FlowLabourApi.Models
{
    /// <summary>
    /// 用户-身份信息表, 对应数据库表: useridentity
    /// </summary>
    public partial class UserIdentity
    {
        /// <summary>
        /// 对应auth_user表中的id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 对应identityinfo表中的id。
        /// </summary>
        public int IdentityId { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public virtual AuthUser User { get; set; } = null!;

        /// <summary>
        /// 真实身份信息
        /// </summary>
        public virtual IdentityInfo Identity { get; set; } = null!;
    }
}

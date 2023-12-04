using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowLabourApi.Models
{
    /// <summary>
    /// 邮件验证码
    /// </summary>
    [Table("emailConfirm")]
    public class EmailCode
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 收件人用户id
        /// </summary>
        [Column("authid")]
        public int AuthId { get; set; }
        /// <summary>
        /// 收件人邮箱
        /// </summary>
        [Column("email")]
        [MaxLength(45)]
        public string Email { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Column("code")]
        [MaxLength(10)]
        public string Code { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        [Column("expires")]
        public DateTime ExpireTime { get; set; } = DateTime.Now.AddMinutes(5);

        public bool IsExpired => DateTime.Now > ExpireTime;
    }
}

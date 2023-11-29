using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowLabourApi.Models
{
    [Table("PrivateImages")]
    public class PrivateImage
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [Column("url")]
        [MaxLength(255)]
        public string Url { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        [Column("md5")]
        [MaxLength(64)]
        public string Md5 { get; set; } = null!;
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowLabourApi.Models
{
    /// <summary>
    /// 搜索
    /// </summary>
    [Table("search")]
    public class Search
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 词
        /// </summary>
        [Column("word")]
        [MaxLength(25)]
        public string Word { get; set; }

        /// <summary>
        /// 初次日期
        /// </summary>
        [Column("date")]
        public DateTime Date { get; set; }

        [Column("userid")]
        public int UserId { get; set; }
    }
}

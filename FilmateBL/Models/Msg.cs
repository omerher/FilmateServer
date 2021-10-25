using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FilmateBL.Models
{
    [Table("Msg")]
    public partial class Msg
    {
        [Key]
        [Column("MsgID")]
        public int MsgId { get; set; }
        [Column("AccountID")]
        public int AccountId { get; set; }
        [Column("ChatID")]
        public int ChatId { get; set; }
        [Required]
        [StringLength(255)]
        public string Content { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime SentDate { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty("Msgs")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(ChatId))]
        [InverseProperty("Msgs")]
        public virtual Chat Chat { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FilmateBL.Models
{
    public partial class ChatMember
    {
        [Key]
        [Column("AccountID")]
        public int AccountId { get; set; }
        [Key]
        [Column("ChatID")]
        public int ChatId { get; set; }
        public bool IsAdmin { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty("ChatMembers")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(ChatId))]
        [InverseProperty("ChatMembers")]
        public virtual Chat Chat { get; set; }
    }
}

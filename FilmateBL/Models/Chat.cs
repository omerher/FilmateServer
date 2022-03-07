using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FilmateBL.Models
{
    [Table("Chat")]
    public partial class Chat
    {
        public Chat()
        {
            ChatMembers = new HashSet<ChatMember>();
            Msgs = new HashSet<Msg>();
        }

        [Key]
        [Column("ChatID")]
        public int ChatId { get; set; }
        [Required]
        [StringLength(255)]
        public string ChatName { get; set; }
        [Required]
        [StringLength(255)]
        public string ChatDescription { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [Column("SuggestedMovieID")]
        public int? SuggestedMovieId { get; set; }
        [Required]
        [StringLength(255)]
        public string Icon { get; set; }
        [Required]
        [StringLength(255)]
        public string InviteCode { get; set; }

        [InverseProperty(nameof(ChatMember.Chat))]
        public virtual ICollection<ChatMember> ChatMembers { get; set; }
        [InverseProperty(nameof(Msg.Chat))]
        public virtual ICollection<Msg> Msgs { get; set; }
    }
}

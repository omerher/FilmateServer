using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FilmateBL.Models
{
    [Keyless]
    public partial class ChatSuggestion
    {
        [Column("ChatID")]
        public int ChatId { get; set; }
        [Column("MovieID")]
        public int MovieId { get; set; }
        [Required]
        public bool? IsActive { get; set; }

        [ForeignKey(nameof(ChatId))]
        public virtual Chat Chat { get; set; }
    }
}

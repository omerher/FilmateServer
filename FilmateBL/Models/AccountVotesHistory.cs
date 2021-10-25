using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FilmateBL.Models
{
    [Table("AccountVotesHistory")]
    public partial class AccountVotesHistory
    {
        [Key]
        [Column("VoteID")]
        public int VoteId { get; set; }
        [Column("SuggestionID")]
        public int SuggestionId { get; set; }
        [Column("AccountID")]
        public int AccountId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime VotedDate { get; set; }
        public bool VoteType { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty("AccountVotesHistories")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(SuggestionId))]
        [InverseProperty("AccountVotesHistories")]
        public virtual Suggestion Suggestion { get; set; }
    }
}

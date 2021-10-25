using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FilmateBL.Models
{
    [Table("Suggestion")]
    public partial class Suggestion
    {
        public Suggestion()
        {
            AccountVotesHistories = new HashSet<AccountVotesHistory>();
        }

        [Key]
        [Column("SuggestionID")]
        public int SuggestionId { get; set; }
        [Column("AccountID")]
        public int AccountId { get; set; }
        [Column("OriginalMovieID")]
        public int OriginalMovieId { get; set; }
        [Column("SuggestionMovieID")]
        public int SuggestionMovieId { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PostDate { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty("Suggestions")]
        public virtual Account Account { get; set; }
        [InverseProperty(nameof(AccountVotesHistory.Suggestion))]
        public virtual ICollection<AccountVotesHistory> AccountVotesHistories { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FilmateBL.Models
{
    public partial class LikedMovie
    {
        [Key]
        [Column("AccountID")]
        public int AccountId { get; set; }
        [Key]
        [Column("MovieID")]
        public int MovieId { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty("LikedMovies")]
        public virtual Account Account { get; set; }
    }
}

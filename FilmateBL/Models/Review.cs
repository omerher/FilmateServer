using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FilmateBL.Models
{
    [Table("Review")]
    public partial class Review
    {
        [Key]
        [Column("ReviewID")]
        public int ReviewId { get; set; }
        [Column("AccountID")]
        public int AccountId { get; set; }
        [Column("MovieID")]
        public int MovieId { get; set; }
        public int Rating { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PostDate { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty("Reviews")]
        public virtual Account Account { get; set; }
    }
}

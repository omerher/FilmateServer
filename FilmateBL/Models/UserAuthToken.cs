﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FilmateBL.Models
{
    [Table("UserAuthToken")]
    public partial class UserAuthToken
    {
        [Column("AccountID")]
        public int AccountId { get; set; }
        [Key]
        [StringLength(255)]
        public string AuthToken { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty("UserAuthTokens")]
        public virtual Account Account { get; set; }
    }
}
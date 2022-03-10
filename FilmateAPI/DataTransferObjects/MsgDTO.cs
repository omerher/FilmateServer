using System;
using FilmateBL.Models;

namespace FilmateAPI.DataTransferObjects
{
    public class MsgDTO
    {
        public int AccountId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }
        public DateTime SentDate { get; set; }
        public string AccountName { get; set; }
        public string ProfilePath { get; set; }
    }
}

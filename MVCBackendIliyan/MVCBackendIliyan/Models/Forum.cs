using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCBackendIliyan.Models
{
    public class Forum
    {
        [Key]
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime Time { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public virtual UserModel User { get; set; }
    }
}
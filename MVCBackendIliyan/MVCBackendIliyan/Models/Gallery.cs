using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace MVCBackendIliyan.Models
{
    public class Gallery
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter Title")]
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public string ImageUrl { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public virtual UserModel User { get; set; }
    }
}
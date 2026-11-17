using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectEntyCrud.Models
{
    public class AdminLogin
    {
        [Key]
        [Column("aid")]
        public int aid { get; set; }

        [StringLength(100)]
        [Column("aname")]
        public string aname { get; set; }

        [Column("password")]
        public string password { get; set; }
    }
}

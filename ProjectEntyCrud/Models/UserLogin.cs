using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectEntyCrud.Models
{
    [Table("UserLogin")]
    public class UserLogin
    {
        [Key]
        public int p_id { get; set; }

        public int EmpId { get; set; }

        public int d_id { get; set; }

        [Required]
        public string pwd { get; set; }

        [ForeignKey(nameof(EmpId))]
        public EmpRecords? Employee { get; set; }

        [ForeignKey(nameof(d_id))]
        public Dept? Department { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProjectEntyCrud.Models
{
    [Table("Dept")]
    public class Dept
    {
        [Key]
        [Column("d_id")]
        public int DID { get; set; }

        [Required]
        [StringLength(30)]
        [Column("d_name")]
        public string DName { get; set; }

        public ICollection<EmpRecords> Employees { get; set; }
            = new List<EmpRecords>();
    }
}

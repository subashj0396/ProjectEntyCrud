using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectEntyCrud.Models
{
    public class EmpRecords
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmpId { get; set; }

        [Required]
        [Range(21, 100)]
        public int Age { get; set; }

        [Required]
        [StringLength(80)]
        public string? EmpName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(50)]
        public string? Ph { get; set; }

        [Required]
        public int d_id { get; set; }

        [ForeignKey("d_id")]
        public Dept? Department { get; set; }

        [NotMapped]
        public string? d_name => Department?.DName;

        public string? Desigination { get; set; }

        [Required]
        public DateTime JoiningDate { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectEntyCrud.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceId { get; set; }

        public int EmpID { get; set; }

        public int d_id { get; set; }
        public string d_name { get; set; }
        public DateTime AttendanceDate { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        [ForeignKey("EmpID")]
        public EmpRecords Employee { get; set; }

        [ForeignKey("d_id")]
        public Dept Department { get; set; }
    }
}

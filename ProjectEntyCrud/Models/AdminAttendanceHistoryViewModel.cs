namespace ProjectEntyCrud.Models
{
    public class AdminAttendanceHistoryViewModel
    {
        public int EmpID { get; set; }

        public string EmpName { get; set; }

        public string Email { get; set; }
        public int d_id { get; set; }
        public string d_name { get; set; }
       

        public DateTime AttendanceDate { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }
    }
}

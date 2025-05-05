using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace HospitalFlowSystem.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        public int DoctorCount { get; set; }
        public int PatientCount { get; set; }
        public int AppointmentCount { get; set; }

        public void OnGet()
        {
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                DoctorCount = GetCount(conn, "SELECT COUNT(*) FROM Doctors");
                PatientCount = GetCount(conn, "SELECT COUNT(*) FROM Patients");
                AppointmentCount = GetCount(conn, "SELECT COUNT(*) FROM Appointments");
            }
        }

        private int GetCount(SqlConnection conn, string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}

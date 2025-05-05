using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace HospitalFlowSystem.Pages.Admin
{
    public class EditClinicModel : PageModel
    {
        [BindProperty] public int ClinicID { get; set; }
        [BindProperty] public string ClinicName { get; set; }
        [BindProperty] public string Location { get; set; }
        [BindProperty] public string ContactEmail { get; set; }
        [BindProperty] public string Phone { get; set; }

        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet(int id)
        {
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Clinics WHERE ClinicID = @ClinicID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClinicID", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ClinicID = (int)reader["ClinicID"];
                            ClinicName = reader["Name"].ToString();
                            Location = reader["Location"].ToString();
                            ContactEmail = reader["ContactEmail"].ToString();
                            Phone = reader["Phone"].ToString();
                        }
                        else
                        {
                            ErrorMessage = "Clinic not found.";
                        }
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(ClinicName) ||
                string.IsNullOrWhiteSpace(Location) ||
                string.IsNullOrWhiteSpace(ContactEmail) ||
                string.IsNullOrWhiteSpace(Phone))
            {
                ErrorMessage = "All fields are required.";
                return Page();
            }

            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string updateQuery = @"
                    UPDATE Clinics
                    SET Name = @Name, Location = @Location, ContactEmail = @ContactEmail, Phone = @Phone
                    WHERE ClinicID = @ClinicID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", ClinicName);
                    cmd.Parameters.AddWithValue("@Location", Location);
                    cmd.Parameters.AddWithValue("@ContactEmail", ContactEmail);
                    cmd.Parameters.AddWithValue("@Phone", Phone);
                    cmd.Parameters.AddWithValue("@ClinicID", ClinicID);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        SuccessMessage = "Clinic updated successfully!";
                        return RedirectToPage("/Admin/ManageClinics");
                    }
                    else
                    {
                        ErrorMessage = "Failed to update clinic.";
                        return Page();
                    }
                }
            }
        }
    }
}

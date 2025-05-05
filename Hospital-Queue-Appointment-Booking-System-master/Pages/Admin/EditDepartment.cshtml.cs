using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace HospitalFlowSystem.Pages.Admin
{
    public class EditDepartmentModel : PageModel
    {
        [BindProperty] public int DepartmentID { get; set; }
        [BindProperty] public string DepartmentName { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public int ClinicID { get; set; }

        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public List<Clinic> Clinics { get; set; } = new();

        public void OnGet(int id)
        {
            LoadClinics();

            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Departments WHERE DepartmentID = @DepartmentID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DepartmentID = (int)reader["DepartmentID"];
                            DepartmentName = reader["Name"].ToString();
                            Description = reader["Description"].ToString();
                            ClinicID = (int)reader["ClinicID"];
                        }
                        else
                        {
                            ErrorMessage = "Department not found.";
                        }
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(DepartmentName) || string.IsNullOrWhiteSpace(Description) || ClinicID == 0)
            {
                ErrorMessage = "All fields are required.";
                LoadClinics();
                return Page();
            }

            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string updateQuery = @"
                    UPDATE Departments
                    SET Name = @Name, Description = @Description, ClinicID = @ClinicID
                    WHERE DepartmentID = @DepartmentID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", DepartmentName);
                    cmd.Parameters.AddWithValue("@Description", Description);
                    cmd.Parameters.AddWithValue("@ClinicID", ClinicID);
                    cmd.Parameters.AddWithValue("@DepartmentID", DepartmentID);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        SuccessMessage = "Department updated successfully!";
                        return RedirectToPage("/Admin/ManageDepartments");
                    }
                    else
                    {
                        ErrorMessage = "Failed to update department.";
                    }
                }
            }

            LoadClinics();
            return Page();
        }

        private void LoadClinics()
        {
            Clinics = new List<Clinic>();
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ClinicID, Name FROM Clinics";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Clinics.Add(new Clinic
                        {
                            ClinicID = (int)reader["ClinicID"],
                            Name = reader["Name"].ToString()
                        });
                    }
                }
            }
        }

        public class Clinic
        {
            public int ClinicID { get; set; }
            public string Name { get; set; }
        }
    }
}

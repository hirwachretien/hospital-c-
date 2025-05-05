using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace HospitalFlowSystem.Pages.Admin
{
    public class ManageDepartmentsModel : PageModel
    {
        // Form Bindings
        [BindProperty] public string DepartmentName { get; set; }
        [BindProperty] public string Description { get; set; }
        [BindProperty] public int ClinicID { get; set; }

        // Feedback Messages
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        // Lists for Display
        public List<Department> Departments { get; set; } = new();
        public List<Clinic> Clinics { get; set; } = new();

        public void OnGet()
        {
            LoadClinics();
            LoadDepartments();
        }

        public void OnPost()
        {
            if (string.IsNullOrWhiteSpace(DepartmentName) || string.IsNullOrWhiteSpace(Description) || ClinicID == 0)
            {
                ErrorMessage = "All fields are required.";
                LoadClinics();
                LoadDepartments();
                return;
            }

            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Departments (Name, Description, ClinicID) VALUES (@Name, @Description, @ClinicID)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", DepartmentName);
                    cmd.Parameters.AddWithValue("@Description", Description);
                    cmd.Parameters.AddWithValue("@ClinicID", ClinicID);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        SuccessMessage = "Department added successfully!";
                    }
                    else
                    {
                        ErrorMessage = "Failed to add department.";
                    }
                }
            }

            LoadClinics();
            LoadDepartments();
        }

        public IActionResult OnPostDelete(int id)
        {
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string deleteQuery = "DELETE FROM Departments WHERE DepartmentID = @DepartmentID";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", id);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        SuccessMessage = "Department deleted successfully.";
                    }
                    else
                    {
                        ErrorMessage = "Failed to delete department.";
                    }
                }
            }

            LoadClinics();
            LoadDepartments();
            return Page();
        }


        private void LoadDepartments()
        {
            Departments.Clear();
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT d.DepartmentID, d.Name, d.Description, c.Name AS ClinicName
                    FROM Departments d
                    JOIN Clinics c ON d.ClinicID = c.ClinicID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Departments.Add(new Department
                        {
                            DepartmentID = (int)reader["DepartmentID"],
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            ClinicName = reader["ClinicName"].ToString()
                        });
                    }
                }
            }
        }

        private void LoadClinics()
        {
            Clinics.Clear();
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

        public class Department
        {
            public int DepartmentID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ClinicName { get; set; }
        }

        public class Clinic
        {
            public int ClinicID { get; set; }
            public string Name { get; set; }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using HospitalFlowSystem.Helpers;
using System.Collections.Generic;

namespace HospitalFlowSystem.Pages.Admin
{
    public class ManageDoctorsModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        [BindProperty] public int DepartmentID { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public List<Doctor> Doctors { get; set; } = new();
        public List<Department> Departments { get; set; } = new();

        public void OnGet()
        {
            LoadDepartments();
            LoadDoctors();
        }

        public void OnPost()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                DepartmentID == 0)
            {
                ErrorMessage = "All fields are required.";
                LoadDepartments();
                LoadDoctors();
                return;
            }

            string hashedPassword = PasswordHasher.Hash(Password);
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", conn);
                checkCmd.Parameters.AddWithValue("@Email", Email);
                int exists = (int)checkCmd.ExecuteScalar();
                if (exists > 0)
                {
                    ErrorMessage = "A user with this email already exists.";
                    LoadDepartments();
                    LoadDoctors();
                    return;
                }

                int userId;
                var insertUserCmd = new SqlCommand(@"
                    INSERT INTO Users (Name, Email, Password, Role)
                    OUTPUT INSERTED.UserID
                    VALUES (@Name, @Email, @Password, 'Doctor')", conn);

                insertUserCmd.Parameters.AddWithValue("@Name", Name);
                insertUserCmd.Parameters.AddWithValue("@Email", Email);
                insertUserCmd.Parameters.AddWithValue("@Password", hashedPassword);
                userId = (int)insertUserCmd.ExecuteScalar();

                var insertDoctorCmd = new SqlCommand("INSERT INTO Doctors (UserID, DepartmentID) VALUES (@UserID, @DepartmentID)", conn);
                insertDoctorCmd.Parameters.AddWithValue("@UserID", userId);
                insertDoctorCmd.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                insertDoctorCmd.ExecuteNonQuery();

                SuccessMessage = "Doctor added successfully!";
            }

            LoadDepartments();
            LoadDoctors();
        }

        public IActionResult OnPostDelete(int id)
        {
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                new SqlCommand("DELETE FROM Doctors WHERE UserID = @UserID", conn)
                    .AddWithValue("@UserID", id)
                    .ExecuteNonQuery();

                var result = new SqlCommand("DELETE FROM Users WHERE UserID = @UserID", conn);
                result.Parameters.AddWithValue("@UserID", id);

                if (result.ExecuteNonQuery() > 0)
                    SuccessMessage = "Doctor deleted successfully.";
                else
                    ErrorMessage = "Failed to delete doctor.";
            }

            LoadDepartments();
            LoadDoctors();
            return Page();
        }

        private void LoadDoctors()
        {
            Doctors.Clear();
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT u.UserID, u.Name, u.Email, d.DepartmentID, dept.Name AS DepartmentName
                    FROM Users u
                    JOIN Doctors d ON u.UserID = d.UserID
                    JOIN Departments dept ON d.DepartmentID = dept.DepartmentID
                    WHERE u.Role = 'Doctor'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Doctors.Add(new Doctor
                        {
                            UserID = (int)reader["UserID"],
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Department = reader["DepartmentName"].ToString()
                        });
                    }
                }
            }
        }

        private void LoadDepartments()
        {
            Departments.Clear();
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT d.DepartmentID, d.Name, c.Name AS ClinicName
                    FROM Departments d
                    JOIN Clinics c ON d.ClinicID = c.ClinicID", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Departments.Add(new Department
                        {
                            DepartmentID = (int)reader["DepartmentID"],
                            Name = reader["Name"].ToString(),
                            ClinicName = reader["ClinicName"].ToString()
                        });
                    }
                }
            }
        }

        public class Doctor
        {
            public int UserID { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Department { get; set; }
        }

        public class Department
        {
            public int DepartmentID { get; set; }
            public string Name { get; set; }
            public string ClinicName { get; set; }
        }
    }

    // Helper extension
    public static class SqlExtensions
    {
        public static SqlCommand AddWithValue(this SqlCommand cmd, string param, object value)
        {
            cmd.Parameters.AddWithValue(param, value);
            return cmd;
        }
    }
}

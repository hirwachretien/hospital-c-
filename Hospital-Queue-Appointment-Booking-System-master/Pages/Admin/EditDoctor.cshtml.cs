using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using HospitalFlowSystem.Helpers;
using System.Collections.Generic;

namespace HospitalFlowSystem.Pages.Admin
{
    public class EditDoctorModel : PageModel
    {
        [BindProperty] public int UserID { get; set; }
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string NewPassword { get; set; }
        [BindProperty] public int DepartmentID { get; set; }

        public List<Department> Departments { get; set; } = new();
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet(int id)
        {
            UserID = id;
            LoadDepartments();

            string connStr = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = @"
                    SELECT u.Name, u.Email, d.DepartmentID
                    FROM Users u
                    JOIN Doctors d ON u.UserID = d.UserID
                    WHERE u.UserID = @UserID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Name = reader["Name"].ToString();
                            Email = reader["Email"].ToString();
                            DepartmentID = (int)reader["DepartmentID"];
                        }
                        else
                        {
                            ErrorMessage = "Doctor not found.";
                        }
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || DepartmentID == 0)
            {
                ErrorMessage = "All fields except password are required.";
                LoadDepartments();
                return Page();
            }

            string connStr = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                var updateUser = new SqlCommand(@"
                    UPDATE Users
                    SET Name = @Name, Email = @Email
                    WHERE UserID = @UserID", conn);

                updateUser.Parameters.AddWithValue("@Name", Name);
                updateUser.Parameters.AddWithValue("@Email", Email);
                updateUser.Parameters.AddWithValue("@UserID", UserID);
                updateUser.ExecuteNonQuery();

                if (!string.IsNullOrWhiteSpace(NewPassword))
                {
                    var hash = PasswordHasher.Hash(NewPassword);
                    var updatePassword = new SqlCommand("UPDATE Users SET Password = @Password WHERE UserID = @UserID", conn);
                    updatePassword.Parameters.AddWithValue("@Password", hash);
                    updatePassword.Parameters.AddWithValue("@UserID", UserID);
                    updatePassword.ExecuteNonQuery();
                }

                var updateDoctor = new SqlCommand("UPDATE Doctors SET DepartmentID = @DepartmentID WHERE UserID = @UserID", conn);
                updateDoctor.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                updateDoctor.Parameters.AddWithValue("@UserID", UserID);
                updateDoctor.ExecuteNonQuery();

                SuccessMessage = "Doctor updated successfully!";
            }

            return RedirectToPage("/Admin/ManageDoctors");
        }

        private void LoadDepartments()
        {
            Departments.Clear();
            string connStr = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (var conn = new SqlConnection(connStr))
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

        public class Department
        {
            public int DepartmentID { get; set; }
            public string Name { get; set; }
            public string ClinicName { get; set; }
        }
    }
}

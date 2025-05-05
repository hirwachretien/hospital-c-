using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using HospitalFlowSystem.Helpers;
using System;

namespace HospitalFlowSystem.Pages.Account
{
    public class SignupModel : PageModel
    {
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        [BindProperty] public string ConfirmPassword { get; set; }
        [BindProperty] public string ContactInfo { get; set; }
        [BindProperty] public string City { get; set; }
        [BindProperty] public DateTime DateOfBirth { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public void OnPost()
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            string hashedPassword = PasswordHasher.Hash(Password);
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if email already exists
                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", Email);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        ErrorMessage = "An account with this email already exists.";
                        return;
                    }
                }

                // Insert into Users and get new UserID
                string insertUserQuery = @"
                    INSERT INTO Users (Name, Email, Password, Role)
                    OUTPUT INSERTED.UserID
                    VALUES (@Name, @Email, @Password, 'Patient')";

                int newUserId;
                using (SqlCommand insertUserCmd = new SqlCommand(insertUserQuery, conn))
                {
                    insertUserCmd.Parameters.AddWithValue("@Name", Name);
                    insertUserCmd.Parameters.AddWithValue("@Email", Email);
                    insertUserCmd.Parameters.AddWithValue("@Password", hashedPassword);

                    newUserId = (int)insertUserCmd.ExecuteScalar();
                }

                // Insert into Patients table
                string insertPatientQuery = @"
                    INSERT INTO Patients (UserID, ContactInfo, City, DateOfBirth)
                    VALUES (@UserID, @ContactInfo, @City, @DateOfBirth)";

                using (SqlCommand insertPatientCmd = new SqlCommand(insertPatientQuery, conn))
                {
                    insertPatientCmd.Parameters.AddWithValue("@UserID", newUserId);
                    insertPatientCmd.Parameters.AddWithValue("@ContactInfo", ContactInfo);
                    insertPatientCmd.Parameters.AddWithValue("@City", City);
                    insertPatientCmd.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);

                    int rows = insertPatientCmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        SuccessMessage = "Signup successful! Redirecting to login page...";
                        return;
                    }
                    else
                    {
                        ErrorMessage = "Failed to save patient details.";
                    }
                }
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace HospitalFlowSystem.Pages.Admin
{
    public class ManageClinicsModel : PageModel
    {
        [BindProperty] public string ClinicName { get; set; }
        [BindProperty] public string Location { get; set; }
        [BindProperty] public string ContactEmail { get; set; }
        [BindProperty] public string Phone { get; set; }

        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public List<Clinic> Clinics { get; set; } = new();

        public void OnGet()
        {
            LoadClinics();
        }

        public void OnPost()
        {
            if (string.IsNullOrWhiteSpace(ClinicName) ||
                string.IsNullOrWhiteSpace(Location) ||
                string.IsNullOrWhiteSpace(ContactEmail) ||
                string.IsNullOrWhiteSpace(Phone))
            {
                ErrorMessage = "All fields are required.";
                LoadClinics();
                return;
            }

            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    INSERT INTO Clinics (Name, Location, ContactEmail, Phone, CreatedAt)
                    VALUES (@Name, @Location, @ContactEmail, @Phone, @CreatedAt)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", ClinicName);
                    cmd.Parameters.AddWithValue("@Location", Location);
                    cmd.Parameters.AddWithValue("@ContactEmail", ContactEmail);
                    cmd.Parameters.AddWithValue("@Phone", Phone);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        SuccessMessage = "Clinic added successfully!";
                    }
                    else
                    {
                        ErrorMessage = "Failed to add clinic.";
                    }
                }
            }

            LoadClinics();
        }

        public IActionResult OnPostDelete(int id)
        {
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string deleteQuery = "DELETE FROM Clinics WHERE ClinicID = @ClinicID";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ClinicID", id);
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        SuccessMessage = "Clinic deleted successfully.";
                    }
                    else
                    {
                        ErrorMessage = "Failed to delete clinic.";
                    }
                }
            }

            LoadClinics();
            return Page();
        }


        private void LoadClinics()
        {
            Clinics.Clear();
            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ClinicID, Name, Location, ContactEmail, Phone, CreatedAt FROM Clinics";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Clinics.Add(new Clinic
                        {
                            ClinicID = (int)reader["ClinicID"],
                            Name = reader["Name"].ToString(),
                            Location = reader["Location"].ToString(),
                            Email = reader["ContactEmail"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }
        }

        public class Clinic
        {
            public int ClinicID { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}

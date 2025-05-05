using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using HospitalFlowSystem.Helpers;

namespace HospitalFlowSystem.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public IActionResult OnPost()
        {
            // Password Hashing
            string hashedPassword = PasswordHasher.Hash(Password);

            string connectionString = "Data Source=23563-KR\\SQLEXP;Initial Catalog=HospitalFlowDB;Integrated Security=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Role FROM Users WHERE Email = @Email AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    var role = cmd.ExecuteScalar() as string;

                    if (role != null)
                    {
                        // Setting Session Values
                        HttpContext.Session.SetString("UserEmail", Email);
                        HttpContext.Session.SetString("UserRole", role);

                        // Redirecting based on the role
                        if (role == "Admin")
                           // return RedirectToPage("/Index"); // just for testing
                        return RedirectToPage("/Admin/Dashboard");
                        else if (role == "Doctor")
                            return RedirectToPage("/Index"); // just for testing
                        //return RedirectToPage("/Doctor/Dashboard");
                        else if (role == "Patient")
                            return RedirectToPage("/Index"); // just for testing
                       // return RedirectToPage("/Patient/Dashboard");
                    }
                    else
                    {
                        ErrorMessage = "Invalid email or password.";
                    }
                }
            }

            return Page(); // Stay on login page if failed
        }
    }
}

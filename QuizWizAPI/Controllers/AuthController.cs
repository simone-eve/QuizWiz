using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Google.Cloud.Firestore.V1;
using ScrollandScribeAPI.Models;
using QuizWizAPI.Model;
using QuizWIzAPI.Model;

namespace QuizWizAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly FirebaseAuthProvider auth;
        private FirestoreDb db;

        public AuthController(IConfiguration configuration)
        {
            var firebaseApiKey = configuration["FirebaseApiKey"];
            auth = new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey));

            // Initialize Firestore database connection
            string pathToCredentials = Path.GetFullPath("desktop/quizwiz-b38f5-firebase-adminsdk-a223d-aedbfcd3ae.json");
            var builder = new FirestoreClientBuilder { CredentialsPath = pathToCredentials };
            var client = builder.Build(); // Create the FirestoreClient
            db = FirestoreDb.Create("quizwiz-b38f5", client); // Pass the client to FirestoreDb.Create()
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Return a view for registration if needed (modify as needed)
            return View();
        }

        [HttpPost("PostRegister")]
        [Route("~/api/PostRegister")]
        public async Task<IActionResult> Register(LoginModel login)
        {
            try
            {
                var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(login.Email, login.Password);
                string currentUserId = fbAuthLink.User.LocalId;
                string userRole;

                if (login.adminValidation != null)
                {
                    userRole = login.adminValidation;
                }
                else
                {
                    userRole = "customer";
                }

                if (currentUserId != null)
                {
                    var userRef = db.Collection("Users").Document(currentUserId);
                    var user = new UserDetails() // Define your user model class with relevant properties
                    {
                        FirebaseUuid = currentUserId
                    };

                    await userRef.SetAsync(user);

                    // Consider using claims or JWT for further user identification

                    return Created(userRef.Id, user); // Return the created user document
                }
                else
                {
                    return BadRequest(new Error("Registration failed."));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Error("An error occurred while processing your request."));
            }
        }

        [HttpPost("PostLogin")]
        [Route("~/api/PostLogin")]
        [HttpPost]
        public async Task<IActionResult> PostLogin(string Token)
        {
            var userRef = db.Collection("Users").Document(Token);
            var userSnapshot = await userRef.GetSnapshotAsync();

            if (userSnapshot.Exists)
            {
                var user = userSnapshot.ConvertTo<User>(); // Replace User with your user model class
                return Ok(user); // Return the user object
            }
            else
            {
                return NotFound(new Error("No user found!"));
            }
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            // Implement logout logic (e.g., clear session data)
            return RedirectToAction("Login");
        }
    }
}
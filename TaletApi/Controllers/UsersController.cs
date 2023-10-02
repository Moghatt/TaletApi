using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Plugins;
using System;
using TaletApi.Models;
using TaletApi.Models.DTO;
using TaletApi.utility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SalesDbContext _db;
        private readonly JwtService _jwtService;
        private readonly ILogger<SaleController> _logger;

        public UsersController(SalesDbContext db, JwtService jwtService, ILogger<SaleController> logger)
        {
            _db = db;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            if(request.Password != request.ConfirmPassword)
            {
                return BadRequest("Password and ConfirmPassword is not Match");
            }
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (existingUser != null)
            {
                return BadRequest("UserName already exists");
            }

            var user = new User
            {
                UserName = request.UserName,
                PasswordHash = BycryptService.HashPassword(request.Password) // Implement a secure password hashing method
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok("Registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            _logger.LogInformation(BycryptService.VerifyPassword(user.PasswordHash, request.Password).ToString());

         


            if (user == null || string.IsNullOrWhiteSpace(request.Password)||!BycryptService.VerifyPassword(user.PasswordHash, request.Password))
            {
                return Unauthorized("Invalid Credentials");
            }

            var token = _jwtService.GenerateToken(user);

            // Set the token as an HTTP cookie

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true, // Prevents JavaScript from accessing the cookie
                SameSite = SameSiteMode.None, // Improve CSRF protection
                Secure = true, // Send the cookie over HTTPS only in production
                MaxAge = TimeSpan.FromDays(7) // Adjust the token expiration time as needed
            });


            //var cookie = new CookieOptions
            //{
            //    Path = "/",
            //    HttpOnly = true, // Whether the cookie should be accessible only through HTTP (true by default)
            //    IsEssential = true,// Indicates whether the cookie is essential for the application to function
            //    Secure = true,
            //    SameSite = SameSiteMode.None,
            //    Expires = DateTime.Now.AddMinutes(30) // Cookie expiration time
            //};


            //// first way
            //Response.Cookies.Append("Cookiennn", token, cookie);

            return Ok(token);


        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // Clear the token cookie
            Response.Cookies.Delete("jwt");
            return Ok("Logout successful");
        }


        [HttpGet("currentUser")]
        [Authorize]
        public IActionResult GetCurrentUser()

        {


            return Ok("protectec Route");

        }

        //[HttpPost("signInWithGoogle")]
        //public async Task<IActionResult> GoogleLogin([FromBody] object request)
        //{
        //    var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        //    _logger.LogInformation(request.ToString());

        //    if (!authenticateResult.Succeeded)
        //    {
        //        return Unauthorized();
        //    }

        //    // Create or update the user in your database based on the information received from Google.
        //    // Example: UserService.CreateOrUpdateUser(authenticateResult.Principal);

        //    return Ok(new { message = "Logged in successfully" });
        //}
    }
}
using AutoMapper;
using gbemi.Contexts;
using gbemi.Helper;
using gbemi.Models;
using gbemi.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Cryptography;
using gbemi.UtilityService;
using System.Net;
//using System.Data.Entity;



namespace gbemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public UserController(ILogger<UserController> logger, IMapper mapper, IUserRepo userRepo, AppDbContext context, IEmailService emailService)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepo = userRepo;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDTO userObj)
        {
            //try
            //{
                // Check if the request body is null
                if (userObj == null)
                {
                    return BadRequest();
                }

                // Find the user by username
                var user = await _userRepo.FindAsyncByUNameAsync(userObj.UserName);



                // If user not found, return NotFound
                if (user == null)
                    return NotFound(new { Message = "User not found" });

                // Verify the password
                if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
                    return BadRequest(new { Message = "Password is Incorrect" });

                // Generate JWT token
                //var token = CreateJwt(user);

                var userDTO = _mapper.Map<LoginDTO>(user);

                // Return success response with token and user details
                return Ok(new
                {
                   // Token = token,
                    Message = "Login Success!",
                    User = userDTO
                });
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception
            //    _logger.LogError(ex, "Error occurred while authenticating user.");

            //    // Return 500 Internal Server Error status code with an error message
            //    return StatusCode(500, "An error occurred while processing the request. Please try again later.");
            //}
        }

        [HttpPost("register")]
        public async Task<ActionResult<SigninDTO>> CreateUser([FromBody] SigninDTO signinDTO)
        {
            if (signinDTO == null)
                return BadRequest();

            // Check if the provided data is valid
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            // Check if the username already exists
            if (await CheckUserNameExistAsync(signinDTO.UserName))
                return BadRequest(new { Message = "Username already exists" });

            // Check if the email already exists
            if (await CheckEmailExistAsync(signinDTO.Email))
                return BadRequest(new { Message = "Email already exists" });

            // Check the password strength
            var passwordStrengthMessage = CheckPasswordStrength(signinDTO.Password);
            if (!string.IsNullOrEmpty(passwordStrengthMessage))
                return BadRequest(new { Message = passwordStrengthMessage });

            // Hash the password
            signinDTO.Password = PasswordHasher.HashPassword(signinDTO.Password);
            signinDTO.ConfirmPassword = PasswordHasher.HashPassword(signinDTO.ConfirmPassword);
            signinDTO.Role = "User";
            signinDTO.Token = "";

            
            // Map the DTO to User entity
            var newUser = _mapper.Map<User>(signinDTO);

            // Add user to repository
            var addedUser = await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "User registered"
            });

            // Map the added user back to DTO
            //var createdStudentDTO = _mapper.Map<SigninDTO>(addedUser);

            // Return 201 Created status code with the newly created resource
           /* return CreatedAtRoute("UserId", new { id = createdStudentDTO.Id }, createdStudentDTO)*/;
        }

        private async Task<bool> CheckUserNameExistAsync(string userName) =>
            await _context.Users.AnyAsync(s => s.UserName == userName);

        private async Task<bool> CheckEmailExistAsync(string email) =>
            await _context.Users.AnyAsync(s => s.Email == email);

        private string CheckPasswordStrength(string password)
        {
            var sb = new StringBuilder();
            if (password.Length < 7)
                sb.AppendLine("Minimum password length is 7");
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.AppendLine("Password should contain a capital letter and be alphanumeric");
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.AppendLine("Password should contain special characters");
            return sb.ToString();
        }

        private string CreateJwt(User user)
        {
            // Define your secret key
            string secretKey = "veryverysecret.....";

            // Create security key using the secret key
            var key = Encoding.ASCII.GetBytes(secretKey);

            // Create claims for the JWT token
            var claims = new[]
            {
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
    };

            // Create signing credentials using the security key
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            // Create JWT token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), // Set expiration time
                SigningCredentials = credentials
            };

            // Create JWT token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Generate JWT token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Serialize JWT token to a string
            //var tokenString = tokenHandler.WriteToken(token);

            return tokenHandler.WriteToken(token);
        }




        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers() =>
            Ok(await _context.Users.ToListAsync());

        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(a => a.Email == email);
                if (user == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Email Doesn't Exist"
                    });
                }

                // Generate a random token for resetting password
                var tokenBytes = RandomNumberGenerator.GetBytes(64);
                var emailToken = Convert.ToBase64String(tokenBytes);

                //// Set the reset password token and expiry
                //user.ResetPasswordToken = emailToken;
                //user.ResetPasswordExpiry = DateTime.UtcNow.AddMinutes(15);

                // Save changes to the user entity
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Send reset password email
                var emailModel = new EmailModel(email, "Reset Password", EmailBody.EmailStringBody(email, emailToken));
                _emailService.SendEmail(emailModel);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Email sent!"
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error sending reset password email.");

                // Return 500 Internal Server Error status code with an error message
                return StatusCode(500, "An error occurred while sending the reset password email.");
            }
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                // Decode the email token (if needed)
                //var newToken = resetPasswordDTO.EmailToken.Replace(" ", "+");

                // Find the user by email
                var user = await _context.Users.FirstOrDefaultAsync(a => a.Email == resetPasswordDTO.Email);
                if (user == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "User Doesn't Exist"
                    });
                }

                // Check if the reset token is valid and not expired
                //if (user.ResetPasswordToken != newToken || user.ResetPasswordExpiry < DateTime.UtcNow)
                //{
                //    return BadRequest(new
                //    {
                //        StatusCode = 400,
                //        Message = "Invalid Reset Link"
                //    });
                //}

                // Hash the new password and update user's password
                user.Password = PasswordHasher.HashPassword(resetPasswordDTO.NewPassword);

                // Save the changes to the database
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Password Reset Successfully"
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error resetting password.");

                // Return 500 Internal Server Error status code with an error message
                return StatusCode(500, "An error occurred while resetting the password.");
            }
        }

                 

    }
}

using System;
using Microsoft.AspNetCore.Mvc;
using gbemi.Contexts;
using gbemi.Models;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using System.Data.Entity;

namespace gbemi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public ContactController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Contact contact)
        {
            try
            {
                contact.CreatedAt = DateTime.Now; // Set creation timestamp

                _context.Contacts.Add(contact);
                _context.SaveChanges();

                // Send email
                SendEmail(contact);

                //return Ok("Message received successfully!");
                return Ok(new { message = "Message received successfully!" });

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to process the message. Error: " + ex.Message);
                //return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Failed to process the message. Error: " + ex.Message });
            }
        }

        private void SendEmail(Contact contact)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("We're Dad", _config["EmailSettings:From"]));
            emailMessage.To.Add(new MailboxAddress("Your Name", "ebubeashibuogwu@gmail.com"));
            emailMessage.Subject = "Contact Form Submission";

            // Construct the message body using HTML
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                 <head>
                 </head>
                 <body style=""margin:0;padding:0;font-family: Arial, Helvetica, sanserif;"">
                     <div style=""height: auto;background: linear-gradient(to top, #c9c9ff 50%, #6e6ef6 90%) no repeat;width:400px;padding:30px;""> 
                       <div> 
                          <div>
                            <h1>We're Dad Contacts</h1>
                            <hr>
                <p><strong>Name:</strong> {contact.Name}</p>
                <p><strong>Email:</strong> {contact.Email}</p>
                <p><strong>Message:</strong> {contact.Message}</p>       
                            
                            <p>Kind Regards, <br><br>
                            We're Dad Studio</p>
                          </div> 
                       </div>
                      </div>
                 </body>
                </html>";





            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_config["EmailSettings:SmtpServer"],
                                   Convert.ToInt32(_config["EmailSettings:SmtpPort"]),
                                   true);
                    client.Authenticate(_config["EmailSettings:Username"],
                                        _config["EmailSettings:Password"]);
                    client.Send(emailMessage);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Failed to send email: {ex.Message}");
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}

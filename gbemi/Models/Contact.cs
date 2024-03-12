using System;

namespace gbemi.Models
{
    public class Contact
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

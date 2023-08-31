using Studentregistrationsystem.Models;
using Microsoft.AspNetCore.Http;

namespace Studentregistrationsystem.Models
{
    public class StudentViewModel
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string NIC { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public IFormFile ImageFile { get; set; }
    }

}


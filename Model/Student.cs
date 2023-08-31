using System;

namespace Studentregistrationsystem.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string NIC { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public byte[]? ProfileImage { get; set; }

        public string ProfileImageBase64
        {
            get
            {
                return ProfileImage != null ? Convert.ToBase64String(ProfileImage) : null;
            }
        }
    }
}

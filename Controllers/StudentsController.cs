using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studentregistrationsystem.Model;
using Studentregistrationsystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Studentregistrationsystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly StudentContext _dbContext;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(StudentContext dbContext, ILogger<StudentsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _dbContext.Students.Where(s => s != null).ToListAsync();

            if (!students.Any())
            {
                return NotFound();
            }
            return students;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _dbContext.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return student;
        }

        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent([FromForm] StudentViewModel studentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            const int maxImageSize = 5 * 1024 * 1024;

            if (studentViewModel.ImageFile != null)
            {
                if (studentViewModel.ImageFile.Length > maxImageSize)
                {
                    return BadRequest($"Image size exceeds the maximum allowed size of {maxImageSize / 1024 / 1024}MB.");
                }

                if (!IsSupportedImageType(studentViewModel.ImageFile))
                {
                    return BadRequest("Unsupported image type. Supported types are: .jpg, .jpeg, .png");
                }
            }

            byte[] profileImageData = null;
            if (studentViewModel.ImageFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await studentViewModel.ImageFile.CopyToAsync(memoryStream);
                    profileImageData = memoryStream.ToArray();
                }
            }

            var student = new Student
            {
                FirstName = studentViewModel.FirstName,
                LastName = studentViewModel.LastName,
                Mobile = studentViewModel.Mobile,
                Email = studentViewModel.Email,
                NIC = studentViewModel.NIC,
                DateOfBirth = studentViewModel.DateOfBirth,
                Address = studentViewModel.Address,
                ProfileImage = profileImageData
            };

            try
            {
                _dbContext.Students.Add(student);
                await _dbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving the student.");
                return StatusCode(500, "An error occurred while saving the student. Please try again later.");
            }
        }

        private bool IsSupportedImageType(IFormFile file)
        {
            var supportedTypes = new[] { "image/jpg", "image/jpeg", "image/png" };
            return supportedTypes.Contains(file.ContentType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, [FromForm] StudentViewModel studentViewModel)
        {
            var student = await _dbContext.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (studentViewModel.ImageFile != null)
            {
                const int maxImageSize = 5 * 1024 * 1024;
                if (studentViewModel.ImageFile.Length > maxImageSize)
                {
                    return BadRequest($"Image size exceeds the maximum allowed size of {maxImageSize / 1024 / 1024}MB.");
                }

                if (!IsSupportedImageType(studentViewModel.ImageFile))
                {
                    return BadRequest("Unsupported image type. Supported types are: .jpg, .jpeg, .png");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await studentViewModel.ImageFile.CopyToAsync(memoryStream);
                    student.ProfileImage = memoryStream.ToArray();
                }
            }

           
                     student.FirstName = studentViewModel.FirstName;
                     student.LastName = studentViewModel.LastName;
                     student.Mobile = studentViewModel.Mobile;
                     student.Email = studentViewModel.Email;
                     student.NIC = studentViewModel.NIC;
                     student.DateOfBirth = studentViewModel.DateOfBirth;
                        student.Address = studentViewModel.Address;

                 _dbContext.Entry(student).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error while updating student with ID: {Id}", id);
                return Conflict("The student data may have been modified by someone else. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating the student.");
                return StatusCode(500, "An error occurred while updating the student. Please try again later.");
            }

            return Ok(student);
        }


        private bool StudentAvailable(int id)
        {
            return _dbContext.Students?.Any(x => x.Id == id) ?? false;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _dbContext.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _dbContext.Students.Remove(student);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}

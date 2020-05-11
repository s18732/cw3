using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.Models2;
using cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers2
{
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IDbService2 _context;
        public EnrollmentsController(IDbService2 context)
        {
            _context = context;
        }
        [Route("api/cw10/enrollments")]
        [HttpPost]
        public IActionResult EnrollStudent(Models.Student s)
        {
            var result = _context.EnrollStudent(s);
            if (result.Equals("Utworzono enrollment i student") || result.Equals("Utworzono student"))
                return Created("", result);
            else
                return BadRequest(result);
        }
        [Route("api/cw10/enrollments/promotions")]
        [HttpPost]
        public IActionResult PromoteStudents(DTOs.Requests.PostPromotion p)
        {
            var result = _context.PromoteStudents(p);
            if (result == null)
                return BadRequest("Semestr i studia nie istnieja");
            var result2 = new
            {
                IdEnrollment = result.IdEnrollment,
                Semester = result.Semester,
                IdStudy = result.IdStudy,
                StartDate = result.StartDate
            };
            return Created("", result2);
        }
    }

}
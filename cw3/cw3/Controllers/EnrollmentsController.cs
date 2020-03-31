using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cw3.DTOs.Requests;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private const string ConnString = "Data Source=db-mssql;Initial Catalog=s18732;Integrated Security=True";
        private IStudentsDbService _service;
        public EnrollmentsController(IStudentsDbService service)
        {
            _service = service;
        }
        [Route("api/enrollments")]
        [HttpPost]
        public IActionResult PostEnrollment(Student student)
        {
            string s = _service.PostEnrollment(student);
            if (s.Equals("Utworzono enrollment i student") || s.Equals("Utworzono student"))
                return Created("", s);
            else
                return BadRequest(s);
        }
        [Route("api/enrollments/promotions")]
        [HttpPost]
        public IActionResult PostPromotion(PostPromotion req)
        {
            Enrollment en = _service.PostPromotion(req);
            if (en == null)
                return BadRequest("Semestr i studia nie istnieja");
            else
                return Created("", en);
        }
    }
}
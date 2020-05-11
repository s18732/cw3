using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.Models2;
using cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers2
{
    [Route("api/cw10/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private IDbService2 _context;
        public StudentsController(IDbService2 context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = _context.GetStudents();
            return Ok(list);
        }
        [HttpPut]
        public IActionResult ModifyStudents(Student s)
        {
            if (_context.ModifyStudents(s))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete]
        public IActionResult DeleteStudents(Student s)
        {
            if (_context.DeleteStudents(s))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
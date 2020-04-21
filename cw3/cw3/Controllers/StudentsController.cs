using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using cw3.Models;
using cw3.DAL;
using System.Data.SqlClient;
using cw3.Services;
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;

namespace cw3.Controllers
{
    
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private const string ConnString = "Data Source=db-mssql;Initial Catalog=s18732;Integrated Security=True";
        private IStudentsDbService _service;
        public StudentsController(IStudentsDbService service)
        {
            _service = service;
        }
        [Route("api/students")]
        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Student> l =_service.GetStudents();
            return Ok(l);
            
        }
        [Route("api/students")]
        [HttpGet("{id}")]
        public IActionResult GetStudent(string id)
        {
            Enrollment en = _service.GetStudent(id);
            if (en == null)
                return NotFound();
            else
                return Ok(en);
        }
        [Route("api/students")]
        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            TokenResponse tok = _service.Login(request);
            if(tok != null)
            {
                return Ok(new
                {
                    token = tok.JWTtoken,
                    refreshToken = tok.RefreshToken
                });
            }
            else
            {
                return Unauthorized();
            }
            
        }
        [Route("api/students/refresh")]
        [HttpPost]
        public IActionResult Refresh(RefreshRequest request)
        {
            TokenResponse tok = _service.Refresh(request);
            if (tok != null)
            {
                return Ok(new
                {
                    token = tok.JWTtoken,
                    refreshToken = tok.RefreshToken
                });
            }
            else
            {
                return Unauthorized();
            }

        }


        /*[HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            return Ok(student);
        }*/
        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja dokonczona");
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }
    }
}
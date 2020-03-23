using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using cw3.Models;
using cw3.DAL;
using System.Data.SqlClient;

namespace cw3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private const string ConnString = "Data Source=db-mssql;Initial Catalog=s18732;Integrated Security=True";
        //private readonly IDbService _dbService;
        public StudentsController(/*IDbService dbService*/)
        {
           // _dbService = dbService;
        }
        [HttpGet]
        public IActionResult GetStudents() //action method
        {
            var result = new List<Student>();

            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read()){
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = (DateTime)dr["BirthDate"];
                    st.IdEnrollment = (int)dr["IdEnrollment"];
                    result.Add(st);
                }

                return Ok(result);
            }
            //return Ok(_dbService.GetStudents());
        }
        [HttpGet("{id}")]
        public IActionResult GetStudent(string id)
        {
            //var result = new List<Enrollment>();

            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from enrollment where IdEnrollment = (select IdEnrollment from student where indexnumber = @index);";
                //com.CommandText = "select * from students where indexnumber='"+id"';";

                com.Parameters.AddWithValue("index", id);

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var en = new Enrollment();
                    en.IdEnrollment = (int)dr["IdEnrollment"];
                    en.Semester = (int)dr["Semester"];
                    en.IdStudy = (int)dr["IdStudy"];
                    en.StartDate = (DateTime)dr["StartDate"];
                    //result.Add(en);
                    return Ok(en);
                }

                //return Ok(result);
                return NotFound();

            }
        }
        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //... add to database
            //... generating index number
            //student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }
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
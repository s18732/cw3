using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        public IConfiguration Configuration { get; set; }
        private const string ConnString = "Data Source=db-mssql;Initial Catalog=s18732;Integrated Security=True";
        public SqlServerDbService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string PostEnrollment(Student student)
        {
            if (student.FirstName.Length == 0 || student.LastName.Length == 0 || student.IndexNumber.Length == 0 || student.Studies.Length == 0 || student.BirthDate.Length == 0)
            {
                return ("Zle podane wartosci");
            }

            int jest = 0;

            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select count(1) jest from Studies where Name = '" + student.Studies + "';";
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    jest = (int)dr["jest"];
                }
            }
            if (jest == 0)
            {
                return ("Nie ma takich studiow");
            }
            int enrollid = 0;
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IdEnrollment from Enrollment where Semester = 1 and IdStudy = (select IdStudy from Studies where Name = '" + student.Studies + "')" +
                    "and StartDate = (select max(StartDate) from Enrollment where Semester = 1 and IdStudy = (select IdStudy from Studies where Name = '" + student.Studies + "'));";
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    enrollid = (int)dr["IdEnrollment"];
                }
            }
            if (enrollid == 0)
            {
                int unikalny = -1;
                using (SqlConnection con = new SqlConnection(ConnString))
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "select count(1) czy from student where IndexNumber = '" + student.IndexNumber + "';";
                    con.Open();
                    SqlDataReader dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        unikalny = (int)dr["czy"];
                    }
                }
                if (unikalny != 0)
                {
                    return ("Nie jest to unikalny indeks");
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(ConnString))
                    using (SqlCommand com = new SqlCommand())
                    {
                        con.Open();
                        SqlTransaction trans = con.BeginTransaction();
                        com.Connection = con;
                        com.Transaction = trans;
                        try
                        {
                            //return Ok(DateTime.Now.Year +"-" +DateTime.Now.Month +"-"+DateTime.Now.Day);
                            com.CommandText = "insert into Enrollment (IdEnrollment, Semester, IdStudy, StartDate) values ((select max(IdEnrollment)+1 from Enrollment), 1, (select IdStudy " +
                           "from Studies where Name = '" + student.Studies + "'), '" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "');";
                            com.ExecuteNonQuery();
                            com.CommandText = "insert into student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) values ('" + student.IndexNumber + "', '"
                                    + student.FirstName + "', '" + student.LastName + "', '" + student.BirthDate.Split(".")[2] + "-" + student.BirthDate.Split(".")[1] + "-" + student.BirthDate.Split(".")[0] + "', (select max(IdEnrollment) from Enrollment));";
                            com.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            return ("Blad podczas transakcji");
                        }
                        return ("Utworzono enrollment i student");
                    }
                }
            }
            else
            {
                int unikalny = -1;
                using (SqlConnection con = new SqlConnection(ConnString))
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "select count(1) czy from student where IndexNumber = '" + student.IndexNumber + "';";
                    con.Open();
                    SqlDataReader dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        unikalny = (int)dr["czy"];
                    }
                }
                if (unikalny != 0)
                {
                    return ("Nie jest to unikalny indeks");
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(ConnString))
                    using (SqlCommand com = new SqlCommand())
                    {
                        con.Open();
                        SqlTransaction trans = con.BeginTransaction();
                        com.Connection = con;
                        com.Transaction = trans;
                        try
                        {
                            com.CommandText = "insert into student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) values ('" + student.IndexNumber + "', '"
                                + student.FirstName + "', '" + student.LastName + "', '" + student.BirthDate.Split(".")[2] + "-" + student.BirthDate.Split(".")[1] + "-" + student.BirthDate.Split(".")[0] + "', " + enrollid + ");";
                            com.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            return ("Blad podczas transakcji");
                        }
                        return ("Utworzono student");
                    }
                }
            }
        }
        public Enrollment PostPromotion(PostPromotion req)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();
                var trans = con.BeginTransaction();
                com.Transaction = trans;
                //try
                //{
                com.CommandText = "select IdEnrollment from Enrollment join Studies on Enrollment.IdStudy = Studies.IdStudy where Semester = @sem and Name = @nam";
                com.Parameters.AddWithValue("sem", req.Semester);
                com.Parameters.AddWithValue("nam", req.Studies);
                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    trans.Rollback();
                    return null;

                }
                dr.Close();
                //int enrollid = (int)dr["IdEnrollment"];
                com.CommandText = "exec PromoteStudents @nam2, @sem2";
                com.Parameters.AddWithValue("sem2", req.Semester);
                com.Parameters.AddWithValue("nam2", req.Studies);
                com.ExecuteNonQuery();
                var en = new Enrollment();
                com.CommandText = "select * from Enrollment join Studies on Enrollment.IdStudy = Studies.IdStudy where Semester = @sem3 +1 and Name = @nam3";
                com.Parameters.AddWithValue("sem3", req.Semester);
                com.Parameters.AddWithValue("nam3", req.Studies);
                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    en.IdEnrollment = (int)dr["IdEnrollment"];
                    en.Semester = (int)dr["Semester"];
                    en.IdStudy = (int)dr["IdStudy"];
                    en.StartDate = dr["StartDate"].ToString();
                }
                dr.Close();
                trans.Commit();
                return (en);
                //}
                //catch (SqlException ex)
                //{
                //    trans.Rollback();
                //    return BadRequest(ex.Message);
                //}
            }
        }
        public List<Student> GetStudents() //action method
        {
            var result = new List<Student>();

            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.IdEnrollment = (int)dr["IdEnrollment"];
                    result.Add(st);
                }

                return (result);
            }
            //return Ok(_dbService.GetStudents());
        }
        [HttpGet("{id}")]
        public Enrollment GetStudent(string id)
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
                    //en.StartDate = (DateTime)dr["StartDate"];
                    en.StartDate = dr["StartDate"].ToString();
                    //result.Add(en);
                    return (en);
                }

                //return Ok(result);
                return null;

            }
        }

        public bool CheckIndex(string id)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();

                com.CommandText = "select count(1) jest from student where IndexNumber = @id;";
                com.Parameters.AddWithValue("id", id);
                var dr = com.ExecuteReader();
                int jest = 0;
                if (dr.Read())
                {
                    jest = (int)dr["jest"];
                }
                if (jest >= 1)
                    return true;
                else
                    return false;
            }
        }

        public TokenResponse Login(LoginRequest req)
        {
            int jest = 0;
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();

                com.CommandText = "select count(1) jest from student where IndexNumber = @id and Password = @pass;";
                com.Parameters.AddWithValue("id", req.Login);
                com.Parameters.AddWithValue("pass", req.Haslo);
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    jest = (int)dr["jest"];
                }
            }
            //Console.WriteLine(req.Login);
            if (jest > 0)
            {
                var claims = new Claim[2];
                if (req.Login.Equals("s1"))
                {
                    claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, req.Login),
                new Claim(ClaimTypes.Role, "employee")
            };
                }
                else
                {
                    claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, req.Login),
                new Claim(ClaimTypes.Role, "student")
            };
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                
                var token = new JwtSecurityToken(issuer: "Gakko", audience: "Students", claims: claims, expires: DateTime.Now.AddMinutes(10), signingCredentials: creds);

                var token2 = new JwtSecurityTokenHandler().WriteToken(token);
                var refreshToken = Guid.NewGuid();

                using (SqlConnection con = new SqlConnection(ConnString))
                using (SqlCommand com = new SqlCommand())
                {
                    con.Open();
                    SqlTransaction trans = con.BeginTransaction();
                    com.Connection = con;
                    com.Transaction = trans;
                    try
                    {
                        com.CommandText = "update student set RefreshToken = @refreshToken where IndexNumber = @id";
                        com.Parameters.AddWithValue("id", req.Login);
                        com.Parameters.AddWithValue("refreshToken", refreshToken);
                        com.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                }
                return (new TokenResponse
                {
                    JWTtoken = token2,
                    RefreshToken = refreshToken
                });
            }
            else
            {
                return null;
            }
        }

        public TokenResponse Refresh(RefreshRequest req)
        {
            string id = "";
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();

                com.CommandText = "select IndexNumber from student where refreshtoken = @rt;";
                com.Parameters.AddWithValue("rt", req.RefreshToken);
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    id = dr["IndexNumber"].ToString();
                }
            }
            if (id != null)
            {
                {
                    var claims = new Claim[2];
                    if (id.Equals("s1"))
                    {
                        claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, id),
                            new Claim(ClaimTypes.Role, "employee")
                        };
                    }
                    else
                    {
                        claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, id),
                            new Claim(ClaimTypes.Role, "student")
                        };
                    }

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(issuer: "Gakko", audience: "Students", claims: claims, expires: DateTime.Now.AddMinutes(10), signingCredentials: creds);

                    var token2 = new JwtSecurityTokenHandler().WriteToken(token);
                    var refreshToken = Guid.NewGuid();

                    using (SqlConnection con = new SqlConnection(ConnString))
                    using (SqlCommand com = new SqlCommand())
                    {
                        con.Open();
                        SqlTransaction trans = con.BeginTransaction();
                        com.Connection = con;
                        com.Transaction = trans;
                        try
                        {
                            com.CommandText = "update student set RefreshToken = @refreshToken where IndexNumber = @id";
                            com.Parameters.AddWithValue("id", id);
                            com.Parameters.AddWithValue("refreshToken", refreshToken);
                            com.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                        }
                    }
                    return (new TokenResponse
                    {
                        JWTtoken = token2,
                        RefreshToken = refreshToken
                    });
                }
            }
            else
            {
                return null;
            }
        } 
        
    }
}

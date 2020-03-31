using cw3.DTOs.Requests;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private const string ConnString = "Data Source=db-mssql;Initial Catalog=s18732;Integrated Security=True";
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
    }
}

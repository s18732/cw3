using cw3.DTOs.Requests;
using cw3.Models2;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Services
{
    public class EfDbService : IDbService2
    {
        public s18732Context _context { get; set; }
        public EfDbService(s18732Context context)
        {
            _context = context;
        }

        public List<Models2.Student> GetStudents()
        {
            var list = _context.Student.ToList();
            return list;
        }
        public bool DeleteStudents(Models2.Student d)
        {
            try
            {
                /*var d = new Student
                {
                    IndexNumber = index
                };*/
                _context.Attach(d);
                _context.Remove(d);
                _context.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        public bool ModifyStudents(Models2.Student s)
        {
            try
            {
                _context.Attach(s);
                _context.Entry(s).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
            
        }

        public string EnrollStudent(Models.Student s)
        {
            if(!(_context.Studies.Any(st => st.Name == s.Studies)))
            {
                return ("Nie ma takich studiow");
            }
            if((_context.Student.Any(st => st.IndexNumber == s.IndexNumber)))
            {
                // JEST TAKI INDEX
                return ("Nie jest to unikalny index");
            }
            var s2 = new Models2.Student()
            {
                IndexNumber = s.IndexNumber,
                FirstName = s.FirstName,
                LastName = s.LastName,
                BirthDate = DateTime.Parse(s.BirthDate)
            };
            //var result = _context.Studies.Where(stu => stu.Name == s.Studies).;

            var study = _context.Studies.Where(stu => stu.Name == s.Studies).FirstOrDefault();
            int idStudy = study.IdStudy;

            if(!(_context.Enrollment.Any(en => en.IdStudy == idStudy && en.Semester == 1 && en.StartDate.Date == DateTime.Now.Date))) {

                var setStudent = new HashSet<Student> { s2 };

                var enroll = new Models2.Enrollment()
                {
                    IdEnrollment = _context.Enrollment.Select(en => en.IdEnrollment).Max() + 1,
                    Semester = 1,
                    StartDate = DateTime.Now.Date,
                    Student = setStudent,
                };

                study.Enrollment.Add(enroll);
                _context.Add(enroll);
                _context.Attach(study);
                _context.Entry(study).State = EntityState.Modified;
                _context.SaveChanges();

                return ("Utworzono enrollment i student");
            }
            else
            {
                var enroll = _context.Enrollment.Where(en => en.IdStudy == idStudy && en.Semester == 1 && en.StartDate.Date == DateTime.Now.Date).FirstOrDefault();

                enroll.Student.Add(s2);
                _context.Attach(enroll);
                _context.Entry(enroll).State = EntityState.Modified;
                _context.SaveChanges();

                return ("Utworzono student");
            }

        }

        public Enrollment PromoteStudents(PostPromotion p)
        {
            if (!(_context.Studies.Any(st => st.Name == p.Studies)))
            {
                return null;
            }
            var study = _context.Studies.Where(stu => stu.Name == p.Studies).FirstOrDefault();
            int idStudy = study.IdStudy;
            if (!(_context.Enrollment.Any(st => st.Semester == p.Semester && st.IdStudy == idStudy)))
            {
                return null;
            }
            
            //

            if(_context.Enrollment.Any(en => en.IdStudy == idStudy && en.Semester == p.Semester + 1))
            {
                var newEnroll = _context.Enrollment.Where(en => en.IdStudy == idStudy && en.Semester == p.Semester + 1).FirstOrDefault();
                var oldEnroll = _context.Enrollment.Where(en => en.IdStudy == idStudy && en.Semester == p.Semester).FirstOrDefault();

                var oldStud = _context.Student.Where(st => st.IdEnrollment == oldEnroll.IdEnrollment);
                var newStud = _context.Student.Where(st => st.IdEnrollment == newEnroll.IdEnrollment);
                var set = newStud.Union(oldStud);

                newEnroll.Student = set.ToHashSet();


                _context.Attach(newEnroll);
                _context.Entry(newEnroll).State = EntityState.Modified;
                _context.SaveChanges();
            }
            else
            {
                var oldEnroll = _context.Enrollment.Where(en => en.IdStudy == idStudy && en.Semester == p.Semester).FirstOrDefault();
                var oldStud = _context.Student.Where(st => st.IdEnrollment == oldEnroll.IdEnrollment);
                var newEnroll = new Enrollment()
                {
                    IdEnrollment = _context.Enrollment.Select(en => en.IdEnrollment).Max() + 1,
                    Semester = p.Semester + 1,
                    StartDate = DateTime.Now.Date,
                    Student = oldStud.ToHashSet()
                };


                study.Enrollment.Add(newEnroll);

                _context.Add(newEnroll);
                _context.Attach(study);
                _context.Entry(study).State = EntityState.Modified;
                _context.SaveChanges();
            }


            //_context.Database.ExecuteSqlRaw("exec PromoteStudents @nam, @sem", parameters: new { p.Studies, p.Semester });

            var result = _context.Enrollment.Where(st => st.Semester == p.Semester+1 && st.IdStudy == idStudy).FirstOrDefault();

            return result;
        }
    }
}

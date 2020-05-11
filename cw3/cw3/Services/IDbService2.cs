using cw3.Models2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Services
{
    public interface IDbService2
    {
        public List<Student> GetStudents();
        public bool DeleteStudents(Student d);
        public bool ModifyStudents(Student s);
        public string EnrollStudent(cw3.Models.Student s);
        public Enrollment PromoteStudents(cw3.DTOs.Requests.PostPromotion p);
    }
}

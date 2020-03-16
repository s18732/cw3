using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.Models;

namespace cw3.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
    }
}

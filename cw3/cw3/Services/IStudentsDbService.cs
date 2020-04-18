using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Services
{
    public interface IStudentsDbService
    {
        public Enrollment PostPromotion(PostPromotion req);
        public string PostEnrollment(Student student);
        public List<Student> GetStudents();
        public Enrollment GetStudent(string id);
        public Boolean CheckIndex(string id);
        public TokenResponse Login(LoginRequest req);
        public JWTTokenResponse Refresh(RefreshRequest req);
    }
}

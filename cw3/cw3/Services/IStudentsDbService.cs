using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        public TokenResponse Refresh(RefreshRequest req);
        public Boolean CreatePassword(LoginRequest req);
        public static string Create(string value, string salt) {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: value,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }
        public static bool Validate(string value, string salt, string hash) => Create(value, salt) == hash;
        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}

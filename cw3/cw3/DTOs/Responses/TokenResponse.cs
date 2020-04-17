using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DTOs.Responses
{
    public class TokenResponse
    {
        public string JWTtoken { get; set; }
        public Guid RefreshToken { get; set; }
    }
}

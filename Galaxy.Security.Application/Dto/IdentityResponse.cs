using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Security.Application.Dto
{
    public class IdentityResponse
    {
        public bool Success { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Model
{
    public class SignInRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class SignInResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}

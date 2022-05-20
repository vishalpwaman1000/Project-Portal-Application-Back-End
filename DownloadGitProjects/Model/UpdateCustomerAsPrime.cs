using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Model
{
    public class UpdateCustomerAsPrimeRequest
    {
        [Required]
        public int UserID { get; set; }

    }

    public class UpdateCustomerAsPrimeResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

    }
}

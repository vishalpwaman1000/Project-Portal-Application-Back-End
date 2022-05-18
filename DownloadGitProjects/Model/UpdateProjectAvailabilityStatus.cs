using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Model
{
    public class UpdateProjectAvailabilityStatusRequest
    {
        [Required]
        public int ProjectID { get; set; }
        
        [Required]
        public string AvailabiltyStatus { get; set; }
    }

    public class UpdateProjectAvailabilityStatusResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}

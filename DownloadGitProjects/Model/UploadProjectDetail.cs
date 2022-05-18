using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Model
{
    public class UploadProjectDetailRequest
    {
        [Required]
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string FrontEndProjectUrl { get; set; }
        public string BackEndProjectUrl { get; set; }
        public string ProjectDocumentUrl { get; set; }
        public string ProjectStatus { get; set; } // Public , Private        
        public bool IsActive { get; set; }
    }

    public class UploadProjectDetailResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Model
{
    public class DeleteProjectPermanentlyRequest
    {
        [Required]
        public int ProjectID { get; set; }
    }

    public class DeleteProjectPermanentlyResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}

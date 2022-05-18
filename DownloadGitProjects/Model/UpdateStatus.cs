using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Model
{
    public class UpdateStatusRequest
    {

        [Required]
        public string Operation { get; set; } //Active, InActive, Archive, Trash

        [Required]
        public int ProjectID { get; set; }

    }

    public class UpdateStatusResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}

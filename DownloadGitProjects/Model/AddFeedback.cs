using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Model
{
    public class AddFeedbackRequest
    {
        public int UserID { get; set; }
        public string Feedback { get; set; }
    }

    public class AddFeedbackResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}

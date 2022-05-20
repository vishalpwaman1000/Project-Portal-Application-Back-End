using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Model
{
   
    public class GetFeedbackDetailsRequest
    {
        [Required]
        public int PageNumber { get; set; }
        
        [Required]
        public int NumberOfRecordPerPage { get; set; }

    }

    public class GetFeedbackDetailsResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CurrentPage { get; set; }
        public double TotalRecords { get; set; }
        public int TotalPage { get; set; }
        public List<GetFeedbackDetails> data { get; set; }
    }

    public class GetFeedbackDetails
    {
        public int FeedbackID { get; set; }
        public string CreatedDate { get; set; } 
        public int CreatedBy { get; set; }
        public string FeedbackDetail { get; set; } 
        public bool IsActive { get; set; }

    }
}

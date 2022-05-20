using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Model
{
    public class GetCustomerProjectListRequest
    {
        public string Operation { get; set; }
        public int UserId { get; set; }
        public int PageNumber { get; set; }
        public int NumberOfRecordPerPage { get; set; }
    }

    public class GetCustomerProjectListResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CurrentPage { get; set; }
        public double TotalRecords { get; set; }
        public int TotalPage { get; set; }
        public List<GetCustomerProjectList> data { get; set; }
    }

    public class GetCustomerProjectList
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string FrontEndProjectUrl { get; set; }
        public string BackEndProjectUrl { get; set; }
        public string projectDocumentUrl { get; set; }
        public string ProjectStatus { get; set; }
        public string CreatedDate { get; set; }
    }
}

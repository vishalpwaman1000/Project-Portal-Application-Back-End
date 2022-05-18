using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Model
{
    public class GetProjectListRequest
    {
        public string Operation { get; set; } // Active, Archive, Trash
        public int PageNumber { get; set; }
        public int NumberOfRecordPerPage { get; set; }
    }

    public class GetProjectListResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CurrentPage { get; set; }
        public double TotalRecords { get; set; }
        public int TotalPage { get; set; }
        public List<GetProjectList> data { get; set; }
    }

    public class GetProjectList
    {
        //ProjectName, FrontEndProjectUrl, BackEndProjectUrl, projectDocumentUrl, ProjectStatus, IsActive
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string FrontEndProjectUrl { get; set; }
        public string BackEndProjectUrl { get; set; }
        public string projectDocumentUrl { get; set; }
        public string ProjectStatus { get; set; }
        public string CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public bool IsTrash { get; set; }

    }
}

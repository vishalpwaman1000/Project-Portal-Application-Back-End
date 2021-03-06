using DownloadGitProjects.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.DataAccessLayer
{
    public interface IProjectPortalDL
    {
        public Task<SignUpResponse> SignUp(SignUpRequest request);

        public Task<SignInResponse> SignIn(SignInRequest request);

        // Admin API'S

        public Task<GetProjectListResponse> GetProjectList(GetProjectListRequest request);

        public Task<UploadProjectDetailResponse> UploadProjectDetail(UploadProjectDetailRequest request);

        public Task<UpdateProjectDetailResponse> UpdateProjectDetail(UpdateProjectDetailRequest request);

        public Task<UpdateStatusResponse> UpdateStatus(UpdateStatusRequest request);

        public Task<UpdateProjectAvailabilityStatusResponse> UpdateProjectAvailabilityStatus(UpdateProjectAvailabilityStatusRequest request);

        public Task<DeleteProjectPermanentlyResponse> DeleteProjectPermanently(DeleteProjectPermanentlyRequest request);

        // Customer API'S

        public Task<GetCustomerProjectListResponse> GetCustomerProjectList(GetCustomerProjectListRequest request);
        public Task<UpdateCustomerAsPrimeResponse> UpdateCustomerAsPrime(UpdateCustomerAsPrimeRequest request);
        public Task<AddFeedbackResponse> AddFeedback(AddFeedbackRequest request);
        public Task<GetFeedbackDetailsResponse> GetFeedbackDetails(GetFeedbackDetailsRequest request);
    }
}

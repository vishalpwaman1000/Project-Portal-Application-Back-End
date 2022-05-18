using DownloadGitProjects.DataAccessLayer;
using DownloadGitProjects.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class DownloadProjectController : ControllerBase
    {
        private readonly IDownloadProjectDL _downloadProjectDL;

        public DownloadProjectController(IDownloadProjectDL downloadProjectDL)
        {
            _downloadProjectDL = downloadProjectDL;
        }

        [HttpPost]
        public async Task<ActionResult> SignUp(SignUpRequest request)
        {
            SignUpResponse response = new SignUpResponse();
            try
            {
                response = await _downloadProjectDL.SignUp(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> SignIn(SignInRequest request)
        {
            SignInResponse response = new SignInResponse();
            try
            {
                response = await _downloadProjectDL.SignIn(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProjectDetail(UploadProjectDetailRequest request)
        {
            UploadProjectDetailResponse response = new UploadProjectDetailResponse();

            try
            {

                response = await _downloadProjectDL.UploadProjectDetail(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exceptions Occurs : " + ex.Message;
            }

            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateProjectDetail(UpdateProjectDetailRequest request)
        {
            UpdateProjectDetailResponse response = new UpdateProjectDetailResponse();

            try
            {

                response = await _downloadProjectDL.UpdateProjectDetail(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exceptions Occurs : " + ex.Message;
            }

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> GetProjectList(GetProjectListRequest request)
        {
            GetProjectListResponse response = new GetProjectListResponse();

            try
            {

                response = await _downloadProjectDL.GetProjectList(request);

            }catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exceptions Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(UpdateStatusRequest request)
        {
            UpdateStatusResponse response = new UpdateStatusResponse();

            try
            {

                response = await _downloadProjectDL.UpdateStatus(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exceptions Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProjectAvailabilityStatus(UpdateProjectAvailabilityStatusRequest request)
        {
            UpdateProjectAvailabilityStatusResponse response = new UpdateProjectAvailabilityStatusResponse();

            try
            {

                response = await _downloadProjectDL.UpdateProjectAvailabilityStatus(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exceptions Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProjectPermanently(DeleteProjectPermanentlyRequest request)
        {
            DeleteProjectPermanentlyResponse response = new DeleteProjectPermanentlyResponse();

            try
            {

                response = await _downloadProjectDL.DeleteProjectPermanently(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exceptions Occurs : " + ex.Message;
            }

            return Ok(response);
        }

    }
}

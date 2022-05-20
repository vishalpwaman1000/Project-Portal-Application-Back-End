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
    public class ProjectCustomerController : ControllerBase
    {
        private readonly IProjectPortalDL _projectPortalDL;
        public ProjectCustomerController(IProjectPortalDL projectPortalDL)
        {
            _projectPortalDL = projectPortalDL;
        }

        [HttpPost]
        public async Task<IActionResult> GetCustomerProjectList(GetCustomerProjectListRequest request)
        {
            GetCustomerProjectListResponse response = new GetCustomerProjectListResponse();

            try
            {

                response = await _projectPortalDL.GetCustomerProjectList(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exceptions Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateCustomerAsPrime(UpdateCustomerAsPrimeRequest request)
        {
            UpdateCustomerAsPrimeResponse response = new UpdateCustomerAsPrimeResponse();

            try
            {

                response = await _projectPortalDL.UpdateCustomerAsPrime(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exceptions Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedback(AddFeedbackRequest request)
        {
            AddFeedbackResponse response = new AddFeedbackResponse();

            try
            {

                response = await _projectPortalDL.AddFeedback(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exceptions Occurs : " + ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetFeedbackDetails(GetFeedbackDetailsRequest request)
        {
            GetFeedbackDetailsResponse response = new GetFeedbackDetailsResponse();

            try
            {

                response = await _projectPortalDL.GetFeedbackDetails(request);

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

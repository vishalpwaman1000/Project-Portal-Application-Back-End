using DownloadGitProjects.Model;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadGitProjects.DataAccessLayer
{
    public class ProjectPortalDL : IProjectPortalDL
    {
        private readonly IConfiguration _configuration;
        private readonly MySqlConnection _mySqlConnection;
        public ProjectPortalDL(IConfiguration configuration)
        {
            _configuration = configuration;
            _mySqlConnection = new MySqlConnection(_configuration["ConnectionString:MySqlConnection"]);
        }

        public async Task<SignInResponse> SignIn(SignInRequest request)
        {
            SignInResponse response = new SignInResponse();
            response.IsSuccess = true;
            response.Message = "Sign In Successful";
            try
            {

                if (_mySqlConnection != null && _mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong with Mysql Connection";
                    return response;
                }

                string SqlQuery = @"SELECT * 
                                    FROM projectportalapplication.userdetail 
                                    WHERE UserName=@UserName AND PassWord=@PassWord AND Role=@Role;";

                using (MySqlCommand sqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@UserName", request.UserName);
                    sqlCommand.Parameters.AddWithValue("@PassWord", request.Password);
                    sqlCommand.Parameters.AddWithValue("@Role", request.Role);
                    using (DbDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (dataReader.HasRows)
                        {
                            response.Message = "Login Successfully";
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Login Unsuccessfully";
                            return response;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {

            }

            return response;
        }

        public async Task<SignUpResponse> SignUp(SignUpRequest request)
        {
            SignUpResponse response = new SignUpResponse();
            response.IsSuccess = true;
            response.Message = "Sign Up Successful";
            try
            {
                if (_mySqlConnection != null && _mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong with Mysql Connection";
                    return response;
                }

                if (!request.Password.Equals(request.ConfigPassword))
                {
                    response.IsSuccess = false;
                    response.Message = "Password & Confirm Password not Match";
                    return response;
                }

                string SqlQuery = @"INSERT INTO projectportalapplication.userdetail 
                                    (UserName, PassWord, Role) VALUES 
                                    (@UserName, @PassWord, @Role)";

                using (MySqlCommand sqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@UserName", request.UserName);
                    sqlCommand.Parameters.AddWithValue("@PassWord", request.Password);
                    sqlCommand.Parameters.AddWithValue("@Role", request.Role.ToLowerInvariant());
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {

            }

            return response;
        }

        // Admin API'S 

        public async Task<GetProjectListResponse> GetProjectList(GetProjectListRequest request)
        {
            GetProjectListResponse response = new GetProjectListResponse();
            response.IsSuccess = true;
            response.Message = "Fetch Project Detail Successfully";

            try
            {

                if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong with Mysql Connection";
                    return response;
                }

                int Offset = (request.PageNumber - 1) * request.NumberOfRecordPerPage;

                //Active, Archive, Trash
                string SqlQuery = string.Empty;
                if (request.Operation.ToLowerInvariant() == "active")
                {
                    SqlQuery = @"
                                    SELECT ProjectID, ProjectName, ProjectDescription,
                                            (SELECT COUNT(*) FROM crudoperation.ProjectDownload WHERE IsTrash=0 AND IsArchive=0) AS TotalRecord,
                                           FrontEndProjectUrl, BackEndProjectUrl, projectDocumentUrl, ProjectStatus , CreatedDate, IsActive , IsArchive, IsTrash
                                    FROM projectportalapplication.ProjectDownload
                                    WHERE IsTrash=0 AND IsArchive=0
                                    ORDER BY ProjectID DESC
                                    LIMIT @Offset, @NumberOfRecordPerPage
                                    
                                    ";
                }
                else if (request.Operation.ToLowerInvariant() == "archive")
                {
                    SqlQuery = @"
                                 SELECT ProjectID, ProjectName, ProjectDescription,
                                        (SELECT COUNT(*) FROM crudoperation.ProjectDownload WHERE IsArchive=1) AS TotalRecord,
                                        FrontEndProjectUrl, BackEndProjectUrl, projectDocumentUrl, ProjectStatus , CreatedDate, IsActive , IsArchive, IsTrash
                                    FROM projectportalapplication.ProjectDownload
                                    WHERE IsArchive=1
                                    ORDER BY ProjectID DESC
                                    LIMIT @Offset, @NumberOfRecordPerPage
                                ";
                }
                else
                {
                    SqlQuery = @"
                                SELECT ProjectID, ProjectName, ProjectDescription,
                                       (SELECT COUNT(*) FROM crudoperation.ProjectDownload WHERE IsTrash=1) AS TotalRecord,
                                       FrontEndProjectUrl, BackEndProjectUrl, projectDocumentUrl, ProjectStatus , CreatedDate , IsActive , IsArchive, IsTrash
                                    FROM projectportalapplication.ProjectDownload
                                    WHERE IsTrash=1
                                    ORDER BY ProjectID DESC
                                    LIMIT @Offset, @NumberOfRecordPerPage
                                ";
                }

                using (MySqlCommand mySqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.CommandTimeout = 180;
                    mySqlCommand.Parameters.AddWithValue("@Offset", Offset);
                    mySqlCommand.Parameters.AddWithValue("@NumberOfRecordPerPage", request.NumberOfRecordPerPage);
                    using (DbDataReader dbDataReader = await mySqlCommand.ExecuteReaderAsync())
                    {
                        if (dbDataReader.HasRows)
                        {
                            response.data = new List<GetProjectList>();
                            int Count = 0;
                            while (await dbDataReader.ReadAsync())
                            {
                                //ProjectName, FrontEndProjectUrl, BackEndProjectUrl, projectDocumentUrl, ProjectStatus, IsActive
                                response.data.Add(new GetProjectList()
                                {
                                    ProjectID = dbDataReader["ProjectID"] != DBNull.Value ? Convert.ToInt32(dbDataReader["ProjectID"]) : -1,
                                    ProjectName = dbDataReader["ProjectName"] != DBNull.Value ? Convert.ToString(dbDataReader["ProjectName"]) : string.Empty,
                                    ProjectDescription = dbDataReader["ProjectDescription"] != DBNull.Value ? Convert.ToString(dbDataReader["ProjectDescription"]) : string.Empty,
                                    FrontEndProjectUrl = dbDataReader["FrontEndProjectUrl"] != DBNull.Value ? Convert.ToString(dbDataReader["FrontEndProjectUrl"]) : string.Empty,
                                    BackEndProjectUrl = dbDataReader["BackEndProjectUrl"] != DBNull.Value ? Convert.ToString(dbDataReader["BackEndProjectUrl"]) : string.Empty,
                                    projectDocumentUrl = dbDataReader["projectDocumentUrl"] != DBNull.Value ? Convert.ToString(dbDataReader["projectDocumentUrl"]) : string.Empty,
                                    ProjectStatus = dbDataReader["ProjectStatus"] != DBNull.Value ? Convert.ToString(dbDataReader["ProjectStatus"]) : string.Empty,
                                    CreatedDate = dbDataReader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(dbDataReader["CreatedDate"]).ToString("dddd, dd MMMM yyyy hh:mm tt") : string.Empty,
                                    IsActive = dbDataReader["IsActive"] != DBNull.Value ? Convert.ToBoolean(dbDataReader["IsActive"]) : false,
                                    IsArchive = dbDataReader["IsArchive"] != DBNull.Value ? Convert.ToBoolean(dbDataReader["IsArchive"]) : false,
                                    IsTrash = dbDataReader["IsTrash"] != DBNull.Value ? Convert.ToBoolean(dbDataReader["IsTrash"]) : false,
                                });

                                if (Count == 0)
                                {
                                    Count++;
                                    response.TotalRecords = dbDataReader["TotalRecord"] != DBNull.Value ? Convert.ToInt32(dbDataReader["TotalRecord"]) : -1;
                                    response.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(response.TotalRecords / request.NumberOfRecordPerPage)));
                                    response.CurrentPage = request.PageNumber;
                                }
                            }

                        }
                        else
                        {
                            response.Message = "No Project List Found";
                        }
                    }

                }



            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<UpdateStatusResponse> UpdateStatus(UpdateStatusRequest request)
        {
            UpdateStatusResponse response = new UpdateStatusResponse();
            response.IsSuccess = true;
            response.Message = "Update Project Successful";

            try
            {

                if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong with Mysql Connection";
                    return response;
                }

                //Active, Archive, Trash
                string SqlQuery = string.Empty;
                if (request.Operation.ToLowerInvariant() == "active")
                {
                    // Trash => Active Or Archive => Active
                    response.Message = "Move to Active Project";
                    SqlQuery = @"
                                 UPDATE projectportalapplication.ProjectDownload
                                 SET IsArchive=0, IsTrash=0
                                 WHERE ProjectID=@ProjectID
                                ";

                }
                else
                if (request.Operation.ToLowerInvariant() == "inactive")
                {
                    // Active => Inactive
                    response.Message = "Move To InActive Project";
                    SqlQuery = @"
                                   UPDATE projectportalapplication.ProjectDownload
                                   SET IsActive=0
                                   WHERE ProjectID=@ProjectID
                                ";
                }
                else if (request.Operation.ToLowerInvariant() == "archive")
                {
                    // Active => Archive
                    response.Message = "Move To Archive Project";
                    SqlQuery = @"
                                   UPDATE projectportalapplication.ProjectDownload
                                   SET IsArchive=1 , IsTrash=0
                                   WHERE ProjectID=@ProjectID
                                ";
                }
                else
                {
                    // Active => Trash
                    response.Message = "Move To Trash Project";
                    SqlQuery = @"
                                   UPDATE projectportalapplication.ProjectDownload
                                    SET IsTrash=1, IsArchive=0
                                    WHERE ProjectID=@ProjectID
                                ";
                }



                using (MySqlCommand mySqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.CommandTimeout = 180;
                    mySqlCommand.Parameters.AddWithValue("@ProjectID", request.ProjectID); // Public=1,Private=0
                    int Status = await mySqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                        return response;
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<UploadProjectDetailResponse> UploadProjectDetail(UploadProjectDetailRequest request)
        {
            UploadProjectDetailResponse response = new UploadProjectDetailResponse();
            response.IsSuccess = true;
            response.Message = "New Project Added";

            try
            {

                if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong with Mysql Connection";
                    return response;
                }

                string SqlQuery = @"INSERT INTO projectportalapplication.ProjectDownload (ProjectName, ProjectDescription, FrontEndProjectUrl, BackEndProjectUrl, projectDocumentUrl, ProjectStatus, IsActive)
                                    VALUES (@ProjectName, @ProjectDescription, @FrontEndProjectUrl, @BackEndProjectUrl, @projectDocumentUrl, @ProjectStatus, @IsActive)";

                using (MySqlCommand mySqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.CommandTimeout = 180;
                    mySqlCommand.Parameters.AddWithValue("@ProjectName", request.ProjectName);
                    mySqlCommand.Parameters.AddWithValue("@ProjectDescription", request.ProjectDescription);
                    mySqlCommand.Parameters.AddWithValue("@FrontEndProjectUrl", request.FrontEndProjectUrl);
                    mySqlCommand.Parameters.AddWithValue("@BackEndProjectUrl", request.BackEndProjectUrl);
                    mySqlCommand.Parameters.AddWithValue("@projectDocumentUrl", request.BackEndProjectUrl);
                    mySqlCommand.Parameters.AddWithValue("@ProjectStatus", request.ProjectStatus); // Public , Private
                    mySqlCommand.Parameters.AddWithValue("@IsActive", request.IsActive ? 1 : 0); // Public , Private
                    int Status = await mySqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                        return response;
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<UpdateProjectDetailResponse> UpdateProjectDetail(UpdateProjectDetailRequest request)
        {
            UpdateProjectDetailResponse response = new UpdateProjectDetailResponse();
            response.IsSuccess = true;
            response.Message = "Update Successfully";

            try
            {

                if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Something Went Wrong with Mysql Connection";
                    return response;
                }

                string SqlQuery = @"UPDATE projectportalapplication.ProjectDownload 
                                    SET ProjectName=@ProjectName, 
                                        ProjectDescription=@ProjectDescription, 
                                        FrontEndProjectUrl=@FrontEndProjectUrl, 
                                        BackEndProjectUrl=@BackEndProjectUrl, 
                                        projectDocumentUrl=@projectDocumentUrl, 
                                        ProjectStatus=@ProjectStatus,
                                        IsActive=@IsActive
                                    WHERE ProjectID=@ProjectID";

                using (MySqlCommand mySqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.CommandTimeout = 180;
                    mySqlCommand.Parameters.AddWithValue("@ProjectID", request.ProjectID);
                    mySqlCommand.Parameters.AddWithValue("@ProjectName", request.ProjectName);
                    mySqlCommand.Parameters.AddWithValue("@ProjectDescription", request.ProjectDescription);
                    mySqlCommand.Parameters.AddWithValue("@FrontEndProjectUrl", request.FrontEndProjectUrl);
                    mySqlCommand.Parameters.AddWithValue("@BackEndProjectUrl", request.BackEndProjectUrl);
                    mySqlCommand.Parameters.AddWithValue("@projectDocumentUrl", request.BackEndProjectUrl);
                    mySqlCommand.Parameters.AddWithValue("@ProjectStatus", request.ProjectStatus); // Public , Private
                    mySqlCommand.Parameters.AddWithValue("@IsActive", request.IsActive ? 1 : 0);
                    int Status = await mySqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                        return response;
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<UpdateProjectAvailabilityStatusResponse> UpdateProjectAvailabilityStatus(UpdateProjectAvailabilityStatusRequest request)
        {
            UpdateProjectAvailabilityStatusResponse response = new UpdateProjectAvailabilityStatusResponse();
            response.IsSuccess = true;
            response.Message = "Change Project Status Successfully";

            try
            {

                if (!(request.AvailabiltyStatus.ToLowerInvariant() == "public" ||
                     request.AvailabiltyStatus.ToLowerInvariant() == "private"))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid Availability Status  Example : Public , Private";
                    return response;
                }

                if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"
                                    UPDATE projectportalapplication.ProjectDownload
                                    SET ProjectStatus=@ProjectStatus
                                    WHERE ProjectID=@ProjectID
                                    ";

                using (MySqlCommand mySqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.CommandTimeout = 180;
                    mySqlCommand.Parameters.AddWithValue("@ProjectID", request.ProjectID);
                    mySqlCommand.Parameters.AddWithValue("@ProjectStatus", request.AvailabiltyStatus);
                    int Status = await mySqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went To Wrong";
                        return response;
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }

            return response;
        }

        public async Task<DeleteProjectPermanentlyResponse> DeleteProjectPermanently(DeleteProjectPermanentlyRequest request)
        {
            DeleteProjectPermanentlyResponse response = new DeleteProjectPermanentlyResponse();
            response.IsSuccess = true;
            response.Message = "Delete Project Successfully";
            try
            {

                if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                String SqlQuery = @"
                                    DELETE 
                                    FROM projectportalapplication.projectdownload
                                    where ProjectID=@ProjectID
                                    ";

                using (MySqlCommand mySqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.CommandTimeout = 180;
                    mySqlCommand.Parameters.AddWithValue("@ProjectID", request.ProjectID);
                    int Status = await mySqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                        return response;
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }

        // Customer API'S


        public async Task<GetCustomerProjectListResponse> GetCustomerProjectList(GetCustomerProjectListRequest request)
        {
            GetCustomerProjectListResponse response = new GetCustomerProjectListResponse();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {

                if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = string.Empty;

                if (request.Operation.ToLowerInvariant().Trim() == "public")
                {
                    //Public
                    SqlQuery = @"
                                    SELECT ProjectID, ProjectName, ProjectDescription,
                                            (SELECT COUNT(*) FROM crudoperation.ProjectDownload WHERE IsTrash=0 AND IsArchive=0) AS TotalRecord,
                                           FrontEndProjectUrl, BackEndProjectUrl, projectDocumentUrl, ProjectStatus , CreatedDate, IsActive , IsArchive, IsTrash
                                    FROM projectportalapplication.ProjectDownload
                                    WHERE IsActive=1 AND ProjectStatus = 'Public'
                                    ORDER BY ProjectID DESC
                                    LIMIT @Offset, @NumberOfRecordPerPage
                                ";
                }
                else
                {
                    //Private
                    SqlQuery = @"
                                    SELECT ProjectID, ProjectName, ProjectDescription,
		                                    (SELECT COUNT(*) FROM crudoperation.ProjectDownload WHERE IsTrash=0 AND IsArchive=0) AS TotalRecord,
	                                       FrontEndProjectUrl, BackEndProjectUrl, projectDocumentUrl, ProjectStatus , CreatedDate, IsActive , IsArchive, IsTrash
                                    FROM projectportalapplication.ProjectDownload
                                    WHERE IsActive=1 AND 
                                          ProjectStatus = 'Private' AND 
                                          (Select IsPrimeUser from projectportalapplication.userdetail where UserId=@UserId) ='1'
                                    ORDER BY ProjectID DESC
                                    LIMIT @Offset, @NumberOfRecordPerPage
                                ";
                }

                int Offset = (request.PageNumber - 1) * request.NumberOfRecordPerPage;

                using (MySqlCommand mySqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.CommandTimeout = 180;
                    mySqlCommand.Parameters.AddWithValue("@UserId", request.UserId);
                    mySqlCommand.Parameters.AddWithValue("@Offset", Offset);
                    mySqlCommand.Parameters.AddWithValue("@NumberOfRecordPerPage", request.NumberOfRecordPerPage);
                    using (DbDataReader dbDataReader = await mySqlCommand.ExecuteReaderAsync())
                    {
                        if (dbDataReader.HasRows)
                        {
                            response.data = new List<GetCustomerProjectList>();
                            int Count = 0;
                            while (await dbDataReader.ReadAsync())
                            {
                                //ProjectName, FrontEndProjectUrl, BackEndProjectUrl, projectDocumentUrl, ProjectStatus, IsActive
                                response.data.Add(new GetCustomerProjectList()
                                {
                                    ProjectID = dbDataReader["ProjectID"] != DBNull.Value ? Convert.ToInt32(dbDataReader["ProjectID"]) : -1,
                                    ProjectName = dbDataReader["ProjectName"] != DBNull.Value ? Convert.ToString(dbDataReader["ProjectName"]) : string.Empty,
                                    ProjectDescription = dbDataReader["ProjectDescription"] != DBNull.Value ? Convert.ToString(dbDataReader["ProjectDescription"]) : string.Empty,
                                    FrontEndProjectUrl = dbDataReader["FrontEndProjectUrl"] != DBNull.Value ? Convert.ToString(dbDataReader["FrontEndProjectUrl"]) : string.Empty,
                                    BackEndProjectUrl = dbDataReader["BackEndProjectUrl"] != DBNull.Value ? Convert.ToString(dbDataReader["BackEndProjectUrl"]) : string.Empty,
                                    projectDocumentUrl = dbDataReader["projectDocumentUrl"] != DBNull.Value ? Convert.ToString(dbDataReader["projectDocumentUrl"]) : string.Empty,
                                    ProjectStatus = dbDataReader["ProjectStatus"] != DBNull.Value ? Convert.ToString(dbDataReader["ProjectStatus"]) : string.Empty,
                                    CreatedDate = dbDataReader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(dbDataReader["CreatedDate"]).ToString("dddd, dd MMMM yyyy hh:mm tt") : string.Empty,
                                });

                                if (Count == 0)
                                {
                                    Count++;
                                    response.TotalRecords = dbDataReader["TotalRecord"] != DBNull.Value ? Convert.ToInt32(dbDataReader["TotalRecord"]) : -1;
                                    response.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(response.TotalRecords / request.NumberOfRecordPerPage)));
                                    response.CurrentPage = request.PageNumber;
                                }
                            }

                        }
                        else
                        {
                            response.Message = "No Project List Found";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<UpdateCustomerAsPrimeResponse> UpdateCustomerAsPrime(UpdateCustomerAsPrimeRequest request)
        {
            UpdateCustomerAsPrimeResponse response = new UpdateCustomerAsPrimeResponse();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {

                if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @" UPDATE projectportalapplication.userdetail
                                     SET IsPrimeUser = 1
                                     WHERE UserId=@UserID";

                using (MySqlCommand mySqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.CommandTimeout = 180;
                    mySqlCommand.Parameters.AddWithValue("@UserID", request.UserID);
                    int Status = await mySqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<AddFeedbackResponse> AddFeedback(AddFeedbackRequest request)
        {
            AddFeedbackResponse response = new AddFeedbackResponse();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {

                if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"
                                    INSERT INTO projectportalapplication.feedbackdetail
                                    (CreatedBy, FeedbackDetail) values (@CreatedBy, @FeedbackDetail);
                                    ";

                using (MySqlCommand mySqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.CommandTimeout = 180;
                    mySqlCommand.Parameters.AddWithValue("@CreatedBy", request.UserID);
                    mySqlCommand.Parameters.AddWithValue("@FeedbackDetail", request.Feedback);
                    int Status = await mySqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Something Went Wrong";
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<GetFeedbackDetailsResponse> GetFeedbackDetails(GetFeedbackDetailsRequest request)
        {
            GetFeedbackDetailsResponse response = new GetFeedbackDetailsResponse();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {

                if (_mySqlConnection != null && _mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                string SqlQuery = @"
                                    SELECT FeedbackID,
                                    (select Count(*) from projectportalapplication.feedbackdetail) as TotalRecord,
                                    CreatedDate, CreatedBy, FeedbackDetail, IsActive
                                    FROM projectportalapplication.feedbackdetail
                                    ORDER BY FeedbackID DESC
                                    LIMIT @Offset, @NumberOfRecordPerPage";

                int Offset = (request.PageNumber - 1) * request.NumberOfRecordPerPage;

                using (MySqlCommand mySqlCommand = new MySqlCommand(SqlQuery, _mySqlConnection))
                {
                    mySqlCommand.CommandType = CommandType.Text;
                    mySqlCommand.CommandTimeout = 180;
                    mySqlCommand.Parameters.AddWithValue("@Offset", Offset);
                    mySqlCommand.Parameters.AddWithValue("@NumberOfRecordPerPage", request.NumberOfRecordPerPage);

                    using (DbDataReader dataReader = await mySqlCommand.ExecuteReaderAsync())
                    {
                        if (dataReader.HasRows)
                        {
                            response.data = new List<GetFeedbackDetails>();
                            int Count = 0;
                            while (await dataReader.ReadAsync())
                            {
                                //FeedbackID, CratedDate, CreatedBy, FeedbackDetail, IsActive
                                response.data.Add(
                                    new GetFeedbackDetails()
                                    {
                                        FeedbackID = dataReader["FeedbackID"] != DBNull.Value ? (int)dataReader["FeedbackID"] : -1,
                                        CreatedDate = dataReader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["CreatedDate"]).ToString("dddd, dd MMMM yyyy hh:mm tt") : string.Empty,
                                        CreatedBy = dataReader["CreatedBy"] != DBNull.Value ? (int)dataReader["CreatedBy"] : -1,
                                        FeedbackDetail = dataReader["FeedbackDetail"] != DBNull.Value ? (string)dataReader["FeedbackDetail"] : string.Empty,
                                        IsActive = dataReader["IsActive"] != DBNull.Value ? Convert.ToBoolean(dataReader["IsActive"]) : false,
                                    }
                                    )  ;

                                if (Count == 0)
                                {
                                    Count++;
                                    response.TotalRecords = dataReader["TotalRecord"] != DBNull.Value ? Convert.ToInt32(dataReader["TotalRecord"]) : -1;
                                    response.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(response.TotalRecords / request.NumberOfRecordPerPage)));
                                    response.CurrentPage = request.PageNumber;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception Occurs : " + ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }
    }
}

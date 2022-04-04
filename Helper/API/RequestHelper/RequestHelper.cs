using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SehaBMC.Common.Helpers.DataSync;
using SehaBMC.Common.Models.Cancel.Request.BMCAPI;
using SehaBMC.Common.Models.Request.BMCAPI;
using SehaBMC.Common.Models.Responses;
using SehaBMC.Common.Models.Responses.API;
using SehaBMC.Common.Models.Responses.BMCAPI;
using SehaBMC.Common.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static SehaBMC.Common.Helpers.ConstantsHelper;

namespace Bot.Helper.API
{
  public class RequestsHelper : DataSyncHelper, IRequestsHelper
  {
    private readonly BMCOptions _BMCOptions;
    public RequestsHelper(IOptions<BMCOptions> options) : base(options)
    {
      _BMCOptions = options.Value;
    }

    public async Task<Result> GetRequestsFilteredAsync(string filter, int? count = null, int? offset = null, string nextPageUri = null, string sort = null)
    {
      using var client = await GetHttpClientAsync();
      var result = new Result();
      if (!string.IsNullOrWhiteSpace(nextPageUri))
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync(nextPageUri));
      else if (count != null && offset != null && string.IsNullOrWhiteSpace(filter))
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetRequests]}&offset={offset}&limit={count}"));
      else if (!string.IsNullOrWhiteSpace(filter) && count != null && offset != null && string.IsNullOrWhiteSpace(sort))
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetRequests]}&q={filter}&offset={offset}&limit={count}"));
      else if (!string.IsNullOrWhiteSpace(filter) && count != null && offset != null && !string.IsNullOrWhiteSpace(sort))
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetRequests]}&q={filter}&offset={offset}&limit={count}&sort={sort}"));
      else if (!string.IsNullOrWhiteSpace(filter) && !string.IsNullOrWhiteSpace(sort))
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetRequests]}&q={filter}&sort={sort}"));
      else if (!string.IsNullOrWhiteSpace(filter))
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetRequests]}&q={filter}"));
      else
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync(_BMCOptions.Uris[URL.GetRequests]));

      result.entries.Where(x => x.values.Status == RequestStatusTypes.Planning)
          .ToList().ForEach(y => y.values.Status = RequestStatusTypes.Initiated);

      return result;
    }

    public async Task<GetRequestModel> GetRequestAsync(string requestId)
    {
      using var client = await GetHttpClientAsync();
      var requestResult = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetRequests]}&q='Request Number'=\"{requestId}\""));
      if (requestResult.entries.Any())
      {
        var workInfoResult = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetWorkInfo]}&q='SR_RequestNumber'=\"{requestId}\""));
        var approvalsResult = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetApprovals]}&q='Request Number'=\"{requestId}\""));
        return new GetRequestModel { Request = requestResult, WorkInfo = workInfoResult, Approvals = approvalsResult };
      }
      return null;
    }

    public async Task<HttpResponseMessage> CancelRequestAsync(string requestNumber)
    {
      using var client = await GetHttpClientAsync();
      var requestResult = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetRequests]}&q='Request Number'=\"{requestNumber}\""));

      var request = requestResult?.entries?.FirstOrDefault();
      if (request == null)
        throw new Exception($"Request with {requestNumber} not found.");

      if (request.values.Status.Equals(RequestStatusTypes.Rejected)
          || request.values.Status.Equals(RequestStatusTypes.Closed)
          || request.values.Status.Equals(RequestStatusTypes.Cancelled))
        throw new Exception($"Request {request.values.RequestNumber} is not in open state to cancel.");

      if (string.IsNullOrWhiteSpace(request?._links?.self?.FirstOrDefault()?.href))
        throw new Exception($"Request href not found for request {request.values.RequestNumber}.");


      var model = new CancelRequestModel
      {
        values = new Models.Cancel.Request.BMCAPI.Values
        {
          z1DAction = CancelRequestVariables.z1DAction,
          Status = CancelRequestVariables.Status,
          Status_Reason = CancelRequestVariables.StatusReason,
        }
      };

      return await client.PutAsync(request._links.self.FirstOrDefault().href,
              new StringContent(JsonConvert.SerializeObject(model),
                  Encoding.UTF8, "application/json"));

    }

    public async Task<HttpResponseMessage> RequestApprovalAsync(Models.Responses.BMCAPI.Values value, string token, string approvalType)
    {
      var model = new ApiRequestModel
      {
        values = new SehaBMC.Common.Models.Request.BMCAPI.Values
        {
          IntegrationType = PostRequestVariables.IntegrationType,
          TicketNumber = value.RequestNumber,
          Direction = PostRequestVariables.Direction,
          Status = PostRequestVariables.Status,
          AssignedTo = GetEmailFromAadToken(token),
          Body = value.Body,
          InstanceID = value.InstanceID,
          Label3 = value.ShortDescription,
          ShortDescription = approvalType.Equals(PostRequestVariables.Approve) ? PostRequestVariables.ApproveShortDescription : PostRequestVariables.RejectionShortDescription,
          TransactionType = PostRequestVariables.ApprovalTransactionType,
        }
      };

      using var client = await GetHttpClientAsync();
      var request = await client.PostAsync(_BMCOptions.Uris[URL.PostRequest], new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
      return request;
    }

    public async Task<bool> WorkInfoUpdateAsync(Models.WorkInfo.Request.BMCAPI.Values value, string token)
    {
      var user = await GetUserByUpnAsync(GetEmailFromAadToken(token));
      var model = new SehaBMC.Common.Models.WorkInfo.Request.BMCAPI.WorkInfoUpdateModel
      {
        values = new SehaBMC.Common.Models.WorkInfo.Request.BMCAPI.Values
        {
          RequestNumber = value.RequestNumber,
          z1DAction = PostRequestWorkInfoUpdateVariables.z1DAction,
          SRWorkInfoType = PostRequestWorkInfoUpdateVariables.SRWorkInfoTypeAction,
          SR_RequestNumber = value.RequestNumber,
          Summary = value.Summary,
          WorkInfoSubmitter = string.IsNullOrWhiteSpace(user?.entries?.FirstOrDefault()?.values?.RemedyLoginId) ? value.WorkInfoSubmitter : user?.entries?.FirstOrDefault()?.values?.RemedyLoginId,
          SRID = value.RequestNumber,
          SecureLog = PostRequestWorkInfoUpdateVariables.SecureLog,
          ViewAccess = PostRequestWorkInfoUpdateVariables.ViewAccess,
          WorkInfoType = PostRequestWorkInfoUpdateVariables.WorkInfoType,
          SRInstanceId = value.SRInstanceId,
          Notes = value.Notes,
          Status = PostRequestWorkInfoUpdateVariables.Status,
          Submitter = string.IsNullOrWhiteSpace(user?.entries?.FirstOrDefault()?.values?.RemedyLoginId) ? value.Submitter : user?.entries?.FirstOrDefault()?.values?.RemedyLoginId,
          NumberofAttachments = PostRequestWorkInfoUpdateVariables.NumberofAttachments,
          z2AF_Attachment1 = value.z2AF_Attachment1,
        }
      };

      if (string.IsNullOrWhiteSpace(value.z2AF_Attachment1) || string.IsNullOrWhiteSpace(value.z2AF_Attachment1Base64))
      {
        var client = await GetHttpClientAsync();

        model.values.z2AF_Attachment1 = null;
        model.values.NumberofAttachments = null;

        return (await client.PostAsync(_BMCOptions.Uris[URL.PostWorkInfoUpdateRequest], new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"))).IsSuccessStatusCode;
      }
      return (await PostAttachmentRequestRestSharp(model, _BMCOptions.Uris[URL.PostWorkInfoUpdateRequest], value.z2AF_Attachment1Base64, model.values.z2AF_Attachment1, "attach-z2AF_Attachment1")).IsSuccessful;
    }

    public async Task<HttpResponseMessage> SurveySubmissionAsync(Models.Responses.BMCAPI.Values value, string token)
    {
      if ((value.SurveyRating == 1 || value.SurveyRating == 2) && string.IsNullOrWhiteSpace(value.Body))
        throw new Exception("Comments value is empty.");

      var model = new ApiRequestModel
      {
        values = new SehaBMC.Common.Models.Request.BMCAPI.Values
        {
          IntegrationType = PostRequestVariables.IntegrationType,
          TicketNumber = value.RequestNumber,
          Direction = PostRequestVariables.Direction,
          TransactionType = PostRequestVariables.SurveyResponse,
          ShortDescription = PostRequestVariables.SurveyResponse,
          AssignedTo = Convert.ToString(value.SurveyRating),
          Body = value.Body,
          Status = PostRequestVariables.Status,
          InstanceID = value.InstanceID
        }
      };

      using var client = await GetHttpClientAsync();
      var request = await client.PostAsync(_BMCOptions.Uris[URL.PostRequest], new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
      return request;
    }

    public async Task<Result> GetIncidentDropdownAsync(
        string category1,
        string category2,
        string token,
        int? count = null,
        int? offset = null)
    {
      var userDetails = await GetUserByUpnAsync(GetEmailFromAadToken(token));
      //var userDetails = await GetUserByUpnAsync("o-vnavada@seha.ae");
      var user = userDetails.entries.FirstOrDefault().values;
      var filter = $"'SRD ID' = \"" + IncidentDropdownVariables.SrdId + "\" and 'Status' = \"" + IncidentDropdownVariables.Status + "\" and 'BE' = \"" + user.Company + "\"";

      if (category1 != null && category2 == null)
        filter = $"{filter} and 'Navigational Category 1' = \"" + category1 + "\"";
      else if (category1 != null && category2 != null)
        filter = $"{filter} and 'Navigational Category 1' = \"" + category1 + "\" and 'Navigational Category 2' = \"" + category2 + "\"";

      using var client = await GetHttpClientAsync();

      Result result = new();

      if (count != null && offset != null && !string.IsNullOrWhiteSpace(filter))
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.IncidentDropdown]}&q={filter}&offset={offset}&limit={count}"));
      else if (count != null && offset != null && string.IsNullOrWhiteSpace(filter))
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.IncidentDropdown]}&offset={offset}&limit={count}"));
      else if (!string.IsNullOrWhiteSpace(filter))
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.IncidentDropdown]}&q={filter}"));
      else
        result = JsonConvert.DeserializeObject<Result>(await client.GetStringAsync(_BMCOptions.Uris[URL.IncidentDropdown]));

      if (result.entries != null && result.entries.Count > 0)
        if (category1 == null && category2 == null)
          result.entries = result.entries.GroupBy(x => x.values.NavigationalCategory1).Select(y => y.First()).ToList();
        else if (category1 != null && category2 == null)
          result.entries = result.entries.GroupBy(x => x.values.NavigationalCategory2).Select(y => y.First()).ToList();

      return result;
    }

    public async Task<bool> SubmitIncidentAsync(IncidentModel incident, string token)
    {
      var userDetails = await GetUserByUpnAsync(GetEmailFromAadToken(token));
      //var userDetails = await GetUserByUpnAsync("o-vnavada@seha.ae");
      var user = userDetails.entries.FirstOrDefault().values;

      string details = "Working From:" + incident.WorkingFrom + Environment.NewLine +
                       "Extension/Mobile Number:" + incident.MobileNumber + Environment.NewLine +
                       "Current Location/Room Number:" + incident.RoomNumber + Environment.NewLine +
                       "Option:" + incident.NavigationalCategory1 + Environment.NewLine +
                       "External Username:" + incident.ExternalUsername + Environment.NewLine +
                       "Environment:" + incident.Environment + Environment.NewLine +
                       "Error Code/Message:" + incident.ErrorCode + Environment.NewLine +
                       "Component:" + incident.NavigationalCategory2 + Environment.NewLine +
                       "Issue:" + incident.NavigationalCategory3 + Environment.NewLine +
                       "More Details:" + incident.Details + Environment.NewLine;

      var model = new IncidentRequestModel
      {
        values = new IncidentValues
        {
          Z1DAction = SubmitIncidentVariables.z1DAction,
          SourceKeyword = SubmitIncidentVariables.SourceKeyword,
          TitleInstanceID = SubmitIncidentVariables.TitleInstanceID,
          FirstName = user.FirstName,
          LastName = user.LastName,
          CustomerFirstName = user.FirstName,
          CustomerLastName = user.LastName,
          CustomerLogin = user.RemedyLoginId,
          OfferingTitle = SubmitIncidentVariables.OfferingTitle,
          ShortDescription = incident.RequestId,
          Details = details,
          Status = SubmitIncidentVariables.Status,
          SRTypeField43 = incident.RequestId,
          Company = user.Company,
          CustomerCompany = user.Company,
          LocationCompany = user.Company,
          Z1DWorkInfoType = SubmitIncidentVariables.WorkInfoType,
          Z1DWorkInfoSecureLog = SubmitIncidentVariables.SecureLog,
          Z1DWorkInfoViewAccess = SubmitIncidentVariables.ViewAccess,
          Z1DWorkInfoSummary = SubmitIncidentVariables.WorkInfoSummary,
          Z2AFWIAttachment1 = incident.AttachmentName
        }
      };

      if (string.IsNullOrWhiteSpace(incident.AttachmentName) || string.IsNullOrWhiteSpace(incident.AttachmentBase64))
      {
        var client = await GetHttpClientAsync();
        model.values.Z2AFWIAttachment1 = null;
        return (await client.PostAsync(_BMCOptions.Uris[URL.SubmitIncident], new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"))).IsSuccessStatusCode;
      }

      return (await PostAttachmentRequestRestSharp(model, _BMCOptions.Uris[URL.SubmitIncident], incident.AttachmentBase64, model.values.Z2AFWIAttachment1, "attach-z2AF_WIAttachment1")).IsSuccessful;
    }
  }
}

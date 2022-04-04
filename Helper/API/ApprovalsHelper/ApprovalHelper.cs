using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SehaBMC.Common.Helpers.DataSync;
using SehaBMC.Common.Helpers.DataSync.SyncEmployeesHelper;
using SehaBMC.Common.Models.Request.BMCAPI;
using SehaBMC.Common.Models.Responses.BMCAPI;
using SehaBMC.Common.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static SehaBMC.Common.Helpers.ConstantsHelper;

namespace Bot.Helper.API.ApprovalsHelper
{
  public class ApprovalHelper : DataSyncHelper, IApprovalHelper
  {
    private readonly BMCOptions _BMCOptions;
    private readonly ISyncEmployeesHelper _syncEmployeesHelper;
    public ApprovalHelper(IOptions<BMCOptions> options, ISyncEmployeesHelper syncEmployeesHelper) : base(options)
    {
      _BMCOptions = options.Value;
      _syncEmployeesHelper = syncEmployeesHelper;
    }
    public async Task<Result> GetApprovalsAsync(string accessToken = null, int? count = null, int? offset = null, string nextPageUri = null, string emailFromBot = null)
    {
      using var client = await GetHttpClientAsync();

      if (!string.IsNullOrWhiteSpace(nextPageUri))
        return JsonConvert.DeserializeObject<Result>(await client.GetStringAsync(nextPageUri));
      else
      {
        string upn = string.Empty;
        if (string.IsNullOrWhiteSpace(emailFromBot) && !string.IsNullOrWhiteSpace(accessToken))
          upn = GetEmailFromAadToken(accessToken);
        else if (!string.IsNullOrWhiteSpace(emailFromBot) && string.IsNullOrWhiteSpace(accessToken))
          upn = emailFromBot;
        else throw new Exception("No proper token or emailid mentioned.");

        var user = await GetUserByUpnAsync(upn);
        if (user != null)
        {
          if (count != null && offset != null)
            return JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetApprovals]}&q='Approval Status'=\"Pending\" and 'Approvers' LIKE \"%" + user.entries.FirstOrDefault().values.RemedyLoginId + "%\"&offset=" + offset + "&limit=" + count + ""));
          else
            return JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetApprovals]}&q='Approval Status'=\"Pending\" and 'Approvers' LIKE \"%" + user.entries.FirstOrDefault().values.RemedyLoginId + "%\""));
        }
        return null;
      }
    }
    public async Task<Result> GetApprovalsFiltered(string filter, int? count = null, int? offset = null, string nextPageUri = null)
    {
      using var client = await GetHttpClientAsync();
      if (!string.IsNullOrWhiteSpace(nextPageUri))
        return JsonConvert.DeserializeObject<Result>(await client.GetStringAsync(nextPageUri));
      else if (count != null && offset != null && string.IsNullOrWhiteSpace(filter))
        return JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetApprovals]}&offset={offset}&limit={count}"));
      else if (!string.IsNullOrWhiteSpace(filter) && count != null && offset != null)
        return JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetApprovals]}&q={filter}&offset={offset}&limit={count}"));
      else if (!string.IsNullOrWhiteSpace(filter))
        return JsonConvert.DeserializeObject<Result>(await client.GetStringAsync($"{_BMCOptions.Uris[URL.GetApprovals]}&q={filter}"));
      else
        return JsonConvert.DeserializeObject<Result>(await client.GetStringAsync(_BMCOptions.Uris[URL.GetApprovals]));
    }

    public async Task<HttpResponseMessage> ApprovalReassignAsync(Models.Responses.BMCAPI.Values value, string token)
    {
      var model = new ApiRequestModel
      {
        values = new Models.Request.BMCAPI.Values
        {
          IntegrationType = PostRequestVariables.IntegrationType,
          TicketNumber = value.RequestNumber,
          Direction = PostRequestVariables.Direction,
          TransactionType = PostRequestVariables.AssignmentsTransactionType,
          ShortDescription = value.Email,
          AssignedTo = GetEmailFromAadToken(token),
          Status = PostRequestVariables.Status,
          InstanceID = value.InstanceID,
          Label3 = value.ApprovalType
        }
      };

      using var client = await GetHttpClientAsync();
      var request = await client.PostAsync(_BMCOptions.Uris[URL.PostRequest], new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
      return request;
    }
  }
}

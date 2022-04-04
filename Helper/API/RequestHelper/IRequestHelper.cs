using RestSharp;
using SehaBMC.Common.Models.Database.API;
using SehaBMC.Common.Models.Responses.API;
using SehaBMC.Common.Models.Responses.BMCAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Helper.API
{
  public interface IRequestsHelper
  {
    Task<Result> GetRequestsFilteredAsync(string filter, int? count = null, int? offset = null, string nextPageUri = null, string sort = null);
    Task<GetRequestModel> GetRequestAsync(string requestId);
    Task<HttpResponseMessage> RequestApprovalAsync(Values value, string token, string approvalType);
    Task<bool> WorkInfoUpdateAsync(Models.WorkInfo.Request.BMCAPI.Values value, string token);
    Task<HttpResponseMessage> CancelRequestAsync(string requestNumber);
    Task<HttpResponseMessage> SurveySubmissionAsync(Models.Responses.BMCAPI.Values value, string token);
    Task<Result> GetIncidentDropdownAsync(
        string category1,
        string category2,
        string token,
        int? count = null,
        int? offset = null);
    Task<bool> SubmitIncidentAsync(Models.Request.BMCAPI.IncidentModel incident, string token);
  }
}

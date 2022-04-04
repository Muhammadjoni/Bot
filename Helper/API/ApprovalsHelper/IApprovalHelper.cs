using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Helper.API.ApprovalsHelper
{
  public interface IApprovalHelper
  {
    Task<Result> GetApprovalsAsync(string accessToken = null, int? count = null, int? offset = null, string nextPageUri = null, string emailFromBot = null);
    Task<Result> GetApprovalsFiltered(string filter, int? count = null, int? offset = null, string nextPageUri = null);
    Task<HttpResponseMessage> ApprovalReassignAsync(Values value, string token);

  }
}

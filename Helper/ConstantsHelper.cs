using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Helper
{
  public class ConstantsHelper
  {
    public class NotificationStatusType
    {
      public const string Queued = "Queued";

      public const string Sending = "Sending";

      public const string Sent = "Sent";
    }

    public class NotificationType
    {
      public const string General = "General";

      public const string GeneralEndUser = "GeneralEndUser";

      public const string GeneralIT = "GeneralIT";

      public const string Approval = "ApprovalNotifications";

      public const string SLA100 = "SLA100";

      public const string SLA75 = "SLA75";

      public const string SLA50 = "SLA50";

      public const string Survey = "Survey";
    }

    //public class NotificationShortDescriptionType
    //{
    //    public const string TicketSubmission = "SRM-REQ-Submitted-ReqForBy";

    //    public const string TicketClosure = "SRM-REQ-StatusUpdatedCompleted";

    //    public const string WorkinfoUpdates = "SRS-REQ-NotifyEndUser";

    //    public const string ApprovalRemainder = "SRM-REQ-Approver_Notification_FirstInterval";

    //    public const string ApprovalRejection = "SRM-REQ-StatusUpdatedRejected";

    //    public const string Survey = "SRM:SRV:IndividualSurveyMessage";

    //    public const string SLA100 = "SLAAlert50";

    //    public const string SLA75 = "SLAAlert75";

    //    public const string SLA50 = "SLAAlert100";

    //    public static List<string> Approval = new() { "SRM-REQ-Approver_Notification", "CHG-APR-ApprovalNTForIndividual" };

    //    public static List<string> TicketGroupAssignment = new() { "HPD-INC-GroupAssignment", "WOI-WOI-Assignment_Assignee_Group_Notification" };

    //    public static List<string> TicketIndividualAssignment = new() { "HPD-INC-AssigneeAssignment", "WOI-WOI-Assignee_Notification" };

    //    public static List<string> TicketWorkInfoUpdates = new() { "SECU:HPD-INC-WorklogUpdate", "SEHA:HPD-WOI-WorklogUpdate" };
    //}

    public class URL
    {
      public const string GetRequests = "Requests";

      public const string GetWorkInfo = "WorkInfo";

      public const string GetEmployees = "Employees";

      public const string GetApprovals = "Approvals";

      public const string PostRequest = "PostRequest";

      public const string PostWorkInfoUpdateRequest = "PostWorkInfoUpdateRequest";

      public const string IncidentDropdown = "IncidentDropdown";

      public const string GetUserDetails = "UserDetails";

      public const string SubmitIncident = "SubmitIncident";
    }
    public class RequestStatusTypes
    {
      public const string Cancelled = "Cancelled";

      public const string Closed = "Closed";

      public const string Rejected = "Rejected";

      public const string Planning = "Planning";

      public const string Initiated = "Initiated";
    }
    public class PostRequestVariables
    {
      public const string IntegrationType = "Teams Chatbot";

      public const string Direction = "Incoming";

      public const string Approve = "Approval";

      public const string Reject = "Rejection";

      public const string ApproveShortDescription = "Approved";

      public const string RejectionShortDescription = "Rejected";

      public const string Status = "New";

      public const string AssignmentsTransactionType = "Approval Reassignment";

      public const string ApprovalTransactionType = "Approval/Rejection";

      public const string SurveyResponse = "Survey Response";
    }
    public class PostRequestWorkInfoUpdateVariables
    {
      public const string z1DAction = "DONOTSEND";

      public const string SRWorkInfoTypeAction = "WorkInfo created After SR Submit";

      public const string SecureLog = "No";

      public const string ViewAccess = "Public";

      public const string WorkInfoType = "General Information";

      public const string Status = "Enabled";

      public const string NumberofAttachments = "1";
    }
    public class IncidentDropdownVariables
    {
      public const string SrdId = "SRD000000013304";

      public const string Status = "Enabled";

    }
    public class SubmitIncidentVariables
    {
      public const string z1DAction = "CREATE";

      public const string SourceKeyword = "ExternalIntegration";

      public const string TitleInstanceID = "SRGAA5V0GHZGBAPOKAVAPNMQYVW2YS";

      public const string OfferingTitle = "Report an Incident";

      public const string Status = "Submitted";

      public const string WorkInfoType = "General";

      public const string SecureLog = "Yes";

      public const string ViewAccess = "Public";

      public const string WorkInfoSummary = "Attachment from Reques...";
    }
    public class CancelRequestVariables
    {
      public const string z1DAction = "MODFY";

      public const string Status = "Cancelled";

      public const string StatusReason = "By User";
    }
    public class ConversationReferences
    {
      public const string TableName = "ConversationReferences";

      public const string PartitionKey = "ConversationReference";
    }
    public class Log
    {
      public const string TableName = "Log";

      public const string PartitionKey = "Log";
    }
    public class ProcessNotificationStatus
    {
      public const string Processed = "Processed";
    }
    public class ServicesNames
    {
      public const string StorageQueueName = "notificationsqueue";

      public const string StoragePoisonQueueName = "notificationsqueue-poison";
    }
    public class APIRoute
    {
      public const string ApproveRequest = "approve";

      public const string RejectRequest = "reject";

      public const string SyncEmployees = "sync";

      public const string GetToken = "token";

      public const string CreateRequest = "create";

      public const string CreateWorkInfoUpdate = "workinfoupdate";

      public const string CancelRequest = "{requestNumber}/cancel";

      public const string GetRequest = "{requestNumber}";

      public const string SubmitSurvey = "submitsurvey";

      public const string IncidentDropdown = "incidentdropdown";

      public const string SubmitIncident = "submitincident";
    }
    public class TaskModule
    {
      public const string Title = "Seha";

      public const string RequestDetailsUri = "{BaseUri}/request/{requestNo}";

      public const string NotificationDetailsUri = "{baseUri}/notification/{requestId}";

      public const string SurveyNotificationDetailsUri = "{baseUri}/{requestId}/rating/{instanceid}";

      public const int Height = 800;

      public const int Width = 1000;
    }
    public class GeneralNotificationTags
    {
      public const string EndUser = "End User";

      public const string IT = "IT";
    }

    //public class AdaptiveCardTitles
    //{
    //    public const string TicketSubmission = "Ticket submitted successfully";

    //    public const string TicketClosure = "Ticket resolved successfully";

    //    public const string ApprovalRemainder = "Ticket %ticketno% has been pending for approval";

    //    public const string ApprovalRejection = "Ticket %ticketno% has been rejected";

    //    public const string Survey = "Survey";

    //    public const string ApprovalNotification = "Ticket %ticketno% has been approved";

    //    public const string SLA100 = "Ticket has reached 100% SLA";

    //    public const string SLA75 = "Ticket has reached 75% SLA";

    //    public const string SLA50 = "Ticket has reached 50% SLA";
    //}
  }
}

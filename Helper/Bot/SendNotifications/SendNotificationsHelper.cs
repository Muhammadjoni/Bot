using Bot.Helper.Bot.ConversationRef;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Bot.Helper.Bot.SendNotifications
{
  public class SendNotificationshelper : BaseHelper<NotificationEntity>, ISendNotificationsHelper
  {
    private readonly IConversationReferencesHelper _conversationReferenceHelper;
    private readonly IBotFrameworkHttpAdapter _adapter;
    private readonly IConfiguration _configuration;
    public SendNotificationshelper(AppDbContext context, IConversationReferencesHelper conversationReferencesHelper, IBotFrameworkHttpAdapter adapter, IConfiguration configuration) : base(context)
    {
      _conversationReferenceHelper = conversationReferencesHelper;
      _adapter = adapter;
      _configuration = configuration;
    }

    public async Task<NotificationEntity> SendNotificationsAsync(NotificationEntity entity, AppDbContext localContext)
    {
      var conRef = await _conversationReferenceHelper.GetConversationReferenceAsync(entity.Email);

      if (conRef != null)
      {
        ConversationReference reference = new()
        {
          Conversation = new ConversationAccount()
          {
            Id = conRef.ConversationId
          },
          ServiceUrl = conRef.ServiceUrl,
        };

        await ((BotAdapter)_adapter).ContinueConversationAsync(
               _configuration["MicrosoftAppId"],
               reference,
               async (context, token) =>
               {
                 string taskModuleUri = string.Empty;

                 if (!string.IsNullOrWhiteSpace(entity.NotificationType))
                 {
                   if (entity.NotificationType.Equals(NotificationType.Survey))
                     taskModuleUri = TaskModule.SurveyNotificationDetailsUri.Replace("{baseUri}", _configuration["BaseUri"]).Replace("{requestId}", entity.RequestId).Replace("{instanceid}", entity.InstanceId);
                   else
                     taskModuleUri = TaskModule.NotificationDetailsUri.Replace("{baseUri}", _configuration["BaseUri"]).Replace("{requestId}", entity.RequestId);
                 }
                 else
                   taskModuleUri = TaskModule.NotificationDetailsUri.Replace("{baseUri}", _configuration["BaseUri"]).Replace("{requestId}", entity.RequestId);

                 var attachment = MessageFactory.Attachment(AdaptiveCardHelper.GetNotificationAdaptiveCard(entity, taskModuleUri));
                 attachment.Summary = entity.ShortDescription;

                 entity.ActivityId = await BotCallback(attachment, context, token);

                 entity.IsSent = true;
                 entity.SentOn = DateTime.UtcNow;
                 entity.Status = NotificationStatusType.Sent;
               },
               default);
      }
      return entity;
    }

    private static async Task<string> BotCallback(
       IMessageActivity message,
       ITurnContext turnContext,
       CancellationToken cancellationToken)
    {
      return (await turnContext.SendActivityAsync(message, cancellationToken)).Id;
    }
  }
}

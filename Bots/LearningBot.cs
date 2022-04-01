using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Bot.Helper.Bot.ConversationRef;
using Bot.Models.Database;
// using Bot.Models.Database;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;

namespace Bot.Bots
{
  public class LearningBot : TeamsActivityHandler
    {
        private readonly IConversationReferencesHelper _conversationReferenceHelper;
        // private readonly IRequestsHelper _requestsHelper;
        // private readonly IApprovalHelper _approvalHelper;
        private readonly IConfiguration _configuration;

        public LearningBot(IConversationReferencesHelper conversationReferencesHelper, IConfiguration configuration)
        {
            _conversationReferenceHelper = conversationReferencesHelper;
            // _requestsHelper = RequestsHelper;
            _configuration = configuration;
            // _approvalHelper = approvalHelper;
        }

        // protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        // {
        //     ConvRef botConRef = turnContext.Activity.GetConversationReference();
        //     var currentMember = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken);
        //     await _conversationReferenceHelper.AddorUpdateConversationRefrenceAsync(botConRef, currentMember);
        // }
        // protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        // {
        //     ConvRef botConRef = turnContext.Activity.GetConversationReference();
        //     var currentMember = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken);
        //     await _conversationReferenceHelper.AddorUpdateConversationRefrenceAsync(botConRef, currentMember);
        // }
        protected override async Task OnInstallationUpdateActivityAsync(ITurnContext<IInstallationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var activity = turnContext.Activity;
            ConversationReference botConRef = turnContext.Activity.GetConversationReference();
            var currentMember = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken);

            if (activity.Action.Equals("add"))
                await _conversationReferenceHelper.AddorUpdateConversationRefrenceAsync(botConRef, currentMember);
            else if (activity.Action.Equals("remove"))
                await _conversationReferenceHelper.DeleteConversationRefrenceAsync(botConRef, currentMember);

        }
        // protected override Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        // {
        //     var asJobject = JObject.FromObject(taskModuleRequest.Data);
        //     var taskInfo = new TaskModuleTaskInfo
        //     {
        //         Height = TaskModule.Height,
        //         Width = TaskModule.Width,
        //         Title = TaskModule.Title
        //     };
        //     taskInfo.Url = taskInfo.FallbackUrl = (string)asJobject.SelectToken("Url");
        //     return Task.FromResult(taskInfo.ToTaskModuleResponse());
        // }
        // protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        // {
        //     var currentMember = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken);
        //     if(turnContext.Activity.Text.ToLower().Contains("approval"))
        //     {
        //         var approvals = (await _approvalHelper.GetApprovalsAsync(emailFromBot: currentMember.Email.ToLower()))?.entries;
        //         if(approvals != null && approvals.Any())
        //         {
        //             var res = MessageFactory.Attachment(AdaptiveCardHelper.GetApprovalsHeroCardsActivity(approvals));
        //             res.AttachmentLayout = AttachmentLayoutTypes.Carousel;
        //             await turnContext.SendActivityAsync(res, cancellationToken);
        //         }
        //         else
        //             await turnContext.SendActivityAsync(MessageFactory.Text($"No pending approvals for this user."), cancellationToken);
        //         return;
        //     }

        //     MatchCollection matches = new Regex("([a-zA-Z]{3}[0-9]{7})", RegexOptions.IgnoreCase).Matches(turnContext.Activity.Text);
        //     if(matches.Any())
        //     {
        //         var requestDetails = await _requestsHelper.GetRequestAsync(matches.FirstOrDefault().Value);

        //         if (requestDetails?.Request?.entries != null && currentMember.Email.ToLower().Equals(requestDetails.Request.entries.FirstOrDefault().values.Email.ToLower()))
        //         {
        //             var uri = TaskModule.RequestDetailsUri.Replace("{BaseUri}", _configuration["BaseUri"]).Replace("{requestNo}",
        //                                                                                        requestDetails.Request.entries.FirstOrDefault().values.RequestNumber);
        //             await turnContext.SendActivityAsync(MessageFactory.Attachment(AdaptiveCardHelper.GetRequestAdaptiveCard(requestDetails.Request.entries.FirstOrDefault().values, uri)), cancellationToken);
        //         }
        //         else await turnContext.SendActivityAsync(MessageFactory.Text($"Request with the Request No:{matches.FirstOrDefault().Value} not found."), cancellationToken);
        //     }
        //     else await turnContext.SendActivityAsync(MessageFactory.Text($"Sorry, I did not recognise any RequestNo to fetch the details. Can you try again?"), cancellationToken);
        // }
    }
}

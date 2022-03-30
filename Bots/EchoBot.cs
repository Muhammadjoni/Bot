// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.15.2

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace EchoBot.Bots
{
  public class EchoBot : ActivityHandler
  {
    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
      await turnContext.SendActivityAsync(MessageFactory.Text("Service URL is " + turnContext.Activity.ServiceUrl));

      // var replyText = $"Echo: {turnContext.Activity.Text}";
      // await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
      // var teamConversationData = turnContext.Activity.GetChannelData<TeamsChannelData>();
      // await turnContext.SendActivityAsync(MessageFactory.Text("Your ID is " + turnContext.Activity.From.Id), cancellationToken);
      // await turnContext.SendActivityAsync(MessageFactory.Text("My ID is " + turnContext.Activity.Recipient.Id), cancellationToken);
      // await turnContext.SendActivityAsync(MessageFactory.Text("Tenant ID is " + teamConversationData.Tenant.Id));
    }

    // protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    // {
    //   var welcomeText = "Hello and welcome!";
    //   foreach (var member in membersAdded)
    //   {
    //     if (member.Id != turnContext.Activity.Recipient.Id)
    //     {
    //       await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
    //     }
    //   }
    // }
    [Obsolete]
    public async static Task Main(string[] args)
    {
      const string url = "https://mybotapp.azurewebsites.net/";
      const string appId = "ab1404b9-d8c7-4aa4-8802-f6ae6e057cba";
      const string appPassword = "0DI7Q~YJ5iRZF9KmjGgknLq7cy71qSGnHszbr";

      MicrosoftAppCredentials.TrustServiceUrl(url);
      var client = new ConnectorClient(new Uri(url), appId, appPassword);
      // Create or get existing chat conversation with user
      var parameters = new ConversationParameters
      {
        Bot = new ChannelAccount("28:ab1404b9-d8c7-4aa4-8802-f6ae6e057cba"),
        Members = new[] { new ChannelAccount("29:1jc1z3u42nZWnIgaPcKML432LtpgPqaEL-PtHvk0DT6NS5TiOx4HKH68CjRLKSNsv1vTDx3ghuzUqTP4XexQ00A") },
        ChannelData = new TeamsChannelData
        {
          Tenant = new TenantInfo("603439c3-58ad-4a91-8ed3-b53e9a8677b3"),
        },
      };
      var response = await client.Conversations.CreateConversationAsync(parameters);
      // Construct the message to post to conversation
      var newActivity = new Activity
      {
        Text = "Hello",
        Type = ActivityTypes.Message,
        Conversation = new ConversationAccount
        {
          Id = response.Id
        },
      };
      // Post the message to chat conversation with user
      await client.Conversations.SendToConversationAsync(response.Id, newActivity);

    }
  }
}

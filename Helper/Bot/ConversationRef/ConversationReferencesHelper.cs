// using Microsoft.Azure.Cosmos.Table;
using Bot.Context;
using Bot.Models.Database;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using static SehaBMC.Common.Helpers.ConstantsHelper;

namespace Bot.Helper.Bot.ConversationRef
{
  public class ConversationReferencesHelper : IConversationReferencesHelper
  {
    private readonly AppDbContext context;

    public ConversationReferencesHelper(AppDbContext context)
    {
      this.context = context;
    }

    public async Task AddorUpdateConversationRefrenceAsync(ConversationReference reference, TeamsChannelAccount member)
    {

      var id = context.ConversationReference.Where(x => x.UPN.Equals(member.UserPrincipalName)).Select(x => x.UPN).FirstOrDefault();
      if (id != member.UserPrincipalName)
      {
        ConvRef conversationReference = new ConvRef();
        conversationReference.UserID = member.Id;
        conversationReference.UPN = member.UserPrincipalName;
        conversationReference.ConversationID = reference.Conversation.Id;
        conversationReference.ServiceUrl = reference.ServiceUrl;
        conversationReference.ActivityID = reference.ActivityId;
        context.ConversationReference.Add(conversationReference);
        await context.SaveChangesAsync();
      }
    }

    public async Task DeleteConversationRefrenceAsync(ConversationReference reference, TeamsChannelAccount member)
    {
      ConvRef cons = await GetConversationReferenceAsync(member.UserPrincipalName);
      context.ConversationReference.Attach(cons);
      context.ConversationReference.Remove(cons);
      context.SaveChanges();
    }


    public async Task<ConvRef> GetConversationReferenceAsync(string upn)
    {
      ConvRef conversationRef = new ConvRef();
      conversationRef.Id = context.ConversationReference.Where(x => x.UPN.Equals(upn)).Select(i => i.Id).Single();
      conversationRef.UPN = upn;
      conversationRef.ConversationID = context.ConversationReference.Where(x => x.UPN.Equals(upn)).Select(i => i.ConversationID).Single();

      return conversationRef;
    }


    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(context);
    }
  }
}

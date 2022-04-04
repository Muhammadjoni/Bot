// using Microsoft.Azure.Cosmos.Table;
using Bot.Models.Database;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using System.Threading.Tasks;
// using static SehaBMC.Common.Helpers.ConstantsHelper;

namespace Bot.Helper.Bot.ConversationRef
{
  public interface IConversationReferencesHelper
  {
    Task AddorUpdateConversationRefrenceAsync(ConversationReference reference, TeamsChannelAccount member);
    //Task DeleteConversationRefrenceAsync(ConversationReference reference, TeamsChannelAccount member);
    Task<ConvRef> GetConversationReferenceAsync(string upn);

    Task DeleteConversationRefrenceAsync(ConversationReference reference, TeamsChannelAccount member);
  }
}

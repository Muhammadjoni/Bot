// using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Bot.Models.Database
{
  public class ConvRef
  {
    public int Id { get; set; }
    private string _upn;
    public string UPN
    {
      get => _upn;
      set => _upn = value.ToLower();
    }
    public string ConversationID { get; set; }

    public string UserID { get; set; }

    public string ServiceUrl { get; set; }

    public string ActivityID { get; set; }

    public static implicit operator ConversationReference(ConvRef v)
    {
      throw new NotImplementedException();
    }
    // public new string RowKey
    // {
    //   get
    //   {
    //     return base.RowKey;
    //   }
    //   set => base.RowKey = value.ToLower();
    // }
  }
}

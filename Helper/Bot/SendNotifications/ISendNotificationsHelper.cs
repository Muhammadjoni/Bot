using SehaBMC.Common.Context;
using SehaBMC.Common.Models.Database.API; //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Helper.Bot.SendNotifications
{
  public interface ISendNotificationsHelper
  {
    Task<NotificationEntity> SendNotificationsAsync(NotificationEntity entity, AppDbContext localContext);
  }
}

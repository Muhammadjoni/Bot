using Bot.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SehaBMC.Common.Context;
using SehaBMC.Common.Models.Database.API;
using SehaBMC.Common.Models.Responses.BMCAPI;
using SehaBMC.Common.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Helper
{
  public class BaseHelper<TEntity> where TEntity : class
  {
    internal AppDbContext _context;
    internal DbSet<TEntity> _entity;
    public BaseHelper(AppDbContext context)
    {
      _context = context;
      _entity = context.Set<TEntity>();
    }
  }
}

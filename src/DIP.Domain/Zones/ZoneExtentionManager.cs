using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;
using DIP.EServices;
using MakaniPublic;

namespace DIP.Zones
{
    public partial class ZoneManager
    {
        public virtual async Task<string> getMakaniNumber(string lat , string lon)
        {
            var svc = new MakaniPublic.MakaniPublicClient();

          var makaniInfo =   await svc.GetMakaniInfoFromCoordAsync(lat, lon , "");
         
            return makaniInfo;

        }


    }
}
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;
using DIP.PageInfoSections;

namespace DIP.PageInfos
{
    public class PageInfoWithDetails 
    {
        public virtual PageInfo PageInfo { get; set; }
        public virtual List<PageInfoSection> PageInfoSections { get; set; }



        public PageInfoWithDetails()
        {

        }

      

    }
}
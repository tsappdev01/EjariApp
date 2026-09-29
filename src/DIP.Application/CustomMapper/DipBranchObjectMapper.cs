using DIP.DipBranches;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using IObjectMapper = Volo.Abp.ObjectMapping.IObjectMapper;

namespace DIP.CustomMapper
{
    public class DipBranchObjectMapper : IObjectMapper<DipBranch, DipBranchFront>,
        IObjectMapper<List<DipBranch>, List<DipBranchFront>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public DipBranchObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<DipBranchFront> Map(List<DipBranch> source)
        {

            var output = new List<DipBranchFront>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<DipBranchFront> Map(List<DipBranch> source, List<DipBranchFront> destination)
        {
            return Map(source);
        }

        public DipBranchFront Map(DipBranch source)
        {

            DipBranchFront dipBranchFront = new DipBranchFront();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                dipBranchFront = new DipBranchFront()
                {
                    Id = source.Id,
                    IsActive= source.IsActive,
                    AlternativeEmail = source.AlternativeEmail,
                    AlternativePhone = source.AlternativePhone,
                    Email = source.Email,
                    Title = source.TitleEn,
                    SubTitle = source.SubTitleEn,
                    Order = source.Order,
                    Phone = source.Phone
                };
            }
            else
            {
                dipBranchFront = new DipBranchFront()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    AlternativeEmail = source.AlternativeEmail,
                    AlternativePhone = source.AlternativePhone,
                    Email = source.Email,
                    Title = source.TitleAr,
                    SubTitle = source.TitleAr,
                    Order = source.Order,
                    Phone = source.Phone
                };

            }

            return dipBranchFront;
        }

        public DipBranchFront Map(DipBranch source, DipBranchFront destination)
        {
            return Map(source);
        }
    }
}



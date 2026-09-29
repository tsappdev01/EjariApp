using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using DIP.Zones;
using System.Collections.Generic;

namespace DIP.DipBranches
{
    public partial interface IDipBranchesAppService
    {
        Task<List<DipBranchFront>> GetListFrontEndAsync(GetDipBranchesInput input);
    }
}
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.EFormServices;

namespace DIP.EFormServices
{
    public class EFormServicesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IEFormServiceRepository _eFormServiceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public EFormServicesDataSeedContributor(IEFormServiceRepository eFormServiceRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _eFormServiceRepository = eFormServiceRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _eFormServiceRepository.InsertAsync(new EFormService
            (
                id: Guid.Parse("f3a4fb45-a8e8-4d7b-9e5f-53cca28817ce"),
                titleEn: "50a255e8e1a245e394ca4d3cfd8d18d5ac5bd368",
                titleAr: "56359f476381498298ce0732371fdcd168deceb70dd042",
                order: 398731795,
                isActive: true
            ));

            await _eFormServiceRepository.InsertAsync(new EFormService
            (
                id: Guid.Parse("dce0d74c-2d43-49c0-a63e-5450bcefbf99"),
                titleEn: "71334fb8e71945af83cfc8f234c98a62e4670d0",
                titleAr: "06ae111595fd41afbcbd9",
                order: 576894030,
                isActive: true
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}
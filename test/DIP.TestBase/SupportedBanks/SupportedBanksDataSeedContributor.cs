using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.SupportedBanks;

namespace DIP.SupportedBanks
{
    public class SupportedBanksDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly ISupportedBankRepository _supportedBankRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public SupportedBanksDataSeedContributor(ISupportedBankRepository supportedBankRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _supportedBankRepository = supportedBankRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _supportedBankRepository.InsertAsync(new SupportedBank
            (
                id: Guid.Parse("78b0094d-0ab7-4acc-ae8e-7fa0860ffc29"),
                titleAr: "2f1d8b1132074833b536",
                titleEn: "4e4752ef94f64b5",
                isActive: true,
                order: 159639227
            ));

            await _supportedBankRepository.InsertAsync(new SupportedBank
            (
                id: Guid.Parse("e0b9d3dd-d1f6-40ff-8ec7-83ad1910684e"),
                titleAr: "3baf40c230c94947b826eefb47b35b02a01b298cb846401091da6bba04cfe886fa989d0fe1b14750b4",
                titleEn: "cb83c5feadcc45e",
                isActive: true,
                order: 112069566
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}
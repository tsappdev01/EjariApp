using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.ContactForms;

namespace DIP.ContactForms
{
    public class ContactFormsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IContactFormRepository _contactFormRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ContactFormsDataSeedContributor(IContactFormRepository contactFormRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _contactFormRepository = contactFormRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _contactFormRepository.InsertAsync(new ContactForm
            (
                id: Guid.Parse("d7dd5e88-358c-4746-a8e7-0df1a50e5350"),
                fullName: "46cd91f708564ece9f91cb36fbf64ead1593c05b2d8f4ff394f1792ef4040fa",
                email: "6bd9cccc3d@f14dfef275.com",
                subject: "74034960cc1240b0ab231fdd1d0a93a55e6c259251ee49d484d",
                message: "e42d16ded61243659f38c66f6173adea4e13b539d11246b3808f80a323"
            ));

            await _contactFormRepository.InsertAsync(new ContactForm
            (
                id: Guid.Parse("ab8a51d8-4d65-4fb3-ada1-47d0fc6eff9a"),
                fullName: "ae44791550b347a6b0faa5783702bd1b22ae88d740474ffe861b1f91e2c36203b6f73d2eff704af4bd7da2b",
                email: "0bf47f4afdf64@9c6ae9782ed34.com",
                subject: "1992610b7295401f80a9cca779d73cd1de955127eaa345cf8ba2ecc",
                message: "21b9594cc0ff434a970f4e0c9ee14a74dca47d0d0c1a4346a7068a"
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}
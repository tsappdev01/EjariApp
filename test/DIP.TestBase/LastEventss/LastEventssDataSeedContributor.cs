using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.LastEventss;

namespace DIP.LastEventss
{
    public class LastEventssDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly ILastEventsRepository _lastEventsRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public LastEventssDataSeedContributor(ILastEventsRepository lastEventsRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _lastEventsRepository = lastEventsRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _lastEventsRepository.InsertAsync(new LastEvents
            (
                id: Guid.Parse("55e1ef36-18f1-4151-9ee2-f7a4b214ad4d"),
                titleAr: "4eea147c91a14781ab9b5aa9a2933a27f27248370f464f2baada62dc2434253b687e95af80224483b02a5c0a",
                titleEn: "46ba92b17dad4be7a8fe3076e1355b632b1abaa92e634d15a49fa0b88a4d",
                metaTitleAr: "2481daa0aea8481d8ecbd630ca6b60b893041fc64ff94891b3903d940a9ee7c",
                metaTitleEn: "564cc6a709dc4ee281df00d34fa0faf1e7e5628e1d674a0fbe1539fed62ea3554b57506f912e43d6b9b5783",
                metaDescriptionEn: "f6bf3d66c1c54cd69a78f4a76415b4a66a59a967fd4",
                metaDescriptionAr: "911e78b01b17453b826579984f0cdb4",
                isFeatured: true,
                slug: "dd30fb6ceed64965889ec0646f072d71bb0230cec2ae47a49575f9a19f8c63a63d8eae53891e4fdc8ee147e5a",
                image: "201e00754d5c4adb91439f054baa78f1a9abb0417d004dbf8bb9a8c6a",
                headerImage: "917aa8030",
                descriptionAr: "8b74fa3d47e24a86ac12052aa5cf08939a4b21f94c2847ac",
                descriptionEn: "76620e98e3e",
                summaryEn: "196fd53b97474fe48c93be0d7e5599065ebdffb09fc640d784ec63713f98e44e26ba959042494a9e8ad4db327322fb",
                summaryAr: "b14790c632c544d7903d798096ea171ee084d4e970404564bb8d8810ba",
                order: 2041665703,
                startDate: new DateTime(2005, 11, 26),
                endDate: new DateTime(2004, 10, 4),
                isActive: true,
                locationAr: "b06f3f987d7c49e09acdb0dd514d972a5681099ce7b3427eac7888415b956c49ed2800c3a17443c0952575a75070ac0",
                locationEn: "58659c286e"
            ));

            await _lastEventsRepository.InsertAsync(new LastEvents
            (
                id: Guid.Parse("e9df8b4a-8c88-4ed6-b9a4-26ba0bd93ce4"),
                titleAr: "94e2048dc9ec4bf3bd3dea848b10da94f4e2e56c75124c01a784e31f940dc3551de3b4d54c7745",
                titleEn: "dabb7cfd88e54719833425dd5c93092a7ac0c4cd48164",
                metaTitleAr: "56eab7a1b58d41e3a4e44590c5006f447e45652af0d945c28f4ccf278a3c",
                metaTitleEn: "13bdd4f187324a77b4551",
                metaDescriptionEn: "8afd34d875704433946d5d4f3eda780ed280de9e0ac44d05b902a9b79b92",
                metaDescriptionAr: "b5a18023238f4a13af847b24c7b89b6fb3de270c6e2d46fbbdb54c3300e70115126f097351e949a0bd8a401dc05d36ad86",
                isFeatured: true,
                slug: "7436d00f41764",
                image: "5d67ae1aca174",
                headerImage: "abb0e976e0764850ab98a313dddf38ad29461ea5148e47faa5296e3",
                descriptionAr: "f36f2969f6e84094b5e6f7f",
                descriptionEn: "3c4e5b32a4b748c5ace41bc2e162b16cb86f782c4cc941a7813",
                summaryEn: "71c3272d919c4616a4a1a",
                summaryAr: "6d3b35b43",
                order: 1297277745,
                startDate: new DateTime(2002, 8, 10),
                endDate: new DateTime(2020, 7, 23),
                isActive: true,
                locationAr: "9ab8bef8e5e94a8285a2bf191ace18578832a973f43",
                locationEn: "7512b05d"
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}
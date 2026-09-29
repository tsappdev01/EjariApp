using DIP.PageInfos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.PageInfoSections;

namespace DIP.PageInfoSections
{
    public class PageInfoSectionsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IPageInfoSectionRepository _pageInfoSectionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly PageInfosDataSeedContributor _pageInfosDataSeedContributor;

        public PageInfoSectionsDataSeedContributor(IPageInfoSectionRepository pageInfoSectionRepository, IUnitOfWorkManager unitOfWorkManager, PageInfosDataSeedContributor pageInfosDataSeedContributor)
        {
            _pageInfoSectionRepository = pageInfoSectionRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _pageInfosDataSeedContributor = pageInfosDataSeedContributor;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _pageInfosDataSeedContributor.SeedAsync(context);

            await _pageInfoSectionRepository.InsertAsync(new PageInfoSection
            (
                id: Guid.Parse("edc397e5-c605-4cb2-bd10-6ab986f655f6"),
                titleEn: "7fa36a8cb3ec4832997864b0f8a1508bf471649c1db447e4bba804fb8d68c19f62e887d383fd4652b242",
                titleAr: "f457aafce2204",
                subTitleEn: "e3b59741491048448e87c7c5ee4e6ba928266a2b58a84d138af5",
                subTitleAr: "14c2c2039c584477a93d9b214ea9fc0109425fc71591453c9ec714c22772e0e19bd719099ded4895b98fbdaa073a66224f9",
                summaryEn: "b1a5f1177aad4165a0432f75b1bc56d093c2ea335a5d4eadb4eb49344dba0c9f669c4",
                summaryAr: "b7d63bc4d9a34b19b10fc7283f619a6ed5d75268f62e4763bc5210f0ecd",
                descriptionEn: "1a9ccad8dba246838785970375487f83a345fdd0e2604827b09c5bd175f9c47e",
                descriptionAr: "dacaba367",
                pageSectionMedia: "6d0402cc9c6743d08a57dbf126540ee9350ae02777eb40f8ba8962fc5237c4489f",
                youtubeUrl: "0dfcd138cff24d0ab332615cd3a4b5b91dbd690bfab34da29d02d099985b962517341",
                order: 948373647,
                isActive: true,
                pageInfoId: Guid.Parse("c9631135-a498-4113-b1b2-36e40f9baa78")
            ));

            await _pageInfoSectionRepository.InsertAsync(new PageInfoSection
            (
                id: Guid.Parse("838e80ff-792a-44e9-a2d7-4cabe6105c3a"),
                titleEn: "217ee3522bd04d4887cc8268ac282b985522e6e651be413fb8e0ac58d95474",
                titleAr: "d82725fb5d384e7d98e8d13d15feb0c0bd0ef34c09384a1b91802fc444b8bdb20d5f6",
                subTitleEn: "61d6fe99a40442c38ec51d7388d2b34bda3cda012",
                subTitleAr: "98606cb36a4444c49c12e45aedec0",
                summaryEn: "a3a09e88cb7a4eb5b880694",
                summaryAr: "0d136b1dcd7c40a397a198b2e33803283f4c559f1b9b4",
                descriptionEn: "8e0010927ba24a8fa1b3129b6e00ee249e964f5f0fe540999f4cd4e165b5d0",
                descriptionAr: "93e4f439e3b74b12b8759261a7cfe73619ab04f6db8b4fd88fb",
                pageSectionMedia: "ff1e82e597fd4d90a5acbd123250421022ddd35092c04e33b0940c8f78dcf51eeff1c4a5658",
                youtubeUrl: "3b90fdd3403f464e89410fd6e74a37c4dc1ad0356b76",
                order: 606572481,
                isActive: true,
                pageInfoId: Guid.Parse("c9631135-a498-4113-b1b2-36e40f9baa78")
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}
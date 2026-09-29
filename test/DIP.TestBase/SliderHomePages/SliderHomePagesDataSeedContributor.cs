using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.SliderHomePages;

namespace DIP.SliderHomePages
{
    public class SliderHomePagesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly ISliderHomePageRepository _sliderHomePageRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public SliderHomePagesDataSeedContributor(ISliderHomePageRepository sliderHomePageRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _sliderHomePageRepository = sliderHomePageRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _sliderHomePageRepository.InsertAsync(new SliderHomePage
            (
                id: Guid.Parse("be8556e4-a7f1-4783-a891-03d4ebe23a73"),
                titleAr: "cd82749c620c454587c08dff15c40c71fa12924f3d19485a9cd4b3629320c0ee53339298f503407ab01aa42c4406",
                titleEn: "3be647c50",
                descriptionAr: "fa8f345615b245fe9",
                descriptionEn: "ff8e5aeb9",
                buttonTitleAr: "5cbbb0486862",
                buttonTitleEn: "a2f51752c",
                buttonUrlEn: "4abfc926",
                buttonUrlAr: "4abfc926",
                image: "b1b99a53f960413a92ce4e320338801f15a544f1129b4bb1afed88246",
                youtubeUrl: "1d29cd96302147a69f8eff0045f52c9e48820aeed",
                isActive: true,
                order: 250885317
            ));

            await _sliderHomePageRepository.InsertAsync(new SliderHomePage
            (
                id: Guid.Parse("599c48d6-33f6-457e-9220-395b844e732c"),
                titleAr: "8d446e05de06423ea5580b7df",
                titleEn: "8634b6f42e8d480a94b4e1070800a6059594e5ee37594160818b80d68b87a3a4",
                descriptionAr: "546d1861c01644e1b9dbb2fa3f",
                descriptionEn: "9a61f27bd738419cb508428942459346be27d0e8c63e4c238d89f806e67a6196ddbc05494a",
                buttonTitleAr: "f92b80b8e9f54875b01573384a54f601b104515938604730a8b9e",
                buttonTitleEn: "672a60f006f544a495278452d07c",
                buttonUrlEn: "4dfa78fa36474f54b4ea148004fc8e36ddab078a536e41aebc3564a3ed1a00b25b09104628564ef9b14805c750702",
                buttonUrlAr: "4dfa78fa36474f54b4ea148004fc8e36ddab078a536e41aebc3564a3ed1a00b25b09104628564ef9b14805c750702",
                image: "554e07f88c7a4a32a5772d37aef69116cfcbd63cad494fa2a4dcd2d86ffdf195b4de730378704922a1484564436a",
                youtubeUrl: "6c605b19f89141609abb41f3ac6435955ff6ff4b403443b3bbec76f959ce80824b3853c2164848cdb9869200e7d37f10b38",
                isActive: true,
                order: 1568674743
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}
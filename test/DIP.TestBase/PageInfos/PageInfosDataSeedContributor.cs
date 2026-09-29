using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using DIP.PageInfos;

namespace DIP.PageInfos
{
    public class PageInfosDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IPageInfoRepository _pageInfoRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public PageInfosDataSeedContributor(IPageInfoRepository pageInfoRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _pageInfoRepository = pageInfoRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _pageInfoRepository.InsertAsync(new PageInfo
            (
                id: Guid.Parse("83f08243-5154-47a4-a489-639a5bb921f4"),
                titleAr: "c5e1b6013f174684ae42a01af00d6fc9bf9d6ea12362444995e4ba92025d2dcfa733c2940b184a",
                titleEn: "0c427e5a75044a68b3911c423bb24f94de6cf5777db14c49a4ab222d59acb7b324dbf",
                metaTitleAr: "93a6bdcb21fa47a2b71733ec8361c742d2844b9d",
                metaTitleEn: "e9bb25c6",
                metaDescriptionEn: "86d07347253e4717bec11d27e104bd",
                metaDescriptionAr: "10e5b7440541499b8199555962868e41e0ce0d91df944af9b07854ed9a8d85a0225739a0008d45ad96",
                slug: "0ad8f0faea6d4fcdba40995afd140cdd",
                image: "2b3d60894c5b4dc28ae1f70028bc82",
                headerImage: "0ee3c71e722b4401b25dd925d571712facd0f510216c4e929b9fb2f088ab86af3dc618883a184",
                youTubeUrl: "a0598d74bd54430",
                pageInfoArticleTilteEn: "0d1c05adfb1644fb8a72b3af",
                pageInfoArticleTilteAr: "461e8763e4bf4c4bb5fbd222f548b4e85903050c409745b59b8bf",
                pageInfoArticleSubtitleEn: "e3063c2a8d2540fb8f816a209d728ddd38ee79",
                pageInfoArticleSubtitleAr: "d7cb487ce455422c9104f34112a3d2de57dd33640f924480b9bf93c6d36ef75fcd8d23e73537426",
                descriptionAr: "93d7a9874c7b4d268e61d132f4908157b9e2",
                descriptionEn: "f61068a9fd824d129dae341d61a95",
                summaryEn: "7dca2c141e",
                summaryAr: "8432075275f943e28de4a38e400e611329bba4e712684ca4a4bf44ce3c15fb1e",
                order: 95085512,
                isActive: true
            ));

            await _pageInfoRepository.InsertAsync(new PageInfo
            (
                id: Guid.Parse("6298955e-d7d8-4088-8b8c-de1a3aacd46a"),
                titleAr: "65249d27",
                titleEn: "593da23c9d3d4c5d9d2ff358c74351ef790f13f92e364a89ac0e061af",
                metaTitleAr: "962e551dae7246bcb80bb7a030189f9479cd730189de40a681d7f8fe3692e61fd2045731",
                metaTitleEn: "0b93907485c24c",
                metaDescriptionEn: "36580af73d9748ed94538bc2976a7dbb7261f8dc02684af6931d562a0a6c96d9af602633acdc4313b23e8c",
                metaDescriptionAr: "299bd8f8f7f941518ea5da62a368cd22fc97adbf37634ad2bad7dc221f60b7ba173e327d",
                slug: "f2fcf8c6124043b6a7b5a6b8fea932f5c46e0223e4024fb7bfcc4f2f774803",
                image: "611d828c1a764c22a9da5cd077a291a2395d11e3fdb64178bf6bea317c76caf0c1e2ad77a3884598b",
                headerImage: "c854d1e38dd54c949dab841",
                youTubeUrl: "762a271e2e2446b9b0b12ec0e410a3982bdbcc4f5",
                pageInfoArticleTilteEn: "31ac2d3289004a8b9ed2637adf5856b50c62fefbd3ec450b8e3",
                pageInfoArticleTilteAr: "906fd75e6b304edf8874ba5f92bd235ce86e3778c5c",
                pageInfoArticleSubtitleEn: "8abfd649b5974cb1b5e0cc0ac8488508c74a664fda16",
                pageInfoArticleSubtitleAr: "31bca9762bf24b308be70585a1ccfa17b4ac8dc701424d62b23ad2c900bf29dd5920bd5941664569a5dfb",
                descriptionAr: "c2e2d5ee9c0a4af7b6c7cd417baf04bee18e0e08c7f749968261fb2e7a3a3d",
                descriptionEn: "8f7134a2f572424e97524ed775fb4fdc3408c9d73c57491eac0b843",
                summaryEn: "0810efe71f3b4d87885db26c4e5f61c3c62",
                summaryAr: "c8cae16a8e0c45e79f6a519786c452e3be5b6fb",
                order: 1072268217,
                isActive: true
            ));

            await _unitOfWorkManager.Current.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}
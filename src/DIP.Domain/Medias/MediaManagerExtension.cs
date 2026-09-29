using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;
using Microsoft.VisualBasic.FileIO;
using static System.Net.Mime.MediaTypeNames;

namespace DIP.Medias
{
    public partial class MediaManager 
    {

        public async Task<Media> CreateAsync(Guid id,
        Guid? zoneParagraphId, Guid? amenityParagraphId, Guid? mediaGalleryId, string titleEn, string titleAr, string file, int order, bool isActive, [CanBeNull] string concurrencyStamp = null)
        {
                var media = new Media(id, zoneParagraphId, amenityParagraphId, mediaGalleryId, titleEn, titleAr, file, order, isActive);
                return await _mediaRepository.InsertAsync(media, autoSave: true);          
        }   
        

    }
}
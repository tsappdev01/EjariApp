
using DIP.Permissions;
using DIP.Zones;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace DIP.Medias
{

    [Authorize(DIPPermissions.Medias.Default)]
    public partial class MediasAppService 
    {

        [Authorize(DIPPermissions.ZoneParagraphs.Edit)]
        public virtual async Task<bool> UpdateZoneparagraphMediasAsync(Guid zoneparagraphId,  List<MediaDto> input)
        {
            var mediaToDelete = new List<Guid>();

            var existingMedia = await _mediaRepository.GetListAsync(t => t.ZoneParagraphId == zoneparagraphId);
            if (existingMedia != null && existingMedia.Count > 0)
            {
                var existingMediaIds = existingMedia.Select(t => t.Id);
                mediaToDelete = existingMediaIds.Except(input.Select(r => r.Id)).ToList();
            }

            if (!input.IsNullOrEmpty())
            {
                foreach (var item in input)
                {
                    var media = await _mediaRepository.SingleOrDefaultAsync(t => t.Id == item.Id);

                    if (media == null)
                    {
                        if (!item.File.IsNullOrEmpty() && !item.FileContent.IsNullOrEmpty())
                            await _mediaContainer.SaveAsync(item.File, item.FileContent);

                        await _mediaManager.CreateAsync(item.Id, zoneparagraphId, null, null, item.TitleEn, item.TitleAr, item.File, item.Order, item.IsActive, item.ConcurrencyStamp);
                    }
                    else
                    {
                        if (!item.File.IsNullOrEmpty() && !item.FileContent.IsNullOrEmpty() && item.File != item.OldFileName)
                        {
                            if (!item.OldFileName.IsNullOrEmpty())
                                await _mediaContainer.DeleteAsync(item.OldFileName);
                            await _mediaContainer.SaveAsync(item.File, item.FileContent);
                        }
                        else
                        {
                            if (item.File.IsNullOrEmpty() && !item.OldFileName.IsNullOrEmpty())
                            {
                                await _mediaContainer.DeleteAsync(item.OldFileName);
                            }
                        }
                        await _mediaManager.UpdateAsync(item.Id, zoneparagraphId, null, null, item.TitleEn, item.TitleAr, item.File, item.Order, item.IsActive, item.ConcurrencyStamp);
                    }
                       

                   
                }
            }
            var mediaTemp = new Media();
            foreach (var item in mediaToDelete)
            {
                mediaTemp = await _mediaRepository.FirstOrDefaultAsync(m => m.Id == item);
                try
                {
                    if (mediaTemp != null && !mediaTemp.File.IsNullOrEmpty())
                    {
                        if (await _mediaContainer.ExistsAsync(mediaTemp.File))
                            await _mediaContainer.DeleteAsync(mediaTemp.File);
                    }
                }
                catch { }
                await _mediaRepository.DeleteAsync(item, autoSave: true);
            }

            return true;
        }

        [Authorize(DIPPermissions.AmenityParagraphs.Edit)]
        public virtual async Task<bool> UpdateAmenityparagraphMediasAsync(Guid amenityparagraphId, List<MediaDto> input)
        {
            var mediaToDelete = new List<Guid>();

            var existingMedia = await _mediaRepository.GetListAsync(t => t.AmenityParagraphId == amenityparagraphId);
            if (existingMedia != null && existingMedia.Count > 0)
            {
                var existingMediaIds = existingMedia.Select(t => t.Id);
                mediaToDelete = existingMediaIds.Except(input.Select(r => r.Id)).ToList();
            }

            if (!input.IsNullOrEmpty())
            {
                foreach (var item in input)
                {
                    var media = await _mediaRepository.SingleOrDefaultAsync(t => t.Id == item.Id);

                    if (media == null)
                    {
                        if (!item.File.IsNullOrEmpty() && !item.FileContent.IsNullOrEmpty())
                            await _mediaContainer.SaveAsync(item.File, item.FileContent);

                        await _mediaManager.CreateAsync(item.Id, null, amenityparagraphId, null, item.TitleEn, item.TitleAr, item.File, item.Order, item.IsActive, item.ConcurrencyStamp);
                    }
                    else
                    {
                        if (!item.File.IsNullOrEmpty() && !item.FileContent.IsNullOrEmpty() && item.File != item.OldFileName)
                        {
                            if (!item.OldFileName.IsNullOrEmpty())
                                await _mediaContainer.DeleteAsync(item.OldFileName);
                            await _mediaContainer.SaveAsync(item.File, item.FileContent);
                        }
                        else
                        {
                            if (item.File.IsNullOrEmpty() && !item.OldFileName.IsNullOrEmpty())
                            {
                                await _mediaContainer.DeleteAsync(item.OldFileName);
                            }
                        }
                        await _mediaManager.UpdateAsync(item.Id, null, amenityparagraphId, null, item.TitleEn, item.TitleAr, item.File, item.Order, item.IsActive, item.ConcurrencyStamp);
                    }



                }
            }
            var mediaTemp = new Media();
            foreach (var item in mediaToDelete)
            {
                mediaTemp = await _mediaRepository.FirstOrDefaultAsync(m => m.Id == item);
                try
                {
                    if (mediaTemp != null && !mediaTemp.File.IsNullOrEmpty())
                    {
                        if (await _mediaContainer.ExistsAsync(mediaTemp.File))
                            await _mediaContainer.DeleteAsync(mediaTemp.File);
                    }
                }
                catch { }
                await _mediaRepository.DeleteAsync(item, autoSave: true);
            }

            return true;
        }

        [Authorize(DIPPermissions.MediaGalleries.Edit)]
        public virtual async Task<bool> UpdateMediaGalleryMediasAsync(Guid mediaGalleryId, List<MediaDto> input)
        {
            var mediaToDelete = new List<Guid>();

            var existingMedia = await _mediaRepository.GetListAsync(t => t.MediaGalleryId == mediaGalleryId);
            if (existingMedia != null && existingMedia.Count > 0)
            {
                var existingMediaIds = existingMedia.Select(t => t.Id);
                mediaToDelete = existingMediaIds.Except(input.Select(r => r.Id)).ToList();
            }

            if (!input.IsNullOrEmpty())
            {
                foreach (var item in input)
                {
                    var media = await _mediaRepository.SingleOrDefaultAsync(t => t.Id == item.Id);

                    if (media == null)
                    {
                        if (!item.File.IsNullOrEmpty() && !item.FileContent.IsNullOrEmpty())
                            await _mediaContainer.SaveAsync(item.File, item.FileContent);

                        await _mediaManager.CreateAsync(item.Id, null, null, mediaGalleryId, item.TitleEn, item.TitleAr, item.File, item.Order, item.IsActive, item.ConcurrencyStamp);
                    }
                    else
                    {
                        if (!item.File.IsNullOrEmpty() && !item.FileContent.IsNullOrEmpty() && item.File != item.OldFileName)
                        {
                            if (!item.OldFileName.IsNullOrEmpty())
                                await _mediaContainer.DeleteAsync(item.OldFileName);
                            await _mediaContainer.SaveAsync(item.File, item.FileContent);
                        }
                        else
                        {
                            if (item.File.IsNullOrEmpty() && !item.OldFileName.IsNullOrEmpty())
                            {
                                await _mediaContainer.DeleteAsync(item.OldFileName);
                            }
                        }
                        await _mediaManager.UpdateAsync(item.Id, null, null, mediaGalleryId, item.TitleEn, item.TitleAr, item.File, item.Order, item.IsActive, item.ConcurrencyStamp);
                    }
                }
            }
            var mediaTemp = new Media();
            foreach (var item in mediaToDelete)
            {
                mediaTemp = await _mediaRepository.FirstOrDefaultAsync(m => m.Id == item);
                try
                {
                    if (mediaTemp != null && !mediaTemp.File.IsNullOrEmpty())
                    {
                        if (await _mediaContainer.ExistsAsync(mediaTemp.File))
                            await _mediaContainer.DeleteAsync(mediaTemp.File);
                    }
                }
                catch { }
                await _mediaRepository.DeleteAsync(item, autoSave: true);
            }

            return true;
        }
    }
}
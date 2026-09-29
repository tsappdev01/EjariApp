using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using System.Collections.Generic;

namespace DIP.Medias
{
    public partial interface IMediasAppService
    {
        Task<bool> UpdateZoneparagraphMediasAsync(Guid zoneparagraphId, List<MediaDto> input);
        Task<bool> UpdateAmenityparagraphMediasAsync(Guid amenityparagraphId, List<MediaDto> input);
        Task<bool> UpdateMediaGalleryMediasAsync(Guid mediaGalleryId, List<MediaDto> input);
    }
}
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.PageInfos
{
    public class PageInfoHeaderFrontEnd 
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Slug { get; set; }
    }
}
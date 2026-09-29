using DIP.AmenityParagraphs;
using DIP.TimeLines;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.TimeLineCategories
{
    public class TimeLineCategoryFrontEnd
    {
        public string Title { get; set; }
        public int Order { get; set; }

        public List<TimeLineFrontEnd> TimeLineFrontEndList { get; set; }
        public bool IsActive { get; set; }
        public Guid Id { get; set; }
    }
}
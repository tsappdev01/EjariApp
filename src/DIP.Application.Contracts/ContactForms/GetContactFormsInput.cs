using Volo.Abp.Application.Dtos;
using System;

namespace DIP.ContactForms
{
    public class GetContactFormsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }

        public GetContactFormsInput()
        {

        }
    }
}
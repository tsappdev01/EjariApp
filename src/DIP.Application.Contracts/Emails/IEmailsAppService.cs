using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using System.Collections.Generic;

namespace DIP.Emails
{
    public partial interface IEmailsAppService
    {

        Task SendEmail(string to, string subject, string body);
    }
}
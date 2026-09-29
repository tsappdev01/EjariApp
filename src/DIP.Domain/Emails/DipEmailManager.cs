
using DotLiquid;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace DLD.DSGP.Email
{
    public class DipEmailManager : DomainService
    {
        public DipEmailManager()
        {

        }
        public async Task<string> GenerateEmailAsync(string siteUrl, string firstName, string facebookUrl, 
                                                        string twitterUrl, string instegramUrl, string linkedinUrl, string youtubeUrl, string poBox, string phone)
        {
            var template = await System.IO.File.ReadAllTextAsync("wwwroot/EmailTemplates/emailTemplate.html");

            var templateData = Template.Parse(template);

            var output = templateData.Render(Hash.FromAnonymousObject(new
            {
                  siteUrl = siteUrl,
                firstName = firstName,
                facebookUrl = facebookUrl,
                twitterUrl = twitterUrl,
                instegramUrl = instegramUrl,
                linkedinUrl = linkedinUrl,
                youtubeUrl = youtubeUrl,
                poBox = poBox,
                phone = phone
            }));

            return output;
        }

    }
}

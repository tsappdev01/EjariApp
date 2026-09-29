using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Server.AspNetCore;
using DIP.Blazor.Menus;
using DIP.EntityFrameworkCore;
using DIP.Localization;
using DIP.MultiTenancy;
using Microsoft.OpenApi.Models;
using DIP.Blazor.Components.Layout;
using Volo.Abp;
using Volo.Abp.Account.Pro.Admin.Blazor.Server;
using Volo.Abp.Account.Pro.Public.Blazor.Server;
using Volo.Abp.Account.Public.Web;
using Volo.Abp.Account.Public.Web.ExternalProviders;
using Volo.Abp.Account.Public.Web.Impersonation;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Components.Server.LeptonXTheme;
using Volo.Abp.AspNetCore.Components.Server.LeptonXTheme.Bundling;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonX;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonX.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AuditLogging.Blazor.Server;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Gdpr.Blazor.Extensions;
using Volo.Abp.Gdpr.Blazor.Server;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Pro.Blazor;
using Volo.Abp.Identity.Pro.Blazor.Server;
using Volo.Abp.LanguageManagement.Blazor.Server;
using Volo.Abp.LeptonX.Shared;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Pro.Blazor.Server;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TextTemplateManagement.Blazor.Server;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Saas.Host;
using Volo.Saas.Host.Blazor;
using Volo.Saas.Host.Blazor.Server;
using Blazorise.RichTextEdit;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using static Volo.Abp.UI.Navigation.DefaultMenuNames.Application;
using System.Linq;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using System.Reflection.Metadata;
using System.Collections.Generic;
using static DIP.Blazor.DIPBlazorModule.RemoveJqueryScriptContributor;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using DIP.SoapServices;

namespace DIP.Blazor;

[DependsOn(
    typeof(DIPApplicationModule),
    typeof(DIPEntityFrameworkCoreModule),
    typeof(DIPHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAccountPublicWebImpersonationModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAccountPublicWebOpenIddictModule),
    typeof(AbpAccountPublicBlazorServerModule),
    typeof(AbpAccountAdminBlazorServerModule),
    typeof(AbpAuditLoggingBlazorServerModule),
    typeof(AbpIdentityProBlazorServerModule),
    typeof(AbpAspNetCoreComponentsServerLeptonXThemeModule),
    typeof(AbpAspNetCoreMvcUiLeptonXThemeModule),
    typeof(AbpOpenIddictProBlazorServerModule),
    typeof(LanguageManagementBlazorServerModule),
    typeof(SaasHostBlazorServerModule),
    typeof(TextTemplateManagementBlazorServerModule),
    typeof(AbpGdprBlazorServerModule)
   )]
[DependsOn(typeof(AbpBlobStoringFileSystemModule))]
    public class DIPBlazorModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.AddServerSideBlazor().AddHubOptions(hub => hub.MaximumReceiveMessageSize = 100 * 1024 * 1024);
        //For Development
        //context.Services.AddServerSideBlazor(options =>
        //{
        //    options.DetailedErrors = configuration.GetValue<bool>("DetailedErrors");
        //}).AddHubOptions(hub => hub.MaximumReceiveMessageSize = 100 * 1024 * 1024);
        context.Services.AddSignalR(e => {
            e.MaximumReceiveMessageSize = 102400000;
        });
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(DIPResource),
                typeof(DIPDomainModule).Assembly,
                typeof(DIPDomainSharedModule).Assembly,
                typeof(DIPApplicationModule).Assembly,
                typeof(DIPApplicationContractsModule).Assembly,
                typeof(DIPBlazorModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("DIP");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });
        
        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(builder =>
            {
                //builder.AddSigningCertificate(GetSigningCertificate(hostingEnvironment, configuration));
                //builder.AddEncryptionCertificate(GetSigningCertificate(hostingEnvironment, configuration));
                //builder.SetIssuer(new Uri(configuration["AuthServer:Authority"]));

                builder.AddEphemeralEncryptionKey().AddEphemeralSigningKey();
                builder.SetIssuer(new Uri(configuration["App:SelfUrl"]));
                builder.SetAccessTokenLifetime(TimeSpan.FromDays(30));
            });
        }

        //if (!hostingEnvironment.IsDevelopment())
        //{
        //    PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
        //    {
        //        options.AddDevelopmentEncryptionAndSigningCertificate = false;
        //    });

        //    PreConfigure<OpenIddictServerBuilder>(builder =>
        //    {
        //        // In production, it is recommended to use two RSA certificates, 
        //        // one for encryption, one for signing.
        //        builder.AddEncryptionCertificate(
        //                GetEncryptionCertificate(hostingEnvironment, context.Services.GetConfiguration()));
        //        builder.AddSigningCertificate(
        //                GetSigningCertificate(hostingEnvironment, context.Services.GetConfiguration()));
        //    });
        //}
    }

    //private X509Certificate2 GetSigningCertificate(IWebHostEnvironment hostingEnv,
    //                     IConfiguration configuration)
    //{
    //    var fileName = $"cert-signing.pfx";
    //    var passPhrase = configuration["MyAppCertificate:X590:PassPhrase"];
    //    var file = Path.Combine(hostingEnv.ContentRootPath, fileName);
    //    if (File.Exists(file))
    //    {
    //        var created = File.GetCreationTime(file);
    //        var days = (DateTime.Now - created).TotalDays;
    //        if (days > 600)
    //            File.Delete(file);
    //        else
    //            return new X509Certificate2(file, passPhrase,
    //                         X509KeyStorageFlags.MachineKeySet);
    //    }

    //    // file doesn't exist or was deleted because it expired
    //    using var algorithm = RSA.Create(keySizeInBits: 2048);
    //    var subject = new X500DistinguishedName("CN=Fabrikam Signing Certificate");
    //    var request = new CertificateRequest(subject, algorithm,
    //                        HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    //    request.CertificateExtensions.Add(new X509KeyUsageExtension(
    //                        X509KeyUsageFlags.DigitalSignature, critical: true));
    //    var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow,
    //                        DateTimeOffset.UtcNow.AddYears(2));
    //    File.WriteAllBytes(file, certificate.Export(X509ContentType.Pfx, string.Empty));
    //    return new X509Certificate2(file, passPhrase,
    //                        X509KeyStorageFlags.MachineKeySet);
    //}

    //private X509Certificate2 GetEncryptionCertificate(IWebHostEnvironment hostingEnv,
    //                             IConfiguration configuration)
    //{
    //    var fileName = $"cert-encryption.pfx";
    //    var passPhrase = configuration["MyAppCertificate:X590:PassPhrase"];
    //    var file = Path.Combine(hostingEnv.ContentRootPath, fileName);
    //    if (File.Exists(file))
    //    {
    //        var created = File.GetCreationTime(file);
    //        var days = (DateTime.Now - created).TotalDays;
    //        if (days > 600)
    //            File.Delete(file);
    //        else
    //            return new X509Certificate2(file, passPhrase,
    //                            X509KeyStorageFlags.MachineKeySet);
    //    }

    //    // file doesn't exist or was deleted because it expired
    //    using var algorithm = RSA.Create(keySizeInBits: 2048);
    //    var subject = new X500DistinguishedName("CN=Fabrikam Encryption Certificate");
    //    var request = new CertificateRequest(subject, algorithm,
    //                        HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    //    request.CertificateExtensions.Add(new X509KeyUsageExtension(
    //                        X509KeyUsageFlags.KeyEncipherment, critical: true));
    //    var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow,
    //                        DateTimeOffset.UtcNow.AddYears(2));
    //    File.WriteAllBytes(file, certificate.Export(X509ContentType.Pfx, string.Empty));
    //    return new X509Certificate2(file, passPhrase, X509KeyStorageFlags.MachineKeySet);
    //}


    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        
        if (!Convert.ToBoolean(configuration["App:DisablePII"]))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }
        
        if (!Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            }); 
        }

        Configure<AbpBlobStoringOptions>(options =>
        {
            options.Containers.ConfigureDefault(container =>
            {
                container.UseFileSystem(fileSystem =>
                {
                    fileSystem.BasePath = Path.Combine(hostingEnvironment.WebRootPath, "dipassets");
                });
            });
        });
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = false; // or configure specific paths
        });

        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureImpersonation(context, configuration);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureSwaggerServices(context.Services);
     
        ConfigureExternalProviders(context, configuration);
        ConfigureAutoApiControllers();
        ConfigureBlazorise(context);
        ConfigureRouter(context);
        ConfigureMenu(context);
        ConfigureCookieConsent(context);
        context.Services.AddHttpClient();

        // Configure SOAP Services
        Configure<SoapServicesConfiguration>(context.Services.GetConfiguration().GetSection("SoapServices"));

        ConfigureSignalRToUploadFile();
        ConfigureTheme();

        //context.Services.Configure<AbpBundlingOptions>(options =>
        //{
        //    options
        //        .ScriptBundles
        //        .Configure("MyGlobalBundle", bundle =>
        //        {
        //            bundle.AddContributors(typeof(MyExtensionGlobalStyleContributor));
        //        });
        //});

    }

    private void ConfigureSignalRToUploadFile()
    {
        Configure<HubOptions>(options =>
        {
            options.DisableImplicitFromServicesParameters = true;
        });

    }
    private void ConfigureRichTextEdit(ServiceConfigurationContext context)
    {
        context.Services
         .AddBlazoriseRichTextEdit(options =>
         {
             options.UseBubbleTheme = true;
         });
    }
    private void ConfigureCookieConsent(ServiceConfigurationContext context)
    {
        context.Services.AddAbpCookieConsent(options =>
        {
            options.IsEnabled = true;
            options.CookiePolicyUrl = "/CookiePolicy";
            options.PrivacyPolicyUrl = "/PrivacyPolicy";
        });
    }

    private void ConfigureTheme()
    {
        Configure<LeptonXThemeOptions>(options =>
        {
            options.DefaultStyle = LeptonXStyleNames.System;
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {

            // MVC UI
            options.StyleBundles.Configure(
                LeptonXThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );

            // Blazor UI
            options.StyleBundles.Configure(
                BlazorLeptonXThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/blazor-global-styles.css");
                    //You can remove the following line if you don't use Blazor CSS isolation for components
                    bundle.AddFiles("/DIP.Blazor.styles.css");
                }
            );


      
                //options
                //    .ScriptBundles
                //    .Configure(BlazorLeptonXThemeBundles.Scripts.Global, bundle => {
                //        bundle.AddContributors(typeof(MyExtensionGlobalStyleContributor));
                //    });








            //options.ScriptBundles.Configure(BlazorLeptonXThemeBundles.Scripts.Global, bundle =>
            //{
            //    bundle.AddContributors(typeof(MyExtensionGlobalStyleContributor));
            //});


            //options.ScriptBundles.Configure(BlazorLeptonXThemeBundles.Scripts.Global, bundle => {
            //    bundle.AddContributors(typeof(RemoveJqueryScriptContributor));
            //});
            //options.ScriptBundles.Configure(BlazorLeptonXThemeBundles.Scripts.Global,
            //    bundle =>
            //    {


            //        //bundle.AddFiles("/assets/js/jquery-3.6.0.min.js");
            //        //bundle.AddFiles("/assets/js/bootstrap.min.js");
            //        //bundle.AddFiles("/assets/js/jquery.fancybox.js");
            //        //bundle.AddFiles("/assets/js/owl.js");
            //        //bundle.AddFiles("/assets/js/swiper.min.js");
            //        //bundle.AddFiles("/assets/js/isotope.min.js");
            //        //bundle.AddFiles("/assets/js/appear.js");
            //        //bundle.AddFiles("/assets/js/dip-extension.js");
            //    });

        });
    }

    private void ConfigureImpersonation(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.Configure<SaasHostBlazorOptions>(options =>
        {
            options.EnableTenantImpersonation = true;
        });
        context.Services.Configure<AbpIdentityProBlazorOptions>(options =>
        {
            options.EnableUserImpersonation = true;
        });
        context.Services.Configure<AbpAccountOptions>(options =>
        {
            options.TenantAdminUserName = "admin";
            options.ImpersonationTenantPermission = SaasHostPermissions.Tenants.Impersonation;
            options.ImpersonationUserPermission = IdentityPermissions.Users.Impersonation;
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<DIPDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}DIP.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<DIPDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}DIP.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<DIPApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}DIP.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<DIPApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}DIP.Application"));
                options.FileSets.ReplaceEmbeddedByPhysical<DIPBlazorModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "DIP API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }

    private void ConfigureExternalProviders(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, _ => {})
            .WithDynamicOptions<GoogleOptions, GoogleHandler>(
                GoogleDefaults.AuthenticationScheme,
                options =>
                {
                    options.WithProperty(x => x.ClientId);
                    options.WithProperty(x => x.ClientSecret, isSecret: true);
                }
            )
            .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
            {
                //Personal Microsoft accounts as an example.
                options.AuthorizationEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
                options.TokenEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
            })
            .WithDynamicOptions<MicrosoftAccountOptions, MicrosoftAccountHandler>(
                MicrosoftAccountDefaults.AuthenticationScheme,
                options =>
                {
                    options.WithProperty(x => x.ClientId);
                    options.WithProperty(x => x.ClientSecret, isSecret: true);
                }
            )
            .AddTwitter(TwitterDefaults.AuthenticationScheme, options => options.RetrieveUserDetails = true)
            .WithDynamicOptions<TwitterOptions, TwitterHandler>(
                TwitterDefaults.AuthenticationScheme,
                options =>
                {
                    options.WithProperty(x => x.ConsumerKey);
                    options.WithProperty(x => x.ConsumerSecret, isSecret: true);
                }
            );
    }

 
    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        context.Services
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
    }

    private void ConfigureMenu(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new DIPMenuContributor());
        });
    }

    private void ConfigureRouter(ServiceConfigurationContext context)
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(DIPBlazorModule).Assembly;
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(DIPApplicationModule).Assembly);
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<DIPBlazorModule>();
        });
    }
    
    //private X509Certificate2 GetSigningCertificate(IWebHostEnvironment hostingEnv, IConfiguration configuration)
    //{
    //    var fileName = "authserver.pfx";
    //    var passPhrase = "";
    //    var file = Path.Combine(hostingEnv.ContentRootPath, fileName);

    //    if (!File.Exists(file))
    //    {
    //        throw new FileNotFoundException($"Signing Certificate couldn't found: {file}");
    //    }

    //    return new X509Certificate2(file, passPhrase);
    //}

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
            app.UseHsts();
        }

     

        app.UseCorrelationId();
        app.UseAbpSecurityHeaders();
        app.UseStaticFiles();
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/robots.txt"))
            {
                var robotsTxtPath = Path.Combine(env.ContentRootPath, "robots.txt");
                string output = "User-agent: *  \nDisallow: /";
                if (File.Exists(robotsTxtPath))
                {
                    output = await File.ReadAllTextAsync(robotsTxtPath);
                }
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(output);
            }
            else if (context.Request.Path.StartsWithSegments("/sitemap.xml"))
            {
                var robotsTxtPath = Path.Combine(env.ContentRootPath, "sitemap.xml");
                string output = "";
                if (File.Exists(robotsTxtPath))
                {
                    output = await File.ReadAllTextAsync(robotsTxtPath);
                }
                context.Response.ContentType = "text/xml";
                await context.Response.WriteAsync(output);
            }
            else await next();
        });
        app.UseCustomMiddleware();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

       // app.UseUnitOfWork();
        app.UseAuthorization();
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "DIP API");
            });
        }
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
    public class RemoveJqueryScriptContributor : BundleContributor
    {
        public override void ConfigureBundle(BundleConfigurationContext context)
        {
            var jquery = context.Files.FirstOrDefault(x => x.FileName.EndsWith("jquery.min.js", StringComparison.InvariantCultureIgnoreCase));
            if (jquery != null)
            {
                context.Files.Remove(jquery);
                var t = context;
            }
            //context.Files.Add("/assets/js/owl.js");

            //context.Files.Add("/assets/js/jquery-3.6.0.min.js");
            //context.Files.Add("/assets/js/bootstrap.min.js");
            //context.Files.Add("/assets/js/jquery.fancybox.js");
            //context.Files.Add("/assets/js/owl.js");
            //context.Files.Add("/assets/js/swiper.min.js");
            //context.Files.Add("/assets/js/isotope.min.js");
            //context.Files.Add("/assets/js/appear.js");
            //context.Files.Add("/assets/js/dip-extension.js");
            //var jquery = context.Files.FirstOrDefault(x => x.EndsWith("jquery.min.js", StringComparison.InvariantCultureIgnoreCase));
            //if (jquery != null)
            //{
            //    context.Files.Remove(jquery);
            //    var t = context;
            //}
            //jquery = context.Files.FirstOrDefault(x => x.EndsWith("bootstrap.bundle.min.js", StringComparison.InvariantCultureIgnoreCase));
            //if (jquery != null)
            //{
            //    context.Files.Remove(jquery);
            //    var t = context;
            //}

        }


        //public class MyExtensionGlobalStyleContributor : BundleContributor
        //{
        //    public override void ConfigureBundle(BundleConfigurationContext context)
        //    {
        //        context.Files.ReplaceOne(
        //            "/libs/bootstrap/css/bootstrap.css",
        //            "/assets/js/bootstrap.min.js"
        //        ); 
    
        //    }
        //}
    }
}

using System.Threading.Tasks;
using losol.EventManagement.Domain;
using losol.EventManagement.Services.Messaging;
using losol.EventManagement.Web.Config;
using losol.EventManagement.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace losol.EventManagement.Web.Services
{
    public sealed class MagicLinkSender : ApplicationEmailSender
    {
        private readonly IUrlHelper _urlHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Site _siteConfig;
        private readonly string _requestScheme;

        public MagicLinkSender(
                IEmailSender emailSender, 
                IRenderService renderService,
                IUrlHelperFactory urlHelperFactory,
                IActionContextAccessor actionContextAccessor,
                UserManager<ApplicationUser> userManager,
                Site siteConfig) 
            : base(emailSender, renderService) 
        {
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);;
            _requestScheme = actionContextAccessor.ActionContext.HttpContext.Request.Scheme;
            _userManager = userManager;
            _siteConfig = siteConfig;
        }

        protected override string Template => "Templates/Email/MagicLinkEmail";
        
        public async Task SendMagicLinkAsync(ApplicationUser user)
        {
            await _userManager.UpdateSecurityStampAsync(user);
            var token = await _userManager.GenerateUserTokenAsync(
                user: user,  
                tokenProvider: "MagicLinkTokenProvider", 
                purpose: "magic-link"
            );

            var magiclink = _urlHelper.Link(
                        routeName: "MagicLinkRoute", 
                        values: new { userid = user.Id, token = token,  });
            
            await base.SendAsync(
                emailAddress: user.Email, 
                subject: $"Innloggingslenke {_siteConfig.Title}",
                vm: new MagicLinkVM
                {
                    MagicLink = magiclink
                }
            );
            
        }
    }
}

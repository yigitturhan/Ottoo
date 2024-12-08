using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Widgets.AskToOpenAI.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.AskToOpenAI
{
    public class AskOpenAIPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        public bool HideInWidgetList => false;
        public AskOpenAIPlugin(IWebHelper webhelper, ISettingService settingService)
        {
            _webHelper = webhelper;
            _settingService = settingService;
        }
        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(AskOpenAIViewComponent);
        }
        public async Task<IList<string>> GetWidgetZonesAsync()
        {
            var widgetZones = new List<string> { PublicWidgetZones.ProductDetailsInsideOverviewButtonsAfter};
            return await Task.FromResult(widgetZones);
        }
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/ChatGPT/Configure";
        }
        public override async Task InstallAsync()
        {
            var settings = new Settings
            {
                ApiKey = "",
            };
            await _settingService.SaveSettingAsync(settings);
            await base.InstallAsync();
        }
        public override Task UninstallAsync()
        {
            _settingService.DeleteSettingAsync<Settings>();
            return base.UninstallAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Plugin.Widgets.AskToOpenAI.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Logging;
using System.Threading;
using System.Reflection;

namespace Nop.Plugin.Widgets.AskToOpenAI.Controller
{
    [AutoValidateAntiforgeryToken]
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class ChatGPTController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly Nop.Services.Logging.ILogger _logger; 
        public ChatGPTController(
            ISettingService settingService,
            IStoreContext storeContext,
            INotificationService notificationService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            Nop.Services.Logging.ILogger logger)
        {
            _settingService = settingService;
            _storeContext = storeContext;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _logger = logger;
        }
        
        public async Task<IActionResult> Configure()
        {
            try
            {
                if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                    return AccessDeniedView();
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var settings = await _settingService.LoadSettingAsync<Settings>(storeScope);
                var model = new ConfigurationModel
                {
                    ApiKey = settings.ApiKey,
                    ActiveStoreScopeConfiguration = storeScope,
                    Question_1 = settings.Question_1,
                    Question_2 = settings.Question_2,
                    Question_3 = settings.Question_3,
                    Question_4 = settings.Question_4,
                    Question_5 = settings.Question_5,
                    Question_6 = settings.Question_6,
                    Question_7 = settings.Question_7,
                    Question_8 = settings.Question_8,
                    Question_9 = settings.Question_9,
                    Question_10 = settings.Question_10,
                    QuestionCount = settings.QuestionCount
                };
                if (storeScope > 0)
                {
                    model.ApiKey_override = await _settingService.SettingExistsAsync(settings, x => x.ApiKey, storeScope);

                    for (int i = 1; i <= 10; i++)
                    {
                        string propertyName = $"Question_{i}";
                        model.Overridable[i - 1] = await _settingService.SettingExistsAsync(settings, x => GetPropertyValue(x, propertyName), storeScope);
                    }
                }
                await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Information, "Configuration is success.", "Success.");
                return View("~/Plugins/Widgets.AskToOpenAI/Views/ChatGPT/Configure.cshtml", model);
            }
            catch(Exception ex)
            {
                _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error", ex.Message);
                return View(ex.Message);
            }
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [AuthorizeAdmin]
        public async Task<IActionResult> Configure(ConfigurationModel model, bool generate)
        {
            if (generate)
            {
                if (model.QuestionCount > 0 && model.QuestionCount <= 10)
                {
                    return View("~/Plugins/Widgets.AskToOpenAI/Views/ChatGPT/Configure.cshtml", model);
                }
                else
                {
                    ModelState.AddModelError("QuestionCount", "Please enter a number between 1 and 10.");
                    return View("~/Plugins/Widgets.AskToOpenAI/Views/ChatGPT/Configure.cshtml", model);
                }
            }
            else
            {
                try
                {
                    var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                    var settings = await _settingService.LoadSettingAsync<Settings>(storeScope);
                    settings.ApiKey = model.ApiKey;
                    settings.Question_1 = model.Question_1;
                    settings.Question_2 = model.Question_2;
                    settings.Question_3 = model.Question_3;
                    settings.Question_4 = model.Question_4;
                    settings.Question_5 = model.Question_5;
                    settings.Question_6 = model.Question_6;
                    settings.Question_7 = model.Question_7;
                    settings.Question_8 = model.Question_8;
                    settings.Question_9 = model.Question_9;
                    settings.Question_10 = model.Question_10;
                    model.Overridable = new List<bool>();
                    for(int i = 0; i< 10; i++)
                    {
                        model.Overridable.Add(true);
                    }
                    await _settingService.SaveSettingOverridablePerStoreAsync(settings, x => x.ApiKey, model.ApiKey_override, storeScope, false);
                    for (int i = 1; i <= 10; i++)
                    {
                        string propertyName = "Question_" + i;
                        PropertyInfo property = settings.GetType().GetProperty(propertyName);
                        if (property != null)
                        {
                            PropertyInfo modelProperty = model.GetType().GetProperty(propertyName);
                            if (modelProperty != null)
                            {
                                property.SetValue(settings, modelProperty.GetValue(model));
                            }
                        }
                    }
                    await _settingService.ClearCacheAsync();
                    await _settingService.SaveSettingAsync(settings);
                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
                    await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Information, "Configuration is completed successfully.", "Success.");
                    return await Configure();
                }
                catch (Exception ex)
                {
                    _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error", ex.Message);
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the configuration.");
                    return View(ex.Message);
                }
            }
        }
        private object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }
    }
}

using Nop.Services.Logging;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.AskToOpenAI.Models;
using Nop.Plugin.Widgets.AskToOpenAI.Services;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Presentation;
using Humanizer;
using NUglify.JavaScript.Syntax;

namespace Nop.Plugin.Widgets.AskToOpenAI.Components
{
    public class AskOpenAIViewComponent : NopViewComponent
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger; 
        public AskOpenAIViewComponent(
            ISettingService settingService,
            IStoreContext storeContext,
            ILogger logger)
        {
            _storeContext = storeContext;
            _settingService = settingService;
            _logger = logger;
        }
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            try
            {
                var model = (ProductDetailsModel)additionalData;
                var response = new ChatGPTModel();
                var storeScope = _storeContext.GetActiveStoreScopeConfigurationAsync().Result;
                var settings = await _settingService.LoadSettingAsync<Settings>(storeScope);
                var chatGPTService = new ChatGPTClient(settings.ApiKey);
                response.Questions = new List<string>();
                if (string.IsNullOrEmpty(settings.ApiKey))
                {
                    response.Questions.Add("Please set the API key before use");
                    _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Error, "Api key is not set", "Error");
                }
                else
                {
                    response.ProductName = model.Name;
                    for (int i = 1; i <= 10; i++)
                    {
                        string question = (string)settings.GetType().GetProperty("Question_" + i.ToString()).GetValue(settings);
                        if (!string.IsNullOrEmpty(question))
                        {
                            response.Questions.Add(question + " of " + model.Name);
                        }
                    }
                    _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Information, "Plugin Invoke is completed successfully.", "Success.");
                }
                return await Task.FromResult(View("~/Plugins/Widgets.AskToOpenAI/Views/AskOpenAI.cshtml", response));
            }
            catch (Exception ex)
            {
                _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Error, "Error", ex.Message);
                return await Task.FromResult(View(ex.Message));
            }
        }
    }
}

using Microsoft.Extensions.Caching.Memory;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using Nop.Services.Logging;
using Nop.Plugin.Widgets.AskToOpenAI.Services;
using System;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Widgets.AskToOpenAI.Controller
{
    public class MessageController : BaseController
    {
        private readonly IMemoryCache _cacheManager;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        public MessageController(IMemoryCache cacheManager,
            ISettingService settingService,
            IStoreContext storeContext,
            ILogger logger)
        {
            _cacheManager = cacheManager;
            _settingService = settingService;
            _storeContext = storeContext;
            _logger = logger;
        }
        [HttpGet]
        public string GetResponse(string userQuestion)
        {
            try
            {
                var storeScope = _storeContext.GetActiveStoreScopeConfigurationAsync().Result;
                var cacheKey = $"Response_GetResponse_{userQuestion}";
                var settings = _settingService.LoadSettingAsync<Settings>(storeScope).Result;
                if (!_cacheManager.TryGetValue(cacheKey, out string response))
                {
                    var client = new ChatGPTClient(settings.ApiKey);
                    string result = client.SendMessage(userQuestion);
                    _cacheManager.Set(cacheKey, result, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                    });
                    response = result;
                }
                _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Information,
                    $"New ChatGPT API request called. Sending the request {userQuestion}", "Success.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error", ex.Message);
                return "An error occured";
            }
        }
    }
}

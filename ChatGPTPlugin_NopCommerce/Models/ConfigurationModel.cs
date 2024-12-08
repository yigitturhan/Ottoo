using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentMigrator.Infrastructure;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.AskToOpenAI.Models
{
    public record class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
        public string ApiKey { get; set; }
        public bool ApiKey_override { get; set; }
        public List<bool> Overridable { get; set; }
        
        [Range(1, 10, ErrorMessage = "Please enter a number between 1 and 10.")]
        [Display(Name = "Number of Questions")]
        public int QuestionCount { get; set; }

        public string Question_1 { get; set; }
        public string Question_2 { get; set; }
        public string Question_3 { get; set; }
        public string Question_4 { get; set; }
        public string Question_5 { get; set; }
        public string Question_6 { get; set; }
        public string Question_7 { get; set; }
        public string Question_8 { get; set; }
        public string Question_9 { get; set; }
        public string Question_10 { get; set; }

    }
}

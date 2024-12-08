using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentMigrator.Infrastructure;

namespace Nop.Plugin.Widgets.AskToOpenAI.Models
{
    public class ChatGPTModel
    {
        public string ProductName { get; set; }
        public List<string> Questions { get; set; } = new List<string>();
    }
}

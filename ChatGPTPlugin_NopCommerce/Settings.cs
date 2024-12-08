using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.AskToOpenAI
{
    public class Settings : ISettings
    {
        public string ApiKey { get; set; }
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

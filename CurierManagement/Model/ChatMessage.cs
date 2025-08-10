using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CurierManagement.Model
{
    public class ChatMessage
    {
        public string Text { get; set; } = "";
        public string Header { get; set; } = "";
        public bool IsCurrentUser { get; set; }
        public string BackgroundColor { get; set; } = "#F5F5F5";
        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
    }
}

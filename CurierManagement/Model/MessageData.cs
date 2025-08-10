using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.Model
{
    public class MessageData
    {
        public string Type { get; set; } = "";
        public string User { get; set; } = "";
        public string Message { get; set; } = "";
        public string Time { get; set; } = "";
    }
}

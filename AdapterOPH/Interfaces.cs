using Models;
using System;
using System.Threading.Tasks;
using NLog;

namespace AdapterOPH
{
    public interface IReport
    {
        ReportDefinition Report { get; }
        DateTime Start { get; set; }
        DateTime End { get; set; }
        bool ConsoleMode { get; set; }
        bool GetAttr();
        bool Generate();
        bool ToFile();
       int Progress { get; set; }
       bool MultiSheets { get; set; }
       void State(int percent, int? level, string message);
       //delegate void ReportEvent(object sender, ReportEventArgs e);
       // event ReportEvent ReportChanged;
      
    }
    public class ReportEventArgs
    {
        public int Value { get; }
        public string Message { get; }
        public int? Level { get; }

        public bool WriteLog;
        public ReportEventArgs(int value, int? level, string message)
        {
            Value = value;
            Message = message;
            Level = level;
        }
    }

}

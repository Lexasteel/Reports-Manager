using AdapterOPH;
using Models;
using NLog;
using Reports;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ReportMgr
{
    public static class Program
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


        //private static List<Historian> hist;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        public static void Main(string[] args)
        {
            var mode = args.Length > 0 ? args[0] : "-gui"; //default to gui
            var log = LogManager.GetCurrentClassLogger();
            //try
            //{
                switch (mode)
                {
                    case "-gui":
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new FrmMain());
                        break;
                    case "-con":
                    {
                        AttachConsole(-1);
                        Console.WriteLine();
                        Console.Write("Connect to database");
                        var t = Task.Factory.StartNew(()=> ReportDefinition.GetByEnabled(FrmMain.DataConnection()));
                        while (!t.IsCompleted && !t.IsFaulted)
                        {
                            Console.Write(".");
                            Thread.Sleep(1000);
                            if (t.Status == TaskStatus.Created)
                            {
                                t.Start();
                            }
                        }
                        Console.WriteLine("OK");
                        var reports = t.Result;
                        var count = reports.Count;
                        Console.WriteLine($"Total reports: {count}");
                        foreach (var report in reports)
                        {
                            var res = true;
                            report.HistPoints = ReportDefinition
                                .GetHistPoints(FrmMain.DataConnection(), report.reportdefinitionid);
                            report.Historians = Historian.GetAll(FrmMain.DataConnection());

                            while (res)
                            {
                                var hCount = (66 - report.reportname.Length) / 2;
                                var header = "|" + new string(' ', hCount) + report.reportname +
                                             new string(' ', hCount) + "|";
                                Console.WriteLine(new string('=', header.Length));
                                Console.WriteLine(header);
                                Console.WriteLine(new string('=', header.Length));
                                var end = report.nextevent.Value;
                                var start = HelpersAdapter.DateCalc(report.nextevent.Value,
                                    report.timeperiodinfo, report.timeformatid, true);
                                var target = CreateReport(report, start, end);
                                target.ConsoleMode = true;
                                res = target.Generate();
                                if (res)
                                {
                                    var nextEvent = HelpersAdapter.DateCalc(report.nextevent.Value,
                                        report.timeperiodinfo, report.timeformatid);
                                    report.nextevent = nextEvent;
                                    report.lastused = DateTime.Now;
                                    ReportDefinition.Update(FrmMain.DataConnection(), report);
                                    if (nextEvent > DateTime.Now)
                                    {
                                        res = false;
                                    }
                                }
                                else
                                {
                                    res = false;
                                }
                            }
                        }

                        var gl = "GOOD LUCK!!!";
                        var fc = (66 - gl.Length) / 2;
                        Console.WriteLine(new string('=', fc) + gl + new string('=', fc));
                        
                        System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                        //Thread.Sleep(5000);
                        FreeConsole();
                        break;
                    }
                    default:
                        Console.WriteLine();
                        Console.WriteLine("ReportMgr -gui or empty      window mode");
                        Console.WriteLine("ReportMgr -con               console mode");
                        System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                        FreeConsole();
                        break;
                }
            //}
            //catch (Exception e)
            //{
            //    log.Error(e.Message);
            //    Console.WriteLine(e.Message);
              
            //}
        }

        static IReport CreateReport(ReportDefinition report, DateTime start, DateTime end)
        {
            switch (report.reporttypeid)
            {
                case 1: //simple
                    return new SimpleReport(report, start, end);
                case 2: //oper.events
                    break;
                case 3: //alarms
                    break;
                case 4: //RAW
                    return new RawReport(report, start, end);


                //}
            }
            return new SimpleReport(report, start, end);
            //static SQLiteConnection CreateConnection()
            //{
            //    SQLiteConnection sqlite_conn;
            //    sqlite_conn = new SQLiteConnection("Data Source=Rpt.db;Version=3;New=true;Compress=true;");
            //    try
            //    {
            //        sqlite_conn.Open();
            //    }
            //    catch (Exception ex)
            //    {

            //        Console.WriteLine(ex.Message);
            //    }
            //    return sqlite_conn;
            //}


        }
    }
}
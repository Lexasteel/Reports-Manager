using DevExpress.Compression;
using Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AdapterOPH
{
    public class SimpleReport : IReport
    {
        public SimpleReport(ReportDefinition report, DateTime start, DateTime end, bool test=false)
        {
            this.Report = report;
            this.ReportChanged += Report_ReportChanged;
            this.Start = start;
            this.End = end;
        }

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public delegate void ReportEvent(object sender, ReportEventArgs e);
        public event ReportEvent ReportChanged;

        public void State(int percent, int? level, string message)
        {
            ReportChanged?.Invoke(this, new ReportEventArgs(percent, level, message));
            this.Progress = percent;
        }

        public bool Test = true;
        private int _totalSamples = 1;
        public int Progress { get; set; }
        public ReportDefinition Report { get; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool ConsoleMode { get; set; }
        public bool MultiSheets { get; set; }

        public bool GetAttr()
        {
            if (Report.HistPoints != null)
            {
                var points = Report.HistPoints.FirstOrDefault(s => s.pointname.Contains("."));
                if (points == null) return false;
            }

            if (Report.HistPoints != null)
            {
                var units= Report.HistPoints.Where(w=>w.pointname.Contains(".")).GroupBy(g => g.pointname.Split('.'));
            
                foreach (var unit in units)
                {
                    if (unit.Key.Length<2) continue;
                    if (string.IsNullOrEmpty(unit.Key[1]) || string.IsNullOrWhiteSpace(unit.Key[1])) continue;
                    State(Progress, null, $"Getting point attributes: {unit.Key[1]}");

                    if (Report.Historians == null) continue;
                    var ip = Report.Historians.FirstOrDefault(f => f.unitnet == unit.Key[1])?.ip;
                    var connection = new ConnectToHist();

                    var conn = connection.Open(ip, this);
                    if (conn == null) return false;
                    var client = new OvHNetDataClient();
                    client.Connect(conn);

                    var sk = 0;
                    var p = 0;
                    while (p < unit.Count())
                    {
                        p += 800;
                        var pointNames = unit.Select(s => s.pointname).Skip(sk).Take(p - sk).ToArray();
                        var count = (uint)pointNames.Length;
                        var maxItems = count;
                        var ovhErrors = new int[maxItems];
                        var items = new uint[count];
                        var collection = new PointConfigurationCollection();
                        var attributeCollection = new PointAttributeCollection();
                        var handles = client.GetHandles(count, pointNames, items, ovhErrors);
                        var config = client.GetPointConfig(pointNames.Length, pointNames, collection, ovhErrors);
                        var attrib = client.GetPointAttributes(new HistTimeStamp(), new HistTimeStamp(), count, items, ref maxItems, attributeCollection, ovhErrors);
                        List<PointAttribute> listattr = new List<PointAttribute>();
                        for (int i = 0; i < attributeCollection.Count; i++)
                        {
                            listattr.Add(attributeCollection.Item(i));
                        }

                        for (int i = 0; i < pointNames.Length; i++)
                        {
                            HistPoint h = unit.Skip(sk).Take(p - sk).ToList()[i];
                            h.Handle = items[i];
                            h.Type = collection.Item(i).DataType;
                            //h.ScanMsec = collection.Item(i).scanMsecs;

                            if (h.Handle > 0)
                            {
                                PointAttribute pointAttribute = listattr.FirstOrDefault(f => f.hPoint == h.Handle);
                                h.description = pointAttribute.lpszDesc;
                               // h.Units = pointAttribute.lpszEngUnits;
                            }
                            else
                            {
                                h.description = "INVALID POINT NAME";
                            }
                            if (h.Type == "P") h.description += " [" + h.bitnumber.ToString() + "]";

                        }
                        sk = p;
                    }

                    State(Progress + 1, 2, $"Get attributes...{unit.Key}");
                    State(Progress, null, $"Point names received {unit.Key}");
                    connection.Close();
                }
            }

            if (Report.HistPoints != null)
                State(Progress, null, $"Names of {Report.HistPoints.Count} points received.");
            return true;
        }

        public bool Generate()
        {
            var h = $"Generate {Report.reportname} from {Start:G} to {End:G}";
            State(0, 2, h);//LogLevel.Info

            TimeSpan period = HelpersAdapter.DateCalc(new DateTime(), Report.timeperiodinfo, Report.timeformatid).Subtract(new DateTime());
            int totalSeconds = (int)End.Subtract(Start).TotalSeconds;
            if (Report.sampletimeformatid == 6)
            {
                if (Report.HistPoints != null)
                    _totalSamples = Convert.ToInt32(Report.HistPoints.Count * (totalSeconds / 0.1));
            }
            else
            {
                var denum = (int)TimeSpan.Parse(Report.sampletimeperiodinfo).TotalSeconds;
                if (denum>0)
                    if (Report.HistPoints != null)
                        _totalSamples = Report.HistPoints.Count * (totalSeconds / denum);
            }


            if (Report.HistPoints != null)
            {
                var units = Report.HistPoints.GroupBy(g => g.pointname.Split('.')[1]);
                if (!GetAttr()) return false;

                foreach (var unit in units)
                {
                    if (Report.Historians != null)
                    {
                        var ip = Report.Historians.FirstOrDefault(f => f.unitnet == unit.Key)?.ip;
                        var conn = new ConnectToHist();
                        var client = new OvHNetDataClient();
                        client.Connect(conn.Open(ip, this));


                        for (var reportPeriod = Start; reportPeriod
                                                       < End; reportPeriod = HelpersAdapter.DateCalc(reportPeriod, Report.timeperiodinfo, Report.timeformatid))
                        {
                            //if (HelpersAdapter.DateCalc(reportPeriod, Report.timeperiodinfo, Report.timeformatid) > End) End = end;
                            var step = new TimeSpan(24, 0, 0);
                            var pointCount = (uint)Report.HistPoints.Count;
                            uint maxItems = 10000;
                            var ovhErrors = new int[maxItems];
                            var val = new TimeVal();
                            switch (Report.sampletimeformatid)
                            {
                                case 1:
                                    val.tv_sec = Convert.ToInt32(TimeSpan.Parse(Report.sampletimeperiodinfo).TotalSeconds);
                                    break;
                                case 6:
                                    var dNsec = double.Parse(Report.sampletimeperiodinfo, CultureInfo.InvariantCulture);
                                    var nsec = Convert.ToInt32(1000000000 * dNsec);
                                    val.tv_nsec = nsec;
                                    break;
                                default:
                                    val.tv_sec = 86400;
                                    break;
                            }

                            var reportStart = Start.ToUniversalTime();
                            var reportEnd = End.ToUniversalTime();
                            var reportStartHist = OvHNetHelper.DateTimeToHistTime(reportStart);
                            var reportEndHist = new HistTimeStamp();
                            var s =
                                $"Getting data from {Start:G} to {End:G}";
                            State(Progress++, 2, s);
                            while (reportStart < reportEnd)
                            {
                                reportEndHist = OvHNetHelper.DateTimeToHistTime(reportStart.Add(step));
                                if (reportStart.Add(step) > End.ToUniversalTime().AddSeconds(-val.tv_sec))
                                {
                                    reportEndHist = OvHNetHelper.DateTimeToHistTime(reportEnd);
                                }
                                reportStartHist = OvHNetHelper.DateTimeToHistTime(reportStart);
                                HelpersAdapter.ReadProcessed(client, unit, reportStartHist, reportEndHist, val, pointCount, maxItems, ovhErrors, Report, _totalSamples);
                                reportStart = reportStart.Add(step);
                                var percents = Convert.ToInt32(reportStart.Subtract(Start).TotalSeconds * 75 / reportEnd.Subtract(Start).TotalSeconds);
                                percents += Progress;
                                State(percents, 2, $"{percents}%");
                            }


                           // SaveFile(Report.HistPoints, histstart, histend, shiftday);
                            
                        }

                        if (client.IsConnected()) client.Disconnect();
                    }
                }
            }

            this.ReportChanged -= Report_ReportChanged;
            if (Report.HistPoints != null)
            {
                var count = Report.HistPoints.Select(s => s.FValues.Count).Sum();
                if (count > 0)
                {
                    //ExportFile.Export(report);
                    foreach (var item in Report.HistPoints)
                    {
                        item.FValues = new Dictionary<DateTime, float>();
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;

            

        }

        public bool ToFile()
        {
            if ((bool)Report.arhive)
            {
                string tempPath = "";
                using (ZipArchive archive = new ZipArchive())
                {
                    string filename = Path.GetFullPath(Report.destinationinfo);
                    archive.AddFile(tempPath, "/");
                    archive.Save(Path.GetDirectoryName(tempPath) + "/" +
                                 Path.GetFileNameWithoutExtension(tempPath) + ".zip");
                    File.Delete(tempPath);
                }
            }

            if ((bool)Report.arhive)
            {
                // DateTime start = OvHNetHelper.HistTimeToDateTime(histstart).ToLocalTime();
                string source = Path.GetDirectoryName(Report.destinationinfo) + @"\" +
                                Start.ToLocalTime().ToString("yyyy");
                string destination = Path.GetDirectoryName(Report.destinationinfo) + @"\" +
                                     Start.ToLocalTime().ToString("yyyyMMdd") + ".zip ";

                using (ZipArchive archive = new ZipArchive())
                {
                    archive.AddDirectory(source, Start.ToString("yyyy"));
                    archive.Save(destination);
                    Directory.Delete(source, true);
                }
            }

            return false;
        }

        private void Report_ReportChanged(object sender, ReportEventArgs e)
        {


                Console.WriteLine("\r{0}%   {1}", e.Value, e.Message);

                switch (e.Level)
                {
                    case 0: //Trace
                    {
                        _log.Trace(e.Message);
                        break;
                    }
                    case 1: //Debug
                    {
                        _log.Debug(e.Message);
                        break;
                    }
                    case 2: //Info
                    {
                        _log.Info(Report.reportname + ":" + e.Message);
                        break;
                    }
                    case 3: //Warn
                    {
                        _log.Warn(e.Message);
                        break;
                    }
                    case 4: //Error
                    {
                        _log.Error(Report.reportname + ":" + e.Message);
                        break;
                    }
                    case 5: //Fatal
                    {
                        _log.Fatal(e.Message);
                        break;
                    }
                }
            //}
        }

    }
}

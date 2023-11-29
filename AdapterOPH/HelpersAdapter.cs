using Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterOPH
{
     public class HelpersAdapter
    {
        public static PointParamsCollection PointParamsCollectionInit(IEnumerable<HistPoint> _histPoints, bool RAW)
        {
            PointParamsCollection pointParamsCollection = new PointParamsCollection();
            foreach (var item in _histPoints)
            {
                PointParams pointParams = new PointParams()
                {
                    dwBitPosition = Convert.ToUInt32(item.bitnumber),
                    flConstant = Convert.ToSingle(item.integconst, new CultureInfo("en-US")),
                    hPoint = item.Handle,
                    dwOptions = 1
                };
                pointParams.dwAggregate = !RAW ? Convert.ToUInt32(item.proctype) : (uint)4096;
                pointParamsCollection.Add(pointParams);
            }
            return pointParamsCollection;
        }

        public static void ParseData(HistItemCollection histItemCollection, IEnumerable<HistPoint> histPoints, int SampleTimeFormatID)
        {

            //   Dictionary<uint, HistPoint> dictHistPoints = histPoints.ToDictionary(d => d.Handle, s => s);

            List<HistItem> items = new List<HistItem>();
            foreach (HistItem item in histItemCollection)
            {
                items.Add(item);
            }

            Parallel.ForEach(histPoints, hp =>
            {

                IEnumerable<HistItem> histItems = items.Where(w => w.hPoint == hp.Handle);
                int decim = 2;
                if (hp.format.Contains("."))
                {
                    string[] format = hp.format.Split('.');
                    if (format.Length > 1) decim = format[1].Length;
                }

                foreach (HistItem i in histItems)
                {


                    DateTime d;
                    if (i.dwNSec > 0)
                    {
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //
                        d = SampleTimeFormatID==6 ? OvHNetHelper.HistTimeToDateTime(i.tsTimeStamp).AddTicks(i.dwNSec / 100) : OvHNetHelper.HistTimeToDateTime(i.tsTimeStamp).AddSeconds(1);
                    }
                    else
                    {
                        d = OvHNetHelper.HistTimeToDateTime(i.tsTimeStamp);
                    }

                    switch (hp.Type)
                    {
                        case "R":
                            hp.F_Values[d] = (float)Math.Round(i.flValue, decim);
                            break;
                        case "D":
                            // uint bit = (i.dwStatus >> 1) & 1;
                            hp.F_Values[d] = Convert.ToSingle(GetBit(i.dwStatus, 0));
                            break;
                        case "P":
                            hp.F_Values[d] = Convert.ToSingle(GetBit(i.dwRawValue, hp.bitnumber));
                            break;
                        default:
                            break;
                    }


                    if (i.dwIndicator == 0)
                    {
                        //  LogWrite(String.Join(";", Points[i.hPoint].pointname, i.tsTimeStamp.ToString(), i.dwSampFlags, i.dwIndicator, i.dwStatus));
                    }
                    //if (GetBit(i.dwStatus, 8) == true && OnlyGP)
                    //{
                    //    // LogWrite(String.Join(";", Points[i.hPoint].pointname, i.tsTimeStamp.ToString(), i.dwSampFlags, i.dwIndicator, i.dwStatus, "BAD"));
                    //    continue;
                    //}

                }
            });
            items.Clear();
        }

        private static bool GetBit(uint b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        public static void ReadProcessed(OvHNetDataClient client, IEnumerable<HistPoint> _histPoints, HistTimeStamp _histstart, HistTimeStamp _histend, TimeVal _val, uint pointCount, uint maxItems, int[] ovhErrors, ReportDefinition report, int TotalSamples)
        {

            HistSummaryCollection _histSummary = new HistSummaryCollection();
            int pr = 0;
            int count = 0;
            HistItemCollection _histItems = new HistItemCollection();

            PointParamsCollection pointParams = HelpersAdapter.PointParamsCollectionInit(_histPoints, false);
            pr = client.SyncReadProcessed(_histstart, _histend, _val, (uint)_histPoints.Count(), pointParams, _histSummary, ref maxItems, _histItems, ovhErrors);
            if (pr < 0) { throw new Exception(new OvHNetClientErrors().GetErrString(pr)); }
            HelpersAdapter.ParseData(_histItems, _histPoints, report.sampletimeformatid);
            string first = _histPoints.First().F_Values.First().Key.ToLocalTime().ToString("G");
            string last = _histPoints.First().F_Values.Last().Key.ToLocalTime().ToString("G");
            string s = $"Getting data from {first} to {last}";
            count += _histItems.Count;
            var percs = count * 75 / TotalSamples;
            //???????????????????????????????
            //if (report.Progress < percs) report.State(percs, LogLevel.Off, s);
            if (_val.tv_sec == 0) _val.tv_sec = 1;
            while (pr > 0)
            {
                _histItems = new HistItemCollection();
                pr = client.SyncReadNextProcessed(ref maxItems, _histItems, ovhErrors);
                HelpersAdapter.ParseData(_histItems, _histPoints, report.sampletimeformatid);
                count += _histItems.Count;
                percs = count * 75 / TotalSamples;
                first = _histPoints.First().F_Values.First().Key.ToLocalTime().ToString("G");
                last = _histPoints.First().F_Values.Last().Key.ToLocalTime().ToString("G");
                s = $"Getting data from {first} to {last}";
               //??????????????????????????????????????????????
                // if (report.Progress < percs) report.State(percs, LogLevel.Off, s);
            }

        }

        public static DateTime DateCalc(DateTime date, string TimePeriodInfo, int TimeFormatID, bool back = false)
        {
            int days = int.Parse(TimePeriodInfo.Split(':')[0]);
            switch (TimeFormatID)
            {
                //timespan
                case 1:
                    TimeSpan ts = TimeSpan.Parse(TimePeriodInfo);
                    if (back) return date.Subtract(ts);
                    else return date.Add(ts);
                //day
                case 2:
                    if (back) return date.AddDays(-days);
                    else return date.AddDays(days);
                //month
                case 3:
                    if (back) return date.AddMonths(-days);
                    else return date.AddMonths(days);
                default: return new DateTime();
            }
        }

     

    }
}

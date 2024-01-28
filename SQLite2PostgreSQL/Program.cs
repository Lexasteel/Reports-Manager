using Dapper;
using Models;
using Npgsql;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace SQLite2PostgreSQL
{
    class Program
    {
        static void Main(string[] args)
        {


            var npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 5432,
                Database = "Reports",
                UserName = "postgres",
                Password = "postgres"
            };

            var connNpg = new NpgsqlConnection(npgsqlConnectionStringBuilder.ConnectionString);


            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder()
            {
                DataSource = @"c:\DEV\rpt.db",

            };

            var conn = new SQLiteConnection(builder.ConnectionString);

           
            
            var reports = ReportDefinition.GetAll(conn).OrderByDescending(o => o.nextevent).ThenBy(t=>t.reportname);
            var sql = "SELECT * from histpoints";
            var results = conn.Query<HistPoint>(sql);
            var points = results.GroupBy(g => g.reportdefinitionid).ToDictionary(d => d.Key, d => d.ToList());


            Database db = new Database();
            db.Init(connNpg);

            foreach (var report in reports)
            {
                var pointsByReport = new List<HistPoint>();
                if (report.reporttypeid == 1 || report.reporttypeid == 4)
                {
                    if (!points.ContainsKey(report.reportdefinitionid)) continue;
                    pointsByReport = points[report.reportdefinitionid].OrderBy(o => o.pointposn).ToList();
                        if (pointsByReport.Count == 0) continue;
                }

                
                    var newId = ReportDefinition.Insert(connNpg, report);
                    if (report.reporttypeid == 2 || report.reporttypeid == 3) continue;

                    for (int i = 0; i < pointsByReport.Count; i++)
                    {
                        pointsByReport[i].reportdefinitionid = newId;
                        pointsByReport[i].pointposn = i + 1;
                        var id = HistPoint.Insert(connNpg, pointsByReport[i]);
                    }
                
               

            }

            var historians = Historian.GetAll(conn);
            foreach (var item in historians)
            {
                var h = Historian.Insert(connNpg, item);
            }
        }

    }
}

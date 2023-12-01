using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace Models
{
    public class ReportDefinition
    {
        [Key]
        public int reportdefinitionid { get; set; }
        public string reportname { get; set; } = "Отчет по блоку";
        public int reporttypeid { get; set; } = 1;
        public int reportdestid { get; set; } = 3;
        public string? destinationinfo { get; set; }
        public bool arhive { get; set; }
        public string header2 { get; set; } = "2,3";
        public int timeformatid { get; set; } = 2;
        public string timeperiodinfo { get; set; } = "1";
        public int sampletimeformatid { get; set; } = 1;
        public string sampletimeperiodinfo { get; set; } = "0:0:0";
        public string? additionalparams { get; set; }
        public int unit { get; set; }
        public string shift { get; set; } = new TimeSpan(0, 0, 0).ToString();
        public DateTime? nextevent { get; set; }
        public DateTime? lastused { get; set; }
        public bool enable { get; set; }
        [Write(false)]
        public List<HistPoint>? HistPoints { get; set; }

        [Write(false)]
        public IEnumerable<Historian>? Historians { get; set; }

        public override bool Equals(object obj)
        {
            //As the obj parameter type is object, so we need to
            //cast it to Student Type
            return this.reportdefinitionid == ((ReportDefinition)obj).reportdefinitionid
                   && this.reportname == ((ReportDefinition)obj).reportname
                   && this.reporttypeid == ((ReportDefinition)obj).reporttypeid
                   && this.reportdestid == ((ReportDefinition)obj).reportdestid
                   && this.destinationinfo == ((ReportDefinition)obj).destinationinfo
                   && this.arhive == ((ReportDefinition)obj).arhive
                   && this.header2 == ((ReportDefinition)obj).header2
                   && this.timeformatid == ((ReportDefinition)obj).timeformatid
                   && this.timeperiodinfo == ((ReportDefinition)obj).timeperiodinfo
                   && this.sampletimeformatid == ((ReportDefinition)obj).sampletimeformatid
                   && this.sampletimeperiodinfo == ((ReportDefinition)obj).sampletimeperiodinfo
                   && this.additionalparams == ((ReportDefinition)obj).additionalparams
                   && this.unit == ((ReportDefinition)obj).unit
                   && this.shift == ((ReportDefinition)obj).shift
                   && this.nextevent == ((ReportDefinition)obj).nextevent
                   && this.lastused == ((ReportDefinition)obj).lastused
                   && this.enable == ((ReportDefinition)obj).enable;
        }
        public override int GetHashCode()
        {
            var idHashCode = this.reportdefinitionid.GetHashCode();
            var nameHashCode = this.reportname == null ? 0 : this.reportname.GetHashCode();
            var typeIdHashCode = this.reporttypeid == null ? 0 : this.reporttypeid.GetHashCode();
            var destIdHashCode = this.reportdestid == null ? 0 : this.reportdestid.GetHashCode();
            var destInfoHashCode = this.destinationinfo == null ? 0 : this.destinationinfo.GetHashCode();
            var archHashCode = this.arhive == null ? 0 : this.arhive.GetHashCode();
            var headerHashCode = this.header2 == null ? 0 : this.header2.GetHashCode();
            var timeIdHashCode = this.timeformatid == null ? 0 : this.timeformatid.GetHashCode();
            var timePeriodHashCode = this.timeperiodinfo == null ? 0 : this.timeperiodinfo.GetHashCode();
            var sampleIdHashCode = this.sampletimeformatid == null ? 0 : this.sampletimeformatid.GetHashCode();
            var samplePeriodHashCode = this.sampletimeperiodinfo == null ? 0 : this.sampletimeperiodinfo.GetHashCode();
            var addParHashCode = this.additionalparams == null ? 0 : this.additionalparams.GetHashCode();
            var unitHashCode = this.unit == null ? 0 : this.unit.GetHashCode();
            var shiftHashCode = this.shift == null ? 0 : this.shift.GetHashCode();
            var nextHashCode = this.nextevent == null ? 0 : this.nextevent.GetHashCode();
            var lastHashCode = this.lastused == null ? 0 : this.lastused.GetHashCode();
            var enableHashCode = this.enable == null ? 0 : this.enable.GetHashCode();


            return idHashCode ^ nameHashCode ^ typeIdHashCode ^ destIdHashCode ^ destInfoHashCode ^ archHashCode ^
                   headerHashCode ^ timeIdHashCode ^ timePeriodHashCode ^ sampleIdHashCode ^ samplePeriodHashCode ^ addParHashCode ^
                   unitHashCode ^ shiftHashCode ^ nextHashCode ^ lastHashCode ^ enableHashCode;
        }



        public static async Task<IEnumerable<ReportDefinition>> GetAll(IDbConnection connection)
        {
            var sql = "SELECT * from reportdefinitions ORDER BY reportname";
            //Console.WriteLine(sql);
            connection.Close();
            return await connection.QueryAsync<ReportDefinition>(sql).ConfigureAwait(false);
        }

        public static async Task<List<HistPoint>> GetHistPoints(IDbConnection connection, int id)
        {
            var sql = "SELECT * from histpoints WHERE reportdefinitionid=@Id ORDER BY pointposn";
            //Console.WriteLine(sql);
           
            var results = await connection.QueryAsync<HistPoint>(sql, new { Id = id }).ConfigureAwait(false);
            connection.Close();
            return results.ToList();
        }

        public static async Task<ReportDefinition?> GetById(IDbConnection connection, int id)
        {
            var sql = "SELECT * from reportdefinitions WHERE reportdefinitionid=@Id";
            //Console.WriteLine(sql);
            IEnumerable<ReportDefinition?> result= await connection.QueryAsync<ReportDefinition>(sql, new { Id = id }).ConfigureAwait(false);
            connection.Close();
            foreach (var definition in result) return definition;
            return null;
        }
        public static async Task<List<ReportDefinition>> GetByEnabled(IDbConnection connection)
        {
            var now = DateTime.Now;
            var enable = true;
            var sql = "SELECT * FROM reportdefinitions WHERE nextevent < @Now AND enable=@enable";
            //Console.WriteLine(sql);
            var results = await connection.QueryAsync<ReportDefinition>(sql, new { Now = now, Enable = enable }).ConfigureAwait(false);
            connection.Close();
            return results.ToList();
        }

        public static async Task<int> Update(IDbConnection connection, ReportDefinition report)
        {

            const string sql = @"UPDATE reportdefinitions SET reportname = @reportname,
                reporttypeid = @reporttypeid,
                reportdestid = @reportdestid,
                destinationinfo = @destinationinfo,
                arhive = @arhive,
                header2 = @header2,
                timeformatid = @timeformatid,
                timeperiodinfo = @timeperiodinfo,
                sampletimeformatid = @sampletimeformatid,
                sampletimeperiodinfo = @sampletimeperiodinfo,
                additionalparams = @additionalparams,
                unit = @unit,
                shift = @shift,
                nextevent = @nextevent,
                lastused = @lastused,
                enable = @enable 
                WHERE reportdefinitionid = @reportdefinitionid";
            //Console.WriteLine(sql);
            var c = await connection.ExecuteAsync(sql, report).ConfigureAwait(false);
            Console.WriteLine("Update ReportDefinition " + c + " rows");
            connection.Close();
            return c;

        }
        public static async Task<int> Insert(IDbConnection connection, ReportDefinition report)
        {

            var sql = @"INSERT INTO reportdefinitions ( reportname,
                reporttypeid,
                reportdestid,
                destinationinfo,
                arhive,
                header2,
                timeformatid,
                timeperiodinfo,
                sampletimeformatid,
                sampletimeperiodinfo,
                additionalparams,
                unit,
                shift,
                nextevent,
                lastused,
                enable) VALUES (@reportname, @reporttypeid, @reportdestid, @destinationinfo, @arhive, @header2, @timeformatid,
                @timeperiodinfo, @sampletimeformatid, @sampletimeperiodinfo, @additionalparams, @unit, @shift, @nextevent,
                @lastused, @enable) RETURNING reportdefinitionid;";
            //Console.WriteLine(sql);
            var c = await connection.ExecuteScalarAsync<int>(sql, report).ConfigureAwait(false);
            Console.WriteLine("Insert ReportDefinition  with Id=" + c);
            connection.Close();
            return c;

        }

        public static async Task<int> Delete(IDbConnection connection, int id)
        {
            const string sql = "DELETE FROM reportdefinitions WHERE reportdefinitionid=@Id";
            Console.WriteLine(sql);
            var c = await connection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
            //Console.WriteLine("Delete ReportDefinition " + c + " rows");
            connection.Close();
            return c;
        }
    }


}

//    [Table("Tablename")] to specify the table name
//    [Key] to mark an auto-generated key field
//    [ExplicitKey] to mark a field that isn't generated automatically
//    [Write(true / false)] to mark(non)writable properties
//    [Computed] to mark calculated properties.
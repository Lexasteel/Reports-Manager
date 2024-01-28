using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public class HistPoint
    {
        [Key]
        public int histpointid { get; set; }
        public int pointposn { get; set; }
        public string pointname { get; set; } 
        public int bitnumber { get; set; }
        public int proctype { get; set; }
        public string integconst { get; set; } = "1.0";
       // public int glitchdetect { get; set; }
        //public int summaryenable { get; set; }
        public int reportdefinitionid { get; set; }

        public string description { get; set; }
        public string format { get; set; } = "0.0";
        [Write(false)]
        public string Type { get; set; }
        //[Write(false)]
        //public string Units { get; set; } = string.Empty;
        //[Write(false)]
        //public int ScanMsec { get; set; }
        //[Write(false)]
        //public int SignificantDigits { get; set; }
        [Write(false)]
        public Dictionary<DateTime, float> FValues { get; set; } = new Dictionary<DateTime, float>();

        [Write(false)]
        public uint Handle { get; set; }



        public static IEnumerable<HistPoint> GetById(IDbConnection connection, int id)
        {
            var sql = "SELECT * from histpoints WHERE histpointid=@Id";
            var results = connection.Query<HistPoint>(sql, new { Id = id });
            connection.Close();
            return results;
        }
        public static int Update(IDbConnection connection, HistPoint point)
        {

            const string sql = @"UPDATE histpoints SET pointposn = @pointposn,
                            pointname = @pointname,
                            bitnumber = @bitnumber,
                            proctype = @proctype,
                            integconst = @integconst,
                            reportdefinitionid = @reportdefinitionid,
                            format = @format,
                            description = @description 
                WHERE histpointid = @histpointid";
            var c = connection.Execute(sql, point);
            connection.Close();
            return c;

        }
        public static  int Insert(IDbConnection connection, HistPoint point)
        {
            var sql = @"INSERT INTO histpoints (pointposn, pointname, bitnumber, proctype, integconst, reportdefinitionid, format, description) VALUES (@pointposn,
                            @pointname, @bitnumber, @proctype, @integconst, @reportdefinitionid, @format, @description) RETURNING histpointid;";
            var c = connection.ExecuteScalar<int>(sql, point);
            connection.Close();
            return c;

        }

        public static int Delete(IDbConnection connection, int id)
        {
            const string sql = "DELETE FROM histpoints WHERE histpointid=@Id";
            var c =  connection.Execute(sql, new { Id = id });
            connection.Close();
            return  c;
        }

        public override bool Equals(object obj)
        {
            //As the obj parameter type is object, so we need to
            //cast it to Student Type
            return this.histpointid == ((HistPoint)obj).histpointid
                   && this.pointposn == ((HistPoint)obj).pointposn
                   && this.pointname == ((HistPoint)obj).pointname
                   && this.bitnumber == ((HistPoint)obj).bitnumber
                   && this.proctype == ((HistPoint)obj).proctype
                   && this.integconst == ((HistPoint)obj).integconst
                  // && this.glitchdetect == ((HistPoint)obj).glitchdetect
                  // && this.summaryenable == ((HistPoint)obj).summaryenable
                   && this.reportdefinitionid == ((HistPoint)obj).reportdefinitionid
                   && this.description == ((HistPoint)obj).description
                   && this.format == ((HistPoint)obj).format;
        }
        public override int GetHashCode()
        {
            var idHashCode = this.histpointid.GetHashCode();
            var posnHashCode =  this.pointposn.GetHashCode();
            var nameHashCode = this.pointname == null ? 0 : this.pointname.GetHashCode();
            var bitHashCode =  this.bitnumber.GetHashCode();
            var procHashCode =  this.proctype.GetHashCode();
            var integHashCode = this.integconst == null ? 0 : this.integconst.GetHashCode();
            //var glitchHashCode = this.glitchdetect == null ? 0 : this.glitchdetect.GetHashCode();
           // var summarHashCode = this.summaryenable == null ? 0 : this.summaryenable.GetHashCode();
            var repoIdHashCode = this.reportdefinitionid.GetHashCode();
            var descrHashCode = this.description == null ? 0 : this.description.GetHashCode();
            var formatHashCode = this.format == null ? 0 : this.format.GetHashCode();

            return idHashCode ^ nameHashCode ^ posnHashCode ^ bitHashCode ^ procHashCode ^ integHashCode 
                   ^ repoIdHashCode ^ descrHashCode ^ formatHashCode;
        }
    }

    //public class PointFilter
    //{
    //    [Key]
    //    public int ID { get; set; }
    //    public string Name { get; set; }
    //}

    //public class HistPointComparer : IEqualityComparer<HistPoint>
    //{
    //    public bool Equals(HistPoint x, HistPoint y)
    //    {
    //        //First check if both object reference are equal then return true
    //        if (object.ReferenceEquals(x, y))
    //        {
    //            return true;
    //        }
    //        //If either one of the object refernce is null, return false
    //        if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
    //        {
    //            return false;
    //        }
    //        //Comparing all the properties one by one
    //        return x.reportdefinitionid == y.reportdefinitionid && x.pointname == y.pointname;
    //    }
    //    public int GetHashCode(HistPoint obj)
    //    {
    //        //If obj is null then return 0
    //        if (obj == null)
    //        {
    //            return 0;
    //        }
    //        //Get the ID hash code value
    //        int IDHashCode = obj.reportdefinitionid.GetHashCode();
    //        //Get the Name HashCode Value
    //        int NameHashCode = obj.pointname == null ? 0 : obj.pointname.GetHashCode();
    //        return IDHashCode ^ NameHashCode;
    //    }
    //}
}

using Dapper;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public class Historian
    {
        [Key]
        public int historianid { get; set; }
        public string ip { get; set; }
        public int unit { get; set; }
        public string unitnet { get; set; }
 

      
        public static IEnumerable<Historian> GetAll(IDbConnection connection)
        {
            var sql = "SELECT * from historians";
            //Console.WriteLine(sql);
            var results =  connection.Query<Historian>(sql);
            connection.Close();
            return results;
        }

        public static  int Insert(IDbConnection connection, Historian historian)
        {
            var sql = @"INSERT INTO historians (ip, unit, unitnet) VALUES (@ip,
                            @unit, @unitnet) RETURNING historianid;";
            var c =  connection.ExecuteScalar<int>(sql, historian);
            connection.Close();
            return c;

        }

        public override bool Equals(object obj)
        {
            //As the obj parameter type is object, so we need to
            //cast it to Student Type
            return this.historianid == ((Historian)obj).historianid
                   && this.ip == ((Historian)obj).ip
                   && this.unit == ((Historian)obj).unit
                   && this.unitnet == ((Historian)obj).unitnet;
        }
        public override int GetHashCode()
        {
            var idHashCode = this.historianid.GetHashCode();
            var ipHashCode = this.ip == null ? 0 : this.ip.GetHashCode();
            var unitHashCode = this.unit.GetHashCode();
            var unitNetHashCode = this.unitnet == null ? 0 : this.unitnet.GetHashCode();

            return idHashCode ^ ipHashCode ^ unitHashCode ^ unitNetHashCode;
        }
    }
}
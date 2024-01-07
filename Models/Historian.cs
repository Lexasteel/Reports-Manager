using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Models
{
    public class Historian
    {
        [Key]
        public int historianid { get; set; }
        public string ip { get; set; }
        public int unit { get; set; }
        public string unitnet { get; set; }
 

      
        public static async Task<IEnumerable<Historian>> GetAll(IDbConnection connection)
        {
            var sql = "SELECT * from historians";
            //Console.WriteLine(sql);
            var results = await connection.QueryAsync<Historian>(sql).ConfigureAwait(false);
            connection.Close();
            return results;
        }

        public static async Task<int> Insert(IDbConnection connection, Historian historian)
        {
            var sql = @"INSERT INTO historians (ip, unit, unitnet) VALUES (@ip,
                            @unit, @unitnet) RETURNING historianid;";
            var c = await connection.ExecuteScalarAsync<int>(sql, historian).ConfigureAwait(false);
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
            var unitHashCode = this.unit == null ? 0 : this.unit.GetHashCode();
            var unitNetHashCode = this.unitnet == null ? 0 : this.unitnet.GetHashCode();

            return idHashCode ^ ipHashCode ^ unitHashCode ^ unitNetHashCode;
        }
    }
}
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
 
        public string password { get; set; }
      
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
            var sql = @"INSERT INTO historians (ip, unit, unitnet, password) VALUES (@ip,
                            @unit, @unitnet, @password) RETURNING historianid;";
            var c = await connection.ExecuteScalarAsync<int>(sql, historian).ConfigureAwait(false);
            connection.Close();
            return c;

        }
    }
}
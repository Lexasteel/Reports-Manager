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
        public int HistorianID { get; set; }
        public string IP { get; set; } = String.Empty;
        public int Unit { get; set; }

        public string UnitNet { get; set; }
 
        public string Password { get; set; }
      
        public static async Task<IEnumerable<Historian>> GetAll(IDbConnection connection)
        {
            var sql = "SELECT * from historians";
            //Console.WriteLine(sql);
            var results = await connection.QueryAsync<Historian>(sql).ConfigureAwait(false);
            connection.Close();
            return results;
        }

    }
}
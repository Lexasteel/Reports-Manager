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
        public int historianId { get; set; }
        public string ip { get; set; } = String.Empty;
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

    }
}
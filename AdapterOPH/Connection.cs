#nullable enable
using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace AdapterOPH
{
    public interface IConnection
    {
        OvHNetClientConnection Open(string? ip, IReport report);
        void Close();
    }
    public class ConnectToHist : IConnection
    {
        private readonly OvHNetClientConnection _connection = new OvHNetClientConnection();

        public void Close()
        {
            _connection.Close();
        }

        public  OvHNetClientConnection Open(string? ip, IReport report)
        {
            if (ip == null) return null!;
            
           var task = Task.Run(() => _connection.Open(ip, "", "", "ReportManager"));
            
            var c = $"Connecting to: {ip}";
            var sec = 0;
            while (!task.IsCompleted)
            {
                report.State(report.Progress, null, c + $" - {sec} sec");
                Task.Delay(1000).Wait();
                sec++;
            }
            int errConnection = task.Result;
            if (errConnection >= 0) return _connection;
            var err = $"{new OvHNetClientErrors().GetErrString(errConnection)} - {ip}";
            report.State(0, 4, err);

            return null!;

        }
    }

    public class ConnectDB
    {
      public static IDbConnection GetConnection()
        {

            NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder();
            npgsqlConnectionStringBuilder.Host = "localhost";
            npgsqlConnectionStringBuilder.Port = 5432;
            npgsqlConnectionStringBuilder.Database = "Reports";
            npgsqlConnectionStringBuilder.Username = "postgres";
            npgsqlConnectionStringBuilder.Password = "postgres";

            var conn = new NpgsqlConnection(npgsqlConnectionStringBuilder.ConnectionString);
            return conn;
        }
    }

   
}

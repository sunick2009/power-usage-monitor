using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace power_usage_monitor.Controllers
{
    public class DataCollecterService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<DataCollecterService> _logger;
        private Timer? _timer = null;
        private readonly IConfiguration _config;

        //ExcuteCmd() 方法可執行 SqlCommand 來編輯資料表
        private void ExcuteCmd(SqlCommand cmd)
        {
            string constr = _config.GetConnectionString("DefaultDatabase");
            SqlConnection con = new SqlConnection();
            con.ConnectionString = constr;
            con.Open();
            cmd.Connection = con;
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public DataCollecterService(ILogger<DataCollecterService> logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Collecter Service is running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);
            int plugCount = 6;
            List<double> curPowerData = new List<double>();
            //從延長線取得資料
            try
            {
                var smartStrip = new TPLinkSmartDevices.Devices.TPLinkSmartStrip("192.168.1.204");
                TPLinkSmartDevices.Model.PowerData smartStripData = smartStrip.ReadRealtimePowerData(1);
                for (int i = 1; i <= plugCount; i++)
                {
                    smartStripData = smartStrip.ReadRealtimePowerData(i);
                    curPowerData.Add(smartStripData.Power);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Data Collecter Service encountered an error in collecting data with {ex}", ex);
            }
            //向資料庫寫入資料
            try
            {
                DateTime curTime = DateTime.Now;
                for (int i = 0; i < plugCount; i++)
                {
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText =
                        "INSERT INTO Usage(Time, Device_ID, PowerUsed)" +
                        "VALUES (@Time, @Device_ID, @PowerUsed)";
                    sqlCommand.Parameters.Add(new SqlParameter
                        ("@Time", SqlDbType.DateTime)).Value =
                        curTime;
                    sqlCommand.Parameters.Add(new SqlParameter
                        ("@Device_ID", SqlDbType.Int)).Value =
                        (i+1);
                    sqlCommand.Parameters.Add(new SqlParameter
                        ("@PowerUsed", SqlDbType.Float)).Value =
                        curPowerData[i];
                    ExcuteCmd(sqlCommand);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Data Collecter Service encountered an error in saving data with {ex}", ex);
                return;
            }
            DateTime time;
            _logger.LogInformation(
                "Data Collecter Service is working at {time}. Count: {Count}", time = DateTime.Now, count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Collecter Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

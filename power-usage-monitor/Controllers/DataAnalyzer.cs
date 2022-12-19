using Microsoft.Data.SqlClient;
using System.Data;
using power_usage_monitor.Models;
using NETCore.MailKit.Core;

namespace power_usage_monitor.Controllers
{
    public class DataAnalyzerService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<DataAnalyzerService> _logger;
        private Timer? _timer = null;
        private readonly IConfiguration _config;
        private readonly Models.IEmailService _emailService;

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
        public DataAnalyzerService(ILogger<DataAnalyzerService> logger, IConfiguration config
            , Models.IEmailService emailservice)
        {
            _config = config;
            _logger = logger;
            _emailService = emailservice;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Analyzer Service is running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);
            DateTime time = DateTime.Now;
            //send test e-mail
            //_emailService.Send("sunick2009@gmail.com", "測試 " + time.ToString(), "test");
            _logger.LogInformation(
                "Data Analyzer Service is working at {time}. Count: {Count}", time = DateTime.Now, count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Analyzer Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

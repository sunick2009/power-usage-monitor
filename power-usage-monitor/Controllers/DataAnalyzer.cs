using Microsoft.Data.SqlClient;
using System.Data;
using power_usage_monitor.Models;
using NETCore.MailKit.Core;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Text.RegularExpressions;
using NuGet.Packaging.Signing;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace power_usage_monitor.Controllers
{
    public class DataAnalyzerService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<DataAnalyzerService> _logger;
        private Timer? _timer = null;
        private readonly IConfiguration _config;
        private readonly Models.IEmailService _emailService;
        private readonly IServiceScopeFactory scopeFactory;

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
            , Models.IEmailService emailservice, IServiceScopeFactory scopeFactory)
        {
            _config = config;
            _logger = logger;
            _emailService = emailservice;
            this.scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Analyzer Service is running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                //TimeSpan.FromSeconds(60));
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {

            var count = Interlocked.Increment(ref executionCount);
			await Task.Run(() =>WORK());
        }
        private async void WORK(string? target = null)
        {
			DateTime time = DateTime.Now;
			using (var scope = scopeFactory.CreateScope())
			{
				if (executionCount == 1)
				{
					_logger.LogInformation(
						"Data Analyzer Service is skipping first mission to prevent no data at {time}.", time = DateTime.Now);
					return;
				}
				try
				{
					await Task.Run(() => {
						var dbContext = scope.ServiceProvider.GetRequiredService<power_usage_monitorContext>();
						var curDeviceSetting = dbContext.Devices.AsNoTracking().OrderBy(e => e.DeviceId)
							.AsEnumerable().ToList();

						//var change = (from e in dbContext.Devices
						//             where e.DeviceId == 1
						//             select e).FirstOrDefault();
						//change.StandbyTime = "00:00-23:59";
						//await dbContext.SaveChangesAsync();
						//未指定則處理昨天的數據
						var targetDate  = DateTime.Today.AddDays(-1);
						//指定處理某天的數據
                        if (target != null) {
							targetDate = DateTime.Parse(target);
						}
                        DateTime startDateTime = targetDate; //someday at 00:00:00
						DateTime endDateTime = targetDate.AddDays(1).AddTicks(-1); //someday at 23:59:59
						var curUsage = dbContext.Usages.GroupBy(e => e.DeviceId)
							.Select(gr =>
							gr.Where(a => a.Time >= startDateTime && a.Time <= endDateTime)
								.OrderByDescending(e => e.Time).AsEnumerable())
							.ToList();
						//var result = (from a in dbContext.Usages.OrderByDescending(e => e.Time)
						//              where (a.Time >= startDateTime && a.Time <= endDateTime)
						//              select a.PowerUsed);
						//檢查有沒有取到資料
						if (curUsage == null)
						{
							//send test e-mail
							//_emailService.Send("sunick2009@gmail.com", "測試 ", "test" + curUsage.ToJson());
							_logger.LogError("Data Analyzer Service encountered an error " +
								"in collecting data with nothing get");
							return;
						}
						//檢查是否在規範時間外運作
						foreach (var item in curDeviceSetting)
						{
							string[] timeRequired = item.StandbyTime.
								Split(new char[] { ':', '-' }, StringSplitOptions.RemoveEmptyEntries);
							double powerRequired = dbContext.Statistics.Where(e => e.DeviceId == item.DeviceId)
							.Select(e => e.AveragePower).FirstOrDefault();

							//單次超過5分鐘以上才回報為一個區間
							var curDeviceData = curUsage[item.DeviceId - 1].Where(e => e.DeviceId == item.DeviceId).ToList();
							var curDeviceDataByTime = curDeviceData.GroupBy(e =>
							{
								var stamp = e.Time;
								stamp = stamp.AddMinutes(-(stamp.Minute % 5));
								stamp = stamp.AddMilliseconds(-stamp.Millisecond - 1000 * stamp.Second);
								return stamp;
							}).Select(g => new
							{
								TimeStamp = g.Key
							,
								Value = g.Average(s => s.PowerUsed)
							}).ToList();
							//檢查資料是否為null及是否檢查該設備
							if (curDeviceDataByTime != null && item.Status == "1")
							{
								double totalUsage = 0;
								double avgUsage = 0;
								foreach (var eachData in curDeviceDataByTime)
								{
									int hour = eachData.TimeStamp.Hour;
									int minute = eachData.TimeStamp.Minute;
									//先檢查用電量是否超過正常使用
									if (eachData.Value >= Convert.ToDouble(powerRequired))
									{
										//檢查小時或分鐘是否在範圍內
										if (hour >= Convert.ToInt32(timeRequired[0]) && hour <= Convert.ToInt32(timeRequired[2])
										&& minute >= Convert.ToInt32(timeRequired[1]) && hour <= Convert.ToInt32(timeRequired[3]))
										{
											//正確不做事，到最後則紀錄statics
										}
										else
										{
											//超過則記錄至資料庫
											SqlCommand sqlCommand2 = new SqlCommand();
											sqlCommand2.CommandText =
												"INSERT INTO Abnormal(Device_ID, Abnormal_Usage, Abnormal_Time, Noticed_User) " +
												"VALUES (@ID, @Usage, @Time, @User)";
											sqlCommand2.Parameters.Add(new SqlParameter
												("@ID", SqlDbType.Int)).Value =
												item.DeviceId;
											sqlCommand2.Parameters.Add(new SqlParameter
												("@Usage", SqlDbType.Float)).Value =
												eachData.Value;
											sqlCommand2.Parameters.Add(new SqlParameter
												("@Time", SqlDbType.DateTime)).Value =
												eachData.TimeStamp;
											sqlCommand2.Parameters.Add(new SqlParameter
												("@User", SqlDbType.Int)).Value =
												dbContext.Users.SingleOrDefault()?.UserName;
											ExcuteCmd(sqlCommand2);
											_logger.LogInformation(
												"Data Analyzer Service is found an abnormal data and save to \"Abnormal\" at {time}.",
												time = DateTime.Now);
										}
									}
									//計算整日用量
									totalUsage += eachData.Value;
								}
								avgUsage = totalUsage / Convert.ToDouble(curDeviceDataByTime.Count());
								//將統計數據存入資料庫
								SqlCommand sqlCommand = new SqlCommand();
								sqlCommand.CommandText =
									"INSERT INTO \"Statistics\"(Device_ID, Period, Total_Usage, AveragePower)" +
									"VALUES (@Device_ID, @Period, @Total_Usage, @AveragePower)";
								sqlCommand.Parameters.Add(new SqlParameter
									("@Device_ID", SqlDbType.Int)).Value =
									item.DeviceId;
								sqlCommand.Parameters.Add(new SqlParameter
									("@Period", SqlDbType.NVarChar)).Value =
									endDateTime.Year + "/" + endDateTime.Month + "/" + endDateTime.Day;
								sqlCommand.Parameters.Add(new SqlParameter
									("@Total_Usage", SqlDbType.Float)).Value =
									totalUsage;
								sqlCommand.Parameters.Add(new SqlParameter
									("@AveragePower", SqlDbType.Float)).Value =
									avgUsage;
								ExcuteCmd(sqlCommand);
								_logger.LogInformation(
									"Data Analyzer Service is saving data to  \"Statistics\" at {time}.", time = DateTime.Now);
							}
						}
					});
				}
				catch (Exception ex)
				{
					_logger.LogError("Data Analyzer Service encountered an error with {ex}", ex);
					throw;
				}
			}
			_logger.LogInformation(
				"Data Analyzer Service is working at {time}. Count: {Count}", time = DateTime.Now, executionCount);
			Interlocked.Increment(ref executionCount);
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

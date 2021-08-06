using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ITA.Common;
using log4net;

namespace BusinessComponent
{
    public class FileWriterService : IDisposable
    {
        private ILog _logger = Log4NetItaHelper.GetLogger("FileWriterService");
        private string _path = @"d:\TestNetCoreApplication.txt";

        private Timer _timer;
        private int _timeout = 30;

        public FileWriterService(int timeout, string path)
        {
            _timeout = timeout;
            _path = path;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                (e) => WriteTimeToFile(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(_timeout));

            return Task.CompletedTask;
        }

        public void WriteTimeToFile()
        {
            if (!File.Exists(_path))
            {
                using (var sw = File.CreateText(_path))
                {
                    var outString = DateTime.UtcNow.ToString("O");
                    sw.WriteLine(outString);
                    Console.WriteLine(outString);
                    _logger.Debug(outString);
                }
            }
            else
            {
                using (var sw = File.AppendText(_path))
                {
                    var outString = DateTime.UtcNow.ToString("O");
                    sw.WriteLine(outString);
                    Console.WriteLine(outString);
                    _logger.Debug(outString);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

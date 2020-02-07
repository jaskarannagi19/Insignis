using InsignisIllustrationGenerator.Data;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Scheduler
{
    public class FetchDataTask : BackgroundService
    {
        
        private readonly DataProvider _dataProvider;

        public FetchDataTask()
        {
            _dataProvider = new DataProvider();    

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _dataProvider.UpdateString(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}

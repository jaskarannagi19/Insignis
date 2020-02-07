using InsignisIllustrationGenerator.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Scheduler
{
    public class FetchDataTask : IHostedService
    {
        
        private readonly DataProvider _dataProvider;


        private ApplicationDbContext _context;

        public FetchDataTask(DataProvider data)//IServiceProvider serviceProvider)
        {

            //using (IServiceScope scope = serviceProvider.CreateScope())
            //{
            //    _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //}
            _dataProvider = data;    

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _dataProvider.UpdateString(cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        var result = await _dataProvider.UpdateString(stoppingToken);
        //        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        //    }
        //}
    }
}

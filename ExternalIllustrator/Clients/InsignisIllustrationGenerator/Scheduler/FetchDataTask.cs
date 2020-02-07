using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Manager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Scheduler
{
    public class FetchDataTask : IHostedService
    {
        
        private readonly DataProvider _dataProvider;
        private BankHelper _bankHelper;
        private AutoMapper.IMapper _mapper;


        private ApplicationDbContext _context;

        private readonly IServiceProvider _serviceProvider;

        public FetchDataTask(DataProvider data)
        {
            _dataProvider = data;    
        }


        public FetchDataTask(DataProvider data ,IServiceProvider serviceProvider, AutoMapper.IMapper mapper)
        {

            _serviceProvider = serviceProvider;
            _dataProvider = data;
            _mapper = mapper;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _dataProvider.UpdateString(cancellationToken);

                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    _bankHelper = new BankHelper(_mapper, _context);
                    _bankHelper.SaveBank(result);
                }

                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

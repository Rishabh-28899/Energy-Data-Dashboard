using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Emergy_report.Data;
using Emergy_report.models;

namespace Emergy_report.Services
{
    public class Oueueprocessor : BackgroundService

    {
        private readonly Immemoryqueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Oueueprocessor> _logger;

        private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
        // For testing → TimeSpan.FromSeconds(5)

        public Oueueprocessor(
            Immemoryqueue queue,
            IServiceScopeFactory scopeFactory,
            ILogger<Oueueprocessor> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        // 🔁 Runs automatically
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Worker started...");
            using PeriodicTimer timer = new PeriodicTimer(_interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Processing queue...");
                await ProcessQueue(stoppingToken);
            }
        }

        // ⚙️ Queue → DB
        private async Task ProcessQueue(CancellationToken stoppingToken)
        {


            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var validRecords = new List<Emerguapp>();

            while (_queue.TryDequeue(out var record))
            {
                if (record == null || record.Date == default)
                    continue;

                record.Date = record.Date.Date;
                validRecords.Add(record);

                Console.WriteLine($"Processing Block: {record.Block}");
            }

            if (validRecords.Count > 0)
            {
                context.Emerguapp.AddRange(validRecords);
                await context.SaveChangesAsync();
            }
        }
    }
}
    
    
    
        
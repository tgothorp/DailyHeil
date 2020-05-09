using System;
using DailyHeil.Functions.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DailyHeil.Functions.Infrastructure.DailyHeilContextFactory
{
    public interface IDailyHeilContextFactory
    {
        DailyHeilContext Create();
    }
    
    public class DailyHeilContextFactory : IDailyHeilContextFactory
    {
        private readonly DbContextOptions<DailyHeilContext> _options;

        public DailyHeilContextFactory(FunctionConfiguration functionConfiguration)
        {
            _options = new DbContextOptionsBuilder<DailyHeilContext>()
                .UseSqlServer(
                    functionConfiguration.DatabaseConnectionString,
                    options =>
                    {
                        options.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            errorNumbersToAdd: null,
                            maxRetryDelay: TimeSpan.FromSeconds(15));
                    })
                .Options;
        }
        
        public DailyHeilContext Create()
        {
            return new DailyHeilContext(_options);
        }
    }
}
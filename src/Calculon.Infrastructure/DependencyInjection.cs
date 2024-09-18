using Calculon.Core.Enums;
using Calculon.Core.Interfaces;
using Calculon.Core.Models;
using Calculon.Infrastructure.DelimiterStrategies;
using Calculon.Infrastructure.OperationStrategies;
using Microsoft.Extensions.DependencyInjection;

namespace Calculon.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddStringCalculator(this IServiceCollection services, CalculatorOptions options)
        {
            services.AddSingleton(options);

            services.AddSingleton<IDelimiterStrategy>(sp => new DefaultDelimiterStrategy(options.AlternateDelimiter));

            services.AddSingleton<IOperationStrategy, AddStrategy>();
            services.AddSingleton<IOperationStrategy, SubtractStrategy>();
            services.AddSingleton<IOperationStrategy, MultiplyStrategy>();
            services.AddSingleton<IOperationStrategy, DivideStrategy>();

            services.AddSingleton<IDictionary<OperationType, IOperationStrategy>>(sp =>
            {
                var strategies = sp.GetServices<IOperationStrategy>();
                return new Dictionary<OperationType, IOperationStrategy>
                {
                    { OperationType.Add, strategies.OfType<AddStrategy>().First() },
                    { OperationType.Subtract, strategies.OfType<SubtractStrategy>().First() },
                    { OperationType.Multiply, strategies.OfType<MultiplyStrategy>().First() },
                    { OperationType.Divide, strategies.OfType<DivideStrategy>().First() }
                };
            });

            services.AddSingleton<IStringCalculator, StringCalculator>();

            return services;
        }
    }
}
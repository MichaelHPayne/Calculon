using Calculon.Core.Common;
using Calculon.Core.Enums;
using Calculon.Core.Models;

namespace Calculon.Core.Interfaces
{
    public interface IStringCalculator
    {
        Result<CalculationResult> Calculate(RawInput input, OperationType operationType);
    }
}

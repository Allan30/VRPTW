using VRPTW.Core.Operators;
using VRPTW.UI.Extensions;

namespace VRPTW.UI.ViewModels;

public sealed class OperatorViewModel
{
    public OperatorViewModel(OperatorEnum operatorType)
    {
        OperatorType = operatorType;
    }
    public OperatorEnum OperatorType { get; }
    public override string ToString() => OperatorType.GetFriendlyName();
}

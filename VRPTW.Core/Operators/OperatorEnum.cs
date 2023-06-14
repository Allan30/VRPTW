using System.ComponentModel;

namespace VRPTW.Core.Operators;

public enum OperatorEnum
{
    [Description("Échange inter")]
    ExchangeInter,
    [Description("Échange intra")]
    ExchangeIntra,
    [Description("Relocate inter")]
    RelocateInter,
    [Description("Relocate intra")]
    RelocateIntra,
    [Description("Two-opt")]
    TwoOpt
}
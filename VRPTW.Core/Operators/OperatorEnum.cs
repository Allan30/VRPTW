using System.ComponentModel;

namespace VRPTW.Core.Operators;

public enum OperatorEnum
{
    [Description("�change inter")]
    ExchangeInter,
    [Description("�change intra")]
    ExchangeIntra,
    [Description("Relocate inter")]
    RelocateInter,
    [Description("Relocate intra")]
    RelocateIntra,
    [Description("Two-opt")]
    TwoOpt
}
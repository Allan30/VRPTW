using System.ComponentModel;

namespace VRPTW.Concret;

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
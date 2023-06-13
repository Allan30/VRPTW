using System.ComponentModel;
using System;
using System.Linq;
using System.Reflection;

namespace VRPTW.UI.Extensions;

public static class EnumExtensions
{
    public static string GetFriendlyName(this Enum value) => 
        value
            .GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DescriptionAttribute>()
            ?.Description
        ?? value.ToString();
}

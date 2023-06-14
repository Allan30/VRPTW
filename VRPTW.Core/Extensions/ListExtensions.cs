namespace VRPTW.Core.Extensions;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list, Random? rnd = null)
    {
        rnd ??= new Random();
        for (var i = list.Count; i > 0; i--)
        {
            list.Swap(0, rnd.Next(0, i));
        }
    }

    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        (list[j], list[i]) = (list[i], list[j]);
    }
}

namespace Relax.Utils
{
    public static class Extensions
    {
        public static T[] Add<T>(this T[] array, T newItem)
        {
            var list = array.ToList();
            list.Add(newItem);
            return list.ToArray();
        }
    }
}
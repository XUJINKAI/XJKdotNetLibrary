namespace XJKdotNetLibrary
{
    public static class ObjectExtension
    {
        public static T CastTo<T>(this object obj)
        {
            return (T)obj;
        }
    }
}

using System;

namespace XJK
{
    public static class ExceptionExtension
    {
        public static string ToStringLong(this Exception e)
        {
            string msg = $"{e.Message}{C.LF}{e.StackTrace}";
            if (e.InnerException != null)
            {
                return ToStringLong(e.InnerException) + "\r\n--- InnerException ---\r\n" + msg;
            }
            return msg;
        }

        public static string ToStringShort(this Exception e)
        {
            if (e.InnerException != null)
            {
                return ToStringShort(e.InnerException);
            }
            return $"{e.Message}{C.LF}{e.StackTrace}";
        }
    }
}

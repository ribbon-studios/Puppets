using System;

namespace Puppets.Utils
{
    /// <summary>
    /// Used for getting DateTime.Now(), time is changeable for unit testing
    /// </summary>
    public static class SystemTime
    {
        /// <summary> Normally this is a pass-through to DateTime.Now, but it can be overridden with SetDateTime( .. ) for testing or debugging.
        /// </summary>
        public static Func<DateTime> Now = () => DateTime.Now.ToUniversalTime();
        public static Func<DateTime> NowUTC = () => SystemTime.Now().ToUniversalTime();

        /// <summary> Set time to return when SystemTime.Now() is called.
        /// </summary>
        public static void SetDateTime(DateTime dateTimeNow)
        {
            Now = () => dateTimeNow;
        }

        /// <summary> Resets SystemTime.Now() to return DateTime.Now.
        /// </summary>
        public static void ResetDateTime()
        {
            Now = () => DateTime.Now;
        }
    }
}

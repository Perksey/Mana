using System;
using System.Threading;

namespace Mana.Utilities
{
    public static class ThreadHelper
    {
        internal static int MainThreadID = -1;

        public static bool IsMainThread
        {
            get
            {
                if (MainThreadID == -1)
                    throw new InvalidOperationException("Main thread has not been set.");

                return Thread.CurrentThread.ManagedThreadId == MainThreadID;
            }
        }
    }
}
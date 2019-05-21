using System;
using System.Threading;

namespace Mana.Utilities
{
    public static class ThreadHelper
    {
        internal static int MainThreadID = -1;

        /// <summary>
        /// Gets a value that indicates whether the executing thread is the application's main thread.
        /// </summary>
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
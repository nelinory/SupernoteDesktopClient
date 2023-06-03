using System;
using System.Threading.Tasks;

namespace SupernoteDesktopClient.Extensions
{
    public static class TaskExtension
    {
        // Credit: Brian Lagunas - https://www.youtube.com/watch?v=O1Tx-k4Vao0
        public async static void Await(this Task task, Action completedCallback = null, Action<Exception> exceptionCallback = null)
        {
            try
            {
                await task;
                completedCallback?.Invoke();
            }
            catch (Exception ex)
            {
                exceptionCallback?.Invoke(ex);
            }
        }
    }
}

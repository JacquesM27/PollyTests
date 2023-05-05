using Polly;

namespace PollyConsoleTests
{
    internal static class ConsoleLogger
    {
        internal static void LogRetryException(Exception exception, 
            TimeSpan timeSpan,
            int retryCount,
            Context context)
        {
            string method = context is null ? "unknown method" : context.First().Key;
            var methodDescription = context is null ? "unknown method description" : context.First().Value;
            string message = $"Trial no:{retryCount} | method: {method} - {methodDescription} : {exception.Message}";
            Console.WriteLine(message);
        }

        internal static Task LogTimeoutException(Context context, TimeSpan timeSpan, Task task)
        {
            string method = context is null ? "unknown method" : context.First().Key;
            var methodDescription = context is null ? "unknown method description" : context.First().Value;

            task.ContinueWith(t =>
            {
                string message = "";
                if (t.IsFaulted)
                {
                    message = $"Method: {method} - {methodDescription} " +
                    $"has ended after {timeSpan.TotalSeconds} seconds " +
                    $"with exception: {t.Exception}";
                }
                else if (t.IsCanceled)
                {
                    message = $"Method: {method} - {methodDescription} " +
                    $"has ended after {timeSpan.TotalSeconds} seconds " +
                    $"and has been cancelled";
                }
                Console.WriteLine(message);
            });
            return task;
        }
    }
}

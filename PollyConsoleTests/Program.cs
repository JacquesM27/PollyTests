// See https://aka.ms/new-console-template for more information
using Polly.Timeout;
using Polly;
using PollyConsoleTests;

Console.WriteLine("Hello! It's combination of Retry and TimeOut policies");
Console.ReadKey();
Console.WriteLine("\nType 'z' for cancel operation");

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

Task.Factory.StartNew(async () => await ExecuteTask(cancellationToken));

char readedKey = ' ';
do
{
    Console.WriteLine();
    readedKey = Console.ReadKey().KeyChar;
    if (readedKey.Equals("z"))
    {
        cancellationTokenSource.Cancel();
        Console.WriteLine("The cancellation has been sent. The operation will be aborted.");
    }
} while (!readedKey.Equals('z'));
Console.WriteLine("End of work");

async Task ExecuteTask(CancellationToken cancellationToken)
{
    var retryPolicy = Policy.Handle<Exception>()
        .WaitAndRetryAsync(
        retryCount: 4,
        sleepDurationProvider: i => TimeSpan.FromSeconds(2),
        //onRetry: (exception, timeSpan, retryCount, context) => RetryAction(exception, timeSpan, retryCount, context));
        onRetry: ConsoleLogger.LogRetryException);

    var timeoutPolicy = Policy.TimeoutAsync(
        seconds: 4,
        timeoutStrategy: TimeoutStrategy.Pessimistic,
        onTimeoutAsync: ConsoleLogger.LogTimeoutException);

    var policyWrap = Policy.WrapAsync(retryPolicy, timeoutPolicy);

    await policyWrap.ExecuteAsync(async (context, token) =>
    {
        Console.WriteLine("Executing process...");
        string result = await Sabotage.RandomlyFailAsync(cancellationToken);
        Console.WriteLine($"Method result: {result}");
    },new Dictionary<string, object> { { "Fake operation name", "Fake description" } }, cancellationToken);

}
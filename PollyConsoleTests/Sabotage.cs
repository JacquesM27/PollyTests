using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollyConsoleTests
{
    internal static class Sabotage
    {
        internal static async Task<string> RandomlyFailAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Task cancelled before lauch.");
                cancellationToken.ThrowIfCancellationRequested();
            }
            await Task.Delay(1000);

            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Task cancelled on secondary chapter.");
                cancellationToken.ThrowIfCancellationRequested();
            }

            Random random = new();
            int randomValue = random.Next(1, 10);
            Console.WriteLine($"randomValue: {randomValue}");
            if (randomValue <= 4)
            {
                Console.WriteLine("random caused a delay!");
                await Task.Delay(6000);
            }

            if(cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Task cancelled on third chapter caused timeout.");
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (randomValue > 4 && randomValue <= 8)
            {
                Console.WriteLine("random caused an exception!");
                throw new Exception("500 | random caused an exception!");
            }

            return "Method failed successfully.";
        }
    }
}

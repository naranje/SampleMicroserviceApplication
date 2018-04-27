using System;
using System.Threading.Tasks;
using BookRequest.LibraryCatalog;
using Polly;
using Polly.CircuitBreaker;

internal class ResiliencePolicies
{
    // Break the circuit after the specified number of consecutive exceptions
    // and keep circuit broken for the specified duration,
    // calling an action on change of circuit state.
    private static readonly Action<Exception, TimeSpan> OnBreak = (exception, timespan) => { Console.WriteLine("CIRCUIT OPEN"); };
    private static readonly Action OnReset = () => { Console.WriteLine("CIRCUIT RESET"); };

    public static readonly Policy CircuitBreaker = Policy
        .Handle<Exception>()
        //The circuit will break if the exception is raised once
        //It will stay broken for 10 seconds, any retries during that time 
        //will automatically throw a BrokenCircuitException that wraps the exception
        //that caused it.
        .CircuitBreakerAsync(1, TimeSpan.FromSeconds(30), OnBreak, OnReset);

    public static readonly Policy ExponentialRetryPolicy =
        Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                //Retry 3 times
                3,
                // using a function to calculate the duration to wait between retries based on 
                // the current retry attempt (allows for exponential backoff)
                // In this case will wait for
                //  2 ^ 1 = 2 seconds then
                //  2 ^ 2 = 4 seconds then
                //  2 ^ 3 = 8 seconds
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (ex, _) => Console.WriteLine(ex.ToString()));


    public static readonly Policy FallbackPolicy =
        Policy
            .Handle<Exception>()
            .FallbackAsync((token) =>
            {
                Console.WriteLine("RUN PLAN B");
                return Task.FromResult(0);
            });

}
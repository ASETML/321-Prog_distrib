using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Transat;

public sealed class FaillibleQos0Storage
{
    private volatile int _dataCounter = 0; 
    private static readonly Random RandomGenerator = new ();
    
    private readonly ConcurrentDictionary<string,int> _data = new ();

    public ImmutableList<int> Values => _data.Values.ToImmutableList();

    public ImmutableDictionary<string, int> Data => _data.ToImmutableDictionary();

    public void Store(string senderId,int data)
    {
        //Fake operational time
        Thread.Sleep(RandomGenerator.Next(50));
        
        //Fake failure
        if (RandomGenerator.Next(2) == 1)
        {
            _data[$"{senderId}-{DateTime.Now:s}-{_dataCounter}"]=data;
            Interlocked.Increment(ref _dataCounter);
            //progress
            Console.Write("+");
        }
        else
        {
            Console.Write("-");
        }
    }
}
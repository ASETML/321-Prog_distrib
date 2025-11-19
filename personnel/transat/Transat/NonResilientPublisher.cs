using System.Diagnostics;

namespace Transat;

public class NonResilientPublisher(FaillibleQos0Storage storage1, FaillibleQos0Storage storage2)
{
    private readonly string _id = Guid.NewGuid().ToString("n");
    
    public void Send(int message)
    {
        storage1.Store(_id,message);
        storage2.Store(_id, message);
    }
}
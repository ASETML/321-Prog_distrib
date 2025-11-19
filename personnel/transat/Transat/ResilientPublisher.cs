namespace Transat;

public class ResilientPublisher(FaillibleQos0Storage storage1, FaillibleQos0Storage storage2)
{
    private readonly string _id = Guid.NewGuid().ToString("n");
    
    public void Send(int message)
    {
        string id = Guid.NewGuid().ToString("n");
        while (!storage1.Data.Keys.Any(k => (k.StartsWith(id))))
        {
            storage1.Store(id, message);
        }

        while (!storage2.Data.Keys.Any(k => (k.StartsWith(id))))
        {
            storage2.Store(id, message);
        }
    }
}
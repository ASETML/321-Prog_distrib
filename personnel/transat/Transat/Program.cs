using Transat;

const int iterations = 500;

FaillibleQos0Storage storage1 = new();
FaillibleQos0Storage storage2 = new();

var publishers = new List<ResilientPublisher>
{
    new(storage1, storage2),
    new(storage1, storage2),
    new(storage1, storage2)
};

//Assign fake job
var tasks = new List<Task>();
for (var i = 0; i < iterations; i++)
{
    publishers.ToList().ForEach(
        publisher => tasks.Add(new Task(() =>
            publisher.Send(1)
        )));
}
tasks.ForEach(task=>task.Start());
Task.WaitAll(tasks.ToArray());

//Compute and show results
int sum1 = storage1.Values.Sum();
int sum2 = storage2.Values.Sum();

Console.WriteLine($"\nTotal expected : {iterations * 3}");
Console.WriteLine($"Sum storage1: {sum1}");
Console.WriteLine($"Sum storage2: {sum2}");
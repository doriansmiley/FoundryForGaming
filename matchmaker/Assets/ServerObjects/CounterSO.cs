using GPF.ServerObjects;

[DataStorePath("sync.counter")]
[Syncable]
[Register("counter")]
public class CounterSO : ServerObject
{
    public int Count;

    public class Increment : ServerObjectMessage { }
    [FromClient]
    void Handler(Increment msg)
    {
        Count++;
    }
}

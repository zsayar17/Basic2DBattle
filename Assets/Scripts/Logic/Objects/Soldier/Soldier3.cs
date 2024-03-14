using System.Diagnostics;

public class Soldier3 : Unit
{
    protected override void Awake()
    {
        base.Awake();
        ObjectType = ObjectType.SOLDIER3;
    }
}

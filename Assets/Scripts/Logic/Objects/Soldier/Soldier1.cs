

using System.Diagnostics;
using Unity.Collections;

public class Soldier1 : Unit
{
    protected override void Awake()
    {
        base.Awake();
        ObjectType = ObjectType.SOLDIER1;
    }
}

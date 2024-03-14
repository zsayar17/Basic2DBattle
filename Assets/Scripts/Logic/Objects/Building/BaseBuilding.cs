using UnityEngine;

/*
    Buildinglerin turetildigi metot. Barrack ve powerplant gibi
*/

public class BaseBuilding : BaseObject
{
    public override void BeginTheProcess()
    {
        base.BeginTheProcess();
        ChangeObjectColor(Color.white);
    }
}

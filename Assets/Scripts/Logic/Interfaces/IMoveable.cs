using System.Collections.Generic;
using UnityEngine;


/*
    Hareket edebilen objenin sahip olmasi gereken ozellikleri belirleyen interface yapisi
*/
public interface IMoveable : IDynamicObject
{

    public float ObjectSpeed {get;}
    public void PlayRunAnim();
    public void PlayIdleAnim();
}

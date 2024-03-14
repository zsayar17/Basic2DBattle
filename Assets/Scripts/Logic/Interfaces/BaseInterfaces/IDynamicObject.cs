using UnityEngine;


/*
    Hareket edebilen, attack edebilen yani dinamik olan objelerin kalitildigi arayuz
*/
public interface IDynamicObject : IBaseObject {

    IBase Target { get; set; }
    bool IsAttacking { get; set; }

}

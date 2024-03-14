using System.Collections.Generic;
using UnityEngine;


/*
    En temel tur, logicde olan tum elementlerin kalitildigi arayuz
*/
public interface IBase {
    public unsafe Cell* BeginCell { get; set; } // objenin sol ust, hucrenin ise kendisinin referansi
    public Vector2Int Size{ get; } // objenin veya hucrenin boyutu (1, 1) gibi

    public BaseType BaseType{ get; } // hangi tipte oldugunu tutan property

    public bool IsValid { get; } // oyunda valid olup olmama durumunu gosteren property (Poolda olma durumunda valid olmaz)

}

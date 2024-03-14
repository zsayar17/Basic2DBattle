using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
    Hareket edebilen objelerin hareket islemlerini yaptigi class
*/
public class Movement {

    IMoveable       _MoveableObject;
    List<IntPtr>    _Path; // Gidilecek olan yolu tutar.


    public Movement(IMoveable moveableObject) => _MoveableObject = moveableObject;

    /*
        Eger Hedef bos ise(grid disinda biryere tikladi ise) ya da kendine tikladi ise yada yada attack yapiyorsa hedefleme yapilmaz
        Ardindan halihazirda gittigi bir konum varmiydi ona bakar eger vae ise o zaman duzenleme islemleri yapacagi metoda yonlendirilir.
        Eger duragan ise yeni bir yol bulur.
    */
    public void Set()
    {
        if (!IsTargetValid()) return; //Hedef alinan valid degilse girme

        if (IsPathStillValid()) { // hali hazirda bir yolda ilerliyorsa beklenmedik durumu cozmeye calis
            FixUnexpectedSituation();
            return;
        }

        SetThePath(); //Gidilecek yolu belirle
    }

    // hedefin validligi kontrol edilir
    private bool IsTargetValid() => _MoveableObject.Target != null && _MoveableObject.Target.IsValid;

    // izlenmesi gereken yolun degerlerini bulunduran listenin varligi ve icinde deger olup olmadigi kontrol edilir.
    private bool IsPathStillValid() => _Path != null && _Path.Count != 0;

    // objenin verilen hedefe varip varmadigini kontrol eder
    private bool IsObjectReachedToPoint(Vector3 position)
        => Vector3.Distance(_MoveableObject.BeginPosition, position) < 0.1f;

    // obje durdurulacagi zaman cagirilir objenin gerekli degiskennleri sifirlanir ve bekleme animasyonuna gecer
    private void StopObject()
    {
        _Path = null;
        _MoveableObject.IsMoving = false;
        _MoveableObject.PlayIdleAnim();
    }

    // eger hedefe henuz varmadi ise bir adim sonraki hucreye hedeflemek icin oncekini siler ve ve grid uzerinden de kullanimini kaldirir
    private unsafe void PassToNextCell()
    {
        _MoveableObject.BeginCell = (Cell *)_Path[0]; // change the current cell of object
        if (_Path.Count != 1) GridCoordinator.Instance.PlaceObjectOnGrid(_MoveableObject, CellSituation.EMPTY, (Cell *)_Path[0]); //change situation which is passed, if statment blocks change situation of target cell

        _Path.Remove(_Path[0]);
    }

    // Objenin pozisyonu verilen hedef hucreye dogru ilerletilir.
    private unsafe void MoveToNextCell(Cell *target)
        => _MoveableObject.ChangePosition(Utils.MoveToWards(_MoveableObject, target->cellPosition, _MoveableObject.ObjectSpeed));


    /*
        Componentin duzenli olarak calisan metodudur.
    */
    public unsafe void Action()
    {
        Cell *targetCell;

        if (!IsTargetValid()) FixUnexpectedSituation(); //Target hala gecerli mi kontrol et eger degil ise beklenmedik durum olarak belirle

        if (!IsPathStillValid()) return;


        targetCell = (Cell *)_Path[0];
        MoveToNextCell((Cell *)_Path[0]); // bir sonraki hucreye ilerle

        if (!IsObjectReachedToPoint(targetCell->cellPosition)) return; // eger hedefe ulasti ise hareketi sonlandir.

        PassToNextCell(); // bir sonra ki hucreyi hedefle

        if (_Path.Count == 0) { // eger obje hedefe vardi ise durdur.
            StopObject();
            return;
        }

        if (!HandleUnexpectedSituationIfNeeded() && _Path.Count > 1)  // Eger beklenmedik bir  durum olmadi ise bir sornaki gridi full hale getir
            GridCoordinator.Instance.PlaceObjectOnGrid(_MoveableObject, CellSituation.DYNAMIC_FULL, (Cell *)_Path[0]);
        if (IsPathStillValid())
            _MoveableObject.RotateToDirection(Utils.GetDirectionByCompareCell(targetCell, (Cell *)_Path[0])); //objeyi gittigi pozisyona dogru donder
    }

    /*
        En kisa yol bulunur ve bu yollar listesine verilir eger bir yol dizisi gelmisse objenin gidecegi hedef olan hucre doldurulur
        bir onunde ki hucre ise dinamik olarak doldurulur. ve kosma animasyonuna gecis yapilir.
    */
    private unsafe void SetThePath() {

        _Path = AStar.FindShortestPath(_MoveableObject.BeginCell, _MoveableObject.Target);

        if (_Path == null || _Path.Count == 0) return;
        _MoveableObject.IsMoving = true;
        _MoveableObject.RotateToDirection(Utils.GetDirectionByCompareCell(_MoveableObject.BeginCell, (Cell *)_Path[0]));
        GridCoordinator.Instance.PlaceObjectOnGrid(_MoveableObject, CellSituation.EMPTY);
        GridCoordinator.Instance.PlaceObjectOnGrid(_MoveableObject, CellSituation.DYNAMIC_FULL, (Cell *)_Path[0]);
        GridCoordinator.Instance.PlaceObjectOnGrid(_MoveableObject, CellSituation.PLACED_FULL, (Cell *)_Path[^1]);
        _MoveableObject.PlayRunAnim();
    }


    /*
        Beklenmedik bir durumun olup olmadigi kontrol edilir eger var ise(gidilen yolda onune engel cikmasi durumunda)
    */
    private unsafe bool HandleUnexpectedSituationIfNeeded() {
        if (((Cell *)_Path[0])->Situation == CellSituation.PLACED_FULL) {

            FixUnexpectedSituation();
            return true;
        }

        return false;
    }

    /*
        Beklenmedik durum: targetin valid olmamasi, pathin valid olmamasi, onune engel cikmis olmas
        Beklenmedik durumda cozmek icin kullanilir.
    */
    private unsafe void FixUnexpectedSituation() {

        HandleUnexpectecSituationForValidPath(); // eger halihazirda bir yolda ilerliyorsa gerekli duzenlemeleri yap

        SetThePath(); // yeni yol bul
        if (_Path != null) return; // eger yol bulundu ise devam et

        GridCoordinator.Instance.PlaceObjectOnGrid(_MoveableObject, CellSituation.PLACED_FULL, _MoveableObject.BeginCell); //yol bulunamadigindan bulundugu yere yerlestir

        StopObject(); //objenin hareket etmesini saglayan degerleri sifirla

    }

    /*
        Eger halihazirda bir yol varken beklenmedik durum ile karsilanmasi durumunda doldurulan hucreleri geri bosaltan metot
    */
    unsafe void HandleUnexpectecSituationForValidPath()
    {
        Cell* beginCell, endCell;

        if (_Path == null || _Path.Count == 0) return;

        beginCell = (Cell*)_Path[0];
        endCell = (Cell*)_Path[^1];

        if (beginCell->Situation == CellSituation.DYNAMIC_FULL)
            GridCoordinator.Instance.PlaceObjectOnGrid(_MoveableObject, CellSituation.EMPTY, beginCell);

        GridCoordinator.Instance.PlaceObjectOnGrid(_MoveableObject, CellSituation.EMPTY, endCell);
    }
}

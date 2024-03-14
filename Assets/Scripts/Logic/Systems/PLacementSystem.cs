using UnityEngine;

/*
    Amac:
        - Yaratilan Yerlestirilebilir objenin kontrol, yerlestirme, yapilan islemi renklendirme ile bilgilendirme islemini yapar.
*/
class PlacementSystem : ISystem {

    #region Variables
    static private PlacementSystem _Instance;
    static public PlacementSystem Instance => _Instance == null ? _Instance = new PlacementSystem() : _Instance;

    IBaseObject _PlaceableObjct;

    #endregion


    #region public Attributes

    public bool IsPlacing { get => _PlaceableObjct != null; }

    /*
        - Tetiklendigi zaman objeyi yerlestirmeye calisir.
    */
    public void Action() => TryPlaceObject();

    /*
        - Yerlestirilebilir objeyi degistirir, eger hali hazirda var isi onu deallocate eder.
    */
    public void ChangePlaceableObject(IBaseObject baseObject)
    {
        if (baseObject != null && _PlaceableObjct != null) _PlaceableObjct.SelfDeallocate();
        _PlaceableObjct = baseObject;
    }

    #endregion


    #region Private Attributes

    /*
        - Eger yerlestirebilecegi bir obje var ise yerlestirmeye calisir.
    */
    void TryPlaceObject()
    {
        if (_PlaceableObjct != null) MoveAndTrySetPlaceableObject();
    }

    /*
        - Yerlestirilmeye calisilan objenin pozisyonu mouse'a gore ayarlanir.
        - Eger uygun durumda bulunuyor ise ve sol click yapildi ise obje yerlestirilir eger sag click yapildi ise obje serbest birakilir.
    */
    void MoveAndTrySetPlaceableObject()
    {

        if (Input.GetMouseButtonDown(1)) {
             ReleasePlacementObject();
             return;
        }

        RelaeseSelectedObject();
        MoveAndPlaceObjectIfPossible();
    }


    /*
        - Yerlestirme esnasinda eger herhangi bir obje secilmis ise onu kaldirir.
    */
    void RelaeseSelectedObject()
    {
        if (SelectionSystem.Instance.SelectedObject != null)
            SelectionSystem.Instance.ReleaseObject();
    }

    /*
        - Obje yerlestirilebilir ise yerlestirir ve pozisyonu mouse pozisyonuna gore ayarlanir.
    */
    unsafe void MoveAndPlaceObjectIfPossible()
    {
        Cell*   cell;

        if ((cell = Utils.ScreenToCell) != null) {

            _PlaceableObjct.ChangePosition(Utils.AllignedPositionByCell(cell, _PlaceableObjct.SpriteSize));

            if (_PlaceableObjct.IsPlaceable()) {
                if (Input.GetMouseButtonDown(0))
                {
                    if (LogicManager.IsClickedOnUiElement()) {
                        ReleasePlacementObject();
                        return;
                    }
                    _PlaceableObjct.ActivateObject(cell);
                    ChangePlaceableObject(null);
                }
                else _PlaceableObjct.ChangeObjectColor(Color.green);
            }
            else _PlaceableObjct.ChangeObjectColor(Color.red);
        }
    }

    /*
        - Yerlestirilebilir objeyi serbest birakir ve havuza geri verir.
    */
    unsafe void ReleasePlacementObject()
    {
        _PlaceableObjct.SelfDeallocate();
        ChangePlaceableObject(null);
    }

    #endregion


    #region Static Attributes

    /*
        - Spesifik olarak istenilen cell'e yerlestirme islemi yapar

        baseObject -> yerlestirilmesi istenen obje.
        cell -> yerlestirilmek istenen cell'in referansi

    */
    public static unsafe void PlaceObjectOnGrid(IBaseObject baseObject, Cell *cell)
    {
        if (cell == null || baseObject == null) return;

        baseObject.ActivateObject(cell);
    }
    #endregion

}

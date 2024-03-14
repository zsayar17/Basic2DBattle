using UnityEngine;

/*
    Amac:
        -Herhangi bir objenin secilip secilemediginin kontrol edildigi yerdir.
*/
public class SelectionSystem : ISystem{
    #region Variables

    private static SelectionSystem _Instance;
    public static SelectionSystem Instance { get => _Instance == null ? (_Instance = new SelectionSystem()) : _Instance; }

    #endregion


    #region Public Attributes
    public BaseObject SelectedObject { get; private set; }

    /*
        Secilen bir obje olup olmadigini kontrol eder.
    */
    public bool IsAnyObjectSelected { get => SelectedObject != null; }

    /*
        Secilen objeyi serbest birakir ve bilgilendirme ekranini kapatir.
    */
    public void ReleaseObject()
    {
        SelectedObject = null;
        LogicManager.ShutDownInfo();
    }

    /*
        - sol tik yapildi ise ve tiklanan konum ui elementi degil ise secmeye calisir, eger secilemedi ise ekrani kapatir.
    */
    public void Action()
    {
        if (!Input.GetMouseButtonDown(0) || LogicManager.IsClickedOnUiElement()) return;

        SelectedObject = Utils.ScreenToObject;
        if (IsAnyObjectSelected) LogicManager.ShowInfo(SelectedObject);
        else LogicManager.ShutDownInfo();
    }
    #endregion
}

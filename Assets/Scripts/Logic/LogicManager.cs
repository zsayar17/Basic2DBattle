using System;
using UnityEngine;

/*
    Coordinators: Coordinatorler sahip oldugu ozelligi duzenleyen ve senkronize eden yapilardir.
    Systems: Logic kisminda durumlari belirleyen yapilardir.
/*


/*
    Amac:
        -Tum Logic islemlerinin yonetildigi yerdir.
*/

[Serializable]
public class LogicManager
{
    [Header("Object Pooling")]
    [SerializeField] GameObject[] _Prefabs;
    [SerializeField] int _PoolingCount;

    [Header("Camera")]
    [SerializeField] float _ZoomSpeed;
    [SerializeField] float _MinZoomSize;
    [SerializeField] float _MaxZoomSize;


    public static void ShutDownInfo() => GameManager.Instance.ShutDownInfo();


    public void Awake()
    {
        SetPoolVariables();
        SetCameraVariables();
    }

    public void Update () {
        //GridCoordinator.Instance.ColorizeMap();
        SelectionSystem.Instance.Action();
        PlacementSystem.Instance.Action();
        CameraCoordinator.Instance.Update();
    }

    public void CreateObject(ObjectType objectType) {
        if (objectType == ObjectType.BARRACK || objectType == ObjectType.POWERPLANT) CreateBuilding(objectType);
        else CreateUnit(objectType);
    }

    void CreateUnit(ObjectType objectType) {
        ICreateable createable;

        if ((createable = SelectionSystem.Instance.SelectedObject as ICreateable) == null) return;
        createable.CreateObject(objectType);
    }

    /*
        Amac:
            inspectordan alinan bilgilerle PoolSystem verileri ayarlanir ve ardindan baslatilir.
    */

    void SetPoolVariables()
    {
        PoolSystem.Instance.PoolingCount = _PoolingCount;
        PoolSystem.Instance.Prafabs = _Prefabs;
        PoolSystem.Instance.InitPoolSystem();
    }

    /*
        Amac:
            inspectordan alinan bilgilerle Camera verileri ayarlanir ve ardindan baslatilir.
    */

    void SetCameraVariables()
    {
        CameraCoordinator.Instance.MinSize = _MinZoomSize;
        CameraCoordinator.Instance.MaxSize = _MaxZoomSize;
        CameraCoordinator.Instance.ZoomSpeed = _ZoomSpeed;
    }

    /*
        GameManager uzerinden ui tarafindan gelen yaratma talebini parametre olarak gonderilen object type'a gore uygulayan metot.
        Secilen bir obje var ise o birakilr ve placement sisteme yeni allocate edilen obje verilir
    */
    void CreateBuilding(ObjectType objectType)
    {
        if (PlacementSystem.Instance.IsPlacing) return;

        SelectionSystem.Instance.ReleaseObject();
        PlacementSystem.Instance.ChangePlaceableObject(PoolSystem.Instance.AllocateObject(objectType));
    }

    /*
        Verilen obje turunun sprite'ini return eden metot.
        bunu yapmak icin havuzdan bir tane obje tahsis edilir sprite'i alinir ve sonrasinda geri deallocate edilir.
    */
    public Sprite GetSprite(ObjectType objectType)
    {
        BaseObject baseObject;
        SpriteRenderer spriteRenderer;

        baseObject = PoolSystem.Instance.AllocateObject(objectType);
        spriteRenderer = baseObject.SpriteRenderer;
        PoolSystem.Instance.DeallocateObject(baseObject);
        return spriteRenderer.sprite;
    }

    // GameManager uzerinden Ui elementine tiklama olup olmadigini kontrol eden metot
    public static bool IsClickedOnUiElement() => GameManager.Instance.IsClickedOnUiElement();
    // GameManager uzerinden Ui kismina bilgilendirme gostermek icin talep gonderilir.
    public static void ShowInfo(IUIDisplayable uIDisplayable) => GameManager.Instance.ShowInfo(uIDisplayable);
}

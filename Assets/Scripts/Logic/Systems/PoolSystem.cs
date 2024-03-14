using UnityEngine;
using System.Collections.Generic;

/*
    Amac:
        Object pooling yapilarak surekli yaratma isleminden kurtulunur ve performans kazanilir. Daha once bellekte tahsis etmis olan Sistem uzerinden istenilen tipte obje alinabilir ve geri verilebilr.
*/

public class PoolSystem
{
    #region Attributes
    private static PoolSystem _Instance;
    public static PoolSystem Instance { get => _Instance == null ? _Instance = new PoolSystem() : _Instance; }
    public GameObject PoolObject;
    public int PoolingCount;

    public GameObject[] Prafabs;
    List<BaseObject>[] _AllocatedList;
    List<BaseObject>[] _ReadyToAllocateList;
    static int[] _ObjectId;
    #endregion

    #region Public Attributes
    /*
        - gonderilen obje turune gore onceden havuzlanmis obje gonderilmeye calisilr eger havuzda obje kalmadi ise havuza yeni obje eklenir ve o yollanir.
    */
    public BaseObject AllocateObject(ObjectType objectType)
    {
        BaseObject allocatedObject;

        if (_ReadyToAllocateList[(int)objectType].Count == 0)
            InstantiateNewObject(objectType, _AllocatedList[(int)objectType][0].gameObject);

        allocatedObject = _ReadyToAllocateList[(int)objectType][0];

        _AllocatedList[(int)objectType].Add(allocatedObject);
        _ReadyToAllocateList[(int)objectType].Remove(allocatedObject);

        allocatedObject.TurnOn();

        return allocatedObject;
    }

    /*
        - Havuzdan tahsis edilmis obje geri havuza dondurulur
    */
    public void DeallocateObject(BaseObject baseObject)
    {
        ObjectType objectType;

        objectType = baseObject.ObjectType;
        _AllocatedList[(int)objectType].Remove(baseObject);
        _ReadyToAllocateList[(int)objectType].Add(baseObject);
        baseObject.TurnOff();
    }
    /*
        - Havuzlama sistemi baslatilir.
    */
    public void InitPoolSystem()
    {
        InitilizeLists();
        InitilazeBaseObject();
        InitilazeAllTypes();
    }
    #endregion


    #region Private Atrributes
    /*
        - Listeler icin bellekten alan tahsis eder.
    */
    void InitilizeLists()
    {
        _AllocatedList = new List<BaseObject>[(int)ObjectType.OBJECT_COUNT];
        _ReadyToAllocateList = new List<BaseObject>[(int)ObjectType.OBJECT_COUNT];
        _ObjectId = new int[(int)ObjectType.OBJECT_COUNT];

        for (int i = 0; i < _AllocatedList.Length; i++) _AllocatedList[i] = new List<BaseObject>();
        for (int i = 0; i < _ReadyToAllocateList.Length; i++) _ReadyToAllocateList[i] = new List<BaseObject>();

    }
    /*
        - Havuzlama yapilacak objenin parenti olacak bos bir obje olusturulur
    */
    void InitilazeBaseObject() => PoolObject = new GameObject("PoolingObject");

    /*
        - inspectordan gelen prefablar dinamik olarak BaseObject componentine sahip mi degil mi o kontrol edilir, eger varsa hangi tip oldugu analiz edilerek initilaze etmek icin yollanir.

    */
    void InitilazeAllTypes()
    {
        BaseObject baseObject;

        foreach (GameObject gameObject in Prafabs)
        {
            baseObject = gameObject.GetComponent<BaseObject>();
            if (baseObject == null) return;

            AnalysAndInitilaze(baseObject, gameObject);
        }
    }

    /*
        - Analize gonderilen objenin tipi analiz edildikten sonra yaratmaya gonderilir.
    */
    void AnalysAndInitilaze(BaseObject baseObject, GameObject instantiateObject)
    {
        if (baseObject as Barrack != null) InitilazeSpecificType(ObjectType.BARRACK, instantiateObject);
        else if (baseObject as PowerPlant != null) InitilazeSpecificType(ObjectType.POWERPLANT, instantiateObject);
        else if (baseObject as Soldier1 != null) InitilazeSpecificType(ObjectType.SOLDIER1, instantiateObject);
        else if (baseObject as Soldier2 != null) InitilazeSpecificType(ObjectType.SOLDIER2, instantiateObject);
        else if (baseObject as Soldier3 != null) InitilazeSpecificType(ObjectType.SOLDIER3, instantiateObject);
    }

    /*
        - Gonderilen tipte ki obje inspectorda belirlenen havuzlama sayisi kadar olusturma islemine gonderilir
    */
    void InitilazeSpecificType(ObjectType objectType, GameObject instantiateObject)
    {
        for (int i = 0; i < PoolingCount; i++)
        {
            InstantiateNewObject(objectType, instantiateObject);
        }
    }
    /*
        - Obje yaratilir, parenti onceden yaratilan base object olarak ayarlanir, ismi unique id ile belirlenir ve allocate edilebilir listeye atanir. ardindan obje kapatilir.
    */
    void InstantiateNewObject(ObjectType objectType, GameObject instantiateObject)
    {
        BaseObject baseObject;
        baseObject = GameObject.Instantiate(instantiateObject).GetComponent<BaseObject>();
        baseObject.transform.parent = PoolObject.transform;
        baseObject.transform.name = objectType.ToString() + "_" + (++_ObjectId[(int)objectType]);
        _ReadyToAllocateList[(int)objectType].Add(baseObject);
        baseObject.TurnOff();
    }
    #endregion
}

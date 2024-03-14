using System;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : BaseBuilding, ICreateable
{
    unsafe private Cell* _SeedPoint; // sahip olunan yumurtlama noktasinin referansi
    private int _CreatedObjectCount; // yaratilan obje sayisi(UI'da gosterilmek icin)

    private Transform _SeedTransform; // sahip olunan yumurtlama noktasinin(child object) transformu
    private SpriteRenderer _SeedPointRenderer;
    private string _InfoMessage; // Ui'a gonderilecek mesaj

    /*
        Obje unity tarafindan yaratildiginda gerekli degerler ayarlanir
    */
    protected override void Awake()
    {
        Size = new Vector2Int(4, 4);
        ObjectType = ObjectType.BARRACK;
        _SeedTransform =  transform.GetChild(0);
        _SeedPointRenderer = _SeedTransform.GetComponent<SpriteRenderer>();
    }

    /*
        Obje eger yerlestirilmemis ise devam edilmez eger yerlestirilmis ise dongu icerisinde yumurtlama noktasi olmadigi zaman olana kadar
        bos alan aray metot
    */
    protected override unsafe void Update()
    {
        if (!IsPlaced) return;

        if (_SeedPoint == null) FindSeedPoint();
    }

    /*
        Yumurtlama noktasinin ayarlandigi metot.
        Yumurtlama noktasi objenin cevresinde ki komsu hucrelere bakilir ve ardindan komsularin bos olup olmadigi kontrol edilir.
        eger bos ise yumurtlama noktasinin spritei enable edilir bulunan bos yere pozisyonu ayarlanir.
    */
    private unsafe void FindSeedPoint()
    {
        List<IntPtr> neigbours;

        neigbours = GridCoordinator.Instance.GetNeigbours(this);

        if (neigbours != null) {
            foreach (IntPtr neigbour in neigbours) {
                if (neigbour != null && ((Cell*)neigbour)->Situation != CellSituation.EMPTY) continue;
                _SeedPoint = (Cell*)neigbour;
                _SeedPoint->Situation = CellSituation.DYNAMIC_FULL;
                _SeedPointRenderer.enabled = true;
                _SeedTransform.position = Utils.AllignedPositionByCell(_SeedPoint, new Vector3(1, 1, 0)) ;
                return;
            }
        }
        _SeedPoint = null;
        _SeedPointRenderer.enabled = false;
    }

    /*
        Ui kismindan gamemanager -> logicmanager boru hattindan gelen yaratma isteginin uygulandigi metot
        eger obje yumurtlama noktasina sagip degilse hicbirsey yapilmaz eger sahipse yeni obje havuzdan allocate edilir
        ve yeni bir seed alani bulunur ardindan yaratma sayisi arttirilir ve bilgilendirme mesaji guncellenir.
    */
    public unsafe void CreateObject(ObjectType objectType)
    {
        BaseObject createdObject;

        if (_SeedPoint == null) return;

        createdObject = PoolSystem.Instance.AllocateObject(objectType);
        PlacementSystem.PlaceObjectOnGrid(createdObject, _SeedPoint);
        createdObject.BeginTheProcess();

        FindSeedPoint();

        ++_CreatedObjectCount;
        UpdateInfoMessage();
    }

    public override void BeginTheProcess()
    {
        base.BeginTheProcess();
        UpdateInfoMessage();
    }



    public override unsafe void TurnOff()
    {
        base.TurnOff();
        if (_SeedPoint != null) _SeedPoint->Situation = CellSituation.EMPTY;
        _SeedPoint = null;
        _SeedPointRenderer.enabled = false;
        _CreatedObjectCount = 0;
        _InfoMessage = "Dieddd :(((";

    }
    public override void UpdateInfoMessage() => _InfoMessage = "Created Unit Count: " + _CreatedObjectCount + "\nHealth: " + Health;
    public override string InfoMessage() => _InfoMessage;
}

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseObject : MonoBehaviour, IBaseObject, IUIDisplayable {
    #region IBaseObject Properties
    public int MaxHealth; // Obbjenin maksimum cani
    public int Health { get; set; }


    SpriteRenderer _SpriteRenderer;
    public SpriteRenderer SpriteRenderer { get => _SpriteRenderer == null ?  _SpriteRenderer = GetComponent<SpriteRenderer>() : _SpriteRenderer; }
    public Vector3 SpriteSize { get => SpriteRenderer.bounds.size; }
    public Vector2Int  Size { get; set; }


    public bool IsPlaced { get; set; }
    public bool IsMoving { get; set; }
    public bool IsValid { get => IsPlaced; }


    public unsafe Cell*  BeginCell { get; set; }
    public Vector3 BeginPosition { get => new Vector3(transform.position.x - SpriteSize.x / 2, transform.position.y - SpriteSize.y / 2, 0); } // objenin sol ust kordinati
    public unsafe Vector3 AllignedPosition { get => Utils.AllignedPositionByCell(BeginCell, SpriteSize); } // objenin hucreye gore hizalanmis merkez konumu


    public BaseType BaseType { get => BaseType.Object;}
    public ObjectType ObjectType { get; set; }

    #endregion

    #region  MonoBehaviour Methods
    protected virtual void Awake() { }
    protected virtual void Update() { }
    #endregion



    #region Init / Destroy Methods


    /*
        obje havuzdan tahsis edildigi zaman calisan metot
    */
    public virtual void TurnOn()
    {
        Health = MaxHealth;
        SpriteRenderer.enabled = true;
    }

    /*
        obje havuza iade edildiginde calisan metot
    */
    public virtual unsafe void TurnOff()
    {
        if (IsPlaced) GridCoordinator.Instance.PlaceObjectOnGrid(this, CellSituation.EMPTY);
        IsPlaced = false;
        IsMoving = false;


        SpriteRenderer.enabled = false;
        transform.position = new Vector3(-100, -100, 0);
    }

    /*
        Objeyi aktif hale getirmek icin kullanilan metot.
        bu metot Placementsystem tarafından havuzdan tahsis edilmis objenin yerlestirme islemi yapmasi icin cagirilan metot.
        baslangic kordinati verilen hucre parametresi ile belirlenir.
    */
    public unsafe void ActivateObject(Cell *cell) {

        BeginCell = cell;
        IsPlaced = true;

        transform.position = AllignedPosition;
        GridCoordinator.Instance.PlaceObjectOnGrid(this, CellSituation.PLACED_FULL);

        BeginTheProcess();
    }

    /*
        obje tahsis edildikten sonra donguye girmeden once calisan metot monobehaviour'da ki start gibi dusunulebilir
        virtual olmasi sayesinde ust siniflarda da override edilebilir.
    */
    public virtual void BeginTheProcess() { }

    /*
        kendisini tekrardan havuz sistemini birakan metot
    */
    public void SelfDeallocate() => PoolSystem.Instance.DeallocateObject(this);

    #endregion



    #region Others

    /*
        objenin rengini degistiren metot
    */
    public void ChangeObjectColor(Color color) => SpriteRenderer.color = color;

    /*
        objenin pozisyonunu degistiren metot
    */
    public void ChangePosition(Vector3 position) => transform.position = position;

    /*
        objenin yerlestirilebilir olup olmadigini kontrol eden metot
    */
    public bool IsPlaceable() => Utils.IsPlaceable(this);

    /*
        objenin rotasyonunu degistiren metot
    */
    public void ChangeRotation(Quaternion rotation, bool flipX) {
        SpriteRenderer.flipX = flipX;
        transform.rotation = rotation;
    }

    /*
        objeyi istenilen directiona donduren metot
    */
    public void RotateToDirection(Direction direction) => Utils.LookAt(this, direction);

    /*
        objenin demage almasini saglayan metot, infomessage update edilerek ui kisminda guncellenmesini saglanmaktadir.
    */
    public unsafe bool GetDemage(IAttackable attackFrom) {
        Health -= attackFrom.DamageAmount;
        UpdateInfoMessage();

        return Health <= 0;
    }
    #endregion



    #region  IUIDisplayable Methods
    public string Name { get => transform.name; }

    /*
        Ui kismina verilecek mesaj bilgisi metodunun return edildigi metot
    */
    public IUIDisplayable.StringReturningMethod GetInfoMessage() => InfoMessage;
    /*
        Ui elementine referansi yollanan metot, bu metot uzerinden ui kismi Logic kismin ile iletisim kurmadan yansitilacak mesajlari guncelleyebilir.

    */
    public virtual string InfoMessage() =>  "";

    /*
        Ui elementi uzerinden erisilecek mesajin guncellendigi metot
    */
    public virtual void UpdateInfoMessage() { }
    #endregion

}

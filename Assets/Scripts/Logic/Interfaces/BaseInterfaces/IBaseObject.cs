using System.Collections.Generic;
using UnityEngine;


/*
    En temel obje arayuzu
*/
public interface IBaseObject : IBase {
    public Vector3 BeginPosition{ get; } // Karakterin sol ust noktasinin pozisyonu
    public unsafe Vector3 AllignedPosition { get; } // Karakterin Hucreye sıgabilmesi icin hizalanmis pozisyon

    public SpriteRenderer SpriteRenderer{get; }
    public Vector3 SpriteSize { get; }

    public bool IsPlaced { get; } // objenin  yerlestirilip yerlestirilmedigini kontrol eden degisken
    public bool IsMoving { get; set; } // karakterin  hareket edip etmedigini kontrol eden degisken

    public bool GetDemage(IAttackable attackFrom); // Karakterin darbe aldigi metod


    public void TurnOn(); // karakter havuzdan cekildigi zaman calisan metod
    public void TurnOff(); // karakter havuza verildiginde calisan metod
    public void BeginTheProcess(); // karakter yerlestirildikten sonra calisan metod

    public void UpdateInfoMessage(); // bilgilendirme ekranina gonderilecek bilginin guncellendigi metod


    public void SelfDeallocate(); // kendisini havuza geri veren metod
    public unsafe void ActivateObject(Cell *cell); // karakterin havuzdan allocate edilmesi icin calistirilan metod


    public void ChangeObjectColor(Color color); // karakterin rengini degistiren metod
    public bool IsPlaceable(); // karakterin bulundugu pozisyonda yerlestirilebilir olup olmadigini kontrol eden metod
    public void ChangePosition(Vector3 position); // karakterin pozisyonu degistiren metod
    public void ChangeRotation(Quaternion rotation, bool flipX); // karakterin rotasyonunu degistiren metod eger flipx degeri dogru gelirse sprite'ın ters donderildigi metod
    public void RotateToDirection(Direction direction); // karakterin istenilen yone bakmasini saglayan metod (Direction yonlerine)
}

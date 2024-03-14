using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class PowerPlant : BaseBuilding
{
    public static int ProducedEnergy; //yaratilan enerji miktari
    public int ProduceTime; // yaratilacak enerjinin yaratma sureci
    public int ProduceAmount; // yaratilacak enerji miktari
    private int _TotalProducedEnergy; // total enerji miktari
    private string _InfoMessage; // ui kisminda goruntulenecek mesaj

    protected override void Awake()
    {
        base.Awake();
        Size = new Vector2Int(2, 3);
        ObjectType = ObjectType.POWERPLANT;
    }

    /*
        obje havuza geri verdiginde mesaj ve uretilen enerji miktari sıfırlanir calistirilan couraitne durdurulur.
    */
    public override unsafe void TurnOff()
    {
        base.TurnOff();
        StopAllCoroutines();
        _TotalProducedEnergy = 0;
        _InfoMessage = "Garibanin yüzü gülürmü:((";
    }

    public override unsafe void TurnOn()
    {
        base.TurnOn();
    }

    // obje yerlestirilme durumu oldugunda calistirilir ve enerji arttirma couratine'i calistilir
    public override void BeginTheProcess()
    {
        base.BeginTheProcess();
        StartCoroutine(ProduceEnergy());
        UpdateInfoMessage();
    }

    /*
        Couratine yapisi icerisinde enerji uretilir  ve mesaj bilgisi guncellenir
    */
    IEnumerator ProduceEnergy()
    {
        while (true)
        {
            yield return new WaitForSeconds(ProduceAmount);
            ProducedEnergy += ProduceAmount;
            _TotalProducedEnergy += ProduceAmount;
            UpdateInfoMessage();
        }
    }
    public override void UpdateInfoMessage() => _InfoMessage = "ProductedEnergy: " + _TotalProducedEnergy + "\nHealth: " + Health;
    public override string InfoMessage() => _InfoMessage;
}

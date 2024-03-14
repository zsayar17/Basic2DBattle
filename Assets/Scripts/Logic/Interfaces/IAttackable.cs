using UnityEngine;


/*
    Attack yapma yetisine sahip olan objelerin kalitildigi interface
*/
public interface IAttackable : IDynamicObject {

    public int DamageAmount { get; } // hedefe verilen hasar miktari
    public int DestroyedObjecCount {get; set;} // yokedilen obje miktari
    public void OnAttackAction(); // attack edildi zaman tetiklenen metot(animasyon tarafindan event olarak tetiklenir)

    public void PlayAttackAnim(); // attack animasyonunu oynatan metot
    public void PlayIdleAnim(); // idle animasyonunu oynatan metot

}

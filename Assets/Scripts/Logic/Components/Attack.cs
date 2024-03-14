using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mono.Cecil;

/*
    Amac:
        Atak islemlerinin yapilen objelerin atak islemlerini yaptirdigi class
*/
public class Attack {
    IAttackable _BaseAttackable;

    public Attack(IAttackable attackable) => _BaseAttackable = attackable;


    /*
        Eger karakter valid bir hedefe sahip degilse attack etmemek icin deferini falselar.
        Hedefin bir obje olup olmadigi kontrol edilir cunku hedef bir hucrede olabilir.
        eger obje attacka degilse ve attacka hazir ise attack islemi baslatilir ve animasyonu oynatilmaya baslanir.
        eger attack isleminde ise saldirdigi objeninde kendisine hedeflenmeye cabalanilir.
    */
    public void Action()
    {
        if (_BaseAttackable.Target == null || _BaseAttackable.Target.IsValid == false) _BaseAttackable.IsAttacking = false;

        if (_BaseAttackable.Target as IBaseObject == null) return;

        if (!_BaseAttackable.IsAttacking && IsReadyToAttack())
        {
            _BaseAttackable.IsAttacking = true;
            _BaseAttackable.PlayAttackAnim();
        }
        if (_BaseAttackable.IsAttacking) GainANewEnemy();

    }

    //Animasyonda tetiklenen metodun tetiklendigi metod eger saldiriya hazir ise dusmana hasar verir
    public void TryHitTarget()
    {
        if (IsReadyToAttack()) HitTarget();
    }

    /*
        targetin hala gecerli olunmasi ve hedef ile sahip oldugu objenin hareket edip etmedigi kontrol edilir, bu durumlarda saldiri yapilmaz.
        ardindan targetin cevresinde ki hucreler kontrol edilir. eger cevresinde ki hucrelerde de yoksa o zaman false return edilir.
    */
    private unsafe bool IsReadyToAttack() {
        List<IntPtr> neigbours;

        if (_BaseAttackable.Target == null || ((IBaseObject)_BaseAttackable.Target).IsMoving  || _BaseAttackable.IsMoving)
            return false;
        neigbours = GridCoordinator.Instance.GetNeigbours(_BaseAttackable.Target);
        if (neigbours == null || !neigbours.Contains((IntPtr)_BaseAttackable.BeginCell))
            return false;

        return true;
    }

    /*
        targeta hasar verilir eger hasar sonucu cani 0'in altina dustu ise hedef oldurulur.
    */
    private void HitTarget()
    {
        IBaseObject baseTarget;

        baseTarget = (IBaseObject)_BaseAttackable.Target;
        if (baseTarget.GetDemage(_BaseAttackable)) KillEnemy(baseTarget);
    }

    /*
        Hedefin saldirabilir bir karakter olup olmadigi kontrol edilir eger oyle ise o objenin hedefi kontrol edilir.
        kontrol sonucu hedefin hedefi anlik obje olarak belirlenir ve atak animasyonu oynatilmaya baslanir
    */
    private void GainANewEnemy()
    {
        IAttackable attackable;

        if (_BaseAttackable.Target as IAttackable == null) return;
        attackable = (IAttackable)_BaseAttackable.Target;
        if ((attackable.Target != null && attackable.Target.BaseType == BaseType.Object) || attackable.Target == this) return;
        attackable.Target = _BaseAttackable;
        attackable.IsAttacking = true;
        attackable.PlayAttackAnim();
    }

    /*
        hedefin hedef ve atak degerleri sifirlanir, oldurenin oldurme sayisi bir arttirilir ve tahsis edilen obje havuza geri verilir.
        obje tekrardan idle animasyonuna gecer.
    */
    private unsafe void KillEnemy(IBaseObject boomer)
    {
        _BaseAttackable.IsAttacking = false;
        _BaseAttackable.Target = null;

        _BaseAttackable.DestroyedObjecCount++;
        _BaseAttackable.UpdateInfoMessage();

        PoolSystem.Instance.DeallocateObject((BaseObject)boomer);

        _BaseAttackable.PlayIdleAnim();
    }
}


using UnityEngine;

[System.Serializable]

/*
    Bu class soldier'larin turedigi classtir ve soldeierlarin ihtiyaci olan hareket ve saldiri islemleri, ui bilgilerini
    guncelleme islerini yapar.
*/
public class Unit : BaseObject, IMoveable, IAttackable {

    public int Damage; // Verilecek hasar, prefabe inspectordan verilir.
    public float Speed; // karakterin hizi inspectordan verilir
    Animator _Animator; // animasyon componenti

    Movement _Movement; // Movement componenti
    Attack _Attack; // Attack componenti
    private string _InfoMessage; // Ui'a yollanacak mesaj
    public int DestroyedObjecCount {get; set;} // yok edilen obje sayisini hesaplayan property

    public int DamageAmount { get => Damage; }
    public float ObjectSpeed { get => Speed; }
    public IBase Target {get; set;} // Hareket edilecek ya da saldirilacak hedefin referansi
    public bool IsAttacking { get; set;}

    protected override void Awake()
    {
        base.Awake();
        _Animator = GetComponent<Animator>();

        Size = new Vector2Int(1, 1);
        _Attack = new Attack(this);
        _Movement = new Movement(this);
    }

    protected override void Update()
    {
        if (IsPlaced) Action();
    }

    /*
        her dongude calisam metot mouse sol tiklanmasi durumunda ve ui elementine tiklanmamais olmasi durumunda yeni target
        bulunmaya calisilir ardindan movement companenti setlenir (yeni gidilecek yol guzergahini bulur). target invalid ise onu duzenler
        ardindan movement ve attack
        componentlerini aksiyona sokar.
    */
    public unsafe void Action()
    {
        if (SelectionSystem.Instance.SelectedObject == this) {
            if (Input.GetMouseButtonDown(1) && !LogicManager.IsClickedOnUiElement()) {

                TrySetTarget();
                _Movement.Set();
            }
        }

        IfTargetrInvalidFix();
        _Movement.Action();
        _Attack.Action();
    }

    /*
        Animasyonda tetiklenen metot, tetiklendigi zaman attack companenti uzerinden hedefe hasar verir
    */
    public void OnAttackAction() => _Attack.TryHitTarget();

    /*
        Havuzdan tahsis edilmesi durumunda animasyonu baslatir
    */
    public override void TurnOn()
    {
        base.TurnOn();
        _Animator.enabled = true;
        PlayIdleAnim();
    }


    public override void BeginTheProcess() => UpdateInfoMessage();

    public override void TurnOff()
    {
        base.TurnOff();

        DestroyedObjecCount = 0;
        _InfoMessage = "Babalar ölmezzzz...";
        _Animator.enabled = false;
        Target = null;
        IsAttacking = false;
        RotateToDirection(Direction.RIGHT);
    }

    /*
        Eger hedef invalid ise ya da target kendisi ise target nullanir.
    */
    private void IfTargetrInvalidFix()
    {
        if (Target == null || !Target.IsValid || (Target == (IBase)this)) Target = null;
    }

    /*
        tiklanan yerdeki base'i(Cell ya da obje) Targeta esitlemeye calisr calisir
    */
    private void TrySetTarget()
    {
        if (IsAttacking) return;

        Target = Utils.ScreenToBase;
    }

    public void PlayAttackAnim() {
        _Animator.SetBool("Idle", false);
        _Animator.SetBool("Run", false);
        _Animator.SetBool("Attack", true);
    }
    public void PlayIdleAnim() {
        _Animator.SetBool("Attack", false);
        _Animator.SetBool("Run", false);
        _Animator.SetBool("Idle", true);
    }
    public void PlayRunAnim() {
        _Animator.SetBool("Idle", false);
        _Animator.SetBool("Attack", false);
        _Animator.SetBool("Run", true);
    }

    public override void UpdateInfoMessage() => _InfoMessage = "Destroyed object count: " + DestroyedObjecCount
        + "\nHealth: " + Health + "\nDemage: " + DamageAmount + "\nSpeed: " + Speed;
    public override string InfoMessage() => _InfoMessage;

}

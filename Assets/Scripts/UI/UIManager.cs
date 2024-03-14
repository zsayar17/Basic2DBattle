using UnityEngine;
using System;

[Serializable]

/*
    Uı elemenleri ile Logic kismi arasinda kopru gorevi goren class
*/
class UIManager
{

    [SerializeField] InformationMenu InformationMenu;
    [SerializeField] ProductMenu  ProductMenu;

    static public Sprite[] Sprites = new Sprite[(int)ObjectType.OBJECT_COUNT]; //Ui uzerinden yaratilabilir objelerin spritelarinin tutuldugu dizi

    public void Awake()
    {
        InitImages();
        InformationMenu.Awake();
        ProductMenu.Awake();
    }

    public void Update()
    {
        InformationMenu.Update();
        ProductMenu.Update();
    }

    // spritelarin GameManager uzerinden talep edildigi metot
    public void InitImages() {
        for (int i = 0; i < (int)ObjectType.OBJECT_COUNT; i++) Sprites[i] = GameManager.Instance.GetSprite((ObjectType)i);
    }

    // GameManager uzerinden Logic kismina gelen taleplerin yonlendirildigi metotolar
    public void TurnInformationMenuOn(IUIDisplayable uIDisplayable) => InformationMenu.TurnOn(uIDisplayable);
    public void TurnInformationMenuOff() => InformationMenu.TurnOff();

    // herhangi bir ui elementine tiklama olup olmadigini kontol eden metot
    public bool IsClickedOnUiElement() => InformationMenu.IsClickedOnUiElement() || ProductMenu.IsClickedOnUiElement();

    //ui kismindan talep edilmesi durumunda Game manager uzerinden Logic manager tarafindan obje yaratma  talebinde bulunma
    public static void CreateObject(ObjectType objectType) => GameManager.Instance.CreateObject(objectType);
}

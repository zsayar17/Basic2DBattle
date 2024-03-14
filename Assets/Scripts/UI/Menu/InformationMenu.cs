
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]

/*
    Information Menu uzerinden yapilacklarin ayarlandigi bilgilerin guncenllendigi metot
*/

public class InformationMenu
{
    [SerializeField] TextMeshProUGUI Name, Info; // Uı kisminda gosterilecek name ve info TMP'leri

    [SerializeField] Button[] Buttons; // barrack'in yaratabilecegi button objeleri
    [SerializeField] Image Image; // panelde gosterilecek gorsel
    [SerializeField] RectTransform  _PanelRectTransform; // panelin transformu
    IUIDisplayable.StringReturningMethod InfoMessage; // logic tarafinda guncellenecek olan bilgilendirme mesajinin tutuldugu fonksiyon referans
    ObjectType[] ButtonObjectTypes = { //button tiplerinin sirasini belirleyen liste
        ObjectType.SOLDIER1, ObjectType.SOLDIER2, ObjectType.SOLDIER3
    };

        private bool _ShowButtons, _ShowInfo;

    /*
        buttonlarin gorsellerinin verildigi ve listener eklendigi metot
    */
    public void InitilazeButtons()
    {
        Buttons[0].image.sprite = UIManager.Sprites[(int)ButtonObjectTypes[0]];
        Buttons[0].onClick.AddListener(() => UIManager.CreateObject(ButtonObjectTypes[0]));

        Buttons[1].image.sprite = UIManager.Sprites[(int)ButtonObjectTypes[1]];
        Buttons[1].onClick.AddListener(() => UIManager.CreateObject(ButtonObjectTypes[1]));

        Buttons[2].image.sprite = UIManager.Sprites[(int)ButtonObjectTypes[2]];
        Buttons[2].onClick.AddListener(() => UIManager.CreateObject(ButtonObjectTypes[2]));
    }

    /*
        Uyandiigi zaman buttonlar initilaze edilir ve panel deactive edilir
    */
    public void Awake()
    {
        InitilazeButtons();

        _PanelRectTransform.gameObject.SetActive(false);
    }

    /*
        Eger panel acik durumda ise txt'nin metot uzeirnden guncellenir
    */
    public void Update()
    {
        if (!_ShowInfo) return;
        Info.text = InfoMessage();
    }

    // panel kapatildiginda degerler sifirlanir
    public void TurnOff()
    {
        if (_ShowInfo == false) return;
        Image.sprite = null;
        Name.text = null;
        InfoMessage = null;

        _ShowButtons = false;
        _PanelRectTransform.gameObject.SetActive(false);

        foreach(Button button in Buttons) button.gameObject.SetActive(false);

        _ShowInfo = false;
    }

    //panel acildiginda degerler tekrardan initilaze edilir.
    public void TurnOn(IUIDisplayable uIDisplayable)
    {
        _ShowInfo = true;

        Image.sprite = UIManager.Sprites[(int)uIDisplayable.ObjectType];
        Name.text = uIDisplayable.Name;
        InfoMessage = uIDisplayable.GetInfoMessage();

        _ShowButtons = uIDisplayable as ICreateable != null;
        _PanelRectTransform.gameObject.SetActive(true);

        foreach(Button button in Buttons) button.gameObject.SetActive(_ShowButtons);
    }

    // panele tiklanip tiklanmadigini return eder
    public bool IsClickedOnUiElement() =>
        _ShowInfo && RectTransformUtility.RectangleContainsScreenPoint(_PanelRectTransform, Input.mousePosition);
}

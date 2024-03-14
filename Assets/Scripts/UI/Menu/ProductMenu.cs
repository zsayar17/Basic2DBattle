using System;
using UnityEngine;
using UnityEngine.UI;

/*
    Product Menu uzerinden yapilacklarin ayarlandigi, sonsuz şekilde yukari dogu scrollanmasini saglayan class
*/

[Serializable]
public class ProductMenu
{
    [SerializeField] private Button BarrackButton, PowerPlantButton;
    [SerializeField] private ScrollRect ScrollRect;
    [SerializeField] private RectTransform ContentPanelTransform;
    [SerializeField] private RectTransform ScrollViewPortTransform;
    [SerializeField] private RectTransform Item;

    private const int ItemsToAdd = 30;
    private float itemHeight;

    /*
        Baslangic degerleri olusturulur
    */
    public void Awake()
    {
        BarrackButton.onClick.AddListener(() => UIManager.CreateObject(ObjectType.BARRACK));
        PowerPlantButton.onClick.AddListener(() => UIManager.CreateObject(ObjectType.POWERPLANT));

        BarrackButton.image.sprite = UIManager.Sprites[(int)ObjectType.BARRACK];
        PowerPlantButton.image.sprite = UIManager.Sprites[(int)ObjectType.POWERPLANT];

        itemHeight = Item.rect.height + ContentPanelTransform.GetComponent<VerticalLayoutGroup>().spacing;

        for (int i = 0; i < ItemsToAdd; i++) CreateItem();
        ContentPanelTransform.localPosition = new Vector3(ContentPanelTransform.localPosition.x, -ItemsToAdd * itemHeight, ContentPanelTransform.localPosition.z);
    }

    public void Update()
    {
        float contentY = ContentPanelTransform.localPosition.y;

        if (contentY > 0) ScrollUp();

    }

    /*
        contentler initiaze edilir ve listenerlar verilir
    */
    private void CreateItem()
    {
        RectTransform rectTransform = GameObject.Instantiate(Item, ContentPanelTransform);
        rectTransform.SetAsLastSibling();
        Button[] buttons = rectTransform.gameObject.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => UIManager.CreateObject(ObjectType.POWERPLANT));
        buttons[1].onClick.AddListener(() => UIManager.CreateObject(ObjectType.BARRACK));
    }

    /*
        Yukari suruklenmesini saglayan metot
    */
    private void ScrollUp()
    {
        float minimumScrollAmount = -itemHeight;
        float newYPosition = Mathf.Max(ContentPanelTransform.localPosition.y - itemHeight, minimumScrollAmount);
        ContentPanelTransform.localPosition = new Vector3(ContentPanelTransform.localPosition.x, newYPosition, ContentPanelTransform.localPosition.z);
        Canvas.ForceUpdateCanvases();
    }

    /*
        eger ui elementi uzerine carpildiysa gonderen metot
    */
    public bool IsClickedOnUiElement() =>
        RectTransformUtility.RectangleContainsScreenPoint(ScrollViewPortTransform, Input.mousePosition);
}


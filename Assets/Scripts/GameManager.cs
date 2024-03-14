using UnityEngine;


/*
    Amac:
        - Tum oyun yonetimi GameManager uzerinden yurutulur.
        - Logic ve UI kisimlari arasinda kopru kurmaktadir.
*/
public class GameManager : MonoBehaviour
{
    [SerializeField] LogicManager _LogicManager;
    [SerializeField] UIManager _UIManager;

    private static GameManager _Instance;
    public static GameManager Instance { get => _Instance; }

    public void Awake()
    {
        _Instance = this;

        _LogicManager.Awake();
        _UIManager.Awake();
    }

    public void Update()
    {
        _UIManager.Update();
        _LogicManager.Update();

    }

    #region UI

    //ui elementine tiklama olup olmadigini kontrol eder
    public bool IsClickedOnUiElement() => _UIManager.IsClickedOnUiElement();

    //information panelinin acilmasini saglar
    public void ShowInfo(IUIDisplayable uIDisplayable) =>_UIManager.TurnInformationMenuOn(uIDisplayable);

    //information panelinin kapanmasini saglar
    public void ShutDownInfo() => _UIManager.TurnInformationMenuOff();


    #endregion

    #region Logic
    // verilen obje tipinden sprite almayi saglar
    public Sprite GetSprite(ObjectType objectType) => _LogicManager.GetSprite(objectType);
    //istenilen obje turunden obje yaratilmasini saglar
    public void CreateObject(ObjectType objectType) => _LogicManager.CreateObject(objectType);

    #endregion
}

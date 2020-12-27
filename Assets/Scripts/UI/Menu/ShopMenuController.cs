using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuController : MonoBehaviour
{
    [SerializeField] private int hpCost;
    [SerializeField] private TextMeshProUGUI textHp;
    [SerializeField] private Button buttonHp;

    [SerializeField] private int drCost;
    [SerializeField] private TextMeshProUGUI textDr;
    [SerializeField] private Button buttonDr;

    [SerializeField] private int x8Cost;
    [SerializeField] private TextMeshProUGUI textX8;
    [SerializeField] private Button buttonX8;

    [SerializeField] private int x16Cost;
    [SerializeField] private TextMeshProUGUI textX16;
    [SerializeField] private Button buttonX16;

    [SerializeField] private int x32Cost;
    [SerializeField] private TextMeshProUGUI textX32;
    [SerializeField] private Button buttonX32;

    [SerializeField] private Button buttonClose;

    private PlayerController _playerController;
    
    private void Awake()
    {
        GameObject[] gameObjects = gameObject.scene.GetRootGameObjects();
        _playerController = gameObjects[0].transform.GetChild(2).GetComponentInChildren<PlayerController>();
        
        textHp.SetText(hpCost.ToString());
        textDr.SetText(drCost.ToString());
        textX8.SetText(x8Cost.ToString());
        textX16.SetText(x16Cost.ToString());
        textX32.SetText(x32Cost.ToString());
        
        buttonHp.onClick.AddListener(OnBuyRegeneration);
        buttonDr.onClick.AddListener(OnBuyResistance);
        buttonX8.onClick.AddListener(OnBuyPineConeX8);
        buttonX16.onClick.AddListener(OnBuyPineConeX16);
        buttonX32.onClick.AddListener(OnBuyPineConeX32);
        
        buttonClose.onClick.AddListener(OnCloseShop);
    }

    private void OnDestroy()
    {
        buttonHp.onClick.RemoveListener(OnBuyRegeneration);
        buttonDr.onClick.RemoveListener(OnBuyResistance);
        buttonX8.onClick.RemoveListener(OnBuyPineConeX8);
        buttonX16.onClick.RemoveListener(OnBuyPineConeX16);
        buttonX32.onClick.RemoveListener(OnBuyPineConeX32);
        
        buttonClose.onClick.RemoveListener(OnCloseShop);
    }
    
    private void OnBuyRegeneration()
    {
        if(_playerController.CurrentCoins < hpCost) return;
        
        _playerController.OnChangeCoins(-hpCost);
        _playerController.OnChangeHealth(100);
    }
    
    private void OnBuyResistance()
    {
        if(_playerController.CurrentCoins < drCost) return;

        _playerController.OnChangeCoins(-drCost);
        _playerController.OnChangeHealth(100);
        _playerController.OnChangeShieldBoost();
    }

    private void OnBuyPineConeX8()
    {
        if(_playerController.CurrentCoins < x8Cost) return;
        
        _playerController.OnChangeCoins(-x8Cost);
        _playerController.OnChangePineCones(8);
    }

    private void OnBuyPineConeX16()
    {
        if(_playerController.CurrentCoins < x16Cost) return;
        
        _playerController.OnChangeCoins(-x16Cost);
        _playerController.OnChangePineCones(16);
    }

    private void OnBuyPineConeX32()
    {
        if(_playerController.CurrentCoins < x32Cost) return;
        
        _playerController.OnChangeCoins(-x32Cost);
        _playerController.OnChangePineCones(32);
    }
    
    private void OnCloseShop()
    {
        // Middle Panels
        transform.parent.parent.gameObject.SetActive(false);
    }
}

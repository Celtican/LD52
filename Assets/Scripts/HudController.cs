using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    
    public static HudController Instance { get; private set; }

    [SerializeField] private Image hull;
    [SerializeField] private Image hullBar;
    
    [SerializeField] private Image load;
    [SerializeField] private Image loadBar;
    [SerializeField] private Image loadBarProfit;
    
    public TMP_Text hullText;
    public TMP_Text loadText;

    public float barRevealTime = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    public void SetHullPercent(float percent)
    {
        hullBar.fillAmount = Mathf.Clamp01(percent);
        hullText.text = "Hull (" + (int)(percent * 100) + "%)";
    }

    public void SetLoadPercent(float percent, float startProfitPercent)
    {
        loadBar.fillAmount = Mathf.Clamp(percent, 0, startProfitPercent);
        loadBarProfit.fillAmount = Mathf.Clamp01(percent);
        if (ShopController.instance != null)
        {
            int profit = (int)ShopController.instance.CalculateMoney();
            loadText.text = profit <= 0 ? "Load" : "Load ($" + $"{profit:n0}" + ")";
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
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

    public float barRevealTime = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    public void SetHullPercent(float percent)
    {
        hullBar.fillAmount = Mathf.Clamp01(percent);
    }

    public void SetLoadPercent(float percent, float startProfitPercent)
    {
        loadBar.fillAmount = Mathf.Clamp(percent, 0, startProfitPercent);
        loadBarProfit.fillAmount = Mathf.Clamp01(percent);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopController : MonoBehaviour
{
    public static ShopController instance { get; private set; }

    public Spawner asteroidSpawner;
    public WarpGate warpGate;
    public TMP_Text tipText;
    public AudioController.Audio audioOnWarp;
    
    public PowerUpButton[] buttons;
    public TMP_Text moneyText;
    private List<PowerUp> powerUps;
    private PowerUp oopsPowerUp;
    
    private PlayerController player;
    private Hull playerHull;
    private Gun playerGun;
    private Collector playerCollector;
    private PointTowards playerCompass;
    private BeamController playerBeam;
    
    private float tax = 0.9f;
    private float crystalPrice = 1000f;
    private float upgradePriceModifier = 1f;
    private float money;
    private PirateSpawner pirateSpawner;

    public ShopController()
    {
        instance = this;
    }

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        playerHull = player.gameObject.GetComponent<Hull>();
        playerGun = player.gameObject.GetComponent<Gun>();
        playerCollector = player.gameObject.GetComponent<Collector>();
        playerCompass = player.gameObject.GetComponentInChildren<PointTowards>();
        playerBeam = player.gameObject.GetComponentInChildren<BeamController>(true);
        pirateSpawner = FindObjectOfType<PirateSpawner>();
        
        powerUps = new List<PowerUp>(new[]
        {
            new PowerUp("Overshield", 20000, true, "A mixture of rare metal casings and charged shielding.\nDoubles the strength of your hull.",
                () => playerHull.maxHull *= 2),
            new PowerUp("Automatic Bullet Chargers", 20000, true, "Utilizes AI Robotics to reload faster than previously possible.\nDoubles firing speed.",
                () => playerGun.timeBetweenShots /= 2),
            new PowerUp("Ether Crystal Transmuter", 20000, true, "Removes imperfections in crystals with unknown technology.\nIncreases money per crystal.",
                () => crystalPrice *= 1.5f),
            new PowerUp("Quantum Load", 20000, true, "Stabilizes quantum instabilities to create spaces bigger on the inside.\nDoubles your maximum load.",
                () => playerCollector.maxLoad *= 2),
            
            new PowerUp("Heavy-Metal Shielding", 10000, true, "An archaic and flimsy coating that barely does the job.\nIncreases hull strength, decreases shoot speed.",
                () => { playerHull.maxHull *= 1.5f; playerGun.timeBetweenShots *= 0.8f; }),
            new PowerUp("Carbon-Alloy Weaponry", 10000, true, "An archaic modification to weaponry that occasionally works as intended.\nIncreases shoot speed, decreases hull strength.",
                () => { playerGun.timeBetweenShots *= 0.8f; playerHull.maxHull *= 0.8f; }),
            new PowerUp("Empathic Communicator", 15000, true, "An archaic personal augmentation that enhancing one's bartering skills.\nDecreases future prices of upgrades.",
                () => upgradePriceModifier *= 0.85f),
            new PowerUp("Load Reorganizer", 10000, true, "An archaic practice that removes useful parts of a ship for other useful parts.\nIncreases maximum load, decreases hull strength.",
                () => { playerCollector.maxLoad *= 1.5f; playerHull.maxHull *= 0.8f; }),
            
            new PowerUp("FTL Overdrive", 60000, false, "Alien technology designed for intergalactic affairs.\nRemoves your maximum speed.",
                () => player.velocityMax = 100),
            new PowerUp("VIP License", 30000, false, "Temporary proof that you are very very important.\nRemoves the 10% Federations tax.",
                () => tax = 1),
            new PowerUp("Bullet Synthesizer", 30000, false, "Generates bullets with the flux of space.\nRemoves bullet energy cost.",
                () => playerGun.energyTax = 0),
            new PowerUp("Ether Projector", 20000, false, "A large-scale Ether Crystal device that aids in harvesting more crystals.\nDoubles the size of the harvester beam.",
            () => playerBeam.transform.localScale *= 2),
        });
        oopsPowerUp = new PowerUp("On-Sale Ticket", 499, true, "The only thing you can afford is this lowly ticket to another sector.",
            () => {});
    }

    private void OnEnable()
    {
        player.enabled = false;
        
        SetMoney(Mathf.Max(CalculateMoney(), 499));
        
        foreach (PowerUpButton powerUpButton in buttons)
        {
            RandomizeButton(powerUpButton);
        }

        if (!CanPurchaseAnyPowerUp())
        {
            SetButtonPowerUp(buttons[1], oopsPowerUp);
        }
        
        RefreshButtons();
        
        DestroyOnWarp[] stuffToDestroy = FindObjectsOfType<DestroyOnWarp>();
        foreach (DestroyOnWarp destroyMe in stuffToDestroy)
        {
            Destroy(destroyMe.gameObject);
        }
        
        asteroidSpawner.gameObject.SetActive(false);

        switch (Random.Range(0, 4))
        {
            case 0: tipText.text = "Tip: Pirates arrive more frequently as you remain in a sector."; break;
            case 1: tipText.text = "Tip: Every time you warp, pirates arrive in greater groups."; break;
            case 2: tipText.text = "Tip: Your hull is fully repaired every time you warp. (for free!)"; break;
            case 3: tipText.text = "Tip: Every action consumes energy. Be careful with how long you hold buttons."; break;
        }

        MenuController.instance.canPause = false;
        
        AudioController.instance.PlaySound(audioOnWarp);
        
        player.accelerateAudioSource.Stop();
    }

    private void RandomizeButton(PowerUpButton powerUpButton)
    {
        PowerUp powerUp;
        do
        {
            int powerUpIndex = Random.Range(0, powerUps.Count);
            powerUp = powerUps[powerUpIndex];

            foreach (PowerUpButton button in buttons)
            {
                if (button.powerUp == powerUp)
                {
                    powerUp = null;
                    break;
                }
            }
        } while (powerUp == null);

        SetButtonPowerUp(powerUpButton, powerUp);
    }

    private void SetButtonPowerUp(PowerUpButton powerUpButton, PowerUp powerUp)
    {
        int cost = (int)(powerUp.cost * upgradePriceModifier);
        powerUpButton.purchased = false;
        powerUpButton.powerUp = powerUp;
        powerUpButton.nameText.text = powerUp.name;
        powerUpButton.descriptionText.text = powerUp.description;
        powerUpButton.priceText.text = '$' + $"{cost:n0}";
        powerUpButton.button.onClick.RemoveAllListeners();
        powerUpButton.button.onClick.AddListener(() =>
        {
            if (cost > money) return;
            SetMoney(money - cost);
            if (!powerUp.isRepeatable) powerUps.Remove(powerUp);
            powerUp.onPurchase();
            powerUpButton.purchased = true;
            powerUpButton.descriptionText.text = "Purchased!";
            RefreshButtons();
            if (!CanPurchaseAnyPowerUp())
            {
                Hide();
            }
        });
    }

    public float CalculateMoney()
    {
        return playerCollector.GetExcess() * crystalPrice * tax;
    }

    private void SetMoney(float amount)
    {
        money = amount;
        moneyText.text = '$' + $"{money:n0}";
    }

    private void RefreshButtons()
    {
        foreach (PowerUpButton powerUpButton in buttons)
        {
            powerUpButton.button.interactable = (powerUpButton.powerUp.cost * upgradePriceModifier) <= money && !powerUpButton.purchased;
        }
    }

    private bool CanPurchaseAnyPowerUp()
    {
        return buttons.Any(powerUpButton => !powerUpButton.purchased && (powerUpButton.powerUp.cost * upgradePriceModifier) <= money);
    }

    public void Hide()
    {
        player.enabled = true;
        gameObject.SetActive(false);
        playerHull.Heal();
        playerCollector.Reset();
        Time.timeScale = 1;
        pirateSpawner.Reset();
        asteroidSpawner.gameObject.SetActive(true);
        player.ResetPhysics();
        playerCompass.pointTowards = warpGate.gameObject;
        MenuController.instance.canPause = true;
        AudioController.instance.PlaySound(audioOnWarp);
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    [Serializable]
    public class PowerUpButton
    {
        public Button button;
        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public TMP_Text priceText;
        public bool purchased = false;
        
        [HideInInspector] public PowerUp powerUp;
    }

    public class PowerUp
    {
        public string name;
        public int cost;
        public bool isRepeatable;
        public string description;
        public UnityAction onPurchase;

        public PowerUp(string name, int cost, bool isRepeatable, string description, UnityAction onPurchase)
        {
            this.name = name;
            this.cost = cost;
            this.isRepeatable = isRepeatable;
            this.description = description;
            this.onPurchase = onPurchase;
        }
    }
}
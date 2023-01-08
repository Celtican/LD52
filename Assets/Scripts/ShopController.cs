using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
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
    public AudioController.Audio audioOnClick;
    public Animator animator;

    public new CinemachineVirtualCamera camera;
    public SpriteRenderer playerShipSpriteRenderer;
    public Sprite coolPlayerShip;
    
    public PowerUpButton[] buttons;
    public TMP_Text moneyText;
    private List<PowerUp> powerUps;
    private PowerUp oopsPowerUp;
    private PowerUp ftlPowerUp;
    
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
    private bool purchasedFTL = false;

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
            new PowerUp("Mk. II Radar", 10000, true, "An extended radar capable of collecting significantly more intelligence.\nIncreases your viewing distance.",
                () => camera.m_Lens.OrthographicSize += 2),
            
            new PowerUp("VIP License", 30000, false, "Temporary proof that you are very very important.\nRemoves the 10% Federations tax.",
                () => tax = 1),
            new PowerUp("Bullet Synthesizer", 30000, false, "Generates bullets with the flux of space.\nRemoves bullet energy cost.",
                () => playerGun.energyTax = 0),
            new PowerUp("Ether Projector", 20000, false, "A large-scale Ether Crystal device that aids in harvesting more crystals.\nDoubles the size of the harvester beam.",
            () => playerBeam.transform.localScale *= 2),
            new PowerUp("Premium Cosmetics", 40000, false, "Cloaking technology allows your ship to appear, frankly, awesome.\nMakes your ship look rad.",
                () => playerShipSpriteRenderer.sprite = coolPlayerShip),
            new PowerUp("Asteroid Disengagers", 15000, false, "An added layer to your shield neutralizes the threat of asteroid collisions.\nPrevent damage from asteroids.",
                () => playerHull.damageTakenFromAsteroid = 0),
            new PowerUp("Auto Scavengers", 20000, false, "AI-enhanced drones deploy into the wreckage of other ships to retrieve lost Ether Crystals.\nGain crystals when destroying pirates.",
                () => Hull.loadOnDestroy = 5),
            new PowerUp("Beam Amplifier", 15000, false, "By directing more power towards the harvester beam, its efficiency is greatly increased.\nIncreases the harvester beam's strength.",
                () => playerBeam.absorbSpeed *= 2f),
        });
        oopsPowerUp = new PowerUp("On-Sale Ticket", 499, true, "The only thing you can afford is this lowly ticket to another sector.",
            () => {});
        ftlPowerUp = new PowerUp("FTL OVERDRIVER", 100000, false, "The most prestigious contraption in history, the Holy Grail of the Space Age.\nREMOVES YOUR MAXIMUM SPEED.",
            () =>
            {
                player.velocityMax = 100;
                purchasedFTL = true;
            });
    }

    private void OnEnable()
    {
        animator.SetBool("ShopOpen", true);
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
        else if (money > 50000 && !purchasedFTL)
        {
            SetButtonPowerUp(buttons[0], ftlPowerUp);
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
        playerBeam.gameObject.SetActive(false);
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
            AudioController.instance.PlaySound(audioOnClick);
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
        // hell yeah this is inefficient. How much time do I have left? 5 hours. And I still need to do visual polish! Why am I writing this comment? GO, GO, GO!
        if (playerCollector == null)
        {
            player = FindObjectOfType<PlayerController>();
            playerCollector = player.GetComponent<Collector>();
        }
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
        animator.SetBool("ShopOpen", false);
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
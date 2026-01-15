using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Меню Паузи")]
    public GameObject pauseMenuPanel; 
    public Button resumeButton;     
    public Button restartButton;    
    public Button quitButton;     

    [Header("Елементи Інтерфейсу")]
    public Slider xpSlider;
    public TextMeshProUGUI levelText;
    public Slider healthBar;
    public Slider staminaBar;
    public TextMeshProUGUI potionCountText;
    public TextMeshProUGUI stateDebug;
        
    [Header("Boss")]
    public Slider bossHPbar;

    [Header("Run Info UI")]
    public TextMeshProUGUI timerText;  
    public TextMeshProUGUI floorText;  

    public void UpdateTimerUI(float time)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time % 60F);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void UpdateFloorText(int current, int max)
    {
        if (floorText != null)
        {
            if (LocalizationManager.Instance != null)
            {
                string format = LocalizationManager.Instance.GetTranslation("hud_floor");
                floorText.text = string.Format(format, current, max);
            }
            else
            {
                floorText.text = $"FLOOR {current} / {max}";
            }
        }
    }
    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject); 
        }
        Instance = this;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }
}
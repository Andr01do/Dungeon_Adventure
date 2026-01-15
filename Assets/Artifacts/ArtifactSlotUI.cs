using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems; // <--- 1. ДОДАЙ ЦЕ
public class ArtifactSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    public Image iconImage;       
    public Image borderImage;     
    public GameObject lockIcon;   
    public Button button;

    private ArtifactSO myArtifact;
    private bool isUnlocked;
    private UnityAction<ArtifactSO> onSlotClicked;

    public void Setup(ArtifactSO artifact, UnityAction<ArtifactSO> clickCallback, bool unlocked)
    {
        myArtifact = artifact;
        onSlotClicked = clickCallback;
        isUnlocked = unlocked;

        if (myArtifact != null)
        {
            iconImage.sprite = myArtifact.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }

        if (isUnlocked)
        {
            iconImage.color = Color.white;
            if (lockIcon != null) lockIcon.SetActive(false);
            button.interactable = true;
        }
        else
        {
            iconImage.color = Color.black;
            if (lockIcon != null) lockIcon.SetActive(true);
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onSlotClicked.Invoke(myArtifact));
    }

    public void UpdateSelectionVisual(bool isSelected)
    {
        if (borderImage != null)
        {
            if (!isUnlocked)
            {
                borderImage.color = new Color(0, 0, 0, 0); 
                return;
            }

            borderImage.color = isSelected ? new Color(0.2f, 0.4f, 0.1f) : new Color(1, 1, 1, 0f);
        }
    }

    public void SetupForDisplay(ArtifactSO artifact)
    {
        myArtifact = artifact;
        isUnlocked = true; 

        if (myArtifact != null)
        {
            iconImage.sprite = myArtifact.icon;
            iconImage.enabled = true;
        }

        if (lockIcon != null) lockIcon.SetActive(false);
        if (borderImage != null) borderImage.color = new Color(0, 0, 0, 0); 

        if (button != null)
        {
            button.interactable = false;
        }
        iconImage.color = Color.white;
    }
    // <--- 3. РЕАЛІЗАЦІЯ НАВЕДЕННЯ МИШКИ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (myArtifact != null)
        {
            if (!isUnlocked)
            {
                // "msg_locked" -> це ключ (буде "Заблоковано" або "Locked")
                // myArtifact.unlockConditionDescription -> це просто текст ("Вбийте 5 ворогів")
                // false -> означає "НЕ перекладай опис"
                TooltipSystem.Instance.Show("msg_locked", myArtifact.unlockConditionDescription, false);
            }
            else
            {
                // Тут true (за замовчуванням), бо назва і опис артефакту — це ключі
                TooltipSystem.Instance.Show(myArtifact.artifactName, myArtifact.description, true);
            }
        }
    }

    // <--- 4. КОЛИ МИШКА ПРИБИРАЄТЬСЯ
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide();
    }
}
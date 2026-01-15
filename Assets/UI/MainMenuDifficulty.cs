using UnityEngine;
using UnityEngine.UI;

public class MainMenuDifficulty : MonoBehaviour
{
    [Header(" нопки (ѕерет€гни сюди Button component)")]
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    [Header("≤конки замк≥в (ќпц≥онально)")]
    public GameObject mediumLockObj;
    public GameObject hardLockObj;  

    private void Start()
    {
        UpdateButtonsState();
    }

    private void OnEnable()
    {
        UpdateButtonsState();
    }

    void UpdateButtonsState()
    {
        bool isMediumUnlocked = GlobalProgressionManager.Instance.saveData.mediumDifficultyUnlocked;
        bool isHardUnlocked = GlobalProgressionManager.Instance.saveData.hardDifficultyUnlocked;
        if (easyButton) easyButton.interactable = true;
        if (mediumButton) mediumButton.interactable = isMediumUnlocked;
        if (mediumLockObj) mediumLockObj.SetActive(!isMediumUnlocked);
        if (hardButton) hardButton.interactable = isHardUnlocked;
        if (hardLockObj) hardLockObj.SetActive(!isHardUnlocked);
    }


    public void SetEasy()
    {
        SelectDifficulty(0);
        Debug.Log("ћеню: ќбрано легку складн≥сть");
    }

    public void SetMedium()
    {
        if (!GlobalProgressionManager.Instance.saveData.mediumDifficultyUnlocked) return;

        SelectDifficulty(1);
        Debug.Log("ћеню: ќбрано середню складн≥сть");
    }

    public void SetHard()
    {
        if (!GlobalProgressionManager.Instance.saveData.hardDifficultyUnlocked) return;

        SelectDifficulty(2);
        Debug.Log("ћеню: ќбрано важку складн≥сть");
    }

    private void SelectDifficulty(int difficultyIndex)
    {
        PlayerPrefs.SetInt("SelectedDifficulty", difficultyIndex);
        PlayerPrefs.Save();
    }
}
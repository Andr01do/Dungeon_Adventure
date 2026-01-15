using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject confirmationPopup;

    [Header("Buttons")]
    public Button resetProgressButton;
    public Button confirmYesButton;
    public Button confirmNoButton;

    private void Start()
    {
        if (resetProgressButton != null)
            resetProgressButton.onClick.AddListener(OnResetParamsClicked);

        if (confirmYesButton != null)
            confirmYesButton.onClick.AddListener(OnConfirmResetClicked);

        if (confirmNoButton != null)
            confirmNoButton.onClick.AddListener(OnCancelResetClicked);
        if (confirmationPopup != null) confirmationPopup.SetActive(false);
    }

    public void OnResetParamsClicked()
    {
        confirmationPopup.SetActive(true);
    }

    public void OnConfirmResetClicked()
    {
        if (GlobalProgressionManager.Instance != null)
        {
            GlobalProgressionManager.Instance.ResetGlobalProgress();
        }
        confirmationPopup.SetActive(false);
    }

    public void OnCancelResetClicked()
    {
        confirmationPopup.SetActive(false);
    }
    private void OnDestroy()
    {
        if (resetProgressButton != null) resetProgressButton.onClick.RemoveListener(OnResetParamsClicked);
        if (confirmYesButton != null) confirmYesButton.onClick.RemoveListener(OnConfirmResetClicked);
        if (confirmNoButton != null) confirmNoButton.onClick.RemoveListener(OnCancelResetClicked);
    }
}
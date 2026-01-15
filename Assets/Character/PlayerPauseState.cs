using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPauseState : IPlayerState
{
    private PlayerController _player;
    private HUDManager _hud;

    public PlayerPauseState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("PAUSE STATE: Entered");

        Time.timeScale = 0f;

        _hud = HUDManager.Instance;
        if (_hud != null && _hud.pauseMenuPanel != null)
        {
            _hud.pauseMenuPanel.SetActive(true);
            if (_hud.resumeButton) _hud.resumeButton.onClick.AddListener(OnResume);
            if (_hud.restartButton) _hud.restartButton.onClick.AddListener(OnRestart);
            if (_hud.quitButton) _hud.quitButton.onClick.AddListener(OnQuit);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        if (_hud != null)
        {
            if (_hud.pauseMenuPanel != null) _hud.pauseMenuPanel.SetActive(false);

            if (_hud.resumeButton) _hud.resumeButton.onClick.RemoveListener(OnResume);
            if (_hud.restartButton) _hud.restartButton.onClick.RemoveListener(OnRestart);
            if (_hud.quitButton) _hud.quitButton.onClick.RemoveListener(OnQuit);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnResume();
        }
    }

    public void Update() { }
    public void FixedUpdate() { }

    private void OnResume()
    {
        _player.ChangeState(_player.idleState);
    }

    private void OnRestart()
    {
        Debug.Log("PAUSE: Перезапуск забігу...");
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewRun();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnQuit()
    {
        Debug.Log("Вихід з гри...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}
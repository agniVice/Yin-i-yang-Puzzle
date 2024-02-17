using System.Collections;
using UnityEngine;

public class PauseWindow : MonoBehaviour, IInitializable, ISubscriber
{
    [SerializeField] private GameObject _panel;

    private bool _isInitialized;

    private void OnEnable()
    {
        if (!_isInitialized)
            return;

        SubscribeAll();
    }
    private void OnDisable()
    {
        UnsubscribeAll();
    }
    public void Initialize()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        Hide();

        _isInitialized = true;
    }
    public void SubscribeAll()
    {
        GameState.Instance.GamePaused += Show;
        GameState.Instance.GameUnpaused += Hide;
        GameState.Instance.GameFinished += Hide;
    }
    public void UnsubscribeAll()
    {
        GameState.Instance.GamePaused -= Show;
        GameState.Instance.GameUnpaused -= Hide;
        GameState.Instance.GameFinished -= Hide;
    }
    private void Show()
    {
        _panel.SetActive(true);
    }
    private void Hide()
    {
        _panel.SetActive(false);
    }
    public void OnContinueButtonClicked()
    { 
        GameState.Instance.UnpauseGame();
    }
    public void OnRestartButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Gameplay");
    }
    public void OnMenuButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Menu");
    }
}
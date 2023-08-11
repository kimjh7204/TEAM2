using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject PlayPanel;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject PauseMenu;

    [SerializeField] private FadeUI fadePanel;

    [SerializeField] private AudioMixer audioMixer;

    public FadeUI.FadeState FadeState
    {
        get
        {
            return fadePanel.CurFadeState;
        }
    }

    private DefaultInputActions inputAction;
    private int popUpCounter = 0;

    private void Awake()
    {
        if (Instance is null) Instance = this;
        else if (!Instance.Equals(this)) Destroy(gameObject);

        eventSystem.SetActive(true);
        DontDestroyOnLoad(gameObject);
        
        // add listener
        GameManager.Instance.onMainSceneLoaded.AddListener(MainSceneSetup);
        GameManager.Instance.onPlaySceneLoaded.AddListener(PlaySceneSetup);
        //
        //
        fadePanel.gameObject.SetActive(true);
        fadePanel.Init();
    }

    private void Start()
    {
        inputAction = new DefaultInputActions();
        inputAction.UI.Enable();

        inputAction.UI.Cancel.started += OnEscapeTrigger;
    }

    private void OnEscapeTrigger(InputAction.CallbackContext context)
    {
        if (popUpCounter <= 0 && PlayPanel.activeSelf
                              && GameManager.Instance.gameState.Equals(GameState.Running))
        {
            popUpCounter = 1;
            PauseMenu.SetActive(true);
            GameManager.Instance.PauseGame(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OnPopupUiDisable()
    {
        popUpCounter--;
        if (popUpCounter <= 0 && PlayPanel.activeSelf)
        {
            popUpCounter = 0;
            GameManager.Instance.PauseGame(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void MainSceneSetup()
    {
        fadePanel.StartFadeIn();

        PlayPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        MainPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
    }
    private void PlaySceneSetup()
    {        
        fadePanel.StartFadeIn();

        MainPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        PlayPanel.SetActive(true);
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void GameOverSetup()
    {
        MainPanel.SetActive(false);
        PlayPanel.SetActive(false);
        GameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
    }


    public void Attempt2LoadNextScene()
    {
        if (GameManager.Instance.LoadNextScene()) print("next scene load complete");
        else print("next scene load failed");
    }

    public void Attempt2LoadMainScene()
    {
        if (GameManager.Instance.LoadScene(SceneType.Main)) print("main scene load complete");
        else print("main scene load failed");
    }

    public void Attempt2QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public void FadeOut() => fadePanel.StartFadeOut();
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    public void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BgmVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BgmVolume", volume);
    }
    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("EffectVolume", volume);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIController : MonoBehaviour
{
    [Header("Menu References")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject modeSelectionPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    
    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Mode Selection Buttons")]
    [SerializeField] private Button freePlayButton;
    [SerializeField] private Button missionModeButton;
    [SerializeField] private Button recipeBookButton;
    [SerializeField] private Button parentChildButton;
    [SerializeField] private Button backFromModeButton;
    
    [Header("Settings Controls")]
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;
    [SerializeField] private Toggle highContrastToggle;
    [SerializeField] private Toggle touchSensitivityToggle;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Button backFromSettingsButton;
    
    [Header("Credits Controls")]
    [SerializeField] private Button backFromCreditsButton;
    
    [Header("Main Character Reference")]
    [SerializeField] private GameObject mainCharacter;
    [SerializeField] private Animator characterAnimator;
    
    // External managers
    private AudioManager audioManager;
    private GameManager gameManager;
    
    public void Initialize()
    {
        // Find managers if not set
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }
        
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        
        // Register button events
        RegisterButtonEvents();
        
        // Set initial panel state
        ShowMainMenuPanel();
        
        // Initialize settings
        InitializeSettings();
        
        // Play character idle animation
        if (characterAnimator != null)
        {
            characterAnimator.Play("Idle");
        }
    }
    
    private void RegisterButtonEvents()
    {
        // Main menu buttons
        if (playButton != null) playButton.onClick.AddListener(OnPlayButtonClicked);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        if (creditsButton != null) creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitButtonClicked);
        
        // Mode selection buttons
        if (freePlayButton != null) freePlayButton.onClick.AddListener(() => OnModeSelected(GameMode.FreePlay));
        if (missionModeButton != null) missionModeButton.onClick.AddListener(() => OnModeSelected(GameMode.Mission));
        if (recipeBookButton != null) recipeBookButton.onClick.AddListener(() => OnModeSelected(GameMode.Recipe));
        if (parentChildButton != null) parentChildButton.onClick.AddListener(() => OnModeSelected(GameMode.ParentChild));
        if (backFromModeButton != null) backFromModeButton.onClick.AddListener(ShowMainMenuPanel);
        
        // Settings buttons
        if (backFromSettingsButton != null) backFromSettingsButton.onClick.AddListener(ShowMainMenuPanel);
        if (bgmVolumeSlider != null) bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        if (voiceVolumeSlider != null) voiceVolumeSlider.onValueChanged.AddListener(OnVoiceVolumeChanged);
        if (highContrastToggle != null) highContrastToggle.onValueChanged.AddListener(OnHighContrastToggled);
        if (touchSensitivityToggle != null) touchSensitivityToggle.onValueChanged.AddListener(OnTouchSensitivityToggled);
        if (languageDropdown != null) languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        
        // Credits button
        if (backFromCreditsButton != null) backFromCreditsButton.onClick.AddListener(ShowMainMenuPanel);
    }
    
    private void InitializeSettings()
    {
        if (bgmVolumeSlider != null) bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        if (voiceVolumeSlider != null) voiceVolumeSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 1.0f);
        if (highContrastToggle != null) highContrastToggle.isOn = PlayerPrefs.GetInt("HighContrast", 0) == 1;
        if (touchSensitivityToggle != null) touchSensitivityToggle.isOn = PlayerPrefs.GetInt("TouchSensitivity", 0) == 1;
        
        // Initialize language dropdown
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            
            List<string> options = new List<string>();
            options.Add("日本語");
            options.Add("English");
            
            languageDropdown.AddOptions(options);
            
            string currentLanguage = PlayerPrefs.GetString("Language", "ja");
            int languageIndex = currentLanguage == "ja" ? 0 : 1;
            languageDropdown.value = languageIndex;
        }
    }
    
    private void ShowMainMenuPanel()
    {
        mainMenuPanel.SetActive(true);
        modeSelectionPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        
        // Play character idle animation
        if (characterAnimator != null)
        {
            characterAnimator.Play("Idle");
        }
    }
    
    private void ShowModeSelectionPanel()
    {
        mainMenuPanel.SetActive(false);
        modeSelectionPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        
        // Play character excited animation
        if (characterAnimator != null)
        {
            characterAnimator.Play("Excited");
        }
    }
    
    private void ShowSettingsPanel()
    {
        mainMenuPanel.SetActive(false);
        modeSelectionPanel.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
        
        // Play character thinking animation
        if (characterAnimator != null)
        {
            characterAnimator.Play("Thinking");
        }
    }
    
    private void ShowCreditsPanel()
    {
        mainMenuPanel.SetActive(false);
        modeSelectionPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
        
        // Play character happy animation
        if (characterAnimator != null)
        {
            characterAnimator.Play("Happy");
        }
    }
    
    private void OnPlayButtonClicked()
    {
        PlayButtonClickSound();
        ShowModeSelectionPanel();
    }
    
    private void OnSettingsButtonClicked()
    {
        PlayButtonClickSound();
        ShowSettingsPanel();
    }
    
    private void OnCreditsButtonClicked()
    {
        PlayButtonClickSound();
        ShowCreditsPanel();
    }
    
    private void OnQuitButtonClicked()
    {
        PlayButtonClickSound();
        
        if (gameManager != null)
        {
            gameManager.ExitGame();
        }
        else
        {
            Debug.LogError("GameManager not found for quit action");
            
            // Fallback quit method
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
    
    private void OnModeSelected(GameMode mode)
    {
        PlayButtonClickSound();
        
        if (gameManager != null)
        {
            gameManager.StartGame(mode);
        }
        else
        {
            Debug.LogError("GameManager not found for mode selection: " + mode);
        }
    }
    
    private void OnBGMVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetBGMVolume(value);
        }
        
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetSFXVolume(value);
            audioManager.PlaySFX("ui_adjust"); // Play sample sound
        }
        
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
    
    private void OnVoiceVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetVoiceVolume(value);
        }
        
        PlayerPrefs.SetFloat("VoiceVolume", value);
        PlayerPrefs.Save();
    }
    
    private void OnHighContrastToggled(bool isOn)
    {
        PlayerPrefs.SetInt("HighContrast", isOn ? 1 : 0);
        PlayerPrefs.Save();
        
        // Apply high contrast setting
        // This would be implemented based on specific requirements
    }
    
    private void OnTouchSensitivityToggled(bool isOn)
    {
        PlayerPrefs.SetInt("TouchSensitivity", isOn ? 1 : 0);
        PlayerPrefs.Save();
        
        // Apply touch sensitivity setting
        // This would be implemented based on specific requirements
    }
    
    private void OnLanguageChanged(int index)
    {
        string language = index == 0 ? "ja" : "en";
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.Save();
        
        // Apply language change
        // This would typically update all UI text through a localization system
    }
    
    private void PlayButtonClickSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX("button_click");
        }
    }
}

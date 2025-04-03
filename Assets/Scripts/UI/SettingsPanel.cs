using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 設定パネルクラス
public class SettingsPanel : MenuPanel
{
    [Header("Audio Settings")]
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;
    
    [Header("Accessibility Settings")]
    [SerializeField] private Toggle highContrastToggle;
    [SerializeField] private Toggle touchSensitivityToggle;
    
    [Header("Language Settings")]
    [SerializeField] private TMP_Dropdown languageDropdown;
    
    // コールバック参照
    private System.Action onBackToMainMenu;
    
    // Audio Manager参照
    private AudioManager audioManager;
    
    public override void Initialize()
    {
        base.Initialize();
        
        // Audio Manager取得
        audioManager = FindObjectOfType<AudioManager>();
        
        // 設定値の初期化
        LoadSettings();
        
        // イベント登録
        RegisterEvents();
        
        // 戻るボタンのコールバック設定
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => {
                PlayButtonSound();
                onBackToMainMenu?.Invoke();
            });
        }
    }
    
    // コールバック設定
    public void SetCallbacks(System.Action backCallback)
    {
        onBackToMainMenu = backCallback;
    }
    
    // 設定読み込み
    private void LoadSettings()
    {
        // BGM音量
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
        }
        
        // SE音量
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        }
        
        // 音声音量
        if (voiceVolumeSlider != null)
        {
            voiceVolumeSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 1.0f);
        }
        
        // ハイコントラストモード
        if (highContrastToggle != null)
        {
            highContrastToggle.isOn = PlayerPrefs.GetInt("HighContrast", 0) == 1;
        }
        
        // タッチ感度調整
        if (touchSensitivityToggle != null)
        {
            touchSensitivityToggle.isOn = PlayerPrefs.GetInt("TouchSensitivity", 0) == 1;
        }
        
        // 言語設定
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
    
    // イベント登録
    private void RegisterEvents()
    {
        // BGM音量スライダー
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.RemoveAllListeners();
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }
        
        // SE音量スライダー
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        // 音声音量スライダー
        if (voiceVolumeSlider != null)
        {
            voiceVolumeSlider.onValueChanged.RemoveAllListeners();
            voiceVolumeSlider.onValueChanged.AddListener(OnVoiceVolumeChanged);
        }
        
        // ハイコントラストトグル
        if (highContrastToggle != null)
        {
            highContrastToggle.onValueChanged.RemoveAllListeners();
            highContrastToggle.onValueChanged.AddListener(OnHighContrastToggled);
        }
        
        // タッチ感度トグル
        if (touchSensitivityToggle != null)
        {
            touchSensitivityToggle.onValueChanged.RemoveAllListeners();
            touchSensitivityToggle.onValueChanged.AddListener(OnTouchSensitivityToggled);
        }
        
        // 言語ドロップダウン
        if (languageDropdown != null)
        {
            languageDropdown.onValueChanged.RemoveAllListeners();
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
    }
    
    // BGM音量変更時
    private void OnBGMVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetBGMVolume(value);
        }
        
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }
    
    // SE音量変更時
    private void OnSFXVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetSFXVolume(value);
            audioManager.PlaySFX("ui_adjust"); // サンプル音再生
        }
        
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
    
    // 音声音量変更時
    private void OnVoiceVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetVoiceVolume(value);
        }
        
        PlayerPrefs.SetFloat("VoiceVolume", value);
        PlayerPrefs.Save();
    }
    
    // ハイコントラストモード切替時
    private void OnHighContrastToggled(bool isOn)
    {
        PlayerPrefs.SetInt("HighContrast", isOn ? 1 : 0);
        PlayerPrefs.Save();
        
        // ハイコントラストモードの適用（実装方法に応じて）
        ApplyHighContrastMode(isOn);
    }
    
    // タッチ感度切替時
    private void OnTouchSensitivityToggled(bool isOn)
    {
        PlayerPrefs.SetInt("TouchSensitivity", isOn ? 1 : 0);
        PlayerPrefs.Save();
        
        // タッチ感度の適用（実装方法に応じて）
        ApplyTouchSensitivity(isOn);
    }
    
    // 言語変更時
    private void OnLanguageChanged(int index)
    {
        string language = index == 0 ? "ja" : "en";
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.Save();
        
        // 言語変更の適用（実装方法に応じて）
        ApplyLanguageChange(language);
    }
    
    // ハイコントラストモードの適用
    private void ApplyHighContrastMode(bool isEnabled)
    {
        // ローカライゼーションシステムなどで対応
        Debug.Log("ハイコントラストモード: " + (isEnabled ? "有効" : "無効"));
    }
    
    // タッチ感度の適用
    private void ApplyTouchSensitivity(bool isEnabled)
    {
        // 入力システムなどで対応
        Debug.Log("タッチ感度調整: " + (isEnabled ? "有効" : "無効"));
    }
    
    // 言語変更の適用
    private void ApplyLanguageChange(string language)
    {
        // ローカライゼーションシステムで対応
        Debug.Log("言語変更: " + language);
    }
    
    // ボタン効果音再生
    private void PlayButtonSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX("button_click");
        }
    }
}
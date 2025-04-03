using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// メインメニューパネルクラス
public class MainMenuPanel : MenuPanel
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Animations")]
    [SerializeField] private GameObject characterObject;
    [SerializeField] private Animator characterAnimator;
    
    // コールバック参照
    private System.Action onPlayButtonPressed;
    private System.Action onSettingsButtonPressed;
    private System.Action onCreditsButtonPressed;
    private System.Action onQuitButtonPressed;
    
    public override void Initialize()
    {
        base.Initialize();
        
        // 各ボタンのイベント登録
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() => {
                PlayButtonSound();
                onPlayButtonPressed?.Invoke();
            });
        }
        
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() => {
                PlayButtonSound();
                onSettingsButtonPressed?.Invoke();
            });
        }
        
        if (creditsButton != null)
        {
            creditsButton.onClick.RemoveAllListeners();
            creditsButton.onClick.AddListener(() => {
                PlayButtonSound();
                onCreditsButtonPressed?.Invoke();
            });
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => {
                PlayButtonSound();
                onQuitButtonPressed?.Invoke();
            });
        }
        
        // キャラクターアニメーション開始
        PlayCharacterIdleAnimation();
    }
    
    // コールバック設定メソッド
    public void SetCallbacks(
        System.Action playCallback, 
        System.Action settingsCallback, 
        System.Action creditsCallback, 
        System.Action quitCallback)
    {
        onPlayButtonPressed = playCallback;
        onSettingsButtonPressed = settingsCallback;
        onCreditsButtonPressed = creditsCallback;
        onQuitButtonPressed = quitCallback;
    }
    
    // キャラクターのアイドルアニメーション再生
    private void PlayCharacterIdleAnimation()
    {
        if (characterAnimator != null)
        {
            characterAnimator.Play("Idle");
        }
    }
    
    // ボタン効果音再生
    private void PlayButtonSound()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySFX("button_click");
        }
    }
}
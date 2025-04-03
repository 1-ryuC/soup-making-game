using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// モード選択パネルクラス
public class ModeSelectionPanel : MenuPanel
{
    [Header("Mode Buttons")]
    [SerializeField] private Button freePlayButton;
    [SerializeField] private Button missionModeButton;
    [SerializeField] private Button recipeBookButton;
    [SerializeField] private Button parentChildButton;
    
    [Header("Mode Icons")]
    [SerializeField] private Image freePlayIcon;
    [SerializeField] private Image missionModeIcon;
    [SerializeField] private Image recipeBookIcon;
    [SerializeField] private Image parentChildIcon;
    
    [Header("Mode Descriptions")]
    [SerializeField] private TextMeshProUGUI modeDescriptionText;
    [SerializeField] private GameObject descriptionPanel;
    
    // コールバック参照
    private System.Action<GameMode> onModeSelected;
    private System.Action onBackToMainMenu;
    
    // モード説明テキスト
    private Dictionary<GameMode, string> modeDescriptions = new Dictionary<GameMode, string>();
    
    public override void Initialize()
    {
        base.Initialize();
        
        // 戻るボタンの動作設定
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => {
                PlayButtonSound();
                onBackToMainMenu?.Invoke();
            });
        }
        
        // 各モードボタンのイベント登録
        if (freePlayButton != null)
        {
            freePlayButton.onClick.RemoveAllListeners();
            freePlayButton.onClick.AddListener(() => {
                PlayButtonSound();
                onModeSelected?.Invoke(GameMode.FreePlay);
            });
            
            // マウスオーバー時の説明表示
            AddHoverEvents(freePlayButton, GameMode.FreePlay);
        }
        
        if (missionModeButton != null)
        {
            missionModeButton.onClick.RemoveAllListeners();
            missionModeButton.onClick.AddListener(() => {
                PlayButtonSound();
                onModeSelected?.Invoke(GameMode.Mission);
            });
            
            // マウスオーバー時の説明表示
            AddHoverEvents(missionModeButton, GameMode.Mission);
        }
        
        if (recipeBookButton != null)
        {
            recipeBookButton.onClick.RemoveAllListeners();
            recipeBookButton.onClick.AddListener(() => {
                PlayButtonSound();
                onModeSelected?.Invoke(GameMode.Recipe);
            });
            
            // マウスオーバー時の説明表示
            AddHoverEvents(recipeBookButton, GameMode.Recipe);
        }
        
        if (parentChildButton != null)
        {
            parentChildButton.onClick.RemoveAllListeners();
            parentChildButton.onClick.AddListener(() => {
                PlayButtonSound();
                onModeSelected?.Invoke(GameMode.ParentChild);
            });
            
            // マウスオーバー時の説明表示
            AddHoverEvents(parentChildButton, GameMode.ParentChild);
        }
        
        // 説明パネルを初期状態では非表示に
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
        
        // モード説明テキストの初期化
        InitializeModeDescriptions();
    }
    
    // コールバック設定メソッド
    public void SetCallbacks(System.Action<GameMode> modeCallback, System.Action backCallback)
    {
        onModeSelected = modeCallback;
        onBackToMainMenu = backCallback;
    }
    
    // モード説明テキストの初期化
    private void InitializeModeDescriptions()
    {
        // 日本語の説明テキスト
        modeDescriptions[GameMode.FreePlay] = "すきな材料をえらんで、じゆうにスープをつくろう！";
        modeDescriptions[GameMode.Mission] = "ミッションをクリアして、いろいろなスープをつくろう！";
        modeDescriptions[GameMode.Recipe] = "レシピどおりにスープをつくって、レシピブックをあつめよう！";
        modeDescriptions[GameMode.ParentChild] = "おうちのひとといっしょに、たのしくスープをつくろう！";
    }
    
    // ホバーイベントの追加
    private void AddHoverEvents(Button button, GameMode mode)
    {
        // UnityEvent Trigger を使用
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
        
        // PointerEnter イベント
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => {
            ShowModeDescription(mode);
        });
        trigger.triggers.Add(enterEntry);
        
        // PointerExit イベント
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            HideModeDescription();
        });
        trigger.triggers.Add(exitEntry);
    }
    
    // モード説明の表示
    private void ShowModeDescription(GameMode mode)
    {
        if (descriptionPanel != null && modeDescriptionText != null)
        {
            if (modeDescriptions.TryGetValue(mode, out string description))
            {
                modeDescriptionText.text = description;
                descriptionPanel.SetActive(true);
                
                // アニメーション（オプション）
                descriptionPanel.transform.localScale = Vector3.zero;
                LeanTween.scale(descriptionPanel, Vector3.one, 0.2f).setEaseOutBack();
            }
        }
    }
    
    // モード説明の非表示
    private void HideModeDescription()
    {
        if (descriptionPanel != null)
        {
            // アニメーション（オプション）
            LeanTween.scale(descriptionPanel, Vector3.zero, 0.1f).setEaseInBack().setOnComplete(() => {
                descriptionPanel.SetActive(false);
            });
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
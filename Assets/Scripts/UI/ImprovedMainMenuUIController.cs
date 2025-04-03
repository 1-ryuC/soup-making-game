using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImprovedMainMenuUIController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private MainMenuPanel mainMenuPanel;
    [SerializeField] private ModeSelectionPanel modeSelectionPanel;
    [SerializeField] private SettingsPanel settingsPanel;
    [SerializeField] private CreditsPanel creditsPanel;
    
    [Header("Character References")]
    [SerializeField] private GameObject mainCharacter;
    [SerializeField] private Animator characterAnimator;
    
    // 外部マネージャー参照
    private GameManager gameManager;
    private AudioManager audioManager;
    private UIManager uiManager;
    
    // 現在アクティブなパネル
    private MenuPanel currentActivePanel;
    
    private void Awake()
    {
        // 外部マネージャー参照を取得
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        uiManager = FindObjectOfType<UIManager>();
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }
        
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found!");
        }
    }
    
    // 初期化メソッド（UIManagerから呼び出される）
    public void Initialize()
    {
        // 各パネルを非表示に
        HideAllPanels();
        
        // 各パネルの初期化とコールバック設定
        InitializeMainMenuPanel();
        InitializeModeSelectionPanel();
        InitializeSettingsPanel();
        InitializeCreditsPanel();
        
        // 最初はメインメニューパネルを表示
        ShowMainMenuPanel();
        
        // キャラクターアニメーション開始
        PlayCharacterAnimation("Idle");
    }
    
    // 全パネルを非表示にする
    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.Hide();
        if (modeSelectionPanel != null) modeSelectionPanel.Hide();
        if (settingsPanel != null) settingsPanel.Hide();
        if (creditsPanel != null) creditsPanel.Hide();
        
        currentActivePanel = null;
    }
    
    // メインメニューパネルの初期化
    private void InitializeMainMenuPanel()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.Initialize();
            mainMenuPanel.SetCallbacks(
                ShowModeSelectionPanel,  // プレイボタン
                ShowSettingsPanel,       // 設定ボタン
                ShowCreditsPanel,        // クレジットボタン
                OnQuitButtonPressed      // 終了ボタン
            );
        }
    }
    
    // モード選択パネルの初期化
    private void InitializeModeSelectionPanel()
    {
        if (modeSelectionPanel != null)
        {
            modeSelectionPanel.Initialize();
            modeSelectionPanel.SetCallbacks(
                OnModeSelected,      // モード選択
                ShowMainMenuPanel    // 戻るボタン
            );
        }
    }
    
    // 設定パネルの初期化
    private void InitializeSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.Initialize();
            settingsPanel.SetCallbacks(
                ShowMainMenuPanel    // 戻るボタン
            );
        }
    }
    
    // クレジットパネルの初期化
    private void InitializeCreditsPanel()
    {
        if (creditsPanel != null)
        {
            creditsPanel.Initialize();
            creditsPanel.SetCallbacks(
                ShowMainMenuPanel    // 戻るボタン
            );
        }
    }
    
    // メインメニューパネルを表示
    private void ShowMainMenuPanel()
    {
        HideAllPanels();
        
        if (mainMenuPanel != null)
        {
            mainMenuPanel.Show();
            currentActivePanel = mainMenuPanel;
        }
        
        PlayCharacterAnimation("Idle");
    }
    
    // モード選択パネルを表示
    private void ShowModeSelectionPanel()
    {
        HideAllPanels();
        
        if (modeSelectionPanel != null)
        {
            modeSelectionPanel.Show();
            currentActivePanel = modeSelectionPanel;
        }
        
        PlayCharacterAnimation("Excited");
    }
    
    // 設定パネルを表示
    private void ShowSettingsPanel()
    {
        HideAllPanels();
        
        if (settingsPanel != null)
        {
            settingsPanel.Show();
            currentActivePanel = settingsPanel;
        }
        
        PlayCharacterAnimation("Thinking");
    }
    
    // クレジットパネルを表示
    private void ShowCreditsPanel()
    {
        HideAllPanels();
        
        if (creditsPanel != null)
        {
            creditsPanel.Show();
            currentActivePanel = creditsPanel;
        }
        
        PlayCharacterAnimation("Happy");
    }
    
    // モード選択時の処理
    private void OnModeSelected(GameMode mode)
    {
        if (gameManager != null)
        {
            gameManager.StartGame(mode);
        }
        else
        {
            Debug.LogError("GameManager not found for mode selection: " + mode);
        }
    }
    
    // 終了ボタン押下時の処理
    private void OnQuitButtonPressed()
    {
        if (gameManager != null)
        {
            gameManager.ExitGame();
        }
        else
        {
            Debug.LogError("GameManager not found for quit action");
            
            // フォールバック終了メソッド
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
    
    // キャラクターアニメーション再生
    private void PlayCharacterAnimation(string animationName)
    {
        if (characterAnimator != null)
        {
            characterAnimator.Play(animationName);
        }
    }
    
    // バックボタンのハンドリング（Androidの戻るボタンなど）
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentActivePanel == mainMenuPanel)
            {
                // メインメニュー表示中の場合は終了確認
                OnQuitButtonPressed();
            }
            else if (currentActivePanel != null)
            {
                // それ以外のパネル表示中はメインメニューに戻る
                ShowMainMenuPanel();
            }
        }
    }
}
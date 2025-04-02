using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static GameManager Instance { get; private set; }
    
    // 各マネージャーへの参照
    [SerializeField] private DataManager dataManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioManager audioManager;
    
    // 現在のゲーム状態
    public GameState CurrentGameState { get; private set; }
    
    // ゲーム状態変更イベント
    public event System.Action<GameState> OnGameStateChanged;
    
    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 初期状態の設定
        CurrentGameState = GameState.MainMenu;
    }
    
    private void Start()
    {
        // ゲームデータの初期ロード
        dataManager.LoadAllData();
        
        // メインメニューUIの表示
        uiManager.ShowMainMenu();
        
        // メインメニュー音楽の再生
        audioManager.PlayBGM("MainTheme");
    }
    
    // ゲーム状態を変更するメソッド
    public void ChangeGameState(GameState newState)
    {
        if (CurrentGameState != newState)
        {
            CurrentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
            
            // 新しい状態に応じた初期化
            switch (newState)
            {
                case GameState.MainMenu:
                    uiManager.ShowMainMenu();
                    audioManager.PlayBGM("MainTheme");
                    break;
                case GameState.Playing:
                    uiManager.ShowGameplayUI();
                    audioManager.PlayBGM("GameplayTheme");
                    break;
                case GameState.Paused:
                    uiManager.ShowPauseMenu();
                    audioManager.PauseBGM();
                    break;
                case GameState.Result:
                    uiManager.ShowResultUI();
                    audioManager.PlayBGM("ResultTheme");
                    break;
            }
        }
    }
    
    // ゲームを開始する
    public void StartGame(GameMode gameMode)
    {
        // ゲームモードに応じた初期化
        switch (gameMode)
        {
            case GameMode.FreePlay:
                // フリープレイモードの初期化
                break;
            case GameMode.Mission:
                // ミッションモードの初期化
                break;
            case GameMode.Recipe:
                // レシピモードの初期化
                break;
            case GameMode.ParentChild:
                // 親子モードの初期化
                break;
        }
        
        // ゲーム状態を「プレイ中」に変更
        ChangeGameState(GameState.Playing);
    }
    
    // ゲームを一時停止する
    public void PauseGame()
    {
        if (CurrentGameState == GameState.Playing)
        {
            ChangeGameState(GameState.Paused);
        }
    }
    
    // 一時停止したゲームを再開する
    public void ResumeGame()
    {
        if (CurrentGameState == GameState.Paused)
        {
            ChangeGameState(GameState.Playing);
        }
    }
    
    // ゲームを終了する
    public void ExitGame()
    {
        // プレイヤーデータを保存
        dataManager.SavePlayerData();
        
        // アプリケーションを終了
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

// ゲーム状態を表す列挙型
public enum GameState
{
    MainMenu,   // メインメニュー
    Playing,    // プレイ中
    Paused,     // 一時停止中
    Result      // 結果画面
}

// ゲームモードを表す列挙型
public enum GameMode
{
    FreePlay,    // フリープレイモード
    Mission,     // ミッションモード
    Recipe,      // レシピモード
    ParentChild  // 親子モード
}

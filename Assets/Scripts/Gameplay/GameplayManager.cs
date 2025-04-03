using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoupMaking.Gameplay
{
    /// <summary>
    /// ゲームプレイ全体を管理するマネージャークラス
    /// </summary>
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance { get; private set; }

        [Header("システム参照")]
        [SerializeField] private CookingSystem cookingSystem;
        [SerializeField] private TastingSystem tastingSystem;
        [SerializeField] private KitchenController kitchenController;

        [Header("UI参照")]
        [SerializeField] private GameplayUIController uiController;

        // 現在のゲームモード
        private GameMode currentGameMode = GameMode.FreePlay;
        
        // 現在のゲーム状態
        private GameplayState currentState = GameplayState.Initializing;

        // イベント
        public event Action<GameplayState> OnGameStateChanged;
        public event Action<GameMode> OnGameModeChanged;

        // プロパティ
        public GameplayState CurrentState => currentState;
        public GameMode CurrentGameMode => currentGameMode;

        private void Awake()
        {
            // シングルトンパターン
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // 各システムの初期化
            InitializeSystems();
            
            // 初期状態に設定
            ChangeState(GameplayState.Ready);
        }

        /// <summary>
        /// ゲームプレイ関連のシステムを初期化
        /// </summary>
        private void InitializeSystems()
        {
            // 必要なシステムの初期化確認
            if (cookingSystem == null)
            {
                cookingSystem = FindObjectOfType<CookingSystem>();
                Debug.LogWarning("CookingSystem was not assigned, attempting to find in scene.");
            }
            
            if (tastingSystem == null)
            {
                tastingSystem = FindObjectOfType<TastingSystem>();
                Debug.LogWarning("TastingSystem was not assigned, attempting to find in scene.");
            }
            
            if (kitchenController == null)
            {
                kitchenController = FindObjectOfType<KitchenController>();
                Debug.LogWarning("KitchenController was not assigned, attempting to find in scene.");
            }
            
            if (uiController == null)
            {
                uiController = FindObjectOfType<GameplayUIController>();
                Debug.LogWarning("GameplayUIController was not assigned, attempting to find in scene.");
            }
        }

        /// <summary>
        /// ゲームモードを設定
        /// </summary>
        /// <param name="mode">設定するゲームモード</param>
        public void SetGameMode(GameMode mode)
        {
            currentGameMode = mode;
            OnGameModeChanged?.Invoke(currentGameMode);
            Debug.Log($"Game mode changed to: {currentGameMode}");
        }

        /// <summary>
        /// ゲーム状態を変更
        /// </summary>
        /// <param name="newState">新しいゲーム状態</param>
        public void ChangeState(GameplayState newState)
        {
            if (newState == currentState) return;
            
            // 現在の状態を終了
            ExitState(currentState);
            
            // 状態を変更
            GameplayState previousState = currentState;
            currentState = newState;
            
            // 新しい状態を開始
            EnterState(currentState);
            
            // イベント発火
            OnGameStateChanged?.Invoke(currentState);
            Debug.Log($"Gameplay state changed from {previousState} to {currentState}");
        }

        /// <summary>
        /// 状態開始時の処理
        /// </summary>
        private void EnterState(GameplayState state)
        {
            switch (state)
            {
                case GameplayState.Ready:
                    // 準備状態の開始処理
                    if (uiController != null) uiController.ShowStartUI();
                    break;
                
                case GameplayState.SelectingIngredients:
                    // 食材選択状態の開始処理
                    if (kitchenController != null) kitchenController.OpenIngredientStorage();
                    if (uiController != null) uiController.ShowIngredientSelectionUI();
                    break;
                
                case GameplayState.Cooking:
                    // 調理状態の開始処理
                    if (cookingSystem != null) cookingSystem.StartCooking();
                    if (uiController != null) uiController.ShowCookingUI();
                    break;
                
                case GameplayState.Tasting:
                    // 試食状態の開始処理
                    if (tastingSystem != null) tastingSystem.PrepareTasting();
                    if (uiController != null) uiController.ShowTastingUI();
                    break;
                
                case GameplayState.Result:
                    // 結果表示状態の開始処理
                    if (uiController != null) uiController.ShowResultUI();
                    break;
            }
        }

        /// <summary>
        /// 状態終了時の処理
        /// </summary>
        private void ExitState(GameplayState state)
        {
            switch (state)
            {
                case GameplayState.Ready:
                    // 準備状態の終了処理
                    if (uiController != null) uiController.HideStartUI();
                    break;
                
                case GameplayState.SelectingIngredients:
                    // 食材選択状態の終了処理
                    if (kitchenController != null) kitchenController.CloseIngredientStorage();
                    if (uiController != null) uiController.HideIngredientSelectionUI();
                    break;
                
                case GameplayState.Cooking:
                    // 調理状態の終了処理
                    if (cookingSystem != null) cookingSystem.FinishCooking();
                    if (uiController != null) uiController.HideCookingUI();
                    break;
                
                case GameplayState.Tasting:
                    // 試食状態の終了処理
                    if (tastingSystem != null) tastingSystem.FinishTasting();
                    if (uiController != null) uiController.HideTastingUI();
                    break;
                
                case GameplayState.Result:
                    // 結果表示状態の終了処理
                    if (uiController != null) uiController.HideResultUI();
                    break;
            }
        }

        /// <summary>
        /// 食材選択状態に進む
        /// </summary>
        public void StartIngredientSelection()
        {
            ChangeState(GameplayState.SelectingIngredients);
        }

        /// <summary>
        /// 調理状態に進む
        /// </summary>
        public void StartCooking()
        {
            ChangeState(GameplayState.Cooking);
        }

        /// <summary>
        /// 試食状態に進む
        /// </summary>
        public void StartTasting()
        {
            ChangeState(GameplayState.Tasting);
        }

        /// <summary>
        /// 結果表示状態に進む
        /// </summary>
        public void ShowResult()
        {
            ChangeState(GameplayState.Result);
        }

        /// <summary>
        /// ゲームをリセット
        /// </summary>
        public void ResetGame()
        {
            // 状態をリセット
            ChangeState(GameplayState.Ready);
            
            // 各システムのリセット
            if (cookingSystem != null) cookingSystem.ResetCooking();
            if (tastingSystem != null) tastingSystem.ResetTasting();
            
            Debug.Log("Game reset");
        }
    }

    /// <summary>
    /// ゲームプレイの状態を表す列挙型
    /// </summary>
    public enum GameplayState
    {
        Initializing,           // 初期化中
        Ready,                  // 準備完了
        SelectingIngredients,   // 食材選択中
        Cooking,                // 調理中
        Tasting,                // 試食中
        Result                  // 結果表示
    }
}
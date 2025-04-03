using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SoupMaking.Gameplay;

namespace SoupMaking.UI.Gameplay
{
    /// <summary>
    /// ゲームの進行状況表示を管理するクラス
    /// </summary>
    public class ProgressUI : MonoBehaviour
    {
        [Header("進行表示UI")]
        [SerializeField] private Image progressBar;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI stateText;
        
        [Header("ステップアイコン")]
        [SerializeField] private GameObject[] stepIcons;
        [SerializeField] private Color activeStepColor = Color.white;
        [SerializeField] private Color inactiveStepColor = Color.gray;
        [SerializeField] private Color completedStepColor = Color.green;
        
        // 参照
        private GameplayManager gameplayManager;
        private CookingSystem cookingSystem;
        
        private void Start()
        {
            // マネージャーの参照を取得
            gameplayManager = GameplayManager.Instance;
            if (gameplayManager == null)
            {
                Debug.LogError("GameplayManager is not found in the scene.");
            }
            else
            {
                // ゲーム状態変更イベントのリスナー登録
                gameplayManager.OnGameStateChanged += HandleGameStateChanged;
            }
            
            // 調理システムの参照を取得
            cookingSystem = FindObjectOfType<CookingSystem>();
            if (cookingSystem == null)
            {
                Debug.LogError("CookingSystem is not found in the scene.");
            }
            else
            {
                // 調理進行イベントのリスナー登録
                cookingSystem.OnCookingProgress += HandleCookingProgress;
            }
            
            // 初期状態を設定
            UpdateProgress(0f);
            UpdateStepIcons(GameplayState.Ready);
        }
        
        private void OnDestroy()
        {
            // イベントリスナーの解除
            if (gameplayManager != null)
            {
                gameplayManager.OnGameStateChanged -= HandleGameStateChanged;
            }
            
            if (cookingSystem != null)
            {
                cookingSystem.OnCookingProgress -= HandleCookingProgress;
            }
        }
        
        /// <summary>
        /// ゲーム状態変更時のハンドラ
        /// </summary>
        /// <param name="newState">新しいゲーム状態</param>
        private void HandleGameStateChanged(GameplayState newState)
        {
            // ステートテキストを更新
            UpdateStateText(newState);
            
            // ステップアイコンを更新
            UpdateStepIcons(newState);
            
            // 調理中でなければプログレスバーをリセット
            if (newState != GameplayState.Cooking)
            {
                UpdateProgress(0f);
            }
        }
        
        /// <summary>
        /// 調理進行時のハンドラ
        /// </summary>
        /// <param name="progress">調理進行度（0-1）</param>
        private void HandleCookingProgress(float progress)
        {
            UpdateProgress(progress);
        }
        
        /// <summary>
        /// 進行バーを更新
        /// </summary>
        /// <param name="progress">進行度（0-1）</param>
        private void UpdateProgress(float progress)
        {
            // プログレスバーを更新
            if (progressBar != null)
            {
                progressBar.fillAmount = progress;
            }
            
            // プログレステキストを更新
            if (progressText != null)
            {
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
            }
        }
        
        /// <summary>
        /// 状態テキストを更新
        /// </summary>
        /// <param name="state">現在のゲーム状態</param>
        private void UpdateStateText(GameplayState state)
        {
            if (stateText == null) return;
            
            // 状態に応じたテキストを設定
            switch (state)
            {
                case GameplayState.Ready:
                    stateText.text = "準備完了";
                    break;
                
                case GameplayState.SelectingIngredients:
                    stateText.text = "食材を選んでね";
                    break;
                
                case GameplayState.Cooking:
                    stateText.text = "スープを作ってるよ";
                    break;
                
                case GameplayState.Tasting:
                    stateText.text = "試食してもらおう";
                    break;
                
                case GameplayState.Result:
                    stateText.text = "できあがり！";
                    break;
                
                default:
                    stateText.text = "";
                    break;
            }
        }
        
        /// <summary>
        /// ステップアイコンを更新
        /// </summary>
        /// <param name="currentState">現在のゲーム状態</param>
        private void UpdateStepIcons(GameplayState currentState)
        {
            if (stepIcons == null || stepIcons.Length == 0) return;
            
            // 各ステップアイコンの状態を更新
            for (int i = 0; i < stepIcons.Length; i++)
            {
                GameObject icon = stepIcons[i];
                if (icon == null) continue;
                
                Image iconImage = icon.GetComponent<Image>();
                if (iconImage == null) continue;
                
                // ステップインデックスに対応するゲーム状態
                GameplayState stepState = GetStepState(i);
                
                // 現在のステップより前なら完了状態
                if (GetStateIndex(stepState) < GetStateIndex(currentState))
                {
                    iconImage.color = completedStepColor;
                }
                // 現在のステップなら活性状態
                else if (stepState == currentState)
                {
                    iconImage.color = activeStepColor;
                }
                // それ以外なら非活性状態
                else
                {
                    iconImage.color = inactiveStepColor;
                }
            }
        }
        
        /// <summary>
        /// ステップインデックスに対応するゲーム状態を取得
        /// </summary>
        /// <param name="stepIndex">ステップインデックス</param>
        /// <returns>対応するゲーム状態</returns>
        private GameplayState GetStepState(int stepIndex)
        {
            switch (stepIndex)
            {
                case 0:
                    return GameplayState.Ready;
                case 1:
                    return GameplayState.SelectingIngredients;
                case 2:
                    return GameplayState.Cooking;
                case 3:
                    return GameplayState.Tasting;
                case 4:
                    return GameplayState.Result;
                default:
                    return GameplayState.Ready;
            }
        }
        
        /// <summary>
        /// ゲーム状態のインデックスを取得（順序比較用）
        /// </summary>
        /// <param name="state">ゲーム状態</param>
        /// <returns>状態のインデックス</returns>
        private int GetStateIndex(GameplayState state)
        {
            switch (state)
            {
                case GameplayState.Initializing:
                    return 0;
                case GameplayState.Ready:
                    return 1;
                case GameplayState.SelectingIngredients:
                    return 2;
                case GameplayState.Cooking:
                    return 3;
                case GameplayState.Tasting:
                    return 4;
                case GameplayState.Result:
                    return 5;
                default:
                    return 0;
            }
        }
    }
}
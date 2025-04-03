using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SoupMaking.Gameplay;
using SoupMaking.Ingredients;

namespace SoupMaking.UI.Gameplay
{
    /// <summary>
    /// ゲームプレイUI全体を管理するコントローラークラス
    /// </summary>
    public class GameplayUIController : MonoBehaviour
    {
        [Header("UIパネル")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject ingredientSelectionPanel;
        [SerializeField] private GameObject cookingPanel;
        [SerializeField] private GameObject tastingPanel;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private GameObject pausePanel;

        [Header("ゲームプレイUI")]
        [SerializeField] private ProgressUI progressUI;
        [SerializeField] private CookingActionUI cookingActionUI;
        [SerializeField] private IngredientSelectionUI ingredientSelectionUI;
        [SerializeField] private TastingUI tastingUI;
        [SerializeField] private ResultUI resultUI;

        [Header("共通UI要素")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button restartButton;

        // 参照
        private GameplayManager gameplayManager;

        private void Start()
        {
            // ゲームプレイマネージャーの参照を取得
            gameplayManager = GameplayManager.Instance;
            if (gameplayManager == null)
            {
                Debug.LogError("GameplayManager is not found in the scene.");
            }

            // ボタンイベントの設定
            SetupButtonEvents();

            // ゲーム状態変更イベントのリスナー登録
            if (gameplayManager != null)
            {
                gameplayManager.OnGameStateChanged += HandleGameStateChanged;
            }

            // 初期状態では全パネルを非表示
            HideAllPanels();
        }

        private void OnDestroy()
        {
            // イベントリスナーの解除
            if (gameplayManager != null)
            {
                gameplayManager.OnGameStateChanged -= HandleGameStateChanged;
            }
        }

        /// <summary>
        /// ボタンイベントの設定
        /// </summary>
        private void SetupButtonEvents()
        {
            // 一時停止ボタン
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(PauseGame);
            }

            // 再開ボタン
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(ResumeGame);
            }

            // メインメニューボタン
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            }

            // リスタートボタン
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }
        }

        /// <summary>
        /// ゲーム状態変更に対応するUI更新
        /// </summary>
        private void HandleGameStateChanged(GameplayState newState)
        {
            // 各状態に対応するUIを表示
            switch (newState)
            {
                case GameplayState.Ready:
                    ShowStartUI();
                    break;

                case GameplayState.SelectingIngredients:
                    ShowIngredientSelectionUI();
                    break;

                case GameplayState.Cooking:
                    ShowCookingUI();
                    break;

                case GameplayState.Tasting:
                    ShowTastingUI();
                    break;

                case GameplayState.Result:
                    ShowResultUI();
                    break;
            }
        }

        /// <summary>
        /// 全パネルを非表示にする
        /// </summary>
        private void HideAllPanels()
        {
            if (startPanel != null) startPanel.SetActive(false);
            if (ingredientSelectionPanel != null) ingredientSelectionPanel.SetActive(false);
            if (cookingPanel != null) cookingPanel.SetActive(false);
            if (tastingPanel != null) tastingPanel.SetActive(false);
            if (resultPanel != null) resultPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        #region パネル表示制御メソッド

        /// <summary>
        /// スタートUIを表示
        /// </summary>
        public void ShowStartUI()
        {
            HideAllPanels();
            if (startPanel != null) startPanel.SetActive(true);
        }

        /// <summary>
        /// スタートUIを非表示
        /// </summary>
        public void HideStartUI()
        {
            if (startPanel != null) startPanel.SetActive(false);
        }

        /// <summary>
        /// 食材選択UIを表示
        /// </summary>
        public void ShowIngredientSelectionUI()
        {
            HideAllPanels();
            if (ingredientSelectionPanel != null) ingredientSelectionPanel.SetActive(true);

            // IngredientSelectionUIの初期化
            if (ingredientSelectionUI != null)
            {
                ingredientSelectionUI.Initialize();
            }
        }

        /// <summary>
        /// 食材選択UIを非表示
        /// </summary>
        public void HideIngredientSelectionUI()
        {
            if (ingredientSelectionPanel != null) ingredientSelectionPanel.SetActive(false);
        }

        /// <summary>
        /// 調理UIを表示
        /// </summary>
        public void ShowCookingUI()
        {
            HideAllPanels();
            if (cookingPanel != null) cookingPanel.SetActive(true);

            // CookingActionUIの初期化
            if (cookingActionUI != null)
            {
                cookingActionUI.Initialize();
            }
        }

        /// <summary>
        /// 調理UIを非表示
        /// </summary>
        public void HideCookingUI()
        {
            if (cookingPanel != null) cookingPanel.SetActive(false);
        }

        /// <summary>
        /// 試食UIを表示
        /// </summary>
        public void ShowTastingUI()
        {
            HideAllPanels();
            if (tastingPanel != null) tastingPanel.SetActive(true);

            // TastingUIの初期化
            if (tastingUI != null)
            {
                tastingUI.Initialize();
            }
        }

        /// <summary>
        /// 試食UIを非表示
        /// </summary>
        public void HideTastingUI()
        {
            if (tastingPanel != null) tastingPanel.SetActive(false);
        }

        /// <summary>
        /// 結果UIを表示
        /// </summary>
        public void ShowResultUI()
        {
            HideAllPanels();
            if (resultPanel != null) resultPanel.SetActive(true);

            // ResultUIの初期化
            if (resultUI != null)
            {
                resultUI.Initialize();
            }
        }

        /// <summary>
        /// 結果UIを非表示
        /// </summary>
        public void HideResultUI()
        {
            if (resultPanel != null) resultPanel.SetActive(false);
        }

        /// <summary>
        /// 一時停止UIを表示
        /// </summary>
        public void ShowPauseUI()
        {
            if (pausePanel != null) pausePanel.SetActive(true);
        }

        /// <summary>
        /// 一時停止UIを非表示
        /// </summary>
        public void HidePauseUI()
        {
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        #endregion

        #region ゲーム制御メソッド

        /// <summary>
        /// ゲームを一時停止
        /// </summary>
        public void PauseGame()
        {
            // タイムスケールを0に設定
            Time.timeScale = 0f;

            // 一時停止UIを表示
            ShowPauseUI();
        }

        /// <summary>
        /// ゲームを再開
        /// </summary>
        public void ResumeGame()
        {
            // タイムスケールを1に戻す
            Time.timeScale = 1f;

            // 一時停止UIを非表示
            HidePauseUI();
        }

        /// <summary>
        /// メインメニューに戻る
        /// </summary>
        public void ReturnToMainMenu()
        {
            // タイムスケールを1に戻す
            Time.timeScale = 1f;

            // メインメニューシーンに遷移
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// ゲームを再開始
        /// </summary>
        public void RestartGame()
        {
            // タイムスケールを1に戻す
            Time.timeScale = 1f;

            // ゲームをリセット
            if (gameplayManager != null)
            {
                gameplayManager.ResetGame();
            }

            // 現在のシーンを再読み込み
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        #endregion

        #region ボタンアクションメソッド

        /// <summary>
        /// 食材選択開始ボタンのアクション
        /// </summary>
        public void OnStartIngredientSelectionButton()
        {
            if (gameplayManager != null)
            {
                gameplayManager.StartIngredientSelection();
            }
        }

        /// <summary>
        /// 調理開始ボタンのアクション
        /// </summary>
        public void OnStartCookingButton()
        {
            if (gameplayManager != null)
            {
                gameplayManager.StartCooking();
            }
        }

        /// <summary>
        /// 試食開始ボタンのアクション
        /// </summary>
        public void OnStartTastingButton()
        {
            if (gameplayManager != null)
            {
                gameplayManager.StartTasting();
            }
        }

        /// <summary>
        /// 結果表示ボタンのアクション
        /// </summary>
        public void OnShowResultButton()
        {
            if (gameplayManager != null)
            {
                gameplayManager.ShowResult();
            }
        }

        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // UI要素参照
    [Header("UI Screens")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject resultUI;
    
    [Header("UI Controllers")]
    [SerializeField] private MainMenuUI mainMenuController;
    [SerializeField] private GameplayUI gameplayController;
    [SerializeField] private ResultUI resultController;
    
    [Header("Common UI Elements")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Button tutorialNextButton;
    
    // シーン別UI管理
    public MainMenuUI MainMenuUIController => mainMenuController;
    public GameplayUI GameplayUIController => gameplayController;
    public ResultUI ResultUIController => resultController;
    
    // メッセージタイマー
    private float messageTimer = 0f;
    private Coroutine messageCoroutine;
    
    // チュートリアルデータ
    private List<string> tutorialSteps = new List<string>();
    private int currentTutorialStep = 0;
    
    // Unity Lifecycle Callbacks
    private void Awake()
    {
        // UIの初期化
        HideAllUI();
    }
    
    // すべてのUIを非表示にするメソッド
    private void HideAllUI()
    {
        mainMenuUI.SetActive(false);
        gameplayUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        resultUI.SetActive(false);
        messagePanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }
    
    // UI表示/非表示メソッド
    public void ShowUI(GameObject uiElement)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(true);
        }
    }
    
    public void HideUI(GameObject uiElement)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
    }
    
    // メインメニューUIを表示するメソッド
    public void ShowMainMenu()
    {
        HideAllUI();
        mainMenuUI.SetActive(true);
        
        // メインメニューUIの初期化
        if (mainMenuController != null)
        {
            mainMenuController.InitializeUI();
        }
    }
    
    // ゲームプレイUIを表示するメソッド
    public void ShowGameplayUI()
    {
        HideAllUI();
        gameplayUI.SetActive(true);
        
        // ゲームプレイUIの初期化
        if (gameplayController != null)
        {
            gameplayController.InitializeUI();
        }
    }
    
    // ポーズメニューUIを表示するメソッド
    public void ShowPauseMenu()
    {
        // ポーズメニューはオーバーレイなので他のUIは非表示にしない
        pauseMenuUI.SetActive(true);
    }
    
    // ポーズメニューUIを非表示にするメソッド
    public void HidePauseMenu()
    {
        pauseMenuUI.SetActive(false);
    }
    
    // 結果UIを表示するメソッド
    public void ShowResultUI()
    {
        HideAllUI();
        resultUI.SetActive(true);
        
        // 結果UIの初期化
        if (resultController != null)
        {
            resultController.InitializeUI();
        }
    }
    
    // スコアを更新するメソッド
    public void UpdateScore(int score)
    {
        if (gameplayController != null)
        {
            gameplayController.UpdateScore(score);
        }
    }
    
    // タイマーを更新するメソッド
    public void UpdateTimer(float time)
    {
        if (gameplayController != null)
        {
            gameplayController.UpdateTimer(time);
        }
    }
    
    // メッセージを表示するメソッド
    public void ShowMessage(string message, float duration = 2.0f)
    {
        // 既存のメッセージタイマーがある場合は停止
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }
        
        // メッセージを表示
        messageText.text = message;
        messagePanel.SetActive(true);
        
        // メッセージタイマーを開始
        messageCoroutine = StartCoroutine(HideMessageAfterDelay(duration));
    }
    
    // 一定時間後にメッセージを非表示にするコルーチン
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messagePanel.SetActive(false);
        messageCoroutine = null;
    }
    
    // チュートリアルステップを表示するメソッド
    public void ShowTutorialStep(string stepKey)
    {
        string localizedStep = LocalizationSystem.GetTranslation(stepKey);
        
        if (!string.IsNullOrEmpty(localizedStep))
        {
            tutorialText.text = localizedStep;
            tutorialPanel.SetActive(true);
        }
    }
    
    // チュートリアルを開始するメソッド
    public void StartTutorial(List<string> steps)
    {
        if (steps != null && steps.Count > 0)
        {
            tutorialSteps = steps;
            currentTutorialStep = 0;
            
            // 最初のステップを表示
            ShowTutorialStep(tutorialSteps[currentTutorialStep]);
            
            // 次へボタンのクリックイベントを設定
            tutorialNextButton.onClick.RemoveAllListeners();
            tutorialNextButton.onClick.AddListener(ShowNextTutorialStep);
        }
    }
    
    // 次のチュートリアルステップを表示するメソッド
    private void ShowNextTutorialStep()
    {
        currentTutorialStep++;
        
        if (currentTutorialStep < tutorialSteps.Count)
        {
            // 次のステップを表示
            ShowTutorialStep(tutorialSteps[currentTutorialStep]);
        }
        else
        {
            // チュートリアル終了
            tutorialPanel.SetActive(false);
            tutorialSteps.Clear();
        }
    }
    
    // チュートリアルをスキップするメソッド
    public void SkipTutorial()
    {
        tutorialPanel.SetActive(false);
        tutorialSteps.Clear();
    }
}

// 以下は各シーン専用のUI管理クラス
// MainMenuUI
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Transform modeSelectionPanel;
    
    public void InitializeUI()
    {
        // ボタンのリスナー設定
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(ShowModeSelection);
        
        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(ShowSettings);
        
        creditsButton.onClick.RemoveAllListeners();
        creditsButton.onClick.AddListener(ShowCredits);
        
        // モード選択パネルを非表示に
        modeSelectionPanel.gameObject.SetActive(false);
    }
    
    private void ShowModeSelection()
    {
        modeSelectionPanel.gameObject.SetActive(true);
    }
    
    private void ShowSettings()
    {
        // 設定画面を表示
        Debug.Log("Settings button clicked");
    }
    
    private void ShowCredits()
    {
        // クレジット画面を表示
        Debug.Log("Credits button clicked");
    }
    
    public void SelectGameMode(int mode)
    {
        GameMode selectedMode = (GameMode)mode;
        GameManager.Instance.StartGame(selectedMode);
    }
}

// GameplayUI
public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Transform ingredientSelectionPanel;
    [SerializeField] private Transform actionButtonsPanel;
    
    private int currentScore = 0;
    private float currentTime = 0f;
    
    public void InitializeUI()
    {
        // ボタンのリスナー設定
        pauseButton.onClick.RemoveAllListeners();
        pauseButton.onClick.AddListener(() => GameManager.Instance.PauseGame());
        
        // スコアとタイマーの初期化
        currentScore = 0;
        scoreText.text = "0";
        
        currentTime = 0f;
        UpdateTimerDisplay();
    }
    
    public void UpdateScore(int score)
    {
        currentScore = score;
        scoreText.text = score.ToString();
    }
    
    public void UpdateTimer(float time)
    {
        currentTime = time;
        UpdateTimerDisplay();
    }
    
    private void UpdateTimerDisplay()
    {
        // 分:秒の形式でタイマーを表示
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public void ShowIngredientSelection()
    {
        ingredientSelectionPanel.gameObject.SetActive(true);
        actionButtonsPanel.gameObject.SetActive(false);
    }
    
    public void ShowActionButtons()
    {
        ingredientSelectionPanel.gameObject.SetActive(false);
        actionButtonsPanel.gameObject.SetActive(true);
    }
    
    public void HideIngredientSelection()
    {
        ingredientSelectionPanel.gameObject.SetActive(false);
    }
    
    public void HideActionButtons()
    {
        actionButtonsPanel.gameObject.SetActive(false);
    }
}

// ResultUI
public class ResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultTitleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform starContainer;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextButton;
    
    public void InitializeUI()
    {
        // ボタンのリスナー設定
        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(() => GameManager.Instance.ChangeGameState(GameState.MainMenu));
        
        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(RetryLevel);
        
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextLevel);
    }
    
    public void SetResultData(string title, int score, int stars)
    {
        resultTitleText.text = title;
        scoreText.text = score.ToString();
        
        // 星の表示をクリア
        foreach (Transform child in starContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 獲得した星を表示
        for (int i = 0; i < stars; i++)
        {
            Instantiate(starPrefab, starContainer);
        }
    }
    
    private void RetryLevel()
    {
        // 現在のレベルをリトライ
        GameManager.Instance.StartGame(GameManager.Instance.CurrentGameMode);
    }
    
    private void NextLevel()
    {
        // 次のレベルに進む
        // この実装はミッションモードなどに依存
        Debug.Log("Next level button clicked");
    }
}

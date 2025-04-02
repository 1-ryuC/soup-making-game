# 「おいしいスープをつくろう」データ設計ドキュメント

このドキュメントは、「おいしいスープをつくろう」プロジェクトのデータ構造、クラス設計、保存システムについて定義しています。

## 1. データモデル

### 1.1 食材データモデル
```csharp
[System.Serializable]
public class IngredientData
{
    public string id;                  // 一意の識別子
    public string nameJP;              // 日本語名
    public string nameEN;              // 英語名
    public IngredientCategory category; // 食材カテゴリー
    public IngredientType type;        // 食材タイプ
    public Color baseColor;            // 基本色
    public float nutritionValue;       // 栄養価値
    public float tasteValue;           // 味の強さ
    public string[] tags;              // タグ（季節、イベントなど）
    public bool isUnlocked;            // アンロック状態
    public string spritePath;          // スプライトのリソースパス
}

public enum IngredientCategory
{
    Vegetable,  // 野菜
    Fruit,      // 果物
    Protein,    // タンパク質
    Seasoning   // 調味料
}

public enum IngredientType
{
    Solid,      // 固形
    Liquid,     // 液体
    Powder      // 粉末
}
```

### 1.2 レシピデータモデル
```csharp
[System.Serializable]
public class RecipeData
{
    public string id;                  // 一意の識別子
    public string nameJP;              // 日本語名
    public string nameEN;              // 英語名
    public string descriptionJP;       // 日本語説明
    public string descriptionEN;       // 英語説明
    public RecipeCategory category;    // カテゴリー
    public int difficulty;             // 難易度（1-5）
    public string[] requiredIngredientIds; // 必要な食材ID
    public CookingStep[] cookingSteps; // 調理ステップ
    public string[] tags;              // タグ（季節、イベントなど）
    public bool isUnlocked;            // アンロック状態
    public string thumbnailPath;       // サムネイル画像のリソースパス
}

[System.Serializable]
public class CookingStep
{
    public CookingAction action;       // 調理アクション
    public string targetIngredientId;  // 対象の食材ID
    public string instructionJP;       // 日本語の指示
    public string instructionEN;       // 英語の指示
}

public enum RecipeCategory
{
    Basic,      // 基本レシピ
    Fantasy,    // ファンタジーレシピ
    Seasonal    // 季節レシピ
}

public enum CookingAction
{
    Wash,       // 洗う
    Cut,        // 切る
    Peel,       // 皮をむく
    Boil,       // 煮る
    Mix,        // 混ぜる
    Season      // 味付け
}
```

### 1.3 キャラクターデータモデル
```csharp
[System.Serializable]
public class CharacterData
{
    public string id;                  // 一意の識別子
    public string nameJP;              // 日本語名
    public string nameEN;              // 英語名
    public CharacterType type;         // キャラクタータイプ
    public PreferenceData[] preferences; // 好み設定
    public string[] unlockConditions;  // アンロック条件
    public bool isUnlocked;            // アンロック状態
    public string spritePath;          // スプライトのリソースパス
    public string[] animationPaths;    // アニメーションのリソースパス
}

[System.Serializable]
public class PreferenceData
{
    public string ingredientId;        // 食材ID
    public float preferenceLevel;      // 好み度（-1.0〜1.0）
}

public enum CharacterType
{
    Chef,       // シェフ（スープくん）
    Taster      // 試食キャラクター
}
```

### 1.4 ミッションデータモデル
```csharp
[System.Serializable]
public class MissionData
{
    public string id;                  // 一意の識別子
    public string nameJP;              // 日本語名
    public string nameEN;              // 英語名
    public string descriptionJP;       // 日本語説明
    public string descriptionEN;       // 英語説明
    public int difficultyLevel;        // 難易度レベル（1-3）
    public MissionObjective[] objectives; // ミッション目標
    public RewardData reward;          // 報酬データ
    public bool isCompleted;           // 完了状態
    public string[] tags;              // タグ（季節、イベントなど）
}

[System.Serializable]
public class MissionObjective
{
    public ObjectiveType type;         // 目標タイプ
    public string targetId;            // 目標のID（食材、キャラクターなど）
    public int targetCount;            // 目標数
    public string conditionJP;         // 日本語の条件説明
    public string conditionEN;         // 英語の条件説明
}

[System.Serializable]
public class RewardData
{
    public int stars;                  // 獲得星数
    public string[] unlockedItemIds;   // アンロックされるアイテムID
}

public enum ObjectiveType
{
    UseIngredient,   // 特定の食材を使用
    MakeRecipe,      // 特定のレシピを作成
    PleaseCharacter, // 特定のキャラクターを喜ばせる
    UseAction        // 特定のアクションを使用
}
```

### 1.5 プレイヤーデータモデル
```csharp
[System.Serializable]
public class PlayerData
{
    public string playerId;            // プレイヤーID
    public string name;                // 名前
    public int age;                    // 年齢
    public int totalStars;             // 合計獲得星数
    public string[] unlockedIngredientIds; // アンロックした食材ID
    public string[] unlockedRecipeIds; // アンロックしたレシピID
    public string[] unlockedCharacterIds; // アンロックしたキャラクターID
    public string[] completedMissionIds; // 完了したミッションID
    public CustomRecipeData[] savedRecipes; // 保存したカスタムレシピ
    public GameSettings gameSettings;  // ゲーム設定
}

[System.Serializable]
public class CustomRecipeData
{
    public string id;                  // 一意の識別子
    public string name;                // レシピ名
    public string[] ingredientIds;     // 使用した食材ID
    public CookingStep[] cookingSteps; // 調理ステップ
    public string createdDate;         // 作成日
    public int rating;                 // 評価（1-5）
}

[System.Serializable]
public class GameSettings
{
    public float bgmVolume;            // BGM音量（0.0-1.0）
    public float sfxVolume;            // 効果音量（0.0-1.0）
    public float voiceVolume;          // ボイス音量（0.0-1.0）
    public string language;            // 言語設定（"ja", "en"など）
    public bool highContrastMode;      // 高コントラストモード
    public bool simplifiedMode;        // 簡易モード
    public float touchSensitivity;     // タッチ感度（0.0-1.0）
}
```

### 1.6 スープデータモデル
```csharp
[System.Serializable]
public class SoupData
{
    public string id;                  // 一意の識別子
    public string name;                // スープ名
    public Color soupColor;            // スープの色
    public float thickness;            // とろみ（0.0-1.0）
    public string[] ingredientIds;     // 使用した食材ID
    public Dictionary<string, float> tasteProfile; // 味プロファイル（sweet, sour, salty, bitter, umami）
    public float nutritionValue;       // 栄養価値
    public float[] characterScores;    // 各キャラクターの評価スコア
    public string thumbnailPath;       // サムネイル画像のリソースパス
}
```

## 2. マネージャークラス設計

### 2.1 GameManager
```csharp
public class GameManager : MonoBehaviour
{
    // シングルトンパターン
    public static GameManager Instance { get; private set; }
    
    // 各種マネージャー参照
    public DataManager DataManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public SceneController SceneController { get; private set; }
    
    // ゲームの状態管理
    public GameState CurrentGameState { get; private set; }
    
    // メソッド
    public void ChangeGameState(GameState newState);
    public void StartGame(GameMode gameMode);
    public void PauseGame();
    public void ResumeGame();
    public void ExitGame();
    
    // イベント
    public event Action<GameState> OnGameStateChanged;
}

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Result
}

public enum GameMode
{
    FreePlay,
    Mission,
    Recipe,
    ParentChild
}
```

### 2.2 DataManager
```csharp
public class DataManager : MonoBehaviour
{
    // データコレクション
    private List<IngredientData> ingredientDataList;
    private List<RecipeData> recipeDataList;
    private List<CharacterData> characterDataList;
    private List<MissionData> missionDataList;
    private PlayerData playerData;
    
    // メソッド
    public void LoadAllData();
    public void SavePlayerData();
    
    // 食材関連
    public IngredientData GetIngredientById(string id);
    public List<IngredientData> GetIngredientsByCategory(IngredientCategory category);
    public List<IngredientData> GetUnlockedIngredients();
    
    // レシピ関連
    public RecipeData GetRecipeById(string id);
    public List<RecipeData> GetRecipesByCategory(RecipeCategory category);
    public List<RecipeData> GetUnlockedRecipes();
    
    // キャラクター関連
    public CharacterData GetCharacterById(string id);
    public List<CharacterData> GetUnlockedCharacters();
    
    // ミッション関連
    public MissionData GetMissionById(string id);
    public List<MissionData> GetMissionsByDifficulty(int difficultyLevel);
    public List<MissionData> GetCompletedMissions();
    
    // プレイヤーデータ関連
    public void UpdatePlayerData();
    public void UnlockIngredient(string id);
    public void UnlockRecipe(string id);
    public void UnlockCharacter(string id);
    public void CompleteMission(string id);
    public void SaveCustomRecipe(CustomRecipeData recipe);
}
```

### 2.3 UIManager
```csharp
public class UIManager : MonoBehaviour
{
    // UI要素参照
    public GameObject mainMenuUI;
    public GameObject gameplayUI;
    public GameObject pauseMenuUI;
    public GameObject resultUI;
    
    // シーン別UI管理
    public MainMenuUI MainMenuUIController { get; private set; }
    public GameplayUI GameplayUIController { get; private set; }
    public ResultUI ResultUIController { get; private set; }
    
    // メソッド
    public void ShowUI(GameObject uiElement);
    public void HideUI(GameObject uiElement);
    public void ShowMainMenu();
    public void ShowGameplayUI();
    public void ShowPauseMenu();
    public void ShowResultUI();
    
    // UI更新
    public void UpdateScore(int score);
    public void UpdateTimer(float time);
    public void ShowMessage(string message, float duration);
    public void ShowTutorialStep(string stepKey);
}
```

### 2.4 AudioManager
```csharp
public class AudioManager : MonoBehaviour
{
    // オーディオソース参照
    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioSource voiceSource;
    
    // オーディオクリップ辞書
    private Dictionary<string, AudioClip> bgmClips;
    private Dictionary<string, AudioClip> sfxClips;
    private Dictionary<string, AudioClip> voiceClips;
    
    // メソッド
    public void PlayBGM(string clipName, bool loop = true);
    public void StopBGM();
    public void FadeBGM(float duration);
    
    public void PlaySFX(string clipName);
    public void PlayRandomSFX(string[] clipNames);
    
    public void PlayVoice(string clipName);
    public void StopVoice();
    
    // 音量調整
    public void SetBGMVolume(float volume);
    public void SetSFXVolume(float volume);
    public void SetVoiceVolume(float volume);
}
```

### 2.5 SceneController
```csharp
public class SceneController : MonoBehaviour
{
    // 現在のシーン
    public string CurrentScene { get; private set; }
    
    // メソッド
    public void LoadScene(string sceneName);
    public void LoadSceneAdditive(string sceneName);
    public void UnloadScene(string sceneName);
    public void ReloadCurrentScene();
    
    // 遷移効果
    public void TransitionToScene(string sceneName, TransitionType transitionType, float duration);
    
    // イベント
    public event Action<string> OnSceneLoaded;
    public event Action<string> OnSceneUnloaded;
}

public enum TransitionType
{
    Fade,
    Slide,
    Zoom
}
```

## 3. ゲームプレイクラス設計

### 3.1 CookingSystem
```csharp
public class CookingSystem : MonoBehaviour
{
    // 現在の調理状態
    public CookingState CurrentState { get; private set; }
    
    // 現在選択中の食材
    public IngredientData SelectedIngredient { get; private set; }
    
    // 現在のスープデータ
    public SoupData CurrentSoup { get; private set; }
    
    // 使用した食材リスト
    public List<string> UsedIngredientIds { get; private set; }
    
    // メソッド
    public void SelectIngredient(string ingredientId);
    public void DeselectIngredient();
    public void PerformAction(CookingAction action);
    public void AddIngredientToSoup(string ingredientId);
    public void AdjustHeat(float heatLevel);
    public void StirSoup(int stirCount);
    public SoupData FinalizeSoup(string soupName);
    
    // イベント
    public event Action<string> OnIngredientSelected;
    public event Action OnIngredientDeselected;
    public event Action<CookingAction> OnActionPerformed;
    public event Action<string> OnIngredientAdded;
    public event Action<SoupData> OnSoupFinalized;
}

public enum CookingState
{
    SelectingIngredient,
    PreparingIngredient,
    AddingToSoup,
    AdjustingHeat,
    Stirring,
    Finalizing
}
```

### 3.2 CharacterReactionSystem
```csharp
public class CharacterReactionSystem : MonoBehaviour
{
    // 試食キャラクター参照
    public CharacterData CurrentCharacter { get; private set; }
    
    // 評価スコア
    public float CurrentScore { get; private set; }
    
    // メソッド
    public void SetCharacter(string characterId);
    public ReactionData EvaluateSoup(SoupData soup);
    public void PlayReaction(ReactionType reactionType);
    public void ShowFeedback(SoupData soup);
    
    // イベント
    public event Action<CharacterData> OnCharacterSet;
    public event Action<ReactionData> OnSoupEvaluated;
}

[System.Serializable]
public class ReactionData
{
    public ReactionType reactionType;   // 反応タイプ
    public float score;                 // スコア（0.0-5.0）
    public string[] feedbackKeys;       // フィードバックメッセージキー
}

public enum ReactionType
{
    VeryHappy,
    Happy,
    Neutral,
    Unhappy,
    VeryUnhappy
}
```

### 3.3 MissionSystem
```csharp
public class MissionSystem : MonoBehaviour
{
    // 現在のミッション
    public MissionData CurrentMission { get; private set; }
    
    // ミッション進捗
    private Dictionary<string, int> objectiveProgress;
    
    // メソッド
    public void SetMission(string missionId);
    public void UpdateProgress(ObjectiveType type, string targetId, int count = 1);
    public bool IsObjectiveComplete(string objectiveId);
    public bool IsMissionComplete();
    public RewardData CompleteMission();
    
    // イベント
    public event Action<MissionData> OnMissionSet;
    public event Action<string, int, int> OnProgressUpdated;
    public event Action<RewardData> OnMissionCompleted;
}
```

## 4. データ保存と読み込み

### 4.1 SaveSystem
```csharp
public class SaveSystem
{
    // 保存先パス
    private static string SAVE_PATH = Application.persistentDataPath + "/savedata/";
    
    // メソッド
    public static void SavePlayerData(PlayerData data);
    public static PlayerData LoadPlayerData();
    public static void SaveCustomRecipe(CustomRecipeData recipe);
    public static List<CustomRecipeData> LoadCustomRecipes();
    public static void DeleteSaveData();
    public static bool HasSaveData();
    
    // ヘルパーメソッド
    private static void EnsureDirectoryExists();
    private static string Serialize<T>(T data);
    private static T Deserialize<T>(string json);
}
```

### 4.2 ローカライゼーションシステム
```csharp
public class LocalizationSystem
{
    // 現在の言語
    public static string CurrentLanguage { get; private set; }
    
    // 翻訳データ
    private static Dictionary<string, Dictionary<string, string>> translationData;
    
    // メソッド
    public static void LoadLanguage(string languageCode);
    public static string GetTranslation(string key);
    public static bool HasTranslation(string key);
    public static string[] GetAvailableLanguages();
    
    // ヘルパーメソッド
    private static void LoadTranslationData(string languageCode);
}
```

## 5. データ初期化と構成

### 5.1 データファイル構造
すべてのゲームデータはJSON形式で保存され、Resources/Dataフォルダに配置されます。

```
Resources/
└── Data/
    ├── Ingredients/
    │   ├── vegetables.json
    │   ├── fruits.json
    │   ├── proteins.json
    │   └── seasonings.json
    ├── Recipes/
    │   ├── basic_recipes.json
    │   ├── fantasy_recipes.json
    │   └── seasonal_recipes.json
    ├── Characters/
    │   ├── chef_characters.json
    │   └── taster_characters.json
    ├── Missions/
    │   ├── level1_missions.json
    │   ├── level2_missions.json
    │   └── level3_missions.json
    └── Localization/
        ├── ja.json
        └── en.json
```

### 5.2 初期データ読み込み
アプリケーション起動時に、DataManagerがすべてのJSON設定ファイルを読み込み、ゲームで使用できるようにメモリに保持します。プレイヤーの進行状況は、SaveSystemを使用して別途保存・読み込みされます。

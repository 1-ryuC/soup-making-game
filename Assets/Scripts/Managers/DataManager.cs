using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DataManager : MonoBehaviour
{
    // データコレクション
    private List<IngredientData> ingredientDataList = new List<IngredientData>();
    private List<RecipeData> recipeDataList = new List<RecipeData>();
    private List<CharacterData> characterDataList = new List<CharacterData>();
    private List<MissionData> missionDataList = new List<MissionData>();
    private PlayerData playerData;
    
    // データパス
    private readonly string dataPath = "Data/";
    
    // プレイヤーデータが初期化されたかどうかのフラグ
    private bool isPlayerDataInitialized = false;
    
    // Unity Lifecycle Callbacks
    private void Awake()
    {
        // 初期化処理
        LoadAllData();
    }
    
    // すべてのデータをロードするメソッド
    public void LoadAllData()
    {
        LoadIngredientData();
        LoadRecipeData();
        LoadCharacterData();
        LoadMissionData();
        LoadPlayerData();
        
        Debug.Log("All game data loaded successfully.");
    }
    
    // プレイヤーデータを保存するメソッド
    public void SavePlayerData()
    {
        if (playerData != null)
        {
            SaveSystem.SavePlayerData(playerData);
            Debug.Log("Player data saved successfully.");
        }
        else
        {
            Debug.LogError("Cannot save player data: Player data is null.");
        }
    }
    
    #region Data Loading Methods
    
    // 食材データをロードするメソッド
    private void LoadIngredientData()
    {
        ingredientDataList.Clear();
        
        // カテゴリー別の食材データをロード
        string[] categories = { "vegetables", "fruits", "proteins", "seasonings" };
        
        foreach (string category in categories)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(dataPath + "Ingredients/" + category);
            if (textAsset != null)
            {
                IngredientDataList categoryData = JsonUtility.FromJson<IngredientDataList>(textAsset.text);
                if (categoryData != null && categoryData.ingredients != null)
                {
                    ingredientDataList.AddRange(categoryData.ingredients);
                }
            }
            else
            {
                Debug.LogWarning("Could not load ingredient data for category: " + category);
            }
        }
        
        Debug.Log($"Loaded {ingredientDataList.Count} ingredients.");
    }
    
    // レシピデータをロードするメソッド
    private void LoadRecipeData()
    {
        recipeDataList.Clear();
        
        // カテゴリー別のレシピデータをロード
        string[] categories = { "basic_recipes", "fantasy_recipes", "seasonal_recipes" };
        
        foreach (string category in categories)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(dataPath + "Recipes/" + category);
            if (textAsset != null)
            {
                RecipeDataList categoryData = JsonUtility.FromJson<RecipeDataList>(textAsset.text);
                if (categoryData != null && categoryData.recipes != null)
                {
                    recipeDataList.AddRange(categoryData.recipes);
                }
            }
            else
            {
                Debug.LogWarning("Could not load recipe data for category: " + category);
            }
        }
        
        Debug.Log($"Loaded {recipeDataList.Count} recipes.");
    }
    
    // キャラクターデータをロードするメソッド
    private void LoadCharacterData()
    {
        characterDataList.Clear();
        
        // カテゴリー別のキャラクターデータをロード
        string[] categories = { "chef_characters", "taster_characters" };
        
        foreach (string category in categories)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(dataPath + "Characters/" + category);
            if (textAsset != null)
            {
                CharacterDataList categoryData = JsonUtility.FromJson<CharacterDataList>(textAsset.text);
                if (categoryData != null && categoryData.characters != null)
                {
                    characterDataList.AddRange(categoryData.characters);
                }
            }
            else
            {
                Debug.LogWarning("Could not load character data for category: " + category);
            }
        }
        
        Debug.Log($"Loaded {characterDataList.Count} characters.");
    }
    
    // ミッションデータをロードするメソッド
    private void LoadMissionData()
    {
        missionDataList.Clear();
        
        // 難易度別のミッションデータをロード
        string[] difficulties = { "level1_missions", "level2_missions", "level3_missions" };
        
        foreach (string difficulty in difficulties)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(dataPath + "Missions/" + difficulty);
            if (textAsset != null)
            {
                MissionDataList difficultyData = JsonUtility.FromJson<MissionDataList>(textAsset.text);
                if (difficultyData != null && difficultyData.missions != null)
                {
                    missionDataList.AddRange(difficultyData.missions);
                }
            }
            else
            {
                Debug.LogWarning("Could not load mission data for difficulty: " + difficulty);
            }
        }
        
        Debug.Log($"Loaded {missionDataList.Count} missions.");
    }
    
    // プレイヤーデータをロードするメソッド
    private void LoadPlayerData()
    {
        // 既存のプレイヤーデータがあれば読み込み、なければ新規作成
        PlayerData loadedData = SaveSystem.LoadPlayerData();
        
        if (loadedData != null)
        {
            playerData = loadedData;
            Debug.Log("Player data loaded successfully.");
        }
        else
        {
            // 新規プレイヤーデータの初期化
            playerData = InitializeNewPlayerData();
            Debug.Log("New player data initialized.");
        }
        
        isPlayerDataInitialized = true;
    }
    
    // 新規プレイヤーデータを初期化するメソッド
    private PlayerData InitializeNewPlayerData()
    {
        PlayerData newData = new PlayerData
        {
            playerId = System.Guid.NewGuid().ToString(),
            name = "Player",
            age = 0,
            totalStars = 0,
            unlockedIngredientIds = GetDefaultUnlockedIngredients(),
            unlockedRecipeIds = GetDefaultUnlockedRecipes(),
            unlockedCharacterIds = GetDefaultUnlockedCharacters(),
            completedMissionIds = new string[0],
            savedRecipes = new CustomRecipeData[0],
            gameSettings = GetDefaultGameSettings()
        };
        
        return newData;
    }
    
    // デフォルトでアンロックされる食材を取得するメソッド
    private string[] GetDefaultUnlockedIngredients()
    {
        // 初期状態でアンロックされる食材ID
        return new string[]
        {
            "veg_carrot",
            "veg_onion",
            "veg_potato",
            "fruit_apple",
            "protein_egg",
            "seasoning_salt"
        };
    }
    
    // デフォルトでアンロックされるレシピを取得するメソッド
    private string[] GetDefaultUnlockedRecipes()
    {
        // 初期状態でアンロックされるレシピID
        return new string[]
        {
            "basic_vegetable_soup"
        };
    }
    
    // デフォルトでアンロックされるキャラクターを取得するメソッド
    private string[] GetDefaultUnlockedCharacters()
    {
        // 初期状態でアンロックされるキャラクターID
        return new string[]
        {
            "chef_soup",  // スープくん
            "taster_bear" // くまさん
        };
    }
    
    // デフォルトのゲーム設定を取得するメソッド
    private GameSettings GetDefaultGameSettings()
    {
        return new GameSettings
        {
            bgmVolume = 0.7f,
            sfxVolume = 0.8f,
            voiceVolume = 1.0f,
            language = "ja",
            highContrastMode = false,
            simplifiedMode = false,
            touchSensitivity = 0.5f
        };
    }
    
    #endregion
    
    #region Data Accessor Methods
    
    // 食材関連
    
    // IDから食材データを取得するメソッド
    public IngredientData GetIngredientById(string id)
    {
        return ingredientDataList.FirstOrDefault(i => i.id == id);
    }
    
    // カテゴリーで食材を取得するメソッド
    public List<IngredientData> GetIngredientsByCategory(IngredientCategory category)
    {
        return ingredientDataList.Where(i => i.category == category).ToList();
    }
    
    // アンロック済みの食材を取得するメソッド
    public List<IngredientData> GetUnlockedIngredients()
    {
        if (playerData == null || playerData.unlockedIngredientIds == null) return new List<IngredientData>();
        
        return ingredientDataList.Where(i => playerData.unlockedIngredientIds.Contains(i.id)).ToList();
    }
    
    // レシピ関連
    
    // IDからレシピデータを取得するメソッド
    public RecipeData GetRecipeById(string id)
    {
        return recipeDataList.FirstOrDefault(r => r.id == id);
    }
    
    // カテゴリーでレシピを取得するメソッド
    public List<RecipeData> GetRecipesByCategory(RecipeCategory category)
    {
        return recipeDataList.Where(r => r.category == category).ToList();
    }
    
    // アンロック済みのレシピを取得するメソッド
    public List<RecipeData> GetUnlockedRecipes()
    {
        if (playerData == null || playerData.unlockedRecipeIds == null) return new List<RecipeData>();
        
        return recipeDataList.Where(r => playerData.unlockedRecipeIds.Contains(r.id)).ToList();
    }
    
    // キャラクター関連
    
    // IDからキャラクターデータを取得するメソッド
    public CharacterData GetCharacterById(string id)
    {
        return characterDataList.FirstOrDefault(c => c.id == id);
    }
    
    // アンロック済みのキャラクターを取得するメソッド
    public List<CharacterData> GetUnlockedCharacters()
    {
        if (playerData == null || playerData.unlockedCharacterIds == null) return new List<CharacterData>();
        
        return characterDataList.Where(c => playerData.unlockedCharacterIds.Contains(c.id)).ToList();
    }
    
    // ミッション関連
    
    // IDからミッションデータを取得するメソッド
    public MissionData GetMissionById(string id)
    {
        return missionDataList.FirstOrDefault(m => m.id == id);
    }
    
    // 難易度別のミッションを取得するメソッド
    public List<MissionData> GetMissionsByDifficulty(int difficultyLevel)
    {
        return missionDataList.Where(m => m.difficultyLevel == difficultyLevel).ToList();
    }
    
    // 完了済みのミッションを取得するメソッド
    public List<MissionData> GetCompletedMissions()
    {
        if (playerData == null || playerData.completedMissionIds == null) return new List<MissionData>();
        
        return missionDataList.Where(m => playerData.completedMissionIds.Contains(m.id)).ToList();
    }
    
    #endregion
    
    #region Player Data Update Methods
    
    // プレイヤーデータを更新するメソッド
    public void UpdatePlayerData()
    {
        if (isPlayerDataInitialized)
        {
            SavePlayerData();
        }
    }
    
    // 食材をアンロックするメソッド
    public void UnlockIngredient(string id)
    {
        if (playerData != null && !playerData.unlockedIngredientIds.Contains(id))
        {
            List<string> updatedIds = playerData.unlockedIngredientIds.ToList();
            updatedIds.Add(id);
            playerData.unlockedIngredientIds = updatedIds.ToArray();
            UpdatePlayerData();
            
            Debug.Log($"Unlocked ingredient: {id}");
        }
    }
    
    // レシピをアンロックするメソッド
    public void UnlockRecipe(string id)
    {
        if (playerData != null && !playerData.unlockedRecipeIds.Contains(id))
        {
            List<string> updatedIds = playerData.unlockedRecipeIds.ToList();
            updatedIds.Add(id);
            playerData.unlockedRecipeIds = updatedIds.ToArray();
            UpdatePlayerData();
            
            Debug.Log($"Unlocked recipe: {id}");
        }
    }
    
    // キャラクターをアンロックするメソッド
    public void UnlockCharacter(string id)
    {
        if (playerData != null && !playerData.unlockedCharacterIds.Contains(id))
        {
            List<string> updatedIds = playerData.unlockedCharacterIds.ToList();
            updatedIds.Add(id);
            playerData.unlockedCharacterIds = updatedIds.ToArray();
            UpdatePlayerData();
            
            Debug.Log($"Unlocked character: {id}");
        }
    }
    
    // ミッションを完了済みにするメソッド
    public void CompleteMission(string id)
    {
        if (playerData != null && !playerData.completedMissionIds.Contains(id))
        {
            List<string> updatedIds = playerData.completedMissionIds.ToList();
            updatedIds.Add(id);
            playerData.completedMissionIds = updatedIds.ToArray();
            
            // ミッションの報酬を獲得
            MissionData mission = GetMissionById(id);
            if (mission != null && mission.reward != null)
            {
                playerData.totalStars += mission.reward.stars;
                
                // アンロック項目があれば処理
                if (mission.reward.unlockedItemIds != null)
                {
                    foreach (string itemId in mission.reward.unlockedItemIds)
                    {
                        if (itemId.StartsWith("veg_") || itemId.StartsWith("fruit_") || 
                            itemId.StartsWith("protein_") || itemId.StartsWith("seasoning_"))
                        {
                            UnlockIngredient(itemId);
                        }
                        else if (itemId.StartsWith("recipe_"))
                        {
                            UnlockRecipe(itemId);
                        }
                        else if (itemId.StartsWith("character_"))
                        {
                            UnlockCharacter(itemId);
                        }
                    }
                }
            }
            
            UpdatePlayerData();
            Debug.Log($"Completed mission: {id}");
        }
    }
    
    // カスタムレシピを保存するメソッド
    public void SaveCustomRecipe(CustomRecipeData recipe)
    {
        if (playerData != null)
        {
            List<CustomRecipeData> updatedRecipes = playerData.savedRecipes.ToList();
            
            // 既存のレシピの更新チェック
            int existingIndex = updatedRecipes.FindIndex(r => r.id == recipe.id);
            if (existingIndex >= 0)
            {
                updatedRecipes[existingIndex] = recipe;
            }
            else
            {
                // 新規レシピの追加
                updatedRecipes.Add(recipe);
            }
            
            playerData.savedRecipes = updatedRecipes.ToArray();
            UpdatePlayerData();
            
            Debug.Log($"Saved custom recipe: {recipe.name}");
        }
    }
    
    #endregion
}

// JSONデシリアライズ用のラッパークラス
[System.Serializable]
public class IngredientDataList
{
    public IngredientData[] ingredients;
}

[System.Serializable]
public class RecipeDataList
{
    public RecipeData[] recipes;
}

[System.Serializable]
public class CharacterDataList
{
    public CharacterData[] characters;
}

[System.Serializable]
public class MissionDataList
{
    public MissionData[] missions;
}

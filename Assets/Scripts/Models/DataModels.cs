using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 食材データモデル
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

// 食材カテゴリー
public enum IngredientCategory
{
    Vegetable,  // 野菜
    Fruit,      // 果物
    Protein,    // タンパク質
    Seasoning   // 調味料
}

// 食材タイプ
public enum IngredientType
{
    Solid,      // 固形
    Liquid,     // 液体
    Powder      // 粉末
}

// レシピデータモデル
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

// 調理ステップ
[System.Serializable]
public class CookingStep
{
    public CookingAction action;       // 調理アクション
    public string targetIngredientId;  // 対象の食材ID
    public string instructionJP;       // 日本語の指示
    public string instructionEN;       // 英語の指示
}

// レシピカテゴリー
public enum RecipeCategory
{
    Basic,      // 基本レシピ
    Fantasy,    // ファンタジーレシピ
    Seasonal    // 季節レシピ
}

// 調理アクション
public enum CookingAction
{
    Wash,       // 洗う
    Cut,        // 切る
    Peel,       // 皮をむく
    Boil,       // 煮る
    Mix,        // 混ぜる
    Season      // 味付け
}

// キャラクターデータモデル
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

// 食材の好み設定
[System.Serializable]
public class PreferenceData
{
    public string ingredientId;        // 食材ID
    public float preferenceLevel;      // 好み度（-1.0〜1.0）
}

// キャラクタータイプ
public enum CharacterType
{
    Chef,       // シェフ（スープくん）
    Taster      // 試食キャラクター
}

// ミッションデータモデル
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

// ミッション目標
[System.Serializable]
public class MissionObjective
{
    public ObjectiveType type;         // 目標タイプ
    public string targetId;            // 目標のID（食材、キャラクターなど）
    public int targetCount;            // 目標数
    public string conditionJP;         // 日本語の条件説明
    public string conditionEN;         // 英語の条件説明
}

// 報酬データ
[System.Serializable]
public class RewardData
{
    public int stars;                  // 獲得星数
    public string[] unlockedItemIds;   // アンロックされるアイテムID
}

// 目標タイプ
public enum ObjectiveType
{
    UseIngredient,   // 特定の食材を使用
    MakeRecipe,      // 特定のレシピを作成
    PleaseCharacter, // 特定のキャラクターを喜ばせる
    UseAction        // 特定のアクションを使用
}

// プレイヤーデータモデル
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

// カスタムレシピデータ
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

// ゲーム設定
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

// スープデータモデル
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
    
    // Dictionaryのシリアライズ対応
    [System.Serializable]
    public class TasteProfileEntry
    {
        public string taste;
        public float value;
    }
    
    [System.NonSerialized]
    private Dictionary<string, float> _tasteProfile;
    
    public TasteProfileEntry[] serializedTasteProfile;
    
    // Unity JSONでDictionaryをシリアライズするための変換
    public void OnBeforeSerialize()
    {
        if (_tasteProfile == null || _tasteProfile.Count == 0)
        {
            serializedTasteProfile = null;
            return;
        }
        
        serializedTasteProfile = new TasteProfileEntry[_tasteProfile.Count];
        int i = 0;
        foreach (var kvp in _tasteProfile)
        {
            serializedTasteProfile[i] = new TasteProfileEntry { taste = kvp.Key, value = kvp.Value };
            i++;
        }
    }
    
    // Unity JSONでDictionaryをデシリアライズするための変換
    public void OnAfterDeserialize()
    {
        if (serializedTasteProfile == null || serializedTasteProfile.Length == 0)
        {
            _tasteProfile = new Dictionary<string, float>();
            return;
        }
        
        _tasteProfile = new Dictionary<string, float>(serializedTasteProfile.Length);
        foreach (var entry in serializedTasteProfile)
        {
            _tasteProfile[entry.taste] = entry.value;
        }
        
        tasteProfile = _tasteProfile;
    }
}

// キャラクターの反応データ
[System.Serializable]
public class ReactionData
{
    public ReactionType reactionType;   // 反応タイプ
    public float score;                 // スコア（0.0-5.0）
    public string[] feedbackKeys;       // フィードバックメッセージキー
}

// 反応タイプ
public enum ReactionType
{
    VeryHappy,    // とても嬉しい
    Happy,        // 嬉しい
    Neutral,      // 普通
    Unhappy,      // 不満
    VeryUnhappy   // とても不満
}

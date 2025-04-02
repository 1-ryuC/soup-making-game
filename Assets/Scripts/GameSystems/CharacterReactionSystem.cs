using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterReactionSystem : MonoBehaviour
{
    // 試食キャラクター参照
    public CharacterData CurrentCharacter { get; private set; }
    
    // 評価スコア
    public float CurrentScore { get; private set; }
    
    // データマネージャーへの参照
    [SerializeField] private DataManager dataManager;
    
    // キャラクター表示オブジェクト
    [SerializeField] private GameObject characterDisplay;
    
    // キャラクターアニメーター
    [SerializeField] private Animator characterAnimator;
    
    // オーディオマネージャーへの参照
    [SerializeField] private AudioManager audioManager;
    
    // 反応の種類ごとのアニメーションキー
    private readonly Dictionary<ReactionType, string> reactionAnimations = new Dictionary<ReactionType, string>()
    {
        { ReactionType.VeryHappy, "VeryHappy" },
        { ReactionType.Happy, "Happy" },
        { ReactionType.Neutral, "Neutral" },
        { ReactionType.Unhappy, "Unhappy" },
        { ReactionType.VeryUnhappy, "VeryUnhappy" }
    };
    
    // 反応の種類ごとの効果音キー
    private readonly Dictionary<ReactionType, string> reactionSounds = new Dictionary<ReactionType, string>()
    {
        { ReactionType.VeryHappy, "character_veryhappy" },
        { ReactionType.Happy, "character_happy" },
        { ReactionType.Neutral, "character_neutral" },
        { ReactionType.Unhappy, "character_unhappy" },
        { ReactionType.VeryUnhappy, "character_veryunhappy" }
    };
    
    // イベント
    public event Action<CharacterData> OnCharacterSet;
    public event Action<ReactionData> OnSoupEvaluated;
    
    // Unity Lifecycle Callbacks
    private void Awake()
    {
        // 初期化
        if (characterDisplay != null)
        {
            characterDisplay.SetActive(false);
        }
    }
    
    // キャラクターを設定するメソッド
    public void SetCharacter(string characterId)
    {
        // データマネージャーからキャラクターデータを取得
        CharacterData character = dataManager.GetCharacterById(characterId);
        
        if (character != null)
        {
            CurrentCharacter = character;
            
            // キャラクター表示を有効化
            if (characterDisplay != null)
            {
                characterDisplay.SetActive(true);
                
                // キャラクタースプライトを設定
                // 注: スプライト設定のコードはここに追加
            }
            
            // キャラクター設定イベント発火
            OnCharacterSet?.Invoke(character);
            
            Debug.Log($"Character set: {character.nameJP}");
        }
        else
        {
            Debug.LogError($"Character not found with ID: {characterId}");
        }
    }
    
    // スープを評価するメソッド
    public ReactionData EvaluateSoup(SoupData soup)
    {
        if (CurrentCharacter == null)
        {
            Debug.LogWarning("Cannot evaluate soup: No character set");
            return null;
        }
        
        // スコアの計算
        float score = CalculateScore(soup);
        CurrentScore = score;
        
        // 反応タイプの決定
        ReactionType reactionType = DetermineReactionType(score);
        
        // フィードバックメッセージの生成
        string[] feedbackKeys = GenerateFeedback(soup, score, reactionType);
        
        // 反応データの作成
        ReactionData reaction = new ReactionData
        {
            reactionType = reactionType,
            score = score,
            feedbackKeys = feedbackKeys
        };
        
        // 評価イベント発火
        OnSoupEvaluated?.Invoke(reaction);
        
        // 反応を再生
        PlayReaction(reactionType);
        
        Debug.Log($"Soup evaluated by {CurrentCharacter.nameJP}, Score: {score}, Reaction: {reactionType}");
        
        return reaction;
    }
    
    // スコアを計算するメソッド
    private float CalculateScore(SoupData soup)
    {
        // 初期スコア
        float score = 2.5f; // 5点満点の中間値からスタート
        
        // キャラクターの好みに基づいてスコアを調整
        if (CurrentCharacter.preferences != null)
        {
            foreach (PreferenceData preference in CurrentCharacter.preferences)
            {
                // 好みの食材が含まれているか確認
                bool containsIngredient = Array.IndexOf(soup.ingredientIds, preference.ingredientId) >= 0;
                
                if (containsIngredient)
                {
                    // 好みの強さに応じてスコアを調整（-1.0〜1.0）
                    score += preference.preferenceLevel;
                }
            }
        }
        
        // とろみの評価（キャラクターごとに好みのとろみがある想定）
        float idealThickness = 0.5f; // デフォルト値
        float thicknessFactor = 1.0f - Mathf.Abs(soup.thickness - idealThickness);
        score += thicknessFactor * 0.5f;
        
        // 味のバランス評価
        float tasteBalance = EvaluateTasteBalance(soup.tasteProfile);
        score += tasteBalance * 0.5f;
        
        // スコアを0〜5の範囲に制限
        return Mathf.Clamp(score, 0f, 5f);
    }
    
    // 味のバランスを評価するメソッド
    private float EvaluateTasteBalance(Dictionary<string, float> tasteProfile)
    {
        // 味のバランス評価の実装
        // この実装は単純化されたもの
        
        // すべての味の合計
        float totalTaste = 0f;
        foreach (var taste in tasteProfile.Values)
        {
            totalTaste += taste;
        }
        
        // 平均的な値（理想的なバランス）
        float averageTaste = totalTaste / tasteProfile.Count;
        
        // 偏差の計算
        float deviation = 0f;
        foreach (var taste in tasteProfile.Values)
        {
            deviation += Mathf.Abs(taste - averageTaste);
        }
        
        // 偏差が少ないほど高いスコア（バランスが良い）
        return 1.0f - (deviation / tasteProfile.Count);
    }
    
    // 反応タイプを決定するメソッド
    private ReactionType DetermineReactionType(float score)
    {
        if (score >= 4.5f)
            return ReactionType.VeryHappy;
        else if (score >= 3.5f)
            return ReactionType.Happy;
        else if (score >= 2.5f)
            return ReactionType.Neutral;
        else if (score >= 1.5f)
            return ReactionType.Unhappy;
        else
            return ReactionType.VeryUnhappy;
    }
    
    // フィードバックメッセージを生成するメソッド
    private string[] GenerateFeedback(SoupData soup, float score, ReactionType reactionType)
    {
        List<string> feedback = new List<string>();
        
        // 反応タイプに基づく基本メッセージ
        switch (reactionType)
        {
            case ReactionType.VeryHappy:
                feedback.Add("feedback.veryhappy");
                break;
            case ReactionType.Happy:
                feedback.Add("feedback.happy");
                break;
            case ReactionType.Neutral:
                feedback.Add("feedback.neutral");
                break;
            case ReactionType.Unhappy:
                feedback.Add("feedback.unhappy");
                break;
            case ReactionType.VeryUnhappy:
                feedback.Add("feedback.veryunhappy");
                break;
        }
        
        // スープの特性に基づく詳細なフィードバック
        
        // とろみに関するフィードバック
        if (soup.thickness > 0.7f)
            feedback.Add("feedback.thickness.high");
        else if (soup.thickness < 0.3f)
            feedback.Add("feedback.thickness.low");
        else
            feedback.Add("feedback.thickness.good");
        
        // 使用食材数に関するフィードバック
        if (soup.ingredientIds.Length > 5)
            feedback.Add("feedback.ingredients.many");
        else if (soup.ingredientIds.Length < 3)
            feedback.Add("feedback.ingredients.few");
        
        // 最も高い味に関するフィードバック
        string dominantTaste = GetDominantTaste(soup.tasteProfile);
        if (!string.IsNullOrEmpty(dominantTaste))
        {
            feedback.Add($"feedback.taste.{dominantTaste}");
        }
        
        return feedback.ToArray();
    }
    
    // 最も強い味を取得するメソッド
    private string GetDominantTaste(Dictionary<string, float> tasteProfile)
    {
        string dominantTaste = null;
        float maxValue = 0f;
        
        foreach (var pair in tasteProfile)
        {
            if (pair.Value > maxValue)
            {
                maxValue = pair.Value;
                dominantTaste = pair.Key;
            }
        }
        
        // 閾値を超える味がない場合は空文字を返す
        return maxValue > 0.3f ? dominantTaste : string.Empty;
    }
    
    // 反応を再生するメソッド
    public void PlayReaction(ReactionType reactionType)
    {
        // アニメーションの再生
        if (characterAnimator != null && reactionAnimations.TryGetValue(reactionType, out string animationKey))
        {
            characterAnimator.Play(animationKey);
            Debug.Log($"Playing animation: {animationKey}");
        }
        
        // 効果音の再生
        if (audioManager != null && reactionSounds.TryGetValue(reactionType, out string soundKey))
        {
            audioManager.PlaySFX(soundKey);
            Debug.Log($"Playing sound: {soundKey}");
        }
    }
    
    // フィードバックを表示するメソッド
    public void ShowFeedback(SoupData soup)
    {
        if (CurrentCharacter == null || soup == null)
        {
            Debug.LogWarning("Cannot show feedback: Character or soup is null");
            return;
        }
        
        // スープを評価
        ReactionData reaction = EvaluateSoup(soup);
        
        if (reaction != null)
        {
            // フィードバックメッセージの表示
            // 注: UIに表示するコードはここに追加
            
            Debug.Log($"Showing feedback for {soup.name}:");
            foreach (string key in reaction.feedbackKeys)
            {
                string localizedText = LocalizationSystem.GetTranslation(key);
                Debug.Log($"- {localizedText}");
            }
        }
    }
}

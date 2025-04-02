using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CookingSystem : MonoBehaviour
{
    // 現在の調理状態
    public CookingState CurrentState { get; private set; }
    
    // 現在選択中の食材
    public IngredientData SelectedIngredient { get; private set; }
    
    // 現在のスープデータ
    public SoupData CurrentSoup { get; private set; }
    
    // 使用した食材リスト
    public List<string> UsedIngredientIds { get; private set; } = new List<string>();
    
    // データマネージャーへの参照
    [SerializeField] private DataManager dataManager;
    
    // スープの初期データ
    [SerializeField] private Color defaultSoupColor = Color.clear;
    [SerializeField] private float defaultThickness = 0.2f;
    
    // 調理関連の定数
    private const float MAX_THICKNESS = 1.0f;
    private const float MIN_THICKNESS = 0.1f;
    private const float STIR_THICKNESS_INCREASE = 0.05f;
    
    // 味プロファイルの定数
    private readonly string[] tasteTypes = { "sweet", "sour", "salty", "bitter", "umami" };
    
    // イベント
    public event Action<string> OnIngredientSelected;
    public event Action OnIngredientDeselected;
    public event Action<CookingAction> OnActionPerformed;
    public event Action<string> OnIngredientAdded;
    public event Action<SoupData> OnSoupFinalized;
    public event Action<CookingState> OnStateChanged;
    
    // Unity Lifecycle Callbacks
    private void Awake()
    {
        // 初期化
        InitializeSoup();
        CurrentState = CookingState.SelectingIngredient;
    }
    
    // スープを初期化するメソッド
    private void InitializeSoup()
    {
        // 新しいスープデータ作成
        CurrentSoup = new SoupData
        {
            id = System.Guid.NewGuid().ToString(),
            name = "New Soup",
            soupColor = defaultSoupColor,
            thickness = defaultThickness,
            ingredientIds = new string[0],
            nutritionValue = 0f,
            characterScores = new float[0],
            thumbnailPath = ""
        };
        
        // 味プロファイル初期化
        CurrentSoup.tasteProfile = new Dictionary<string, float>();
        foreach (string taste in tasteTypes)
        {
            CurrentSoup.tasteProfile[taste] = 0f;
        }
        
        // 使用食材リストをクリア
        UsedIngredientIds.Clear();
        
        Debug.Log("Soup initialized");
    }
    
    // 調理状態を変更するメソッド
    public void ChangeState(CookingState newState)
    {
        if (CurrentState != newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
            
            Debug.Log($"Cooking state changed to: {newState}");
        }
    }
    
    // 食材を選択するメソッド
    public void SelectIngredient(string ingredientId)
    {
        // 現在の状態が食材選択中のみ有効
        if (CurrentState != CookingState.SelectingIngredient)
        {
            Debug.LogWarning($"Cannot select ingredient in current state: {CurrentState}");
            return;
        }
        
        // データマネージャーから食材データを取得
        IngredientData ingredient = dataManager.GetIngredientById(ingredientId);
        
        if (ingredient != null)
        {
            SelectedIngredient = ingredient;
            OnIngredientSelected?.Invoke(ingredientId);
            
            // 状態を準備中に変更
            ChangeState(CookingState.PreparingIngredient);
            
            Debug.Log($"Selected ingredient: {ingredient.nameJP}");
        }
        else
        {
            Debug.LogError($"Ingredient not found with ID: {ingredientId}");
        }
    }
    
    // 食材選択を解除するメソッド
    public void DeselectIngredient()
    {
        if (SelectedIngredient != null)
        {
            SelectedIngredient = null;
            OnIngredientDeselected?.Invoke();
            
            // 状態を食材選択に戻す
            ChangeState(CookingState.SelectingIngredient);
            
            Debug.Log("Ingredient deselected");
        }
    }
    
    // 調理アクションを実行するメソッド
    public void PerformAction(CookingAction action)
    {
        // 選択中の食材がない場合は無効
        if (SelectedIngredient == null)
        {
            Debug.LogWarning("Cannot perform action: No ingredient selected");
            return;
        }
        
        // 現在の状態が準備中のみ有効
        if (CurrentState != CookingState.PreparingIngredient)
        {
            Debug.LogWarning($"Cannot perform action in current state: {CurrentState}");
            return;
        }
        
        // アクションを実行
        OnActionPerformed?.Invoke(action);
        
        Debug.Log($"Performed action: {action} on {SelectedIngredient.nameJP}");
        
        // 特定のアクションの後は自動的にスープに追加状態に移行
        if (action == CookingAction.Cut || action == CookingAction.Peel || action == CookingAction.Wash)
        {
            // 準備完了、スープに追加できる状態に
            ChangeState(CookingState.AddingToSoup);
        }
    }
    
    // スープに食材を追加するメソッド
    public void AddIngredientToSoup(string ingredientId)
    {
        // データマネージャーから食材データを取得
        IngredientData ingredient = dataManager.GetIngredientById(ingredientId);
        
        if (ingredient != null)
        {
            // 食材リストに追加
            UsedIngredientIds.Add(ingredientId);
            
            // スープの特性を更新
            UpdateSoupProperties(ingredient);
            
            // 食材追加イベント発火
            OnIngredientAdded?.Invoke(ingredientId);
            
            // 選択解除
            SelectedIngredient = null;
            
            // 状態を調整に変更
            ChangeState(CookingState.AdjustingHeat);
            
            Debug.Log($"Added ingredient to soup: {ingredient.nameJP}");
        }
        else
        {
            Debug.LogError($"Ingredient not found with ID: {ingredientId}");
        }
    }
    
    // スープの特性を更新するメソッド
    private void UpdateSoupProperties(IngredientData ingredient)
    {
        // 色の更新（ブレンド）
        CurrentSoup.soupColor = Color.Lerp(CurrentSoup.soupColor, ingredient.baseColor, 0.3f);
        
        // 栄養価の更新
        CurrentSoup.nutritionValue += ingredient.nutritionValue;
        
        // 味プロファイルの更新（この実装はダミー、実際には食材ごとに味の影響を定義する必要がある）
        switch (ingredient.category)
        {
            case IngredientCategory.Vegetable:
                CurrentSoup.tasteProfile["umami"] += 0.2f;
                break;
            case IngredientCategory.Fruit:
                CurrentSoup.tasteProfile["sweet"] += 0.3f;
                CurrentSoup.tasteProfile["sour"] += 0.1f;
                break;
            case IngredientCategory.Protein:
                CurrentSoup.tasteProfile["umami"] += 0.4f;
                break;
            case IngredientCategory.Seasoning:
                CurrentSoup.tasteProfile["salty"] += 0.3f;
                break;
        }
        
        // 値を0〜1の範囲に制限
        foreach (string taste in tasteTypes)
        {
            CurrentSoup.tasteProfile[taste] = Mathf.Clamp01(CurrentSoup.tasteProfile[taste]);
        }
        
        // 使用食材IDの更新
        CurrentSoup.ingredientIds = UsedIngredientIds.ToArray();
    }
    
    // 火加減を調整するメソッド
    public void AdjustHeat(float heatLevel)
    {
        // 現在の状態が火加減調整中のみ有効
        if (CurrentState != CookingState.AdjustingHeat)
        {
            Debug.LogWarning($"Cannot adjust heat in current state: {CurrentState}");
            return;
        }
        
        // ヒートレベルを0〜1に制限
        heatLevel = Mathf.Clamp01(heatLevel);
        
        // ここで火加減に応じたスープの特性変更を行う
        // 例: 高温でより色が濃くなる、など
        
        Debug.Log($"Heat adjusted to: {heatLevel}");
        
        // 次の段階に進む
        ChangeState(CookingState.Stirring);
    }
    
    // スープをかき混ぜるメソッド
    public void StirSoup(int stirCount)
    {
        // 現在の状態がかき混ぜ中のみ有効
        if (CurrentState != CookingState.Stirring)
        {
            Debug.LogWarning($"Cannot stir soup in current state: {CurrentState}");
            return;
        }
        
        // かき混ぜ回数に応じてとろみを増加
        CurrentSoup.thickness = Mathf.Clamp(CurrentSoup.thickness + (STIR_THICKNESS_INCREASE * stirCount), MIN_THICKNESS, MAX_THICKNESS);
        
        Debug.Log($"Soup stirred {stirCount} times. New thickness: {CurrentSoup.thickness}");
        
        // かき混ぜ操作後、選択状態に戻る（別の食材を追加できるように）
        ChangeState(CookingState.SelectingIngredient);
    }
    
    // スープを完成させるメソッド
    public SoupData FinalizeSoup(string soupName)
    {
        // スープに名前を設定
        if (!string.IsNullOrEmpty(soupName))
        {
            CurrentSoup.name = soupName;
        }
        
        // キャラクターの評価スコアを計算（仮の実装）
        CalculateCharacterScores();
        
        // スープ完成イベント発火
        OnSoupFinalized?.Invoke(CurrentSoup);
        
        // 結果をローカル変数にコピー
        SoupData finalizedSoup = CurrentSoup;
        
        // 状態を完成に変更
        ChangeState(CookingState.Finalizing);
        
        Debug.Log($"Soup finalized: {CurrentSoup.name}");
        
        return finalizedSoup;
    }
    
    // キャラクターの評価スコアを計算するメソッド（仮の実装）
    private void CalculateCharacterScores()
    {
        // ダミーキャラクター（5キャラクター分のスコア）
        CurrentSoup.characterScores = new float[5];
        
        // キャラクターごとの好みに基づいて評価（仮の実装）
        for (int i = 0; i < 5; i++)
        {
            // 各キャラクターの好みに応じた評価ロジックをここに実装
            // この実装は単純なランダムスコア
            float score = 0f;
            
            // 食材の数が多いほど高評価（仮）
            score += UsedIngredientIds.Count * 0.2f;
            
            // とろみの好み（仮）
            float thicknessPreference = (i % 2 == 0) ? 0.3f : 0.7f; // 偶数番目のキャラクターは薄め、奇数番目は濃いめが好き
            score += (1f - Mathf.Abs(CurrentSoup.thickness - thicknessPreference)) * 0.3f;
            
            // 味の好み（仮）
            string favoriteTaste = tasteTypes[i % tasteTypes.Length];
            score += CurrentSoup.tasteProfile[favoriteTaste] * 0.5f;
            
            // スコアを0〜5の範囲に制限
            CurrentSoup.characterScores[i] = Mathf.Clamp(score, 0f, 5f);
        }
    }
    
    // スープを新しく作り直すメソッド
    public void ResetSoup()
    {
        InitializeSoup();
        ChangeState(CookingState.SelectingIngredient);
        
        Debug.Log("Soup reset");
    }
}

// 調理状態を表す列挙型
public enum CookingState
{
    SelectingIngredient,  // 食材選択中
    PreparingIngredient,  // 食材準備中
    AddingToSoup,         // スープに追加中
    AdjustingHeat,        // 火加減調整中
    Stirring,             // かき混ぜ中
    Finalizing            // 完成
}

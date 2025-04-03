using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoupMaking.Ingredients
{
    /// <summary>
    /// 食材の基本クラス
    /// </summary>
    public class Ingredient : MonoBehaviour
    {
        [Header("基本情報")]
        [SerializeField] private string ingredientName = "にんじん";
        [SerializeField] private IngredientType ingredientType = IngredientType.Carrot;
        [SerializeField] private IngredientCategory category = IngredientCategory.Vegetable;
        
        [Header("表示設定")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite rawSprite;
        [SerializeField] private Sprite preparedSprite;
        [SerializeField] private Sprite cookedSprite;
        
        [Header("食材特性")]
        [SerializeField] private Color ingredientColor = Color.white;
        [SerializeField] private float nutritionValue = 1f;
        [SerializeField] private float flavor = 1f;
        
        // 食材の状態
        private IngredientState currentState = IngredientState.Raw;
        private bool isSelected = false;
        
        // プロパティ
        public string IngredientName => ingredientName;
        public IngredientType IngredientType => ingredientType;
        public IngredientCategory Category => category;
        public IngredientState CurrentState => currentState;
        public Color IngredientColor => ingredientColor;
        public float NutritionValue => nutritionValue;
        public float Flavor => flavor;
        public bool IsSelected => isSelected;

        private void Start()
        {
            // スプライトレンダラーの確認
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    Debug.LogError($"SpriteRenderer is missing on {ingredientName} ingredient.");
                }
            }
            
            // 初期スプライトの設定
            UpdateVisual();
        }

        /// <summary>
        /// 食材の状態を変更
        /// </summary>
        /// <param name="newState">新しい状態</param>
        public void ChangeState(IngredientState newState)
        {
            if (newState == currentState) return;
            
            // 状態を変更
            currentState = newState;
            
            // 見た目を更新
            UpdateVisual();
            
            Debug.Log($"{ingredientName} state changed to {currentState}");
        }

        /// <summary>
        /// 食材を選択状態にする
        /// </summary>
        /// <param name="selected">選択状態</param>
        public void SetSelected(bool selected)
        {
            if (isSelected == selected) return;
            
            isSelected = selected;
            
            // 選択状態の視覚効果
            if (spriteRenderer != null)
            {
                if (isSelected)
                {
                    // 選択時のエフェクト（明るく、少し大きく）
                    spriteRenderer.color = Color.white;
                    transform.localScale = Vector3.one * 1.2f;
                }
                else
                {
                    // 非選択時は通常に戻す
                    spriteRenderer.color = Color.white;
                    transform.localScale = Vector3.one;
                }
            }
        }

        /// <summary>
        /// 視覚的な表現を更新
        /// </summary>
        private void UpdateVisual()
        {
            if (spriteRenderer == null) return;
            
            // 状態に応じたスプライトを設定
            switch (currentState)
            {
                case IngredientState.Raw:
                    spriteRenderer.sprite = rawSprite;
                    break;
                
                case IngredientState.Prepared:
                    spriteRenderer.sprite = preparedSprite != null ? preparedSprite : rawSprite;
                    break;
                
                case IngredientState.Cooked:
                    spriteRenderer.sprite = cookedSprite != null ? cookedSprite : (preparedSprite != null ? preparedSprite : rawSprite);
                    break;
            }
        }

        /// <summary>
        /// 食材の色を取得
        /// </summary>
        /// <returns>食材の色</returns>
        public Color GetIngredientColor()
        {
            // 状態によって色を調整
            switch (currentState)
            {
                case IngredientState.Raw:
                    return ingredientColor;
                
                case IngredientState.Prepared:
                    // 準備状態では少し色が薄くなる
                    return new Color(
                        ingredientColor.r * 0.9f,
                        ingredientColor.g * 0.9f,
                        ingredientColor.b * 0.9f,
                        ingredientColor.a
                    );
                
                case IngredientState.Cooked:
                    // 調理状態では少し色が濃くなる
                    return new Color(
                        Mathf.Clamp01(ingredientColor.r * 1.1f),
                        Mathf.Clamp01(ingredientColor.g * 1.1f),
                        Mathf.Clamp01(ingredientColor.b * 1.1f),
                        ingredientColor.a
                    );
                
                default:
                    return ingredientColor;
            }
        }

        /// <summary>
        /// 食材を準備する（切る、皮をむくなど）
        /// </summary>
        public void Prepare()
        {
            if (currentState != IngredientState.Raw) return;
            
            // 準備状態に変更
            ChangeState(IngredientState.Prepared);
        }

        /// <summary>
        /// 食材を調理する
        /// </summary>
        public void Cook()
        {
            if (currentState == IngredientState.Cooked) return;
            
            // 調理状態に変更
            ChangeState(IngredientState.Cooked);
        }

        /// <summary>
        /// 栄養価を取得
        /// </summary>
        /// <returns>状態に応じた栄養価</returns>
        public float GetNutritionValue()
        {
            // 状態によって栄養価を調整
            switch (currentState)
            {
                case IngredientState.Raw:
                    return nutritionValue * 0.8f; // 生は栄養価が少し低い
                
                case IngredientState.Prepared:
                    return nutritionValue; // 準備済みは基本値
                
                case IngredientState.Cooked:
                    return nutritionValue * 1.2f; // 調理済みは栄養価が高い
                
                default:
                    return nutritionValue;
            }
        }

        /// <summary>
        /// 風味を取得
        /// </summary>
        /// <returns>状態に応じた風味</returns>
        public float GetFlavor()
        {
            // 状態によって風味を調整
            switch (currentState)
            {
                case IngredientState.Raw:
                    return flavor * 0.7f; // 生は風味が弱い
                
                case IngredientState.Prepared:
                    return flavor; // 準備済みは基本値
                
                case IngredientState.Cooked:
                    return flavor * 1.5f; // 調理済みは風味が強い
                
                default:
                    return flavor;
            }
        }

        /// <summary>
        /// 食材の情報文字列を取得
        /// </summary>
        /// <returns>食材情報</returns>
        public override string ToString()
        {
            return $"{ingredientName} ({currentState}): Color={ingredientColor}, Nutrition={GetNutritionValue():F1}, Flavor={GetFlavor():F1}";
        }
    }

    /// <summary>
    /// 食材の状態を表す列挙型
    /// </summary>
    public enum IngredientState
    {
        Raw,       // 生の状態
        Prepared,  // 準備済み（切る、皮をむくなど）
        Cooked     // 調理済み
    }

    /// <summary>
    /// 食材のタイプを表す列挙型
    /// </summary>
    public enum IngredientType
    {
        // 野菜類
        Carrot,
        Onion,
        Potato,
        Tomato,
        Broccoli,
        Spinach,
        Pumpkin,
        Radish,
        Cucumber,
        Corn,
        
        // 果物類
        Apple,
        Banana,
        Strawberry,
        Orange,
        Grape,
        Pineapple,
        
        // タンパク質類
        Chicken,
        Beef,
        Egg,
        Tofu,
        Shrimp,
        Fish,
        Cheese,
        Beans,
        
        // 調味料
        Salt,
        Sugar,
        SoySauce,
        Miso,
        Butter,
        OliveOil
    }
}
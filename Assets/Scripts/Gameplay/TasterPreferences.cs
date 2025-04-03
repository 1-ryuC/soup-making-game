using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoupMaking.Ingredients;

namespace SoupMaking.Gameplay
{
    /// <summary>
    /// 試食キャラクターの好みデータを管理するクラス
    /// </summary>
    public class TasterPreferences : MonoBehaviour
    {
        [Header("好みの設定")]
        [SerializeField] private string tasterName = "くまさん";
        [SerializeField] private string tasterDescription = "甘いスープと赤い食材が好き";
        
        [Header("好きな食材")]
        [SerializeField] private List<IngredientType> likedIngredients = new List<IngredientType>();
        
        [Header("嫌いな食材")]
        [SerializeField] private List<IngredientType> dislikedIngredients = new List<IngredientType>();
        
        [Header("好きな食材カテゴリ")]
        [SerializeField] private List<IngredientCategory> likedCategories = new List<IngredientCategory>();
        
        // プロパティ
        public string TasterName => tasterName;
        public string TasterDescription => tasterDescription;
        public List<IngredientType> LikedIngredients => likedIngredients;
        public List<IngredientType> DislikedIngredients => dislikedIngredients;
        public List<IngredientCategory> LikedCategories => likedCategories;

        private void Start()
        {
            // 好みデータが空の場合、ランダムに設定（デモ/テスト用）
            if (likedIngredients.Count == 0 && dislikedIngredients.Count == 0 && likedCategories.Count == 0)
            {
                GenerateRandomPreferences();
            }
        }

        /// <summary>
        /// ランダムな好みデータを生成（デモ/テスト用）
        /// </summary>
        private void GenerateRandomPreferences()
        {
            // すべての食材タイプを列挙
            var allIngredientTypes = System.Enum.GetValues(typeof(IngredientType)) as IngredientType[];
            
            // すべてのカテゴリを列挙
            var allCategories = System.Enum.GetValues(typeof(IngredientCategory)) as IngredientCategory[];
            
            // ランダムに1〜3個の好きな食材を設定
            int likedCount = Random.Range(1, 4);
            for (int i = 0; i < likedCount; i++)
            {
                IngredientType randomType = allIngredientTypes[Random.Range(0, allIngredientTypes.Length)];
                if (!likedIngredients.Contains(randomType))
                {
                    likedIngredients.Add(randomType);
                }
            }
            
            // ランダムに1〜2個の嫌いな食材を設定
            int dislikedCount = Random.Range(1, 3);
            for (int i = 0; i < dislikedCount; i++)
            {
                IngredientType randomType = allIngredientTypes[Random.Range(0, allIngredientTypes.Length)];
                if (!likedIngredients.Contains(randomType) && !dislikedIngredients.Contains(randomType))
                {
                    dislikedIngredients.Add(randomType);
                }
            }
            
            // ランダムに1個の好きなカテゴリを設定
            IngredientCategory randomCategory = allCategories[Random.Range(0, allCategories.Length)];
            likedCategories.Add(randomCategory);
            
            Debug.Log($"Generated random preferences for {tasterName}");
        }

        /// <summary>
        /// 特定の食材に対する好み度合いを取得
        /// </summary>
        /// <param name="ingredientType">評価する食材タイプ</param>
        /// <returns>好み度合い（-2:大嫌い、-1:嫌い、0:普通、1:好き、2:大好き）</returns>
        public int GetPreferenceForIngredient(IngredientType ingredientType)
        {
            // 好きな食材リストに含まれる場合
            if (likedIngredients.Contains(ingredientType))
            {
                return 2; // 大好き
            }
            
            // 嫌いな食材リストに含まれる場合
            if (dislikedIngredients.Contains(ingredientType))
            {
                return -2; // 大嫌い
            }
            
            // 食材タイプに対応するカテゴリを取得
            IngredientCategory category = GetCategoryForIngredientType(ingredientType);
            
            // 好きなカテゴリリストに含まれる場合
            if (likedCategories.Contains(category))
            {
                return 1; // 好き
            }
            
            // それ以外は普通
            return 0;
        }

        /// <summary>
        /// 食材タイプからカテゴリを取得
        /// </summary>
        /// <param name="ingredientType">食材タイプ</param>
        /// <returns>対応するカテゴリ</returns>
        private IngredientCategory GetCategoryForIngredientType(IngredientType ingredientType)
        {
            // 食材タイプとカテゴリのマッピング（仮実装）
            // 実際の実装では、それぞれの食材タイプが属するカテゴリを適切にマッピングする
            if (ingredientType <= IngredientType.Corn)
            {
                return IngredientCategory.Vegetable;
            }
            else if (ingredientType <= IngredientType.Pineapple)
            {
                return IngredientCategory.Fruit;
            }
            else if (ingredientType <= IngredientType.Beans)
            {
                return IngredientCategory.Protein;
            }
            else
            {
                return IngredientCategory.Seasoning;
            }
        }
    }
}
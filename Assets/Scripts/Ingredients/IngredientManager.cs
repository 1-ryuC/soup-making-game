using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoupMaking.Ingredients
{
    /// <summary>
    /// 食材全体を管理するマネージャークラス
    /// </summary>
    public class IngredientManager : MonoBehaviour
    {
        public static IngredientManager Instance { get; private set; }

        [Header("食材プレハブ")]
        [SerializeField] private List<GameObject> vegetablePrefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> fruitPrefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> proteinPrefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> seasoningPrefabs = new List<GameObject>();

        [Header("食材スポーン設定")]
        [SerializeField] private Transform vegetableStoragePoint;
        [SerializeField] private Transform fruitStoragePoint;
        [SerializeField] private Transform proteinStoragePoint;
        [SerializeField] private Transform seasoningStoragePoint;

        // 生成された食材の辞書（タイプごと）
        private Dictionary<IngredientType, Ingredient> ingredientsDict = new Dictionary<IngredientType, Ingredient>();
        
        // 現在選択されている食材
        private Ingredient selectedIngredient = null;
        
        // イベント
        public event Action<Ingredient> OnIngredientSelected;
        public event Action<Ingredient> OnIngredientDeselected;
        public event Action<Ingredient> OnIngredientSpawned;

        private void Awake()
        {
            // シングルトンパターン
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        private void Start()
        {
            // 食材ストレージポイントの確認
            ValidateStoragePoints();
            
            // 初期食材の生成
            SpawnInitialIngredients();
        }

        /// <summary>
        /// ストレージポイントの検証
        /// </summary>
        private void ValidateStoragePoints()
        {
            if (vegetableStoragePoint == null)
            {
                Debug.LogError("Vegetable storage point is not assigned.");
            }
            
            if (fruitStoragePoint == null)
            {
                Debug.LogError("Fruit storage point is not assigned.");
            }
            
            if (proteinStoragePoint == null)
            {
                Debug.LogError("Protein storage point is not assigned.");
            }
            
            if (seasoningStoragePoint == null)
            {
                Debug.LogError("Seasoning storage point is not assigned.");
            }
        }

        /// <summary>
        /// 初期食材を生成
        /// </summary>
        private void SpawnInitialIngredients()
        {
            // 各カテゴリの食材を生成
            SpawnIngredientsOfCategory(IngredientCategory.Vegetable, vegetablePrefabs, vegetableStoragePoint);
            SpawnIngredientsOfCategory(IngredientCategory.Fruit, fruitPrefabs, fruitStoragePoint);
            SpawnIngredientsOfCategory(IngredientCategory.Protein, proteinPrefabs, proteinStoragePoint);
            SpawnIngredientsOfCategory(IngredientCategory.Seasoning, seasoningPrefabs, seasoningStoragePoint);
            
            Debug.Log($"Spawned {ingredientsDict.Count} ingredients");
        }

        /// <summary>
        /// 特定カテゴリの食材を生成
        /// </summary>
        private void SpawnIngredientsOfCategory(IngredientCategory category, List<GameObject> prefabs, Transform storagePoint)
        {
            if (prefabs == null || prefabs.Count == 0 || storagePoint == null) return;
            
            // 格子状に配置するための変数
            int itemsPerRow = 4;
            float spacing = 1.5f;
            
            // 各プレハブを生成
            for (int i = 0; i < prefabs.Count; i++)
            {
                // 格子位置を計算
                int row = i / itemsPerRow;
                int col = i % itemsPerRow;
                
                Vector3 position = storagePoint.position + new Vector3(
                    col * spacing,
                    0,
                    row * spacing
                );
                
                // 食材を生成
                GameObject ingredientObj = Instantiate(prefabs[i], position, Quaternion.identity, storagePoint);
                ingredientObj.name = $"Ingredient_{prefabs[i].name}";
                
                // 食材コンポーネントを取得
                Ingredient ingredient = ingredientObj.GetComponent<Ingredient>();
                if (ingredient != null)
                {
                    // 辞書に追加
                    if (!ingredientsDict.ContainsKey(ingredient.IngredientType))
                    {
                        ingredientsDict.Add(ingredient.IngredientType, ingredient);
                        
                        // 生成イベント発火
                        OnIngredientSpawned?.Invoke(ingredient);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate ingredient type: {ingredient.IngredientType}");
                    }
                }
                else
                {
                    Debug.LogError($"Ingredient component missing on {prefabs[i].name}");
                }
            }
        }

        /// <summary>
        /// 特定タイプの食材を取得
        /// </summary>
        /// <param name="type">食材タイプ</param>
        /// <returns>食材インスタンス（なければnull）</returns>
        public Ingredient GetIngredient(IngredientType type)
        {
            if (ingredientsDict.TryGetValue(type, out Ingredient ingredient))
            {
                return ingredient;
            }
            
            return null;
        }

        /// <summary>
        /// 特定のカテゴリに属する食材のリストを取得
        /// </summary>
        /// <param name="category">食材カテゴリ</param>
        /// <returns>食材リスト</returns>
        public List<Ingredient> GetIngredientsOfCategory(IngredientCategory category)
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            
            foreach (var ingredient in ingredientsDict.Values)
            {
                if (ingredient.Category == category)
                {
                    ingredients.Add(ingredient);
                }
            }
            
            return ingredients;
        }

        /// <summary>
        /// 食材を選択する
        /// </summary>
        /// <param name="ingredient">選択する食材</param>
        public void SelectIngredient(Ingredient ingredient)
        {
            if (ingredient == null) return;
            
            // 現在選択中の食材があれば選択解除
            if (selectedIngredient != null)
            {
                DeselectIngredient();
            }
            
            // 新しい食材を選択
            selectedIngredient = ingredient;
            selectedIngredient.SetSelected(true);
            
            // 選択イベント発火
            OnIngredientSelected?.Invoke(selectedIngredient);
            
            Debug.Log($"Selected ingredient: {selectedIngredient.IngredientName}");
        }

        /// <summary>
        /// 選択中の食材を選択解除
        /// </summary>
        public void DeselectIngredient()
        {
            if (selectedIngredient == null) return;
            
            // 選択状態を解除
            selectedIngredient.SetSelected(false);
            
            // 選択解除イベント発火
            OnIngredientDeselected?.Invoke(selectedIngredient);
            
            Debug.Log($"Deselected ingredient: {selectedIngredient.IngredientName}");
            
            // 選択中食材をクリア
            selectedIngredient = null;
        }

        /// <summary>
        /// 現在選択中の食材を取得
        /// </summary>
        /// <returns>選択中の食材（なければnull）</returns>
        public Ingredient GetSelectedIngredient()
        {
            return selectedIngredient;
        }

        /// <summary>
        /// 新しい食材インスタンスを生成
        /// </summary>
        /// <param name="type">食材タイプ</param>
        /// <param name="position">生成位置</param>
        /// <returns>生成された食材インスタンス</returns>
        public Ingredient CreateNewIngredientInstance(IngredientType type, Vector3 position)
        {
            // 元の食材を取得
            Ingredient originalIngredient = GetIngredient(type);
            if (originalIngredient == null)
            {
                Debug.LogError($"Original ingredient of type {type} not found.");
                return null;
            }
            
            // 新しいインスタンスを生成
            GameObject newObj = Instantiate(originalIngredient.gameObject, position, Quaternion.identity);
            newObj.name = $"Ingredient_{type}_Instance";
            
            // コンポーネントを取得
            Ingredient newIngredient = newObj.GetComponent<Ingredient>();
            
            return newIngredient;
        }

        /// <summary>
        /// ランダムな食材を取得
        /// </summary>
        /// <returns>ランダムな食材</returns>
        public Ingredient GetRandomIngredient()
        {
            if (ingredientsDict.Count == 0) return null;
            
            // ランダムなインデックスを生成
            int randomIndex = UnityEngine.Random.Range(0, ingredientsDict.Count);
            
            // インデックスに対応する食材を取得
            int i = 0;
            foreach (var ingredient in ingredientsDict.Values)
            {
                if (i == randomIndex)
                {
                    return ingredient;
                }
                i++;
            }
            
            // 通常ここには到達しない
            return null;
        }

        /// <summary>
        /// 食材のクリックイベントハンドラ
        /// </summary>
        /// <param name="ingredient">クリックされた食材</param>
        public void OnIngredientClicked(Ingredient ingredient)
        {
            if (ingredient == null) return;
            
            // 既に選択中の食材なら選択解除
            if (ingredient == selectedIngredient)
            {
                DeselectIngredient();
            }
            else
            {
                // 新しい食材を選択
                SelectIngredient(ingredient);
            }
        }
    }
}
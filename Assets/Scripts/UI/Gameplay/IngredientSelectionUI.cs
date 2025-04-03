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
    /// 食材選択画面のUIを管理するクラス
    /// </summary>
    public class IngredientSelectionUI : MonoBehaviour
    {
        [Header("カテゴリタブ")]
        [SerializeField] private Button vegetableTabButton;
        [SerializeField] private Button fruitTabButton;
        [SerializeField] private Button proteinTabButton;
        [SerializeField] private Button seasoningTabButton;
        
        [Header("食材グリッド")]
        [SerializeField] private Transform ingredientGrid;
        [SerializeField] private GameObject ingredientButtonPrefab;
        
        [Header("情報パネル")]
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private TextMeshProUGUI ingredientNameText;
        [SerializeField] private TextMeshProUGUI ingredientDescriptionText;
        [SerializeField] private Image ingredientImage;
        
        [Header("アクションボタン")]
        [SerializeField] private Button addToPotButton;
        [SerializeField] private Button finishSelectionButton;
        
        // 参照
        private IngredientManager ingredientManager;
        private CookingSystem cookingSystem;
        private GameplayManager gameplayManager;
        
        // 現在選択中のカテゴリ
        private IngredientCategory currentCategory = IngredientCategory.Vegetable;
        
        // 生成された食材ボタンの辞書
        private Dictionary<IngredientType, Button> ingredientButtons = new Dictionary<IngredientType, Button>();
        
        private void Start()
        {
            // マネージャーの参照を取得
            ingredientManager = IngredientManager.Instance;
            if (ingredientManager == null)
            {
                Debug.LogError("IngredientManager is not found in the scene.");
            }
            
            cookingSystem = FindObjectOfType<CookingSystem>();
            if (cookingSystem == null)
            {
                Debug.LogError("CookingSystem is not found in the scene.");
            }
            
            gameplayManager = GameplayManager.Instance;
            if (gameplayManager == null)
            {
                Debug.LogError("GameplayManager is not found in the scene.");
            }
            
            // タブボタンのイベント設定
            SetupTabButtons();
            
            // アクションボタンのイベント設定
            SetupActionButtons();
            
            // 初期状態を設定
            Initialize();
        }
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            // 情報パネルを非表示
            HideInfoPanel();
            
            // 最初のカテゴリを表示
            ShowCategory(IngredientCategory.Vegetable);
        }
        
        /// <summary>
        /// タブボタンのイベント設定
        /// </summary>
        private void SetupTabButtons()
        {
            if (vegetableTabButton != null)
            {
                vegetableTabButton.onClick.AddListener(() => ShowCategory(IngredientCategory.Vegetable));
            }
            
            if (fruitTabButton != null)
            {
                fruitTabButton.onClick.AddListener(() => ShowCategory(IngredientCategory.Fruit));
            }
            
            if (proteinTabButton != null)
            {
                proteinTabButton.onClick.AddListener(() => ShowCategory(IngredientCategory.Protein));
            }
            
            if (seasoningTabButton != null)
            {
                seasoningTabButton.onClick.AddListener(() => ShowCategory(IngredientCategory.Seasoning));
            }
        }
        
        /// <summary>
        /// アクションボタンのイベント設定
        /// </summary>
        private void SetupActionButtons()
        {
            if (addToPotButton != null)
            {
                addToPotButton.onClick.AddListener(AddSelectedIngredientToPot);
            }
            
            if (finishSelectionButton != null)
            {
                finishSelectionButton.onClick.AddListener(FinishIngredientSelection);
            }
        }
        
        /// <summary>
        /// 特定カテゴリの食材を表示
        /// </summary>
        /// <param name="category">表示するカテゴリ</param>
        public void ShowCategory(IngredientCategory category)
        {
            // 現在のカテゴリを更新
            currentCategory = category;
            
            // タブボタンの見た目を更新
            UpdateTabButtonsAppearance();
            
            // 食材グリッドをクリア
            ClearIngredientGrid();
            
            // 選択情報をクリア
            HideInfoPanel();
            
            // 食材マネージャーが利用可能なら
            if (ingredientManager != null)
            {
                // 指定カテゴリの食材リストを取得
                List<Ingredient> ingredients = ingredientManager.GetIngredientsOfCategory(category);
                
                // 食材ボタンを生成
                foreach (var ingredient in ingredients)
                {
                    CreateIngredientButton(ingredient);
                }
            }
        }
        
        /// <summary>
        /// タブボタンの見た目を更新
        /// </summary>
        private void UpdateTabButtonsAppearance()
        {
            // 選択されたカテゴリのタブを強調表示
            if (vegetableTabButton != null)
            {
                vegetableTabButton.interactable = currentCategory != IngredientCategory.Vegetable;
            }
            
            if (fruitTabButton != null)
            {
                fruitTabButton.interactable = currentCategory != IngredientCategory.Fruit;
            }
            
            if (proteinTabButton != null)
            {
                proteinTabButton.interactable = currentCategory != IngredientCategory.Protein;
            }
            
            if (seasoningTabButton != null)
            {
                seasoningTabButton.interactable = currentCategory != IngredientCategory.Seasoning;
            }
        }
        
        /// <summary>
        /// 食材グリッドをクリア
        /// </summary>
        private void ClearIngredientGrid()
        {
            // 既存のボタンを削除
            foreach (Transform child in ingredientGrid)
            {
                Destroy(child.gameObject);
            }
            
            // ボタン辞書をクリア
            ingredientButtons.Clear();
        }
        
        /// <summary>
        /// 食材ボタンを生成
        /// </summary>
        /// <param name="ingredient">ボタンに対応する食材</param>
        private void CreateIngredientButton(Ingredient ingredient)
        {
            if (ingredientButtonPrefab == null || ingredientGrid == null) return;
            
            // ボタンを生成
            GameObject buttonObj = Instantiate(ingredientButtonPrefab, ingredientGrid);
            buttonObj.name = $"Button_{ingredient.IngredientName}";
            
            // ボタンコンポーネント取得
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                // ボタンイベント設定
                button.onClick.AddListener(() => OnIngredientButtonClicked(ingredient));
                
                // ボタン辞書に追加
                ingredientButtons[ingredient.IngredientType] = button;
                
                // ボタン表示を設定
                SetupIngredientButtonDisplay(button, ingredient);
            }
        }
        
        /// <summary>
        /// 食材ボタンの表示を設定
        /// </summary>
        /// <param name="button">設定するボタン</param>
        /// <param name="ingredient">対応する食材</param>
        private void SetupIngredientButtonDisplay(Button button, Ingredient ingredient)
        {
            // ボタンアイコン
            Image iconImage = button.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                // スプライトレンダラーからスプライトを取得
                SpriteRenderer spriteRenderer = ingredient.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    iconImage.sprite = spriteRenderer.sprite;
                    iconImage.color = Color.white;
                }
            }
            
            // ボタンテキスト
            TextMeshProUGUI nameText = button.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = ingredient.IngredientName;
            }
        }
        
        /// <summary>
        /// 食材ボタンクリック時のイベントハンドラ
        /// </summary>
        /// <param name="ingredient">クリックされた食材</param>
        private void OnIngredientButtonClicked(Ingredient ingredient)
        {
            // 食材を選択
            if (ingredientManager != null)
            {
                ingredientManager.SelectIngredient(ingredient);
            }
            
            // 情報パネルを表示
            ShowIngredientInfo(ingredient);
        }
        
        /// <summary>
        /// 食材情報パネルを表示
        /// </summary>
        /// <param name="ingredient">表示する食材</param>
        private void ShowIngredientInfo(Ingredient ingredient)
        {
            if (infoPanel == null) return;
            
            // パネルを表示
            infoPanel.SetActive(true);
            
            // 食材名
            if (ingredientNameText != null)
            {
                ingredientNameText.text = ingredient.IngredientName;
            }
            
            // 食材説明（サンプル）
            if (ingredientDescriptionText != null)
            {
                string description = GetSampleDescription(ingredient);
                ingredientDescriptionText.text = description;
            }
            
            // 食材画像
            if (ingredientImage != null)
            {
                // スプライトレンダラーからスプライトを取得
                SpriteRenderer spriteRenderer = ingredient.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    ingredientImage.sprite = spriteRenderer.sprite;
                    ingredientImage.color = Color.white;
                }
            }
            
            // 鍋に追加ボタンの有効化
            if (addToPotButton != null)
            {
                addToPotButton.interactable = true;
            }
        }
        
        /// <summary>
        /// 食材の説明文を取得（サンプル実装）
        /// </summary>
        /// <param name="ingredient">説明を取得する食材</param>
        /// <returns>説明文</returns>
        private string GetSampleDescription(Ingredient ingredient)
        {
            // カテゴリに応じたサンプル説明
            switch (ingredient.Category)
            {
                case IngredientCategory.Vegetable:
                    return $"{ingredient.IngredientName}は栄養たっぷりの野菜です。\n栄養価: {ingredient.NutritionValue:F1}";
                
                case IngredientCategory.Fruit:
                    return $"{ingredient.IngredientName}は甘くておいしい果物です。\n風味: {ingredient.Flavor:F1}";
                
                case IngredientCategory.Protein:
                    return $"{ingredient.IngredientName}はタンパク質が豊富です。\n栄養価: {ingredient.NutritionValue:F1}";
                
                case IngredientCategory.Seasoning:
                    return $"{ingredient.IngredientName}はスープの味を引き立てます。\n風味: {ingredient.Flavor:F1}";
                
                default:
                    return $"{ingredient.IngredientName}";
            }
        }
        
        /// <summary>
        /// 情報パネルを非表示
        /// </summary>
        private void HideInfoPanel()
        {
            if (infoPanel != null)
            {
                infoPanel.SetActive(false);
            }
            
            // 鍋に追加ボタンの無効化
            if (addToPotButton != null)
            {
                addToPotButton.interactable = false;
            }
        }
        
        /// <summary>
        /// 選択中の食材を鍋に追加
        /// </summary>
        private void AddSelectedIngredientToPot()
        {
            // 選択中の食材を取得
            Ingredient selectedIngredient = ingredientManager?.GetSelectedIngredient();
            if (selectedIngredient == null) return;
            
            // 調理システムに食材を追加
            if (cookingSystem != null)
            {
                // 新しい食材インスタンスを作成
                Ingredient newIngredient = ingredientManager.CreateNewIngredientInstance(
                    selectedIngredient.IngredientType, 
                    selectedIngredient.transform.position + Vector3.up * 0.5f
                );
                
                if (newIngredient != null)
                {
                    // 食材を落とす演出
                    StartCoroutine(cookingSystem.DropIngredientAnimation(newIngredient, newIngredient.transform.position));
                }
            }
            
            // 選択解除
            ingredientManager.DeselectIngredient();
            
            // 情報パネルを非表示
            HideInfoPanel();
        }
        
        /// <summary>
        /// 食材選択を完了
        /// </summary>
        private void FinishIngredientSelection()
        {
            // 調理システムに食材が1つ以上追加されていることを確認
            if (cookingSystem != null && cookingSystem.AddedIngredients.Count > 0)
            {
                // 調理フェーズに移行
                if (gameplayManager != null)
                {
                    gameplayManager.StartCooking();
                }
            }
            else
            {
                Debug.Log("少なくとも1つの食材を追加してください。");
                // TODO: ユーザーへのフィードバック（テキストメッセージなど）
            }
        }
    }
}
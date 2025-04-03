using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SoupMaking.Gameplay;

namespace SoupMaking.UI.Gameplay
{
    /// <summary>
    /// 結果画面UIを管理するクラス
    /// </summary>
    public class ResultUI : MonoBehaviour
    {
        [Header("結果表示")]
        [SerializeField] private GameObject[] starObjects;  // 星評価の表示オブジェクト
        [SerializeField] private GameObject starEffectPrefab;  // 星獲得時のエフェクトプレハブ
        [SerializeField] private Transform starEffectParent;  // エフェクト生成親
        [SerializeField] private TextMeshProUGUI resultTitleText;  // 結果タイトルテキスト
        [SerializeField] private TextMeshProUGUI resultDescriptionText;  // 結果説明テキスト
        
        [Header("スープ情報表示")]
        [SerializeField] private Image soupImage;  // スープ画像
        [SerializeField] private TextMeshProUGUI ingredientCountText;  // 食材数テキスト
        [SerializeField] private Transform usedIngredientsContainer;  // 使用食材一覧表示コンテナ
        [SerializeField] private GameObject ingredientIconPrefab;  // 食材アイコンプレハブ
        
        [Header("ボタン")]
        [SerializeField] private Button retryButton;  // もう一度ボタン
        [SerializeField] private Button mainMenuButton;  // メインメニューボタン
        [SerializeField] private Button saveRecipeButton;  // レシピ保存ボタン（フリープレイモード時のみ有効）
        
        [Header("アニメーション")]
        [SerializeField] private Animator panelAnimator;  // パネルアニメーター
        [SerializeField] private float starRevealDelay = 0.5f;  // 星表示の遅延時間
        
        // 参照
        private CookingSystem cookingSystem;
        private TastingSystem tastingSystem;
        
        // 評価結果
        private int starRating = 0;
        
        private void Start()
        {
            // 調理システムへの参照を取得
            cookingSystem = FindObjectOfType<CookingSystem>();
            if (cookingSystem == null)
            {
                Debug.LogError("CookingSystem not found in scene.");
            }
            
            // 試食システムへの参照を取得
            tastingSystem = FindObjectOfType<TastingSystem>();
            if (tastingSystem == null)
            {
                Debug.LogError("TastingSystem not found in scene.");
            }
            
            // ボタンリスナー設定
            SetupButtonListeners();
            
            // 初期状態では星を非表示
            if (starObjects != null)
            {
                foreach (var star in starObjects)
                {
                    if (star != null)
                    {
                        star.SetActive(false);
                    }
                }
            }
        }
        
        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // 結果表示の準備
            PrepareResultDisplay();
            
            // アニメーション開始
            StartCoroutine(ResultRevealSequence());
            
            Debug.Log("ResultUI initialized");
        }
        
        /// <summary>
        /// ボタンリスナーの設定
        /// </summary>
        private void SetupButtonListeners()
        {
            // リトライボタン
            if (retryButton != null)
            {
                retryButton.onClick.AddListener(OnRetryButtonClicked);
            }
            
            // メインメニューボタン
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            }
            
            // レシピ保存ボタン
            if (saveRecipeButton != null)
            {
                saveRecipeButton.onClick.AddListener(OnSaveRecipeButtonClicked);
                
                // フリープレイモード以外では非表示
                GameMode currentMode = GameplayManager.Instance != null 
                    ? GameplayManager.Instance.CurrentGameMode 
                    : GameMode.FreePlay;
                
                saveRecipeButton.gameObject.SetActive(currentMode == GameMode.FreePlay);
            }
        }
        
        /// <summary>
        /// 結果表示の準備
        /// </summary>
        private void PrepareResultDisplay()
        {
            // 評価の取得
            GetRatingFromSystems();
            
            // スープ情報の表示
            UpdateSoupDisplay();
            
            // 使用食材一覧表示
            DisplayUsedIngredients();
            
            // 評価に応じた結果テキスト設定
            SetResultText();
        }
        
        /// <summary>
        /// システムから評価を取得
        /// </summary>
        private void GetRatingFromSystems()
        {
            // 試食システムから総合評価を取得
            if (tastingSystem != null)
            {
                starRating = tastingSystem.GetOverallRating();
            }
            else if (cookingSystem != null)
            {
                // 試食システムがない場合は調理評価のみ
                starRating = cookingSystem.EvaluateCooking();
            }
            else
            {
                // どちらもない場合はデフォルト値
                starRating = 3;
            }
            
            // 範囲を制限
            starRating = Mathf.Clamp(starRating, 0, 5);
            
            Debug.Log($"Final star rating: {starRating}");
        }
        
        /// <summary>
        /// スープ情報の表示更新
        /// </summary>
        private void UpdateSoupDisplay()
        {
            if (cookingSystem == null) return;
            
            // スープの色を設定
            if (soupImage != null)
            {
                soupImage.color = cookingSystem.CalculateSoupColor();
            }
            
            // 食材数の表示
            if (ingredientCountText != null)
            {
                int count = cookingSystem.AddedIngredients.Count;
                ingredientCountText.text = $"つかった しょくざい: {count}こ";
            }
        }
        
        /// <summary>
        /// 使用した食材一覧の表示
        /// </summary>
        private void DisplayUsedIngredients()
        {
            if (cookingSystem == null || usedIngredientsContainer == null || ingredientIconPrefab == null) return;
            
            // 既存のアイコンをクリア
            foreach (Transform child in usedIngredientsContainer)
            {
                Destroy(child.gameObject);
            }
            
            // 食材ごとにアイコンを生成
            foreach (var ingredient in cookingSystem.AddedIngredients)
            {
                GameObject iconObj = Instantiate(ingredientIconPrefab, usedIngredientsContainer);
                
                // アイコン画像設定
                Image iconImage = iconObj.GetComponentInChildren<Image>();
                if (iconImage != null && ingredient != null)
                {
                    // 食材のアイコン画像を取得（実装によって異なる）
                    Sprite ingredientSprite = ingredient.GetComponentInChildren<SpriteRenderer>()?.sprite;
                    if (ingredientSprite != null)
                    {
                        iconImage.sprite = ingredientSprite;
                    }
                }
                
                // 食材名表示
                TextMeshProUGUI nameText = iconObj.GetComponentInChildren<TextMeshProUGUI>();
                if (nameText != null && ingredient != null)
                {
                    nameText.text = ingredient.name.Replace("(Clone)", "");
                }
            }
        }
        
        /// <summary>
        /// 評価に応じた結果テキスト設定
        /// </summary>
        private void SetResultText()
        {
            if (resultTitleText == null || resultDescriptionText == null) return;
            
            // 星の数に応じたテキスト設定
            switch (starRating)
            {
                case 5:
                    resultTitleText.text = "だいせいこう！";
                    resultDescriptionText.text = "とってもおいしいスープができたね！\nみんなだいよろこび！";
                    break;
                
                case 4:
                    resultTitleText.text = "おおせいこう！";
                    resultDescriptionText.text = "とてもおいしいスープができたね！\nみんなよろこんでるよ！";
                    break;
                
                case 3:
                    resultTitleText.text = "せいこう！";
                    resultDescriptionText.text = "おいしいスープができたね！\nみんなたのしそう！";
                    break;
                
                case 2:
                    resultTitleText.text = "おしい！";
                    resultDescriptionText.text = "もうすこしでおいしくなりそう！\nつぎはもっとがんばろう！";
                    break;
                
                case 1:
                case 0:
                    resultTitleText.text = "ざんねん...";
                    resultDescriptionText.text = "むずかしかったね。\nべつのしょくざいもためしてみよう！";
                    break;
            }
        }
        
        /// <summary>
        /// 結果表示シーケンス
        /// </summary>
        private IEnumerator ResultRevealSequence()
        {
            // パネルアニメーション
            if (panelAnimator != null)
            {
                panelAnimator.SetTrigger("Show");
            }
            
            // 少し待機
            yield return new WaitForSeconds(1.0f);
            
            // 星を順番に表示
            for (int i = 0; i < starRating && i < starObjects.Length; i++)
            {
                if (starObjects[i] != null)
                {
                    starObjects[i].SetActive(true);
                    
                    // 星エフェクト再生
                    PlayStarEffect(starObjects[i].transform.position);
                    
                    // 効果音（AudioManager経由で再生）
                    // AudioManager.Instance.PlaySFX("Star");
                    
                    // 次の星まで待機
                    yield return new WaitForSeconds(starRevealDelay);
                }
            }
            
            // 全ての星表示後に少し待機
            yield return new WaitForSeconds(0.5f);
            
            // 結果テキストアニメーション
            if (resultTitleText != null)
            {
                resultTitleText.gameObject.SetActive(true);
                StartCoroutine(AnimateTextScale(resultTitleText.transform));
            }
            
            yield return new WaitForSeconds(0.5f);
            
            if (resultDescriptionText != null)
            {
                resultDescriptionText.gameObject.SetActive(true);
            }
            
            // ボタンを有効化
            EnableButtons();
        }
        
        /// <summary>
        /// 星獲得エフェクトの再生
        /// </summary>
        /// <param name="position">エフェクト位置</param>
        private void PlayStarEffect(Vector3 position)
        {
            if (starEffectPrefab == null || starEffectParent == null) return;
            
            // エフェクト生成
            GameObject effect = Instantiate(starEffectPrefab, position, Quaternion.identity, starEffectParent);
            
            // 自動削除
            Destroy(effect, 2.0f);
        }
        
        /// <summary>
        /// テキストのスケールアニメーション
        /// </summary>
        private IEnumerator AnimateTextScale(Transform textTransform)
        {
            Vector3 originalScale = textTransform.localScale;
            Vector3 targetScale = originalScale * 1.2f;
            
            float duration = 0.3f;
            float time = 0;
            
            // 拡大
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                
                textTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                
                yield return null;
            }
            
            // 縮小
            time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                
                textTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                
                yield return null;
            }
            
            textTransform.localScale = originalScale;
        }
        
        /// <summary>
        /// ボタンの有効化
        /// </summary>
        private void EnableButtons()
        {
            if (retryButton != null)
            {
                retryButton.gameObject.SetActive(true);
                StartCoroutine(AnimateButtonAppear(retryButton.transform));
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.gameObject.SetActive(true);
                StartCoroutine(AnimateButtonAppear(mainMenuButton.transform, 0.2f));
            }
            
            if (saveRecipeButton != null && saveRecipeButton.gameObject.activeSelf)
            {
                StartCoroutine(AnimateButtonAppear(saveRecipeButton.transform, 0.4f));
            }
        }
        
        /// <summary>
        /// ボタン出現アニメーション
        /// </summary>
        private IEnumerator AnimateButtonAppear(Transform buttonTransform, float delay = 0f)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one;
            
            buttonTransform.localScale = startScale;
            
            float duration = 0.3f;
            float time = 0;
            
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, time / duration);
                
                buttonTransform.localScale = Vector3.Lerp(startScale, endScale, t);
                
                yield return null;
            }
            
            buttonTransform.localScale = endScale;
        }
        
        #region ボタンイベントハンドラ
        
        /// <summary>
        /// リトライボタンがクリックされた時の処理
        /// </summary>
        private void OnRetryButtonClicked()
        {
            // ボタンアニメーション
            PlayButtonAnimation(retryButton.gameObject);
            
            // ゲームをリセット
            GameplayManager gameplayManager = GameplayManager.Instance;
            if (gameplayManager != null)
            {
                gameplayManager.ResetGame();
            }
        }
        
        /// <summary>
        /// メインメニューボタンがクリックされた時の処理
        /// </summary>
        private void OnMainMenuButtonClicked()
        {
            // ボタンアニメーション
            PlayButtonAnimation(mainMenuButton.gameObject);
            
            // メインメニューシーンに遷移
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        
        /// <summary>
        /// レシピ保存ボタンがクリックされた時の処理
        /// </summary>
        private void OnSaveRecipeButtonClicked()
        {
            // ボタンアニメーション
            PlayButtonAnimation(saveRecipeButton.gameObject);
            
            // レシピを保存する処理（未実装）
            Debug.Log("Save recipe functionality not implemented yet");
            
            // 保存完了通知など
            StartCoroutine(ShowSaveConfirmation());
        }
        
        /// <summary>
        /// 保存確認メッセージ表示
        /// </summary>
        private IEnumerator ShowSaveConfirmation()
        {
            // レシピ保存ボタンのテキストを変更
            TextMeshProUGUI buttonText = saveRecipeButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                string originalText = buttonText.text;
                buttonText.text = "ほぞんしました！";
                
                // 一定時間後に元に戻す
                yield return new WaitForSeconds(1.5f);
                
                buttonText.text = originalText;
            }
        }
        
        #endregion
        
        /// <summary>
        /// ボタンのアニメーション再生
        /// </summary>
        /// <param name="buttonObj">アニメーションを再生するボタンオブジェクト</param>
        private void PlayButtonAnimation(GameObject buttonObj)
        {
            // スケールアニメーション
            StartCoroutine(ButtonPressAnimation(buttonObj));
        }
        
        /// <summary>
        /// ボタン押下アニメーションコルーチン
        /// </summary>
        private IEnumerator ButtonPressAnimation(GameObject buttonObj)
        {
            Vector3 originalScale = buttonObj.transform.localScale;
            Vector3 pressedScale = originalScale * 0.9f;
            
            // 押下
            float duration = 0.1f;
            float time = 0;
            
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                
                buttonObj.transform.localScale = Vector3.Lerp(originalScale, pressedScale, t);
                
                yield return null;
            }
            
            // 戻る
            time = 0;
            
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                
                buttonObj.transform.localScale = Vector3.Lerp(pressedScale, originalScale, t);
                
                yield return null;
            }
            
            buttonObj.transform.localScale = originalScale;
        }
    }
}
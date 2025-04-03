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
    /// 調理アクションUIを管理するクラス
    /// </summary>
    public class CookingActionUI : MonoBehaviour
    {
        [Header("調理アクションUI要素")]
        [SerializeField] private Button stirButton;
        [SerializeField] private Button addIngredientButton;
        [SerializeField] private Button finishCookingButton;
        
        [Header("温度調整")]
        [SerializeField] private Slider temperatureSlider;
        [SerializeField] private Image flameImage;
        [SerializeField] private Gradient flameColorGradient;
        
        [Header("調理プログレス")]
        [SerializeField] private Slider cookingProgressSlider;
        [SerializeField] private TextMeshProUGUI cookingStatusText;
        [SerializeField] private Image soupColorImage;
        
        [Header("調理状態視覚化")]
        [SerializeField] private GameObject preparingStateIcon;
        [SerializeField] private GameObject cookingStateIcon;
        [SerializeField] private GameObject simmeringStateIcon;
        [SerializeField] private GameObject completedStateIcon;
        
        [Header("アニメーション")]
        [SerializeField] private Animation stirButtonAnimation;
        [SerializeField] private Animator cookingPotAnimator;
        
        // 参照
        private CookingSystem cookingSystem;
        
        // アニメーションタイマー
        private float buttonAnimCooldown = 0f;
        
        private void Start()
        {
            // 調理システムへの参照を取得
            cookingSystem = FindObjectOfType<CookingSystem>();
            if (cookingSystem == null)
            {
                Debug.LogError("CookingSystem not found in scene.");
                return;
            }
            
            // イベント登録
            cookingSystem.OnCookingProgress += UpdateCookingProgress;
            cookingSystem.OnCookingStateChanged += UpdateCookingState;
            
            // ボタンのリスナー設定
            SetupButtonListeners();
            
            // スライダーのリスナー設定
            SetupSliderListeners();
        }
        
        private void OnDestroy()
        {
            // イベントの登録解除
            if (cookingSystem != null)
            {
                cookingSystem.OnCookingProgress -= UpdateCookingProgress;
                cookingSystem.OnCookingStateChanged -= UpdateCookingState;
            }
        }
        
        private void Update()
        {
            // ボタンアニメーションのクールダウン更新
            if (buttonAnimCooldown > 0)
            {
                buttonAnimCooldown -= Time.deltaTime;
            }
            
            // UIの毎フレーム更新が必要な場合はここに記述
        }
        
        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // ボタンとUIの初期状態設定
            UpdateButtonStates();
            
            // 調理進行度の初期化
            if (cookingProgressSlider != null)
            {
                cookingProgressSlider.value = 0f;
            }
            
            // スープの色を更新
            UpdateSoupColor();
            
            // 調理状態アイコンを更新
            UpdateCookingStateIcons(CookingState.Idle);
            
            Debug.Log("CookingActionUI initialized");
        }
        
        /// <summary>
        /// ボタンのリスナー設定
        /// </summary>
        private void SetupButtonListeners()
        {
            // かき混ぜボタン
            if (stirButton != null)
            {
                stirButton.onClick.AddListener(OnStirButtonClicked);
            }
            
            // 食材追加ボタン
            if (addIngredientButton != null)
            {
                addIngredientButton.onClick.AddListener(OnAddIngredientButtonClicked);
            }
            
            // 調理完了ボタン
            if (finishCookingButton != null)
            {
                finishCookingButton.onClick.AddListener(OnFinishCookingButtonClicked);
            }
        }
        
        /// <summary>
        /// スライダーのリスナー設定
        /// </summary>
        private void SetupSliderListeners()
        {
            // 温度スライダー
            if (temperatureSlider != null)
            {
                temperatureSlider.onValueChanged.AddListener(OnTemperatureChanged);
                
                // 初期値設定
                temperatureSlider.value = 0.5f;
                OnTemperatureChanged(temperatureSlider.value);
            }
        }
        
        /// <summary>
        /// 調理進行度の更新
        /// </summary>
        /// <param name="progress">進行度（0-1）</param>
        private void UpdateCookingProgress(float progress)
        {
            if (cookingProgressSlider != null)
            {
                cookingProgressSlider.value = progress;
            }
        }
        
        /// <summary>
        /// 調理状態の更新
        /// </summary>
        /// <param name="state">調理状態</param>
        private void UpdateCookingState(CookingState state)
        {
            // ボタン状態更新
            UpdateButtonStates();
            
            // 調理状態テキスト更新
            UpdateCookingStatusText(state);
            
            // 調理状態アイコン更新
            UpdateCookingStateIcons(state);
            
            // スープの色更新
            UpdateSoupColor();
        }
        
        /// <summary>
        /// 調理状態テキストの更新
        /// </summary>
        /// <param name="state">調理状態</param>
        private void UpdateCookingStatusText(CookingState state)
        {
            if (cookingStatusText == null) return;
            
            switch (state)
            {
                case CookingState.Idle:
                    cookingStatusText.text = "よういをしよう！";
                    break;
                
                case CookingState.Preparing:
                    cookingStatusText.text = "しょくざいをいれよう！";
                    break;
                
                case CookingState.Cooking:
                    cookingStatusText.text = "ぐつぐつにえてるよ！";
                    break;
                
                case CookingState.Simmering:
                    cookingStatusText.text = "かきまぜてね！";
                    break;
                
                case CookingState.Completed:
                    cookingStatusText.text = "できあがり！";
                    break;
            }
        }
        
        /// <summary>
        /// 調理状態アイコンの更新
        /// </summary>
        /// <param name="state">調理状態</param>
        private void UpdateCookingStateIcons(CookingState state)
        {
            // すべてのアイコンを非表示
            if (preparingStateIcon != null) preparingStateIcon.SetActive(false);
            if (cookingStateIcon != null) cookingStateIcon.SetActive(false);
            if (simmeringStateIcon != null) simmeringStateIcon.SetActive(false);
            if (completedStateIcon != null) completedStateIcon.SetActive(false);
            
            // 現在の状態のアイコンを表示
            switch (state)
            {
                case CookingState.Preparing:
                    if (preparingStateIcon != null) preparingStateIcon.SetActive(true);
                    break;
                
                case CookingState.Cooking:
                    if (cookingStateIcon != null) cookingStateIcon.SetActive(true);
                    break;
                
                case CookingState.Simmering:
                    if (simmeringStateIcon != null) simmeringStateIcon.SetActive(true);
                    break;
                
                case CookingState.Completed:
                    if (completedStateIcon != null) completedStateIcon.SetActive(true);
                    break;
            }
        }
        
        /// <summary>
        /// スープの色を更新
        /// </summary>
        private void UpdateSoupColor()
        {
            if (soupColorImage == null || cookingSystem == null) return;
            
            // 調理システムからスープの色を取得
            Color soupColor = cookingSystem.CalculateSoupColor();
            
            // 透明度を設定（準備中は薄く、調理中は濃く）
            if (cookingSystem.CurrentCookingState == CookingState.Idle || 
                cookingSystem.CurrentCookingState == CookingState.Preparing)
            {
                soupColor.a = 0.5f;
            }
            else
            {
                soupColor.a = 1.0f;
            }
            
            // 色を適用
            soupColorImage.color = soupColor;
        }
        
        /// <summary>
        /// ボタン状態の更新
        /// </summary>
        private void UpdateButtonStates()
        {
            if (cookingSystem == null) return;
            
            // かき混ぜボタン（調理中のみ有効）
            if (stirButton != null)
            {
                stirButton.interactable = cookingSystem.IsCooking;
            }
            
            // 食材追加ボタン（準備中のみ有効）
            if (addIngredientButton != null)
            {
                addIngredientButton.interactable = cookingSystem.CurrentCookingState == CookingState.Preparing;
            }
            
            // 調理完了ボタン（シミュレーション中または完成時に有効）
            if (finishCookingButton != null)
            {
                finishCookingButton.interactable = 
                    cookingSystem.CurrentCookingState == CookingState.Simmering || 
                    cookingSystem.CurrentCookingState == CookingState.Completed;
            }
            
            // 温度スライダー（調理中のみ有効）
            if (temperatureSlider != null)
            {
                temperatureSlider.interactable = cookingSystem.IsCooking;
            }
        }
        
        #region ボタンイベントハンドラ
        
        /// <summary>
        /// かき混ぜボタンがクリックされた時の処理
        /// </summary>
        private void OnStirButtonClicked()
        {
            if (cookingSystem == null || !cookingSystem.IsCooking) return;
            
            // ボタンアニメーション
            PlayButtonAnimation(stirButton.gameObject);
            
            // 調理システムのかき混ぜメソッド呼び出し
            cookingSystem.StirSoup();
            
            // 鍋のアニメーション
            if (cookingPotAnimator != null)
            {
                cookingPotAnimator.SetTrigger("Stir");
            }
        }
        
        /// <summary>
        /// 食材追加ボタンがクリックされた時の処理
        /// </summary>
        private void OnAddIngredientButtonClicked()
        {
            // 食材選択UIを表示
            GameplayUIController uiController = FindObjectOfType<GameplayUIController>();
            if (uiController != null)
            {
                // 食材選択モーダルウィンドウを表示
                // 実装が必要な場合は、追加のメソッドをGameplayUIControllerに実装
            }
            
            // ボタンアニメーション
            PlayButtonAnimation(addIngredientButton.gameObject);
        }
        
        /// <summary>
        /// 調理完了ボタンがクリックされた時の処理
        /// </summary>
        private void OnFinishCookingButtonClicked()
        {
            if (cookingSystem == null) return;
            
            // 調理完了処理
            cookingSystem.FinishCooking();
            
            // UI状態更新
            UpdateButtonStates();
            
            // 試食へ進むボタンを有効化
            // (GameplayUIControllerで対応する必要がある場合あり)
            
            // ボタンアニメーション
            PlayButtonAnimation(finishCookingButton.gameObject);
            
            // 次のステージへ進む
            GameplayManager gameplayManager = GameplayManager.Instance;
            if (gameplayManager != null)
            {
                // 少し待ってから次へ進む
                StartCoroutine(DelayedNextStage());
            }
        }
        
        /// <summary>
        /// 少し待ってから次のステージへ進む
        /// </summary>
        private IEnumerator DelayedNextStage()
        {
            // 完成エフェクトや演出の時間
            yield return new WaitForSeconds(2.0f);
            
            // 次のステージ（試食）へ
            GameplayManager gameplayManager = GameplayManager.Instance;
            if (gameplayManager != null)
            {
                gameplayManager.StartTasting();
            }
        }
        
        #endregion
        
        /// <summary>
        /// 温度が変更された時の処理
        /// </summary>
        /// <param name="value">温度値（0-1）</param>
        private void OnTemperatureChanged(float value)
        {
            // 炎の色を変更
            if (flameImage != null && flameColorGradient != null)
            {
                flameImage.color = flameColorGradient.Evaluate(value);
                
                // サイズも変更するオプション
                flameImage.transform.localScale = Vector3.one * (0.8f + value * 0.4f);
            }
            
            // 調理システムに温度変更を通知
            if (cookingSystem != null && cookingSystem.IsCooking)
            {
                cookingSystem.ChangeTemperature(value);
            }
        }
        
        /// <summary>
        /// ボタンのアニメーション再生
        /// </summary>
        /// <param name="buttonObj">アニメーションを再生するボタンオブジェクト</param>
        private void PlayButtonAnimation(GameObject buttonObj)
        {
            if (buttonAnimCooldown > 0) return;
            
            // スケールアニメーション
            StartCoroutine(ButtonPressAnimation(buttonObj));
            
            // クールダウン設定
            buttonAnimCooldown = 0.3f;
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
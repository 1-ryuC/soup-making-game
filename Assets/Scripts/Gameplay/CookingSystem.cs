using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoupMaking.Ingredients;

namespace SoupMaking.Gameplay
{
    /// <summary>
    /// 調理プロセスを管理するシステムクラス
    /// </summary>
    public class CookingSystem : MonoBehaviour
    {
        [Header("調理オブジェクト")]
        [SerializeField] private Transform cookingPot;
        [SerializeField] private ParticleSystem steamParticle;
        [SerializeField] private ParticleSystem bubbleParticle;
        
        [Header("調理設定")]
        [SerializeField] private float maxCookingTime = 60f;
        [SerializeField] private float minCookingTime = 10f;
        
        [Header("アニメーション設定")]
        [SerializeField] private float stirAnimationDuration = 1.5f;
        [SerializeField] private AnimationCurve stirAnimationCurve;

        // 調理状態
        private CookingState currentCookingState = CookingState.Idle;
        private float currentCookingTime = 0f;
        private List<Ingredient> addedIngredients = new List<Ingredient>();
        
        // 調理アクション変数
        private bool isStirring = false;
        private float stirAnimationTime = 0f;
        private Quaternion potStartRotation;
        private Quaternion potTargetRotation;

        // イベント
        public event Action<CookingState> OnCookingStateChanged;
        public event Action<Ingredient> OnIngredientAdded;
        public event Action<float> OnCookingProgress;

        // プロパティ
        public CookingState CurrentCookingState => currentCookingState;
        public float CookingProgress => Mathf.Clamp01(currentCookingTime / maxCookingTime);
        public List<Ingredient> AddedIngredients => addedIngredients;
        public bool IsCooking => currentCookingState == CookingState.Cooking || currentCookingState == CookingState.Simmering;

        private void Start()
        {
            // 調理鍋の確認
            if (cookingPot == null)
            {
                Debug.LogError("Cooking pot is not assigned.");
            }
            
            // パーティクルシステムの初期化
            if (steamParticle != null)
            {
                steamParticle.Stop();
            }
            
            if (bubbleParticle != null)
            {
                bubbleParticle.Stop();
            }
            
            // 調理状態の初期化
            SetCookingState(CookingState.Idle);
        }

        private void Update()
        {
            // 現在の調理状態に基づく更新
            switch (currentCookingState)
            {
                case CookingState.Cooking:
                case CookingState.Simmering:
                    // 調理時間の更新
                    UpdateCookingTime();
                    break;
            }
            
            // かき混ぜアニメーションの更新
            if (isStirring)
            {
                UpdateStirAnimation();
            }
        }

        /// <summary>
        /// 調理状態を設定
        /// </summary>
        /// <param name="state">新しい調理状態</param>
        private void SetCookingState(CookingState state)
        {
            if (state == currentCookingState) return;
            
            CookingState previousState = currentCookingState;
            currentCookingState = state;
            
            // 状態変更に応じた処理
            switch (currentCookingState)
            {
                case CookingState.Idle:
                    // アイドル状態の処理
                    if (steamParticle != null) steamParticle.Stop();
                    if (bubbleParticle != null) bubbleParticle.Stop();
                    break;
                
                case CookingState.Preparing:
                    // 準備状態の処理
                    break;
                
                case CookingState.Cooking:
                    // 調理開始時の処理
                    if (steamParticle != null) steamParticle.Play();
                    break;
                
                case CookingState.Simmering:
                    // 煮込み状態の処理
                    if (bubbleParticle != null) bubbleParticle.Play();
                    break;
                
                case CookingState.Completed:
                    // 完成状態の処理
                    if (bubbleParticle != null) bubbleParticle.Stop();
                    // 強い蒸気エフェクトなど
                    break;
            }
            
            // イベント発火
            OnCookingStateChanged?.Invoke(currentCookingState);
            Debug.Log($"Cooking state changed from {previousState} to {currentCookingState}");
        }

        /// <summary>
        /// 調理時間の更新
        /// </summary>
        private void UpdateCookingTime()
        {
            currentCookingTime += Time.deltaTime;
            
            // 煮込み状態への移行
            if (currentCookingState == CookingState.Cooking && currentCookingTime >= minCookingTime)
            {
                SetCookingState(CookingState.Simmering);
            }
            
            // 完成状態への移行
            if (currentCookingState == CookingState.Simmering && currentCookingTime >= maxCookingTime)
            {
                SetCookingState(CookingState.Completed);
            }
            
            // 進捗イベント発火
            OnCookingProgress?.Invoke(CookingProgress);
        }

        /// <summary>
        /// かき混ぜアニメーションの更新
        /// </summary>
        private void UpdateStirAnimation()
        {
            stirAnimationTime += Time.deltaTime;
            float t = stirAnimationCurve.Evaluate(Mathf.Clamp01(stirAnimationTime / stirAnimationDuration));
            
            // 鍋を回転
            cookingPot.rotation = Quaternion.Lerp(potStartRotation, potTargetRotation, t);
            
            // アニメーション終了
            if (stirAnimationTime >= stirAnimationDuration)
            {
                isStirring = false;
                cookingPot.rotation = potStartRotation;
            }
        }

        /// <summary>
        /// 調理を開始
        /// </summary>
        public void StartCooking()
        {
            if (currentCookingState != CookingState.Idle && currentCookingState != CookingState.Preparing) return;
            
            // 調理開始状態に変更
            SetCookingState(CookingState.Cooking);
            currentCookingTime = 0f;
            
            Debug.Log("Cooking started");
        }

        /// <summary>
        /// 調理を完了
        /// </summary>
        public void FinishCooking()
        {
            // 強制的に完了状態に変更
            if (currentCookingState == CookingState.Cooking || currentCookingState == CookingState.Simmering)
            {
                SetCookingState(CookingState.Completed);
            }
        }

        /// <summary>
        /// 調理をリセット
        /// </summary>
        public void ResetCooking()
        {
            // 状態をリセット
            SetCookingState(CookingState.Idle);
            currentCookingTime = 0f;
            
            // 食材リストをクリア
            addedIngredients.Clear();
            
            Debug.Log("Cooking reset");
        }

        /// <summary>
        /// 食材を追加
        /// </summary>
        /// <param name="ingredient">追加する食材</param>
        public void AddIngredient(Ingredient ingredient)
        {
            if (ingredient == null) return;
            
            // 食材リストに追加
            addedIngredients.Add(ingredient);
            
            // イベント発火
            OnIngredientAdded?.Invoke(ingredient);
            
            Debug.Log($"Added ingredient: {ingredient.name}");
            
            // 最初の食材を追加したら準備状態に
            if (currentCookingState == CookingState.Idle && addedIngredients.Count == 1)
            {
                SetCookingState(CookingState.Preparing);
            }
        }

        /// <summary>
        /// 食材を鍋に落とすアニメーション
        /// </summary>
        /// <param name="ingredient">落とす食材</param>
        /// <param name="startPosition">開始位置</param>
        /// <returns>アニメーションコルーチン</returns>
        public IEnumerator DropIngredientAnimation(Ingredient ingredient, Vector3 startPosition)
        {
            if (ingredient == null || cookingPot == null) yield break;
            
            // 食材の位置を設定
            ingredient.transform.position = startPosition;
            
            // 落下アニメーション
            float dropDuration = 0.5f;
            float dropTime = 0f;
            Vector3 targetPosition = cookingPot.position + Vector3.up * 0.1f;
            
            while (dropTime < dropDuration)
            {
                dropTime += Time.deltaTime;
                float t = dropTime / dropDuration;
                
                // 放物線を描くように落下
                float height = Mathf.Sin(t * Mathf.PI) * 0.5f;
                Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, t);
                currentPos.y += height;
                
                ingredient.transform.position = currentPos;
                
                yield return null;
            }
            
            // スプラッシュエフェクト（あれば）
            
            // 食材を非表示に
            ingredient.gameObject.SetActive(false);
            
            // 食材を追加
            AddIngredient(ingredient);
        }

        /// <summary>
        /// かき混ぜアクション
        /// </summary>
        public void StirSoup()
        {
            if (!IsCooking || isStirring) return;
            
            // かき混ぜアニメーション開始
            isStirring = true;
            stirAnimationTime = 0f;
            
            // 回転設定
            potStartRotation = cookingPot.rotation;
            potTargetRotation = potStartRotation * Quaternion.Euler(0, 0, 60f);
            
            // 調理促進効果（オプション）
            currentCookingTime += 2f;
            
            Debug.Log("Stirring soup");
        }

        /// <summary>
        /// 調理温度変更
        /// </summary>
        /// <param name="temperatureLevel">温度レベル (0-1)</param>
        public void ChangeTemperature(float temperatureLevel)
        {
            if (!IsCooking) return;
            
            temperatureLevel = Mathf.Clamp01(temperatureLevel);
            
            // 温度に応じた調理速度調整
            float speedMultiplier = 0.5f + temperatureLevel * 1.5f;
            
            // パーティクル効果調整
            if (steamParticle != null)
            {
                var emission = steamParticle.emission;
                emission.rateOverTime = 5f + temperatureLevel * 15f;
            }
            
            if (bubbleParticle != null && currentCookingState == CookingState.Simmering)
            {
                var emission = bubbleParticle.emission;
                emission.rateOverTime = temperatureLevel * 20f;
            }
            
            Debug.Log($"Temperature changed to level: {temperatureLevel:F2}, speed multiplier: {speedMultiplier:F2}");
        }

        /// <summary>
        /// 調理結果を評価
        /// </summary>
        /// <returns>調理評価（0-5点）</returns>
        public int EvaluateCooking()
        {
            if (currentCookingState != CookingState.Completed)
            {
                return 0;
            }
            
            // 基本評価点（1点）
            int score = 1;
            
            // 食材数に応じた加点
            score += Mathf.Min(addedIngredients.Count, 3);
            
            // 調理時間に応じた加点（適切な時間なら+1）
            if (currentCookingTime >= minCookingTime && currentCookingTime <= maxCookingTime * 0.9f)
            {
                score += 1;
            }
            
            // 最大5点までの評価
            return Mathf.Min(score, 5);
        }

        /// <summary>
        /// 現在のスープの色を計算
        /// </summary>
        /// <returns>スープの色</returns>
        public Color CalculateSoupColor()
        {
            if (addedIngredients.Count == 0)
            {
                return Color.clear; // 透明
            }
            
            // 各食材の色を合成
            Color soupColor = Color.clear;
            foreach (var ingredient in addedIngredients)
            {
                soupColor += ingredient.GetIngredientColor();
            }
            
            // 平均色を返す
            soupColor /= addedIngredients.Count;
            
            // 不透明にする
            soupColor.a = 1f;
            
            return soupColor;
        }
    }
}
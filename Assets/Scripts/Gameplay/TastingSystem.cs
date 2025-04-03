using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoupMaking.Ingredients;

namespace SoupMaking.Gameplay
{
    /// <summary>
    /// 試食システムを管理するクラス
    /// </summary>
    public class TastingSystem : MonoBehaviour
    {
        [Header("試食キャラクター")]
        [SerializeField] private Transform tasterCharactersParent;
        [SerializeField] private GameObject[] tasterCharacterPrefabs;
        
        [Header("試食設定")]
        [SerializeField] private float tastingAnimationDuration = 3.0f;
        [SerializeField] private float reactionAnimationDuration = 2.0f;
        
        // 参照
        private CookingSystem cookingSystem;
        
        // 試食キャラクター
        private List<GameObject> activeTasters = new List<GameObject>();
        private int currentTasterIndex = -1;
        
        // 試食状態
        private bool isTasting = false;
        private bool hasCompletedTasting = false;
        
        // イベント
        public event Action<GameObject, CharacterReaction> OnCharacterReaction;
        public event Action OnAllTastingCompleted;

        private void Start()
        {
            // 調理システムへの参照を取得
            cookingSystem = FindObjectOfType<CookingSystem>();
            if (cookingSystem == null)
            {
                Debug.LogError("CookingSystem not found in scene.");
            }
            
            // 試食キャラクター親オブジェクトの確認
            if (tasterCharactersParent == null)
            {
                Debug.LogError("Taster characters parent is not assigned.");
            }
        }

        /// <summary>
        /// 試食の準備をする
        /// </summary>
        public void PrepareTasting()
        {
            if (isTasting) return;
            
            // 前回の試食キャラクターをクリア
            ClearTasters();
            
            // 試食状態をリセット
            isTasting = false;
            hasCompletedTasting = false;
            currentTasterIndex = -1;
            
            // 試食キャラクターを生成
            SpawnTasters();
            
            Debug.Log("Tasting prepared");
        }

        /// <summary>
        /// 試食を開始する
        /// </summary>
        public void StartTasting()
        {
            if (isTasting || hasCompletedTasting || activeTasters.Count == 0) return;
            
            // 試食開始
            isTasting = true;
            
            // 最初のキャラクターから試食開始
            currentTasterIndex = 0;
            StartCoroutine(TastingSequence());
            
            Debug.Log("Tasting started");
        }

        /// <summary>
        /// 試食プロセスをスキップする
        /// </summary>
        public void SkipTasting()
        {
            if (!isTasting || hasCompletedTasting) return;
            
            // 現在の試食アニメーションを停止
            StopAllCoroutines();
            
            // すべてのキャラクターを試食完了状態にする
            for (int i = currentTasterIndex; i < activeTasters.Count; i++)
            {
                var taster = activeTasters[i];
                var reaction = EvaluateTasterReaction(taster);
                
                // リアクション設定
                SetTasterReaction(taster, reaction);
                
                // イベント発火
                OnCharacterReaction?.Invoke(taster, reaction);
            }
            
            // 試食完了
            CompleteTasting();
            
            Debug.Log("Tasting skipped");
        }

        /// <summary>
        /// 試食を終了する
        /// </summary>
        public void FinishTasting()
        {
            if (isTasting)
            {
                // 試食中なら強制的にスキップ
                SkipTasting();
            }
            
            Debug.Log("Tasting finished");
        }

        /// <summary>
        /// 試食をリセットする
        /// </summary>
        public void ResetTasting()
        {
            // 試食アニメーションを停止
            StopAllCoroutines();
            
            // 試食キャラクターをクリア
            ClearTasters();
            
            // 状態をリセット
            isTasting = false;
            hasCompletedTasting = false;
            currentTasterIndex = -1;
            
            Debug.Log("Tasting reset");
        }

        /// <summary>
        /// 試食キャラクターを生成する
        /// </summary>
        private void SpawnTasters()
        {
            if (tasterCharacterPrefabs == null || tasterCharacterPrefabs.Length == 0 || tasterCharactersParent == null)
            {
                Debug.LogError("Taster character prefabs or parent is not set.");
                return;
            }
            
            // ゲームモードに基づいて生成するキャラクター数を決定
            int tasterCount = GetTasterCountForCurrentMode();
            
            // キャラクターを生成
            for (int i = 0; i < tasterCount; i++)
            {
                // ランダムなキャラクタープレハブを選択
                int prefabIndex = UnityEngine.Random.Range(0, tasterCharacterPrefabs.Length);
                GameObject prefab = tasterCharacterPrefabs[prefabIndex];
                
                // 位置と回転を設定
                Vector3 position = GetTasterPosition(i, tasterCount);
                Quaternion rotation = Quaternion.identity;
                
                // キャラクターを生成
                GameObject taster = Instantiate(prefab, position, rotation, tasterCharactersParent);
                taster.name = $"Taster_{i+1}_{prefab.name}";
                
                // リストに追加
                activeTasters.Add(taster);
                
                // 初期状態を設定（アニメーションなど）
                SetTasterInitialState(taster);
            }
            
            Debug.Log($"Spawned {tasterCount} tasters");
        }

        /// <summary>
        /// 現在のゲームモードに基づいて試食キャラクターの数を決定
        /// </summary>
        private int GetTasterCountForCurrentMode()
        {
            // ゲームマネージャーからモードを取得
            GameMode currentMode = GameManager.Instance != null 
                ? GameManager.Instance.CurrentGameMode 
                : GameMode.FreePlay;
            
            // モードに基づいて数を決定
            switch (currentMode)
            {
                case GameMode.Mission:
                    return 1; // ミッションモードでは指定された1キャラクター
                
                case GameMode.Recipe:
                    return 1; // レシピモードでも1キャラクター
                
                case GameMode.ParentChild:
                    return 2; // 親子モードでは2キャラクター
                
                case GameMode.FreePlay:
                default:
                    return UnityEngine.Random.Range(1, 4); // フリープレイでは1〜3キャラクター
            }
        }

        /// <summary>
        /// 試食キャラクターの位置を決定
        /// </summary>
        private Vector3 GetTasterPosition(int index, int totalCount)
        {
            // キャラクターの位置を計算
            float angleStep = 360f / totalCount;
            float angle = index * angleStep;
            
            // 円形に配置（半径1.5）
            float radius = 1.5f;
            float x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            
            // 親オブジェクトからのローカル座標
            return new Vector3(x, 0, z);
        }

        /// <summary>
        /// 試食キャラクターの初期状態を設定
        /// </summary>
        private void SetTasterInitialState(GameObject taster)
        {
            // アニメーターがあれば初期アニメーション設定
            Animator animator = taster.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Idle");
            }
            
            // その他の初期設定（表情など）
        }

        /// <summary>
        /// すべての試食キャラクターを削除
        /// </summary>
        private void ClearTasters()
        {
            foreach (var taster in activeTasters)
            {
                if (taster != null)
                {
                    Destroy(taster);
                }
            }
            
            activeTasters.Clear();
        }

        /// <summary>
        /// 試食シーケンスのコルーチン
        /// </summary>
        private IEnumerator TastingSequence()
        {
            while (currentTasterIndex < activeTasters.Count)
            {
                // 現在のキャラクターを取得
                GameObject currentTaster = activeTasters[currentTasterIndex];
                
                // 試食アニメーション
                yield return StartCoroutine(TasterEatingAnimation(currentTaster));
                
                // リアクションを評価
                CharacterReaction reaction = EvaluateTasterReaction(currentTaster);
                
                // リアクションアニメーション
                yield return StartCoroutine(TasterReactionAnimation(currentTaster, reaction));
                
                // イベント発火
                OnCharacterReaction?.Invoke(currentTaster, reaction);
                
                // 次のキャラクターへ
                currentTasterIndex++;
                
                // 短い待機（次のキャラクターの前に）
                yield return new WaitForSeconds(0.5f);
            }
            
            // すべてのキャラクターが試食完了
            CompleteTasting();
        }

        /// <summary>
        /// 試食アニメーションのコルーチン
        /// </summary>
        private IEnumerator TasterEatingAnimation(GameObject taster)
        {
            // アニメーターがあれば食べるアニメーション再生
            Animator animator = taster.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Eat");
            }
            
            // 試食時間待機
            yield return new WaitForSeconds(tastingAnimationDuration);
        }

        /// <summary>
        /// リアクションアニメーションのコルーチン
        /// </summary>
        private IEnumerator TasterReactionAnimation(GameObject taster, CharacterReaction reaction)
        {
            // リアクション設定
            SetTasterReaction(taster, reaction);
            
            // リアクション時間待機
            yield return new WaitForSeconds(reactionAnimationDuration);
        }

        /// <summary>
        /// キャラクターのリアクションを評価
        /// </summary>
        private CharacterReaction EvaluateTasterReaction(GameObject taster)
        {
            if (cookingSystem == null)
            {
                return CharacterReaction.Neutral;
            }
            
            // キャラクターの好みデータを取得（仮実装）
            TasterPreferences preferences = taster.GetComponent<TasterPreferences>();
            
            if (preferences == null)
            {
                // 好みデータがなければランダムに評価
                return GetRandomReaction();
            }
            
            // 食材との相性を計算
            int matchScore = 0;
            foreach (var ingredient in cookingSystem.AddedIngredients)
            {
                // 好きな食材が含まれていれば加点
                if (preferences.LikedIngredients.Contains(ingredient.IngredientType))
                {
                    matchScore += 2;
                }
                
                // 嫌いな食材が含まれていれば減点
                if (preferences.DislikedIngredients.Contains(ingredient.IngredientType))
                {
                    matchScore -= 3;
                }
                
                // 好きな食材カテゴリが含まれていれば加点
                if (preferences.LikedCategories.Contains(ingredient.Category))
                {
                    matchScore += 1;
                }
            }
            
            // スープの調理度合いに応じた評価
            int cookingScore = cookingSystem.EvaluateCooking();
            
            // 合計スコアでリアクションを決定
            int totalScore = matchScore + cookingScore;
            
            if (totalScore >= 6)
            {
                return CharacterReaction.Love;
            }
            else if (totalScore >= 3)
            {
                return CharacterReaction.Like;
            }
            else if (totalScore >= 0)
            {
                return CharacterReaction.Neutral;
            }
            else if (totalScore >= -3)
            {
                return CharacterReaction.Dislike;
            }
            else
            {
                return CharacterReaction.Disgust;
            }
        }

        /// <summary>
        /// ランダムなリアクションを返す（デモ用）
        /// </summary>
        private CharacterReaction GetRandomReaction()
        {
            int rand = UnityEngine.Random.Range(0, 5);
            return (CharacterReaction)rand;
        }

        /// <summary>
        /// キャラクターのリアクションを設定
        /// </summary>
        private void SetTasterReaction(GameObject taster, CharacterReaction reaction)
        {
            // アニメーターがあればリアクションアニメーション設定
            Animator animator = taster.GetComponent<Animator>();
            if (animator != null)
            {
                switch (reaction)
                {
                    case CharacterReaction.Love:
                        animator.SetTrigger("Love");
                        break;
                    
                    case CharacterReaction.Like:
                        animator.SetTrigger("Like");
                        break;
                    
                    case CharacterReaction.Neutral:
                        animator.SetTrigger("Neutral");
                        break;
                    
                    case CharacterReaction.Dislike:
                        animator.SetTrigger("Dislike");
                        break;
                    
                    case CharacterReaction.Disgust:
                        animator.SetTrigger("Disgust");
                        break;
                }
            }
            
            Debug.Log($"Taster {taster.name} reacted with: {reaction}");
        }

        /// <summary>
        /// 試食完了処理
        /// </summary>
        private void CompleteTasting()
        {
            isTasting = false;
            hasCompletedTasting = true;
            
            // 試食完了イベント発火
            OnAllTastingCompleted?.Invoke();
            
            Debug.Log("All tasting completed");
        }

        /// <summary>
        /// 試食の総合結果を取得
        /// </summary>
        /// <returns>星評価（0-5）</returns>
        public int GetOverallRating()
        {
            if (!hasCompletedTasting || activeTasters.Count == 0)
            {
                return 0;
            }
            
            // 調理の基本評価を取得
            int cookingScore = cookingSystem != null ? cookingSystem.EvaluateCooking() : 3;
            
            // キャラクターの反応に基づく評価をシミュレート
            int reactionSum = 0;
            foreach (var taster in activeTasters)
            {
                CharacterReaction reaction = EvaluateTasterReaction(taster);
                
                // リアクションを点数に変換
                switch (reaction)
                {
                    case CharacterReaction.Love:
                        reactionSum += 5;
                        break;
                    
                    case CharacterReaction.Like:
                        reactionSum += 4;
                        break;
                    
                    case CharacterReaction.Neutral:
                        reactionSum += 3;
                        break;
                    
                    case CharacterReaction.Dislike:
                        reactionSum += 2;
                        break;
                    
                    case CharacterReaction.Disgust:
                        reactionSum += 1;
                        break;
                }
            }
            
            // 平均反応スコア
            float averageReaction = (float)reactionSum / activeTasters.Count;
            
            // 調理スコアと反応スコアを組み合わせて総合評価
            float overallScore = (cookingScore + averageReaction) / 2f;
            
            // 最終評価（星の数）
            return Mathf.RoundToInt(overallScore);
        }
    }
}

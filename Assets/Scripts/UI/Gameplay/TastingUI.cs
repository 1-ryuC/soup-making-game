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
    /// 試食UIを管理するクラス
    /// </summary>
    public class TastingUI : MonoBehaviour
    {
        [Header("試食UI要素")]
        [SerializeField] private Button startTastingButton;
        [SerializeField] private Button skipTastingButton;
        [SerializeField] private Button finishTastingButton;
        
        [Header("試食状態表示")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject tastingInProgressIndicator;
        [SerializeField] private GameObject tastingCompletedIndicator;
        
        [Header("キャラクターリアクション表示")]
        [SerializeField] private GameObject reactionPanel;
        [SerializeField] private Image characterImage;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI reactionCommentText;
        [SerializeField] private GameObject[] reactionStars; // 評価の星アイコン
        
        [Header("視覚エフェクト")]
        [SerializeField] private ParticleSystem reactionParticles;
        [SerializeField] private Animator uiAnimator;
        
        [Header("スープ表示")]
        [SerializeField] private Image soupImage;
        
        // 参照
        private TastingSystem tastingSystem;
        private CookingSystem cookingSystem;
        
        // 状態追跡
        private bool isTastingInProgress = false;
        private bool hasTastingCompleted = false;
        
        // リアクションコメントマッピング
        private Dictionary<CharacterReaction, string[]> reactionComments = new Dictionary<CharacterReaction, string[]>();
        
        private void Start()
        {
            // 試食システムへの参照を取得
            tastingSystem = FindObjectOfType<TastingSystem>();
            if (tastingSystem == null)
            {
                Debug.LogError("TastingSystem not found in scene.");
            }
            
            // 調理システムへの参照を取得
            cookingSystem = FindObjectOfType<CookingSystem>();
            if (cookingSystem == null)
            {
                Debug.LogError("CookingSystem not found in scene.");
            }
            
            // イベントリスナー登録
            if (tastingSystem != null)
            {
                tastingSystem.OnCharacterReaction += HandleCharacterReaction;
                tastingSystem.OnAllTastingCompleted += HandleTastingCompleted;
            }
            
            // ボタンリスナー設定
            SetupButtonListeners();
            
            // リアクションコメントの初期化
            InitializeReactionComments();
            
            // 初期UI状態設定
            UpdateUIState(false, false);
        }
        
        private void OnDestroy()
        {
            // イベントリスナー解除
            if (tastingSystem != null)
            {
                tastingSystem.OnCharacterReaction -= HandleCharacterReaction;
                tastingSystem.OnAllTastingCompleted -= HandleTastingCompleted;
            }
        }
        
        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // 状態リセット
            isTastingInProgress = false;
            hasTastingCompleted = false;
            
            // UI状態更新
            UpdateUIState(isTastingInProgress, hasTastingCompleted);
            
            // リアクションパネル初期化
            HideReactionPanel();
            
            // スープの表示を更新
            UpdateSoupDisplay();
            
            // 試食ガイドテキスト表示
            if (statusText != null)
            {
                statusText.text = "スープをたべてもらおう！";
            }
            
            Debug.Log("TastingUI initialized");
        }
        
        /// <summary>
        /// ボタンリスナーの設定
        /// </summary>
        private void SetupButtonListeners()
        {
            // 試食開始ボタン
            if (startTastingButton != null)
            {
                startTastingButton.onClick.AddListener(OnStartTastingButtonClicked);
            }
            
            // スキップボタン
            if (skipTastingButton != null)
            {
                skipTastingButton.onClick.AddListener(OnSkipTastingButtonClicked);
            }
            
            // 試食完了ボタン
            if (finishTastingButton != null)
            {
                finishTastingButton.onClick.AddListener(OnFinishTastingButtonClicked);
            }
        }
        
        /// <summary>
        /// リアクションコメントの初期化
        /// </summary>
        private void InitializeReactionComments()
        {
            // 様々なリアクションに対応するコメントを設定
            reactionComments[CharacterReaction.Love] = new string[]
            {
                "とってもおいしい！",
                "さいこう！",
                "だいすき！",
                "もっとたべたい！"
            };
            
            reactionComments[CharacterReaction.Like] = new string[]
            {
                "おいしいね！",
                "いいかんじ！",
                "すき！",
                "おいしいよ！"
            };
            
            reactionComments[CharacterReaction.Neutral] = new string[]
            {
                "まあまあだね",
                "ふつう",
                "わるくないよ",
                "うーん"
            };
            
            reactionComments[CharacterReaction.Dislike] = new string[]
            {
                "ちょっとにがてかも",
                "うーん、あまり...",
                "もうすこしがんばろう",
                "もういいかな"
            };
            
            reactionComments[CharacterReaction.Disgust] = new string[]
            {
                "たべられないよ...",
                "むむむ...",
                "ちょっと...",
                "むずかしいな..."
            };
        }
        
        /// <summary>
        /// UI状態の更新
        /// </summary>
        /// <param name="isTasting">試食中かどうか</param>
        /// <param name="isCompleted">試食完了したかどうか</param>
        private void UpdateUIState(bool isTasting, bool isCompleted)
        {
            // 試食中インジケーター
            if (tastingInProgressIndicator != null)
            {
                tastingInProgressIndicator.SetActive(isTasting);
            }
            
            // 試食完了インジケーター
            if (tastingCompletedIndicator != null)
            {
                tastingCompletedIndicator.SetActive(isCompleted);
            }
            
            // ボタン状態
            if (startTastingButton != null)
            {
                startTastingButton.gameObject.SetActive(!isTasting && !isCompleted);
            }
            
            if (skipTastingButton != null)
            {
                skipTastingButton.gameObject.SetActive(isTasting && !isCompleted);
            }
            
            if (finishTastingButton != null)
            {
                finishTastingButton.gameObject.SetActive(isCompleted);
            }
            
            // ステータステキスト
            if (statusText != null)
            {
                if (isTasting)
                {
                    statusText.text = "たべてるよ...";
                }
                else if (isCompleted)
                {
                    statusText.text = "たべおわったよ！";
                }
                else
                {
                    statusText.text = "スープをたべてもらおう！";
                }
            }
        }
        
        /// <summary>
        /// リアクションパネルを表示
        /// </summary>
        private void ShowReactionPanel(GameObject character, CharacterReaction reaction)
        {
            if (reactionPanel == null) return;
            
            // パネルを表示
            reactionPanel.SetActive(true);
            
            // キャラクター情報設定
            if (characterImage != null)
            {
                // キャラクターの画像を取得
                SpriteRenderer charSprite = character.GetComponentInChildren<SpriteRenderer>();
                if (charSprite != null)
                {
                    characterImage.sprite = charSprite.sprite;
                }
            }
            
            // キャラクター名設定
            if (characterNameText != null)
            {
                // キャラクター名を設定（仮の実装）
                string characterName = character.name.Replace("Taster_", "").Split('_')[1];
                characterName = char.ToUpper(characterName[0]) + characterName.Substring(1);
                characterNameText.text = characterName;
            }
            
            // リアクションコメント設定
            if (reactionCommentText != null)
            {
                // ランダムなコメントを選択
                if (reactionComments.ContainsKey(reaction))
                {
                    string[] comments = reactionComments[reaction];
                    int randomIndex = UnityEngine.Random.Range(0, comments.Length);
                    reactionCommentText.text = comments[randomIndex];
                }
                else
                {
                    reactionCommentText.text = "...";
                }
            }
            
            // 星評価の設定
            SetStarRating(reaction);
            
            // アニメーションとエフェクト
            if (uiAnimator != null)
            {
                uiAnimator.SetTrigger("ShowReaction");
            }
            
            PlayReactionParticles(reaction);
        }
        
        /// <summary>
        /// リアクションパネルを非表示
        /// </summary>
        private void HideReactionPanel()
        {
            if (reactionPanel != null)
            {
                reactionPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// 星評価の設定
        /// </summary>
        /// <param name="reaction">キャラクターのリアクション</param>
        private void SetStarRating(CharacterReaction reaction)
        {
            if (reactionStars == null || reactionStars.Length == 0) return;
            
            // リアクションに基づいて星の数を決定
            int starCount = 0;
            switch (reaction)
            {
                case CharacterReaction.Love:
                    starCount = 5;
                    break;
                case CharacterReaction.Like:
                    starCount = 4;
                    break;
                case CharacterReaction.Neutral:
                    starCount = 3;
                    break;
                case CharacterReaction.Dislike:
                    starCount = 2;
                    break;
                case CharacterReaction.Disgust:
                    starCount = 1;
                    break;
            }
            
            // 星アイコンの表示/非表示を設定
            int totalStars = reactionStars.Length;
            for (int i = 0; i < totalStars; i++)
            {
                if (reactionStars[i] != null)
                {
                    reactionStars[i].SetActive(i < starCount);
                }
            }
        }
        
        /// <summary>
        /// リアクションに応じたパーティクルエフェクトを再生
        /// </summary>
        /// <param name="reaction">キャラクターのリアクション</param>
        private void PlayReactionParticles(CharacterReaction reaction)
        {
            if (reactionParticles == null) return;
            
            // パーティクルの色を設定
            var main = reactionParticles.main;
            
            switch (reaction)
            {
                case CharacterReaction.Love:
                    main.startColor = new ParticleSystem.MinMaxGradient(Color.magenta);
                    break;
                case CharacterReaction.Like:
                    main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow);
                    break;
                case CharacterReaction.Neutral:
                    main.startColor = new ParticleSystem.MinMaxGradient(Color.cyan);
                    break;
                case CharacterReaction.Dislike:
                    main.startColor = new ParticleSystem.MinMaxGradient(Color.blue);
                    break;
                case CharacterReaction.Disgust:
                    main.startColor = new ParticleSystem.MinMaxGradient(Color.gray);
                    break;
            }
            
            // パーティクル再生
            reactionParticles.Play();
        }
        
        /// <summary>
        /// スープの表示更新
        /// </summary>
        private void UpdateSoupDisplay()
        {
            if (soupImage == null || cookingSystem == null) return;
            
            // 調理システムからスープの色を取得
            Color soupColor = cookingSystem.CalculateSoupColor();
            
            // 色を適用
            soupImage.color = soupColor;
        }
        
        #region イベントハンドラ
        
        /// <summary>
        /// キャラクターのリアクションイベントハンドラ
        /// </summary>
        /// <param name="character">リアクションしたキャラクター</param>
        /// <param name="reaction">リアクション</param>
        private void HandleCharacterReaction(GameObject character, CharacterReaction reaction)
        {
            // リアクションパネルを表示
            ShowReactionPanel(character, reaction);
        }
        
        /// <summary>
        /// 試食完了イベントハンドラ
        /// </summary>
        private void HandleTastingCompleted()
        {
            // 状態更新
            isTastingInProgress = false;
            hasTastingCompleted = true;
            
            // UI状態更新
            UpdateUIState(isTastingInProgress, hasTastingCompleted);
        }
        
        #endregion
        
        #region ボタンイベントハンドラ
        
        /// <summary>
        /// 試食開始ボタンがクリックされた時の処理
        /// </summary>
        private void OnStartTastingButtonClicked()
        {
            if (tastingSystem == null) return;
            
            // 試食開始
            tastingSystem.StartTasting();
            
            // 状態更新
            isTastingInProgress = true;
            
            // UI状態更新
            UpdateUIState(isTastingInProgress, hasTastingCompleted);
            
            // ボタンアニメーション
            PlayButtonAnimation(startTastingButton.gameObject);
        }
        
        /// <summary>
        /// スキップボタンがクリックされた時の処理
        /// </summary>
        private void OnSkipTastingButtonClicked()
        {
            if (tastingSystem == null || !isTastingInProgress) return;
            
            // 試食をスキップ
            tastingSystem.SkipTasting();
            
            // 状態更新
            isTastingInProgress = false;
            hasTastingCompleted = true;
            
            // UI状態更新
            UpdateUIState(isTastingInProgress, hasTastingCompleted);
            
            // ボタンアニメーション
            PlayButtonAnimation(skipTastingButton.gameObject);
        }
        
        /// <summary>
        /// 試食完了ボタンがクリックされた時の処理
        /// </summary>
        private void OnFinishTastingButtonClicked()
        {
            if (!hasTastingCompleted) return;
            
            // ボタンアニメーション
            PlayButtonAnimation(finishTastingButton.gameObject);
            
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
            // 演出の時間
            yield return new WaitForSeconds(0.5f);
            
            // 次のステージ（結果）へ
            GameplayManager gameplayManager = GameplayManager.Instance;
            if (gameplayManager != null)
            {
                gameplayManager.ShowResult();
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// クレジットパネルクラス
public class CreditsPanel : MenuPanel
{
    [Header("Credits Content")]
    [SerializeField] private ScrollRect creditsScrollRect;
    [SerializeField] private TextMeshProUGUI creditsText;
    
    // コールバック参照
    private System.Action onBackToMainMenu;
    
    // クレジットテキスト
    private string defaultCreditsText = 
        "「おいしいスープをつくろう」\n\n" +
        "開発チーム\n" +
        "プログラマー：○○○○\n" +
        "デザイナー：○○○○\n" +
        "アートディレクター：○○○○\n" +
        "サウンドデザイナー：○○○○\n\n" +
        "音楽\n" +
        "○○○○\n\n" +
        "効果音\n" +
        "○○○○\n\n" +
        "テストプレイ協力\n" +
        "○○○○\n\n" +
        "スペシャルサンクス\n" +
        "○○○○\n\n" +
        "©2025 おいしいスープをつくろうチーム";
    
    public override void Initialize()
    {
        base.Initialize();
        
        // クレジットテキストの設定
        if (creditsText != null)
        {
            creditsText.text = defaultCreditsText;
        }
        
        // スクロールを一番上に設定
        if (creditsScrollRect != null)
        {
            creditsScrollRect.normalizedPosition = new Vector2(0, 1);
        }
        
        // 戻るボタンのコールバック設定
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => {
                PlayButtonSound();
                onBackToMainMenu?.Invoke();
            });
        }
    }
    
    // コールバック設定
    public void SetCallbacks(System.Action backCallback)
    {
        onBackToMainMenu = backCallback;
    }
    
    // 表示時にスクロールアニメーションを開始
    public override void Show()
    {
        base.Show();
        
        // スクロールを一番上に設定
        if (creditsScrollRect != null)
        {
            creditsScrollRect.normalizedPosition = new Vector2(0, 1);
            
            // 自動スクロールアニメーションを開始（オプション）
            StartAutoScroll();
        }
    }
    
    // 自動スクロールアニメーション
    private void StartAutoScroll()
    {
        // すでに実行中のコルーチンを停止
        StopAllCoroutines();
        
        // 新しいコルーチンを開始
        StartCoroutine(AutoScrollCredits());
    }
    
    // 自動スクロールコルーチン
    private IEnumerator AutoScrollCredits()
    {
        float scrollDuration = 20f; // スクロール完了までの時間（秒）
        float elapsedTime = 0f;
        
        // 初期位置を一番上に
        Vector2 startPos = new Vector2(0, 1);
        Vector2 endPos = new Vector2(0, 0);
        
        creditsScrollRect.normalizedPosition = startPos;
        
        while (elapsedTime < scrollDuration)
        {
            float t = elapsedTime / scrollDuration;
            creditsScrollRect.normalizedPosition = Vector2.Lerp(startPos, endPos, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 最終位置を確実に設定
        creditsScrollRect.normalizedPosition = endPos;
    }
    
    // ボタン効果音再生
    private void PlayButtonSound()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySFX("button_click");
        }
    }
}
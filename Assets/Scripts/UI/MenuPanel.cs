using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// メインメニューのパネルの挙動を扱う基底クラス
public abstract class MenuPanel : MonoBehaviour
{
    // 共通参照
    [SerializeField] protected Button backButton;
    [SerializeField] protected GameObject panelContainer;
    
    // パネル初期化
    public virtual void Initialize()
    {
        // バックボタンのイベント登録（実装がある場合）
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackButtonPressed);
        }
    }
    
    // パネルの表示
    public virtual void Show()
    {
        gameObject.SetActive(true);
        PlayShowAnimation();
    }
    
    // パネルの非表示
    public virtual void Hide()
    {
        PlayHideAnimation();
        gameObject.SetActive(false);
    }
    
    // 戻るボタンの処理
    protected virtual void OnBackButtonPressed()
    {
        // デフォルトでは何もしない。サブクラスでオーバーライドする
    }
    
    // 表示アニメーション
    protected virtual void PlayShowAnimation()
    {
        // デフォルトの表示アニメーション（オプション）
        if (panelContainer != null)
        {
            panelContainer.transform.localScale = Vector3.zero;
            LeanTween.scale(panelContainer, Vector3.one, 0.3f).setEaseOutBack();
        }
    }
    
    // 非表示アニメーション
    protected virtual void PlayHideAnimation()
    {
        // デフォルトの非表示アニメーション（オプション）
        if (panelContainer != null)
        {
            LeanTween.scale(panelContainer, Vector3.zero, 0.2f).setEaseInBack();
        }
    }
}
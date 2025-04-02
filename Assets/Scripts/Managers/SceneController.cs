using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneController : MonoBehaviour
{
    // 現在のシーン
    public string CurrentScene { get; private set; }
    
    // 遷移中フラグ
    private bool isTransitioning = false;
    
    // トランジション用オブジェクト
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private CanvasGroup transitionCanvasGroup;
    
    // イベント
    public event Action<string> OnSceneLoaded;
    public event Action<string> OnSceneUnloaded;
    public event Action<string, TransitionType, float> OnTransitionStarted;
    public event Action<string> OnTransitionCompleted;
    
    // Unity Lifecycle Callbacks
    private void Awake()
    {
        // 現在のシーン名を取得
        CurrentScene = SceneManager.GetActiveScene().name;
        
        // トランジションパネルの初期設定
        if (transitionPanel != null)
        {
            transitionPanel.SetActive(false);
        }
    }
    
    private void Start()
    {
        // シーンロードイベントの登録
        SceneManager.sceneLoaded += OnSceneLoadedCallback;
        SceneManager.sceneUnloaded += OnSceneUnloadedCallback;
    }
    
    private void OnDestroy()
    {
        // イベント解除
        SceneManager.sceneLoaded -= OnSceneLoadedCallback;
        SceneManager.sceneUnloaded -= OnSceneUnloadedCallback;
    }
    
    // シーンロードコールバック
    private void OnSceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        CurrentScene = scene.name;
        OnSceneLoaded?.Invoke(scene.name);
        
        Debug.Log($"Scene loaded: {scene.name}, Mode: {mode}");
    }
    
    // シーンアンロードコールバック
    private void OnSceneUnloadedCallback(Scene scene)
    {
        OnSceneUnloaded?.Invoke(scene.name);
        
        Debug.Log($"Scene unloaded: {scene.name}");
    }
    
    // シーンをロードするメソッド
    public void LoadScene(string sceneName)
    {
        if (!isTransitioning && sceneName != CurrentScene)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, LoadSceneMode.Single));
        }
    }
    
    // シーンを加算的にロードするメソッド
    public void LoadSceneAdditive(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, LoadSceneMode.Additive));
        }
    }
    
    // シーンをアンロードするメソッド
    public void UnloadScene(string sceneName)
    {
        if (!isTransitioning && sceneName != SceneManager.GetActiveScene().name)
        {
            StartCoroutine(UnloadSceneRoutine(sceneName));
        }
    }
    
    // 現在のシーンをリロードするメソッド
    public void ReloadCurrentScene()
    {
        if (!isTransitioning)
        {
            LoadScene(CurrentScene);
        }
    }
    
    // 遷移効果付きでシーンを切り替えるメソッド
    public void TransitionToScene(string sceneName, TransitionType transitionType, float duration)
    {
        if (!isTransitioning && sceneName != CurrentScene)
        {
            StartCoroutine(TransitionToSceneRoutine(sceneName, transitionType, duration));
        }
    }
    
    // シーンロードルーチン
    private IEnumerator LoadSceneRoutine(string sceneName, LoadSceneMode mode)
    {
        isTransitioning = true;
        
        // 非同期でシーンをロード
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
        asyncLoad.allowSceneActivation = true;
        
        // ロードが完了するまで待機
        while (!asyncLoad.isDone)
        {
            // 進行状況を取得（0.0〜0.9）
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            // ここで進行状況を表示するUI更新などが可能
            // Debug.Log($"Loading progress: {progress * 100}%");
            
            yield return null;
        }
        
        isTransitioning = false;
    }
    
    // シーンアンロードルーチン
    private IEnumerator UnloadSceneRoutine(string sceneName)
    {
        isTransitioning = true;
        
        // 非同期でシーンをアンロード
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
        
        // アンロードが完了するまで待機
        while (!asyncUnload.isDone)
        {
            yield return null;
        }
        
        isTransitioning = false;
    }
    
    // 遷移効果付きシーン切り替えルーチン
    private IEnumerator TransitionToSceneRoutine(string sceneName, TransitionType transitionType, float duration)
    {
        isTransitioning = true;
        
        // 遷移開始イベント発火
        OnTransitionStarted?.Invoke(sceneName, transitionType, duration);
        
        // トランジションエフェクトを表示
        if (transitionPanel != null)
        {
            transitionPanel.SetActive(true);
            
            // フェード効果
            if (transitionType == TransitionType.Fade && transitionCanvasGroup != null)
            {
                transitionCanvasGroup.alpha = 0f;
                
                // フェードイン
                float time = 0f;
                while (time < duration / 2)
                {
                    time += Time.deltaTime;
                    float t = time / (duration / 2);
                    transitionCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                    yield return null;
                }
                
                transitionCanvasGroup.alpha = 1f;
            }
            // 他のトランジション効果も実装可能
        }
        
        // シーン非同期ロード
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        // ロードが完了するまで待機（90%まで）
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        
        // 少し待機（フェード効果の見栄えのため）
        yield return new WaitForSeconds(0.1f);
        
        // シーンをアクティブ化
        asyncLoad.allowSceneActivation = true;
        
        // シーンのロードが完了するまで待機
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // トランジションエフェクトを非表示
        if (transitionPanel != null)
        {
            // フェード効果
            if (transitionType == TransitionType.Fade && transitionCanvasGroup != null)
            {
                // フェードアウト
                float time = 0f;
                while (time < duration / 2)
                {
                    time += Time.deltaTime;
                    float t = time / (duration / 2);
                    transitionCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                    yield return null;
                }
                
                transitionCanvasGroup.alpha = 0f;
            }
            
            transitionPanel.SetActive(false);
        }
        
        // 遷移完了イベント発火
        OnTransitionCompleted?.Invoke(sceneName);
        
        isTransitioning = false;
    }
}

// 遷移タイプ
public enum TransitionType
{
    Fade,   // フェード効果
    Slide,  // スライド効果
    Zoom    // ズーム効果
}

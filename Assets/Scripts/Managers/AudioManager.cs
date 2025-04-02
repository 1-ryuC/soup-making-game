using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    // オーディオソース
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;
    
    // オーディオアセット
    [Header("Audio Assets")]
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private AudioClip[] voiceClips;
    
    // オーディオディクショナリー
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> voiceDictionary = new Dictionary<string, AudioClip>();
    
    // 音量設定
    private float bgmVolume = 0.7f;
    private float sfxVolume = 0.8f;
    private float voiceVolume = 1.0f;
    
    // クロスフェード用変数
    private Coroutine fadeCoroutine;
    
    // Unity Lifecycle Callbacks
    private void Awake()
    {
        // ディクショナリーの初期化
        InitializeAudioDictionaries();
        
        // オーディオソースの初期設定
        InitializeAudioSources();
    }
    
    // オーディオディクショナリーの初期化
    private void InitializeAudioDictionaries()
    {
        // BGMディクショナリーの初期化
        foreach (AudioClip clip in bgmClips)
        {
            if (clip != null)
            {
                bgmDictionary[clip.name] = clip;
            }
        }
        
        // SFXディクショナリーの初期化
        foreach (AudioClip clip in sfxClips)
        {
            if (clip != null)
            {
                sfxDictionary[clip.name] = clip;
            }
        }
        
        // Voiceディクショナリーの初期化
        foreach (AudioClip clip in voiceClips)
        {
            if (clip != null)
            {
                voiceDictionary[clip.name] = clip;
            }
        }
        
        Debug.Log($"Audio Manager initialized with {bgmDictionary.Count} BGMs, {sfxDictionary.Count} SFXs, {voiceDictionary.Count} Voices");
    }
    
    // オーディオソースの初期設定
    private void InitializeAudioSources()
    {
        // BGM用AudioSourceの設定
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        bgmSource.playOnAwake = false;
        
        // SFX用AudioSourceの設定
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
        sfxSource.playOnAwake = false;
        
        // Voice用AudioSourceの設定
        if (voiceSource == null)
        {
            voiceSource = gameObject.AddComponent<AudioSource>();
        }
        voiceSource.loop = false;
        voiceSource.volume = voiceVolume;
        voiceSource.playOnAwake = false;
    }
    
    // BGM関連メソッド
    
    // BGMを再生するメソッド
    public void PlayBGM(string clipName, bool loop = true)
    {
        if (bgmDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
            Debug.Log($"Playing BGM: {clipName}");
        }
        else
        {
            Debug.LogWarning($"BGM clip not found: {clipName}");
        }
    }
    
    // BGMを停止するメソッド
    public void StopBGM()
    {
        bgmSource.Stop();
    }
    
    // BGMを一時停止するメソッド
    public void PauseBGM()
    {
        bgmSource.Pause();
    }
    
    // 一時停止したBGMを再開するメソッド
    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }
    
    // BGMをフェードするメソッド
    public void FadeBGM(float duration, float targetVolume = 0f)
    {
        // 既存のフェードがある場合は停止
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        // フェードコルーチンを開始
        fadeCoroutine = StartCoroutine(FadeBGMCoroutine(duration, targetVolume));
    }
    
    // BGMフェードコルーチン
    private IEnumerator FadeBGMCoroutine(float duration, float targetVolume)
    {
        float startVolume = bgmSource.volume;
        float time = 0f;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }
        
        bgmSource.volume = targetVolume;
        
        // ターゲットボリュームが0の場合は停止
        if (targetVolume <= 0f)
        {
            StopBGM();
            bgmSource.volume = bgmVolume; // ボリュームを元に戻す
        }
        
        fadeCoroutine = null;
    }
    
    // BGMをクロスフェードするメソッド
    public void CrossFadeBGM(string clipName, float duration = 1.0f, bool loop = true)
    {
        if (bgmDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            StartCoroutine(CrossFadeBGMCoroutine(clip, duration, loop));
        }
        else
        {
            Debug.LogWarning($"BGM clip not found: {clipName}");
        }
    }
    
    // BGMクロスフェードコルーチン
    private IEnumerator CrossFadeBGMCoroutine(AudioClip newClip, float duration, bool loop)
    {
        // 新しいAudioSourceを作成
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = newClip;
        newSource.loop = loop;
        newSource.volume = 0f;
        newSource.Play();
        
        // 現在のBGMをフェードアウト、新しいBGMをフェードイン
        float time = 0f;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            bgmSource.volume = Mathf.Lerp(bgmVolume, 0f, t);
            newSource.volume = Mathf.Lerp(0f, bgmVolume, t);
            yield return null;
        }
        
        // 現在のBGMを停止し、新しいBGMをメインソースに設定
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
        
        // 一時的に作成したソースを破棄
        Destroy(newSource);
    }
    
    // SFX関連メソッド
    
    // 効果音を再生するメソッド
    public void PlaySFX(string clipName)
    {
        if (sfxDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
        else
        {
            Debug.LogWarning($"SFX clip not found: {clipName}");
        }
    }
    
    // ランダムな効果音を再生するメソッド
    public void PlayRandomSFX(string[] clipNames)
    {
        if (clipNames.Length > 0)
        {
            string randomClipName = clipNames[Random.Range(0, clipNames.Length)];
            PlaySFX(randomClipName);
        }
    }
    
    // Voice関連メソッド
    
    // ボイスを再生するメソッド
    public void PlayVoice(string clipName)
    {
        if (voiceDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            voiceSource.Stop(); // 現在再生中のボイスを停止
            voiceSource.clip = clip;
            voiceSource.Play();
        }
        else
        {
            Debug.LogWarning($"Voice clip not found: {clipName}");
        }
    }
    
    // ボイスを停止するメソッド
    public void StopVoice()
    {
        voiceSource.Stop();
    }
    
    // 音量調整メソッド
    
    // BGM音量を設定するメソッド
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }
    
    // SFX音量を設定するメソッド
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }
    
    // Voice音量を設定するメソッド
    public void SetVoiceVolume(float volume)
    {
        voiceVolume = Mathf.Clamp01(volume);
        voiceSource.volume = voiceVolume;
    }
    
    // 全体音量を設定するメソッド
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }
    
    // リソース管理メソッド
    
    // 全てのオーディオクリップをロードするメソッド
    public void LoadAllAudioClips()
    {
        // リソースフォルダから全てのBGMをロード
        AudioClip[] loadedBGMs = Resources.LoadAll<AudioClip>("Audio/BGM");
        foreach (AudioClip clip in loadedBGMs)
        {
            if (clip != null && !bgmDictionary.ContainsKey(clip.name))
            {
                bgmDictionary[clip.name] = clip;
            }
        }
        
        // リソースフォルダから全てのSFXをロード
        AudioClip[] loadedSFXs = Resources.LoadAll<AudioClip>("Audio/SFX");
        foreach (AudioClip clip in loadedSFXs)
        {
            if (clip != null && !sfxDictionary.ContainsKey(clip.name))
            {
                sfxDictionary[clip.name] = clip;
            }
        }
        
        // リソースフォルダから全てのVoiceをロード
        AudioClip[] loadedVoices = Resources.LoadAll<AudioClip>("Audio/Voice");
        foreach (AudioClip clip in loadedVoices)
        {
            if (clip != null && !voiceDictionary.ContainsKey(clip.name))
            {
                voiceDictionary[clip.name] = clip;
            }
        }
        
        Debug.Log($"All audio clips loaded: {bgmDictionary.Count} BGMs, {sfxDictionary.Count} SFXs, {voiceDictionary.Count} Voices");
    }
    
    // 指定したオーディオクリップをロードするメソッド
    public void LoadAudioClip(string clipName, string clipPath)
    {
        AudioClip loadedClip = Resources.Load<AudioClip>(clipPath);
        
        if (loadedClip != null)
        {
            // パスの種類によって適切なディクショナリーに追加
            if (clipPath.Contains("/BGM/"))
            {
                bgmDictionary[clipName] = loadedClip;
            }
            else if (clipPath.Contains("/SFX/"))
            {
                sfxDictionary[clipName] = loadedClip;
            }
            else if (clipPath.Contains("/Voice/"))
            {
                voiceDictionary[clipName] = loadedClip;
            }
            
            Debug.Log($"Audio clip loaded: {clipName}");
        }
        else
        {
            Debug.LogWarning($"Failed to load audio clip: {clipPath}");
        }
    }
    
    // 不要なオーディオクリップをアンロードするメソッド
    public void UnloadUnusedAudioClips()
    {
        Resources.UnloadUnusedAssets();
        Debug.Log("Unloaded unused audio assets");
    }
}

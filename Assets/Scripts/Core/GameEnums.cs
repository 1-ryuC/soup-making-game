using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームの状態を表す列挙型
public enum GameState
{
    MainMenu,       // メインメニュー画面
    Gameplay,       // ゲームプレイ中
    Paused,         // 一時停止中
    GameOver,       // ゲーム終了（失敗）
    GameClear,      // ゲームクリア（成功）
    Loading,        // 読み込み中
    Tutorial        // チュートリアル中
}

// ゲームモードを表す列挙型
public enum GameMode
{
    FreePlay,       // 自由に材料を選んでスープを作れるモード
    Mission,        // ミッションモード（特定の条件を満たす）
    Recipe,         // レシピモード（決まったレシピに従う）
    ParentChild     // 親子モード（協力プレイ）
}

// 食材のカテゴリを表す列挙型
public enum IngredientCategory
{
    Vegetable,      // 野菜類
    Fruit,          // 果物類
    Protein,        // タンパク質（肉、魚、豆類など）
    Seasoning,      // 調味料
    Special         // 特別な食材
}

// 料理の状態を表す列挙型
public enum CookingState
{
    Idle,           // 調理開始前
    Preparing,      // 下準備（洗う、切るなど）
    Cooking,        // 調理中
    Simmering,      // 煮込み中
    Completed       // 完成
}

// キャラクターの反応を表す列挙型
public enum CharacterReaction
{
    Love,           // 大好き（最高評価）
    Like,           // 好き（良い評価）
    Neutral,        // 普通
    Dislike,        // 苦手（悪い評価）
    Disgust         // 最悪（最低評価）
}

# TastingUI プレハブ構築ガイド

## 1. プレハブの基本構造

```
TastingPanel (GameObject)
├── PanelBackground (Image)
├── TitleBar
│   ├── Title (TextMeshPro)
│   └── StatusText (TextMeshPro)
├── TasterArea
│   ├── TasterDisplay (空のコンテナ - 試食キャラクターの映り込み用)
│   ├── SoupContainer
│   │   ├── SoupBowlImage (Image)
│   │   └── SoupImage (Image)
│   └── TastingInProgressIndicator (GameObject)
│       ├── ProgressIcon (Animation)
│       └── WaitText (TextMeshPro)
├── ReactionPanel (GameObject)
│   ├── CharacterImage (Image)
│   ├── CharacterNameText (TextMeshPro)
│   ├── ReactionCommentText (TextMeshPro)
│   ├── ReactionStars
│   │   ├── Star1 (Image)
│   │   ├── Star2 (Image)
│   │   ├── Star3 (Image)
│   │   ├── Star4 (Image)
│   │   └── Star5 (Image)
│   └── ReactionParticles (ParticleSystem)
├── TastingCompletedIndicator (GameObject)
│   ├── CompletedIcon (Image)
│   └── CompletedText (TextMeshPro)
└── ButtonsContainer
    ├── StartTastingButton (Button)
    │   ├── Icon (Image)
    │   └── Label (TextMeshPro)
    ├── SkipTastingButton (Button)
    │   ├── Icon (Image) 
    │   └── Label (TextMeshPro)
    └── FinishTastingButton (Button)
        ├── Icon (Image)
        └── Label (TextMeshPro)
```

## 2. 詳細な構築手順

### 2.1 基本パネルの設定

1. 新しいGameObjectを作成し、名前を「TastingPanel」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Stretch-Stretch
   - Left/Right/Top/Bottom: 0
3. Image コンポーネントを追加
   - Color: 軽い半透明ベージュ (#FFF5E6 with Alpha = 0.9)
   - Image Type: Sliced
   - 丸みを帯びた角のあるスプライトを使用
4. TastingUI スクリプトをアタッチ
5. Animator コンポーネントを追加
   - Controller: TastingPanelAnimator (新規作成)

### 2.2 タイトルバーの設定

1. TastingPanel の下に空のGameObjectを作成し、名前を「TitleBar」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Top-Stretch
   - Height: 120
   - Top: 0
   - Left/Right: 20
3. Horizontal Layout Group コンポーネントを追加
   - Child Alignment: Middle Center
   - Child Force Expand Width: false
   - Spacing: 20
   - Padding: Left=20, Right=20, Top=10, Bottom=10

4. TitleBar の下に TextMeshProUGUI を作成し、名前を「Title」に設定
   - Text: 「スープをたべてもらおう！」
   - Font Size: 48
   - Font Style: Bold
   - Color: プライマリーカラー (#FF9A3C)
   - Alignment: Center

5. TitleBar の下に TextMeshProUGUI を作成し、名前を「StatusText」に設定
   - Text: 「スープをたべてもらおう！」
   - Font Size: 36
   - Font Style: Normal
   - Color: セカンダリーカラー (#4DB6E5)
   - Alignment: Center

### 2.3 試食エリアの設定

1. TastingPanel の下に空のGameObjectを作成し、名前を「TasterArea」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Middle-Center
   - Size: 600 x 400
   - Position: Y=50

3. TasterArea の下に空のGameObjectを作成し、名前を「TasterDisplay」に設定
   - RectTransform: 親と同じサイズ
   - これは3Dシーンの試食キャラクターが見える領域

4. TasterArea の下に空のGameObjectを作成し、名前を「SoupContainer」に設定
   - RectTransform: 幅300 x 高さ200
   - 位置: 下側中央

5. SoupContainer の下に Image を作成し、名前を「SoupBowlImage」に設定
   - Sprite: お椀・器の画像
   - Preserve Aspect: true
   - RectTransform: 親と同じサイズ

6. SoupContainer の下に Image を作成し、名前を「SoupImage」に設定
   - Sprite: 液体のイメージ(円形)
   - Color: #FF9A3C (デフォルト色、後で動的に変更)
   - RectTransform: SoupBowlImageの中に収まるサイズ

7. TasterArea の下に空のGameObjectを作成し、名前を「TastingInProgressIndicator」に設定
   - RectTransform: 幅200 x 高さ200
   - 位置: 中央上部
   - 初期状態は非アクティブ (SetActive(false))

8. TastingInProgressIndicator の下に Image を作成し、名前を「ProgressIcon」に設定
   - Sprite: 回転する食事アイコンやスプーンなど
   - Animator コンポーネントを追加
     - 回転アニメーションを設定
   - RectTransform: 幅100 x 高さ100

9. TastingInProgressIndicator の下に TextMeshProUGUI を作成し、名前を「WaitText」に設定
   - Text: 「たべてるよ...」
   - Font Size: 30
   - Color: セカンダリーカラー (#4DB6E5)
   - RectTransform: ProgressIconの下に配置

### 2.4 リアクションパネルの設定

1. TastingPanel の下に空のGameObjectを作成し、名前を「ReactionPanel」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Middle-Center
   - Size: 500 x 300
   - Position: Y=0
   - 初期状態は非アクティブ (SetActive(false))

3. Image コンポーネントを追加
   - Color: 白 (#FFFFFF with Alpha = 0.95)
   - Image Type: Sliced
   - 丸みを帯びた角のあるスプライトを使用

4. ReactionPanel の下に Image を作成し、名前を「CharacterImage」に設定
   - Sprite: デフォルトキャラクター画像
   - Preserve Aspect: true
   - RectTransform: 左側に配置、幅150 x 高さ150

5. ReactionPanel の下に TextMeshProUGUI を作成し、名前を「CharacterNameText」に設定
   - Text: 「なまえ」
   - Font Size: 30
   - Font Style: Bold
   - Color: プライマリーカラー (#FF9A3C)
   - RectTransform: CharacterImageの上に配置

6. ReactionPanel の下に TextMeshProUGUI を作成し、名前を「ReactionCommentText」に設定
   - Text: 「おいしい！」
   - Font Size: 36
   - Font Style: Bold
   - Color: 黒 (#000000)
   - RectTransform: 中央に配置
   - Alignment: Center

7. ReactionPanel の下に空のGameObjectを作成し、名前を「ReactionStars」に設定
   - RectTransform: 下部中央に配置
   - Horizontal Layout Group コンポーネントを追加
     - Child Alignment: Middle Center
     - Spacing: 10
     - Child Force Expand: Width, Height = false

8. ReactionStars の下に5つのImage（Star1〜Star5）を作成
   - Sprite: 星のアイコン
   - Color: 明るい黄色 (#FFEB3B)
   - RectTransform: 幅50 x 高さ50

9. ReactionPanel の下に ParticleSystemを追加し、名前を「ReactionParticles」に設定
   - シンプルな輝きパーティクル設定
   - リアクションに応じた色で発光

### 2.5 試食完了インジケーターの設定

1. TastingPanel の下に空のGameObjectを作成し、名前を「TastingCompletedIndicator」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Top-Right
   - Size: 300 x 100
   - Position: X=-50, Y=-30
   - 初期状態は非アクティブ (SetActive(false))

3. TastingCompletedIndicator の下に Image を作成し、名前を「CompletedIcon」に設定
   - Sprite: チェックマークや完了アイコン
   - Color: アクセントカラー (#7ED957)
   - RectTransform: 左側に配置

4. TastingCompletedIndicator の下に TextMeshProUGUI を作成し、名前を「CompletedText」に設定
   - Text: 「たべおわったよ！」
   - Font Size: 30
   - Color: アクセントカラー (#7ED957)
   - RectTransform: CompletedIconの右側に配置

### 2.6 ボタン部分の設定

1. TastingPanel の下に空のGameObjectを作成し、名前を「ButtonsContainer」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Bottom-Center
   - Size: 600 x 100
   - Bottom: 30

3. Horizontal Layout Group コンポーネントを追加
   - Child Alignment: Middle Center
   - Spacing: 50
   - Padding: 20
   - Control Child Size: Width, Height = true
   - Child Force Expand: Width, Height = false

4. ButtonsContainer の下に Button を作成し、名前を「StartTastingButton」に設定
   - Transition: Color Tint
   - Colors: Normal=#FF9A3C, Highlighted=明るいオレンジ, Pressed=暗いオレンジ
   - Image Type: Sliced（丸みを帯びた角）
   - Text: 「たべてもらう」（TextMeshPro使用）
   - Font Size: 30
   - Button Size: 200 x 80
   - 「食べる」アイコン画像を追加

5. 同様に「SkipTastingButton」と「FinishTastingButton」を作成
   - SkipTastingButton: 
     - Text: 「スキップする」
     - Color: グレー (#BDBDBD)
     - Size: 200 x 80
     - 「早送り」アイコン画像
     - 初期状態は非アクティブ (SetActive(false))
   
   - FinishTastingButton: 
     - Text: 「つぎへすすむ」
     - Color: アクセントカラー (#7ED957)
     - Size: 200 x 80
     - 「次へ」アイコン画像
     - 初期状態は非アクティブ (SetActive(false))

## 3. コンポーネント参照の設定

TastingUI スクリプトの各参照フィールドに、作成した UI 要素をドラッグ＆ドロップして設定します：

1. **試食UI要素**
   - Start Tasting Button: StartTastingButton
   - Skip Tasting Button: SkipTastingButton
   - Finish Tasting Button: FinishTastingButton

2. **試食状態表示**
   - Status Text: StatusText
   - Tasting In Progress Indicator: TastingInProgressIndicator
   - Tasting Completed Indicator: TastingCompletedIndicator

3. **キャラクターリアクション表示**
   - Reaction Panel: ReactionPanel
   - Character Image: CharacterImage
   - Character Name Text: CharacterNameText
   - Reaction Comment Text: ReactionCommentText
   - Reaction Stars: Star1〜Star5をドラッグ＆ドロップ

4. **視覚エフェクト**
   - Reaction Particles: ReactionParticles
   - UI Animator: TastingPanelのAnimator

5. **スープ表示**
   - Soup Image: SoupImage

## 4. アニメーションとトランジション設定

### 4.1 ボタンのトランジション設定

1. StartTastingButton, SkipTastingButton, FinishTastingButtonそれぞれに対して：
   - Transition: Color Tint
   - Normal Color: 各ボタンの基本色
   - Highlighted Color: 基本色より20%明るく
   - Pressed Color: 基本色より20%暗く
   - Disabled Color: グレー (#BDBDBD)

### 4.2 UIアニメーターの設定

TastingPanelのAnimatorに以下のアニメーションを設定：

1. **ShowReaction**トリガーを作成
2. 「ShowReaction」アニメーションを作成
   - ReactionPanelのスケールを0から1に拡大（0.3秒）
   - ReactionStarsの各星を順番に表示
   - ReactionParticlesを再生

### 4.3 進行中インジケーターのアニメーション

TastingInProgressIndicatorにアニメーションを設定：

1. ProgressIconに回転アニメーションを設定
   - Z軸360度回転（1秒間）、ループ設定
   - 軽いスケール変化も追加して動きを強調

2. WaitTextの点滅アニメーション
   - テキストの透明度を0.5〜1.0の間で変化
   - 1秒周期でループ

## 5. アクセシビリティ対応

1. ボタンサイズは十分に大きく設定（最小80x80ピクセル）
2. テキストフォントサイズは読みやすいサイズ（最小24pt）
3. 色のコントラストは十分に高く設定
4. 状態のテキスト表示とアイコン表示の併用
5. リアクションパネルの背景色と文字色のコントラスト確保

## 6. 完成と確認

1. 前述の設定をすべて完了後、TastingPanelをPrefabsフォルダにドラッグ＆ドロップしてプレハブとして保存
2. 名前を「TastingUI」に設定
3. 実行時の動作を確認し、必要に応じて調整

このプレハブは、試食シーンでGameplayUIControllerから参照され、試食状態に応じて表示されます。
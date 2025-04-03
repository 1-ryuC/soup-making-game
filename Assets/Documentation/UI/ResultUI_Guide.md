# ResultUI プレハブ構築ガイド

## 1. プレハブの基本構造

```
ResultPanel (GameObject)
├── PanelBackground (Image)
├── ResultHeader
│   ├── Title (TextMeshPro)
│   └── ResultTitleText (TextMeshPro)
├── StarsContainer
│   ├── Star1 (Image/GameObject)
│   ├── Star2 (Image/GameObject)
│   ├── Star3 (Image/GameObject)
│   ├── Star4 (Image/GameObject)
│   └── Star5 (Image/GameObject)
├── ResultDescriptionText (TextMeshPro)
├── SoupDisplay
│   ├── SoupImage (Image)
│   ├── SoupBowlImage (Image)
│   └── SoupCharacter (Image/Animation)
├── IngredientInfoContainer
│   ├── IngredientCountText (TextMeshPro)
│   └── UsedIngredientsContainer (GameObject)
│       ├── IngredientIcon1 (Prefab Instance)
│       ├── IngredientIcon2 (Prefab Instance)
│       └── ... (動的に生成)
└── ButtonsContainer
    ├── RetryButton (Button)
    │   ├── Icon (Image)
    │   └── Label (TextMeshPro)
    ├── MainMenuButton (Button)
    │   ├── Icon (Image)
    │   └── Label (TextMeshPro)
    └── SaveRecipeButton (Button)
        ├── Icon (Image)
        └── Label (TextMeshPro)
```

## 2. 詳細な構築手順

### 2.1 基本パネルの設定

1. 新しいGameObjectを作成し、名前を「ResultPanel」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Stretch-Stretch
   - Left/Right/Top/Bottom: 0
3. Image コンポーネントを追加
   - Color: 軽い半透明ベージュ (#FFF5E6 with Alpha = 0.95)
   - Image Type: Sliced
   - 丸みを帯びた角のあるスプライトを使用
4. ResultUI スクリプトをアタッチ
5. Animator コンポーネントを追加
   - Controller: ResultPanelAnimator (新規作成)

### 2.2 結果ヘッダーの設定

1. ResultPanel の下に空のGameObjectを作成し、名前を「ResultHeader」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Top-Stretch
   - Height: 180
   - Top: 0
   - Left/Right: 20
3. Vertical Layout Group コンポーネントを追加
   - Child Alignment: Middle Center
   - Spacing: 10
   - Padding: 20
   - Control Child Size: Width, Height = true
   - Child Force Expand: Width = true, Height = false

4. ResultHeader の下に TextMeshProUGUI を作成し、名前を「Title」に設定
   - Text: 「けっか はっぴょう！」
   - Font Size: 48
   - Font Style: Bold
   - Color: プライマリーカラー (#FF9A3C)
   - Alignment: Center

5. ResultHeader の下に TextMeshProUGUI を作成し、名前を「ResultTitleText」に設定
   - Text: 「せいこう！」(スクリプトで動的に変更)
   - Font Size: 60
   - Font Style: Bold
   - Color: アクセントカラー (#7ED957)
   - Alignment: Center
   - 初期状態は非アクティブ (SetActive(false))

### 2.3 星評価表示部分の設定

1. ResultPanel の下に空のGameObjectを作成し、名前を「StarsContainer」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Top-Center
   - Size: 500 x 100
   - Position: Y=-200
3. Horizontal Layout Group コンポーネントを追加
   - Child Alignment: Middle Center
   - Spacing: 20
   - Padding: 0
   - Control Child Size: Width, Height = true
   - Child Force Expand: Width, Height = false
   - Child Size: 80, 80

4. StarsContainer の下に5つの星オブジェクト（Star1〜Star5）を作成
   - 各星はGameObjectとして作成
   - 各星にImage コンポーネントを追加
     - Sprite: 金色の星アイコン
     - Color: 明るい黄色 (#FFEB3B)
     - 初期状態は非アクティブ (SetActive(false))
   - 各星にScale Animation用のAnimator を追加（オプション）

5. StarEffectParent という空のGameObjectを作成し、星エフェクト用の親として設定
   - Position: StarsContainerと同じ位置
   - これは動的に生成される星エフェクトの親になる

### 2.4 結果説明テキストの設定

1. ResultPanel の下に TextMeshProUGUI を作成し、名前を「ResultDescriptionText」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Middle-Center
   - Size: 600 x 150
   - Position: Y=-20
3. テキスト設定
   - Text: 「おいしいスープができたね！\nみんなたのしそう！」(スクリプトで動的に変更)
   - Font Size: 36
   - Font Style: Normal
   - Color: 黒 (#000000)
   - Alignment: Center
   - 初期状態は非アクティブ (SetActive(false))

### 2.5 スープ表示部分の設定

1. ResultPanel の下に空のGameObjectを作成し、名前を「SoupDisplay」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Middle-Left
   - Size: 300 x 300
   - Position: X=150, Y=-50
3. SoupDisplay の下に Image を作成し、名前を「SoupBowlImage」に設定
   - Sprite: お椀・器の画像
   - Preserve Aspect: true
   - RectTransform: 親と同じサイズ

4. SoupDisplay の下に Image を作成し、名前を「SoupImage」に設定
   - Sprite: 液体のイメージ(円形)
   - Color: #FF9A3C (デフォルト色、後で動的に変更)
   - RectTransform: SoupBowlImageの中に収まるサイズ

5. SoupDisplay の下に Image を作成し、名前を「SoupCharacter」に設定（オプション）
   - Sprite: スープくんのキャラクター
   - RectTransform: 小さめに設定し、スープの上に配置
   - Animator コンポーネントを追加
     - 喜びのアニメーションを設定

### 2.6 食材情報表示部分の設定

1. ResultPanel の下に空のGameObjectを作成し、名前を「IngredientInfoContainer」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Middle-Right
   - Size: 400 x 400
   - Position: X=-150, Y=-50
3. Vertical Layout Group コンポーネントを追加
   - Child Alignment: Upper Center
   - Spacing: 20
   - Padding: 20
   - Control Child Size: Width, Height = false
   - Child Force Expand: Width, Height = false

4. IngredientInfoContainer の下に TextMeshProUGUI を作成し、名前を「IngredientCountText」に設定
   - Text: 「つかった しょくざい: 0こ」
   - Font Size: 30
   - Color: 黒 (#000000)
   - Alignment: Center

5. IngredientInfoContainer の下に空のGameObjectを作成し、名前を「UsedIngredientsContainer」に設定
   - RectTransform: 幅380 x 高さ300
   - Grid Layout Group コンポーネントを追加
     - Cell Size: 80, 80
     - Spacing: 10, 10
     - Start Corner: Upper Left
     - Start Axis: Horizontal
     - Child Alignment: Upper Left
     - Constraint: Flexible

6. 食材アイコンプレハブの作成（IngredientIconPrefab）
   - 小さなGameObjectを作成
   - Image コンポーネントを追加（食材のアイコン）
   - TextMeshProUGUI を追加（食材名）
   - 適切にサイズと配置を調整
   - プレハブとして保存

### 2.7 ボタン部分の設定

1. ResultPanel の下に空のGameObjectを作成し、名前を「ButtonsContainer」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Bottom-Center
   - Size: 800 x 100
   - Bottom: 30
3. Horizontal Layout Group コンポーネントを追加
   - Child Alignment: Middle Center
   - Spacing: 50
   - Padding: 20
   - Control Child Size: Width, Height = true
   - Child Force Expand: Width, Height = false

4. ButtonsContainer の下に Button を作成し、名前を「RetryButton」に設定
   - Transition: Color Tint
   - Colors: Normal=#FF9A3C, Highlighted=明るいオレンジ, Pressed=暗いオレンジ
   - Image Type: Sliced（丸みを帯びた角）
   - Text: 「もういちど」（TextMeshPro使用）
   - Font Size: 30
   - Button Size: 220 x 80
   - リトライアイコン画像を追加
   - 初期状態は非アクティブ (SetActive(false))

5. 同様に「MainMenuButton」と「SaveRecipeButton」を作成
   - MainMenuButton: 
     - Text: 「メニューにもどる」
     - Color: セカンダリーカラー (#4DB6E5)
     - Size: 220 x 80
     - ホームアイコン画像
     - 初期状態は非アクティブ (SetActive(false))
   
   - SaveRecipeButton: 
     - Text: 「レシピをほぞん」
     - Color: アクセントカラー (#7ED957)
     - Size: 220 x 80
     - 保存アイコン画像
     - 初期状態は非アクティブ (SetActive(false))

## 3. コンポーネント参照の設定

ResultUI スクリプトの各参照フィールドに、作成した UI 要素をドラッグ＆ドロップして設定します：

1. **結果表示**
   - Star Objects: Star1〜Star5をArray要素としてドラッグ＆ドロップ
   - Star Effect Prefab: 星エフェクトのプレハブ（別途作成）
   - Star Effect Parent: StarEffectParent
   - Result Title Text: ResultTitleText
   - Result Description Text: ResultDescriptionText

2. **スープ情報表示**
   - Soup Image: SoupImage
   - Ingredient Count Text: IngredientCountText
   - Used Ingredients Container: UsedIngredientsContainer
   - Ingredient Icon Prefab: IngredientIconPrefab

3. **ボタン**
   - Retry Button: RetryButton
   - Main Menu Button: MainMenuButton
   - Save Recipe Button: SaveRecipeButton

4. **アニメーション**
   - Panel Animator: ResultPanelのAnimator

## 4. アニメーションとトランジション設定

### 4.1 ボタンのトランジション設定

1. RetryButton, MainMenuButton, SaveRecipeButtonそれぞれに対して：
   - Transition: Color Tint
   - Normal Color: 各ボタンの基本色
   - Highlighted Color: 基本色より20%明るく
   - Pressed Color: 基本色より20%暗く
   - Disabled Color: グレー (#BDBDBD)

### 4.2 パネルアニメーションの設定

ResultPanelのAnimatorに以下のアニメーションを設定：

1. **Show**トリガーを作成
2. 「Show」アニメーション
   - パネル全体のフェードイン（0.5秒）
   - 軽いスケールアップエフェクト

### 4.3 星アニメーションの設定

各星（Star1〜Star5）に対して：

1. 表示時のスケールアニメーション
   - 0から1.2へスケールアップ（0.2秒）
   - 1.2から1.0へスケールダウン（0.1秒）
   - 光るエフェクト（色の明るさ変動）

### 4.4 星エフェクトプレハブの作成

1. 新しいGameObjectを作成し、名前を「StarEffectPrefab」に設定
2. ParticleSystem コンポーネントを追加
   - 星型または輝き型のパーティクル
   - 短時間（1-2秒）で消滅
   - 明るい黄色の色調
3. プレハブとして保存

## 5. アクセシビリティ対応

1. ボタンサイズは十分に大きく設定（最小80x80ピクセル）
2. テキストフォントサイズは読みやすいサイズ（最小30pt）
3. 色のコントラストは十分に高く設定
4. 結果の視覚的表現（星）と同時にテキストによる説明も提供
5. ボタンには適切なアイコンとテキストラベルの両方を配置

## 6. 完成と確認

1. 前述の設定をすべて完了後、ResultPanelをPrefabsフォルダにドラッグ＆ドロップしてプレハブとして保存
2. 名前を「ResultUI」に設定
3. 実行時の動作を確認し、必要に応じて調整

このプレハブは、結果表示シーンでGameplayUIControllerから参照され、ゲーム結果に応じて表示されます。
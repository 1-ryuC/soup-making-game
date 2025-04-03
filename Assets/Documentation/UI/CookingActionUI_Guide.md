# CookingActionUI プレハブ構築ガイド

## 1. プレハブの基本構造

```
CookingPanel (GameObject)
├── PanelBackground (Image)
├── TitleBar
│   ├── Title (TextMeshPro)
│   └── StatusText (TextMeshPro)
├── SoupContainer
│   ├── PotImage (Image)
│   ├── SoupImage (Image)
│   └── SteamParticle (ParticleSystem)
├── ProgressContainer
│   ├── ProgressBar (Slider)
│   ├── ProgressText (TextMeshPro)
│   └── CookingStateIcons
│       ├── PreparingStateIcon (GameObject)
│       ├── CookingStateIcon (GameObject)
│       ├── SimmeringStateIcon (GameObject)
│       └── CompletedStateIcon (GameObject)
├── ActionButtonsContainer
│   ├── StirButton (Button)
│   │   ├── Icon (Image)
│   │   └── Label (TextMeshPro)
│   ├── AddIngredientButton (Button)
│   │   ├── Icon (Image)
│   │   └── Label (TextMeshPro)
│   └── FinishCookingButton (Button)
│       ├── Icon (Image)
│       └── Label (TextMeshPro)
└── TemperatureControlContainer
    ├── Label (TextMeshPro)
    ├── TemperatureSlider (Slider)
    │   ├── Background (Image)
    │   ├── Fill (Image)
    │   └── Handle (Image)
    └── FlameImage (Image)
```

## 2. 詳細な構築手順

### 2.1 基本パネルの設定

1. 新しいGameObjectを作成し、名前を「CookingPanel」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Stretch-Stretch
   - Left/Right/Top/Bottom: 0
3. Image コンポーネントを追加
   - Color: 軽い半透明ベージュ (#FFF5E6 with Alpha = 0.9)
   - Image Type: Sliced
   - 丸みを帯びた角のあるスプライトを使用
4. CookingActionUI スクリプトをアタッチ

### 2.2 タイトルバーの設定

1. CookingPanel の下に空のGameObjectを作成し、名前を「TitleBar」に設定
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
   - Text: 「りょうりをしよう！」
   - Font Size: 48
   - Font Style: Bold
   - Color: プライマリーカラー (#FF9A3C)
   - Alignment: Center

5. TitleBar の下に TextMeshProUGUI を作成し、名前を「StatusText」に設定
   - Text: 「よういをしよう！」
   - Font Size: 36
   - Font Style: Normal
   - Color: セカンダリーカラー (#4DB6E5)
   - Alignment: Center

### 2.3 スープ表示部分の設定

1. CookingPanel の下に空のGameObjectを作成し、名前を「SoupContainer」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Middle-Center
   - Size: 400 x 400
   - Pivot: 0.5, 0.5
   - Position: Y=50

3. SoupContainer の下に Image を作成し、名前を「PotImage」に設定
   - Source Image: 鍋の画像（円形または楕円形）
   - Preserve Aspect: true
   - RectTransform: 幅 = 400, 高さ = 250

4. SoupContainer の下に Image を作成し、名前を「SoupImage」に設定
   - Sprite: 液体のイメージ(円形、楕円形)
   - Color: #FF9A3C (デフォルト色、後で動的に変更)
   - RectTransform: 
     - 位置: PotImageの中央に配置
     - サイズ: PotImageより少し小さく設定

5. SoupContainer の下に ParticleSystem を追加し、名前を「SteamParticle」に設定
   - シンプルな蒸気パーティクル設定
   - Position: SoupImage の上部

### 2.4 進行状態表示部分の設定

1. CookingPanel の下に空のGameObjectを作成し、名前を「ProgressContainer」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Bottom-Stretch
   - Height: 100
   - Bottom: 100
   - Left/Right: 50

3. ProgressContainer の下に Slider を作成し、名前を「ProgressBar」に設定
   - Direction: Left to Right
   - Min Value: 0, Max Value: 1
   - Value: 0 (初期値)
   - Transition: Color Tint
   - Colors: Normal=白, Highlighted=薄いグレー
   - Fill Rect: 虹色のグラデーションイメージ
   - デザイン: 丸みを帯びた角、カラフルなフィル
   - RectTransform: Height = 40

4. ProgressContainer の下に TextMeshProUGUI を作成し、名前を「ProgressText」に設定
   - Text: "0%"
   - Font Size: 24
   - Alignment: Center
   - RectTransform: ProgressBarの右側

5. ProgressContainer の下に空のGameObjectを作成し、名前を「CookingStateIcons」に設定
   - RectTransformコンポーネント: ProgressBarの上側に配置
   - Horizontal Layout Group コンポーネント
     - Child Alignment: Middle Center
     - Spacing: 20
     - Control Child Size: Width, Height = true
     - Child Force Expand: Width, Height = false
     - Child Width, Height = 40

6. CookingStateIcons の下に各状態アイコンを作成（アイコンごとに Image コンポーネント）
   - PreparingStateIcon: 材料準備アイコン
   - CookingStateIcon: 調理中アイコン
   - SimmeringStateIcon: 煮込み中アイコン
   - CompletedStateIcon: 完成アイコン
   - 各アイコンには状態に応じた画像を設定

### 2.5 アクションボタン部分の設定

1. CookingPanel の下に空のGameObjectを作成し、名前を「ActionButtonsContainer」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Right-Middle
   - Size: 200 x 500
   - Right: 50
   - Position: Y=0

3. Vertical Layout Group コンポーネントを追加
   - Child Alignment: Middle Center
   - Spacing: 30
   - Padding: 20
   - Control Child Size: Width, Height = true
   - Child Force Expand: Width, Height = false

4. ActionButtonsContainer の下に Button を作成し、名前を「StirButton」に設定
   - Transition: Color Tint
   - Colors: Normal=#7ED957, Highlighted=明るい緑, Pressed=暗い緑
   - Image Type: Sliced（丸みを帯びた角）
   - Text: 「かきまぜる」（TextMeshPro使用）
   - Font Size: 24
   - Button Size: 180 x 80
   - 「かき混ぜる」アイコン画像を追加

5. 同様に「AddIngredientButton」と「FinishCookingButton」を作成
   - AddIngredientButton: 
     - Text: 「しょくざいを いれる」
     - Color: セカンダリーカラー (#4DB6E5)
     - Size: 180 x 80
     - 「食材追加」アイコン画像
   - FinishCookingButton: 
     - Text: 「できあがり！」
     - Color: プライマリーカラー (#FF9A3C)
     - Size: 180 x 80
     - 「完成」アイコン画像

### 2.6 温度調整部分の設定

1. CookingPanel の下に空のGameObjectを作成し、名前を「TemperatureControlContainer」に設定
2. RectTransformコンポーネントを設定
   - Anchors: Left-Bottom
   - Size: 250 x 200
   - Left: 50
   - Bottom: 150

3. TemperatureControlContainer の下に TextMeshProUGUI を作成し、名前を「Label」に設定
   - Text: 「あたたかさ」
   - Font Size: 24
   - Alignment: Center
   - RectTransform: トップに配置

4. TemperatureControlContainer の下に Slider を作成し、名前を「TemperatureSlider」に設定
   - Direction: Left to Right
   - Min Value: 0, Max Value: 1
   - Value: 0.5 (初期値)
   - Handle Size: 40 x 40（大きめ）
   - 垂直方向に配置
   - Fill Rect: 温度に応じたグラデーション（青から赤）
   - デザイン: 丸みを帯びた角、温度を表す色合い

5. TemperatureControlContainer の下に Image を作成し、名前を「FlameImage」に設定
   - Sprite: 炎のイメージ
   - Preserve Aspect: true
   - RectTransform: スライダーの近くに配置
   - 色は温度スライダーの値に応じて変化

## 3. コンポーネント参照の設定

CookingActionUI スクリプトの各参照フィールドに、作成した UI 要素をドラッグ＆ドロップして設定します：

1. **調理アクションUI要素**
   - Stir Button: StirButton
   - Add Ingredient Button: AddIngredientButton
   - Finish Cooking Button: FinishCookingButton

2. **温度調整**
   - Temperature Slider: TemperatureSlider
   - Flame Image: FlameImage
   - Flame Color Gradient: 青(#4DB6E5)から赤(#FF5252)へのグラデーションを設定

3. **調理プログレス**
   - Cooking Progress Slider: ProgressBar
   - Cooking Status Text: StatusText
   - Soup Color Image: SoupImage

4. **調理状態視覚化**
   - Preparing State Icon: PreparingStateIcon
   - Cooking State Icon: CookingStateIcon
   - Simmering State Icon: SimmeringStateIcon
   - Completed State Icon: CompletedStateIcon

5. **アニメーション**
   - Cooking Pot Animator: 調理シーンの鍋アニメーター参照

## 4. アニメーションとトランジション設定

### 4.1 ボタンのトランジション設定

1. StirButton, AddIngredientButton, FinishCookingButtonそれぞれに対して：
   - Transition: Color Tint
   - Normal Color: 各ボタンの基本色
   - Highlighted Color: 基本色より20%明るく
   - Pressed Color: 基本色より20%暗く
   - Disabled Color: グレー (#BDBDBD)

### 4.2 ボタンクリック時のアニメーション

`ButtonPressAnimation` コルーチンによる軽い縮小-拡大アニメーションが実装済み。

### 4.3 状態変更時のアイコン切り替えエフェクト

1. 各状態アイコン（PreparingStateIcon, CookingStateIcon, SimmeringStateIcon, CompletedStateIcon）に対して：
   - 非アクティブ時は暗いグレースケール
   - アクティブ時は明るいカラー表示
   - アクティブ化時にスケールアップアニメーション

## 5. アクセシビリティ対応

1. ボタンサイズは十分に大きく設定（最小80x80ピクセル）
2. テキストフォントサイズは読みやすいサイズ（最小24pt）
3. 色のコントラストは十分に高く設定
4. 状態の視覚的な表現と同時にテキストによる表示も実施

## 6. 完成と確認

1. 前述の設定をすべて完了後、CookingPanelをPrefabsフォルダにドラッグ＆ドロップしてプレハブとして保存
2. 名前を「CookingActionUI」に設定
3. 実行時の動作を確認し、必要に応じて調整

このプレハブは、調理シーンでGameplayUIControllerから参照され、調理状態に応じて表示されます。
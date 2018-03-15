### バージョン

- Unity:  
2017.2.1p2
- PUN（Photon Unity Network）  
1.88

### 使い方

1. 新しくプロジェクトを作って下さい
2. Asset Storeから**Photon Unity Network Free**をダウンロードし、インポートして下さい
3. *Asset*フォルダの中に*Tutorial*フォルダを作って下さい
3. 本レポジトリをダウンロードして下さい
5. ダウンロードしたものを*Tutorial*フォルダに移動させて下さい
2. **Build Settings**の**Scene List**に*Tutorial/Scenes*フォルダに入っているシーンを追加して下さい
3. Gitクライアントに*Tutorial*フォルダを**Add**して下さい
8. あとは自由にコミットを切り替えることができます

### 補足

#### Rect Transform Anchor Presets の変え方

![Anchorの変え方](Others/Anchorの変え方.png)

#### PUNのバージョン

なお、PUNのバージョンは以下のようにして確かめることができます：

```csharp
Debug.Log(PhotonNetwork.versionPUN);
```
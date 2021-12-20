# UnityEditorAssetChoice

Unity で Package を Export 時に表示されるような アセット選択モーダルウィンドウを表示するツール

<img src="https://github.com/yayorozu/ImageUploader/blob/master/AssetChoice/Top.png" width="400">

基本的には指定したパスを表示しますが、オプションでディレクトリだった場合に更にアセットを表示することも可能です

# サンプル

```cs
    /// <summary>
    /// 選択したアセットを選択肢として表示
    /// </summary>
    [MenuItem("Tools/Sample/Asset Choice")]
    private static void AssetChoice()
    {
        AssetChoiceModal.ShowModalFromGUID(Selection.assetGUIDs, ShowSelectAssets);
    }

    /// <summary>
    /// 選択したアセット以下ディレクトリの中身を選択肢として表示
    /// </summary>
    [MenuItem("Tools/Sample/Asset Choice TopDirectory")]
    private static void AssetChoiceTopDirectory()
    {
        AssetChoiceModal.ShowModalFromGUID(Selection.assetGUIDs, ShowSelectAssets, Option.TopDirectoryOnly);
    }
        
    /// <summary>
    /// 選択したアセット以下の全部を選択肢として表示
    /// </summary>
    [MenuItem("Tools/Sample/Asset Choice AllDirectories")]
    private static void AssetChoiceAllDirectories()
    {
        AssetChoiceModal.ShowModalFromGUID(Selection.assetGUIDs, ShowSelectAssets, Option.AllDirectories);
    }
    
    private static void ShowSelectAssets(string[] paths)
    {
        var builder = new StringBuilder(paths.Length);
        foreach (var path in paths)
        {
            builder.AppendLine(path);
        }
        Debug.Log("[Choice Assets]\n" + builder);
    }
```

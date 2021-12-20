using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Yorozu.EditorTool.AssetChoice
{
    public delegate void ChooseAssets(string[] guids);
    
    /// <summary>
    /// パスにディレクトリがあった場合に更に探索するかどうかの設定
    /// </summary>
    public enum Option
    {
        None,
        /// Includes only the current directory
        TopDirectoryOnly,
        /// <summary><para>Includes the current directory and all its subdirectories in a search operation. This option includes reparse points such as mounted drives and symbolic links in the search.</para></summary>
        AllDirectories,
    }
    
    public class AssetChoiceModal : EditorWindow 
    {
        private static void Show(IList<string> paths, ChooseAssets chooseAction, Option option)
        {
            if (paths.Count <= 0)
            {
                Debug.LogWarning("Paths is Empty.");
                return;
            }
            
            var window = CreateInstance(typeof(AssetChoiceModal)) as AssetChoiceModal;
            window.titleContent = new GUIContent("Asset Choice");
            window.Setup(paths, chooseAction, option);
            window.ShowUtility();
        }

        /// <summary>
        /// モーダルウィンドウを表示
        /// </summary>
        /// <param name="guids">表示するGUID一覧</param>
        /// <param name="chooseAction">選択したパス一覧 ※ディレクトリは除く</param>
        /// <param name="option">ディレクトリの場合の挙動</param>
        public static void ShowModalFromGUID(IList<string> guids, ChooseAssets chooseAction, Option option = Option.None)
        {
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            ShowModal(paths, chooseAction, option);
        }

        /// <summary>
        /// モーダルウィンドウを表示
        /// </summary>
        /// <param name="paths">表示するパス一覧</param>
        /// <param name="chooseAction">選択したパス一覧 ※ディレクトリは除く</param>
        /// <param name="option">ディレクトリの場合の挙動</param>
        public static void ShowModal(IList<string> paths, ChooseAssets chooseAction, Option option = Option.None)
        {
            Show(paths, chooseAction, option);
        }

        private AssetChoiceTreeState _state;
        private AssetChoiceTreeView _treeView;
        
        private void Setup(IList<string> paths, ChooseAssets chooseAction, Option option)
        {
            _state = new AssetChoiceTreeState(paths, chooseAction, option);
            _treeView = new AssetChoiceTreeView(_state);
            minSize = new Vector2(300, 470f);
        }

        private void OnGUI()
        {
            // モーダルウィンドウなので、例外は無いと思われるがヌルになれば閉じる
            if (_state == null || _treeView == null) 
                Close();
            
            var rect = GUILayoutUtility.GetRect(0, 0, position.width, position.height);
            _treeView.OnGUI(rect);

            // 選択終了
            if (GUILayout.Button("Choice"))
            {
                _state.Close();
                Close();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Yorozu.EditorTool.AssetChoice
{
    [Serializable]
    internal class AssetChoiceTreeState : TreeViewState
    {
        private List<string> _paths;
        private HashSet<string> _deletePaths = new HashSet<string>();
        private ChooseAssets _action;

        internal AssetChoiceTreeState(IList<string> paths, ChooseAssets chooseAction, Option option)
        {
            _action = chooseAction;
            _paths = paths.ToList();
            switch (option)
            {
                case Option.None:    
                    break;
                case Option.TopDirectoryOnly:
                    foreach (var path in paths) 
                        AddFolderAssets(path, false);
                    break;
                case Option.AllDirectories:
                    foreach (var path in paths) 
                        AddFolderAssets(path, true);
                    break;
            }
        }

        private void AddFolderAssets(string path, bool isRecursive)
        {
            if (!AssetDatabase.IsValidFolder(path))
                return;

            var option = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(path, "*", option)
                .Where(p => !p.EndsWith(".meta"));
            
            var list = files.Select((p, index) => new {path = p, index});
            foreach (var pair in list)
            {

                if (_paths.Contains(pair.path))
                    continue;

                _paths.Add(pair.path);
            }
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        internal void Close()
        {
            // フォルダは除く
            var deletePaths = _deletePaths
                .Where(p => !AssetDatabase.IsValidFolder(p))
                .ToArray();
            
            _action?.Invoke(deletePaths);
        }
        
        /// <summary>
        /// 依存が0のファイル一覧を取得
        /// </summary>
        internal TreeViewItem GetTreeRoot()
        {
            var root = new TreeViewItem(0, -1, "");

            if (_paths == null) 
                return root;
            
            var builder = new StringBuilder();
            
            var count = _paths.Count;
            EditorUtility.DisplayProgressBar("Process", $"0/{count}", 0f);
            try
            {
                for (var index = 0; index < _paths.Count; index++)
                {
                    var path = _paths[index];
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    if (obj == null)
                        continue;
                    
                    // フォルダのみの追加は許容しない
                    if (AssetDatabase.IsValidFolder(path))
                        continue;

                    EditorUtility.DisplayProgressBar("Process", $"{index}/{count}", index / (float) count);

                    builder.Clear();
                    // 分割
                    var divide = path.Split('/');

                    var parent = root;
                    for (var i = 0; i < divide.Length - 1; i++)
                    {
                        if (builder.Length > 0)
                            builder.Append("/");

                        builder.Append(divide[i]);
                        parent = GetNext(parent, builder.ToString(), divide[i]);
                    }

                    var id = obj.GetInstanceID();
                    if (parent.children != null && parent.children.Any(i => i.id == id))
                        continue;

                    var fileName = divide.Last();
                    var item = new AssetChoiceTreeViewItem(id)
                    {
                        displayName = fileName,
                        icon = AssetDatabase.GetCachedIcon(path) as Texture2D,
                        Path = path
                    };
                    parent.AddChild(item);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return root;
        }

        /// <summary>
        /// 次の要素を探す
        /// </summary>
        private static TreeViewItem GetNext(TreeViewItem current, string path, string name)
        {
            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            var id = obj.GetInstanceID();
            if (current.hasChildren)
            {
                var index = current.children.FindIndex(c => c.id == id);
                if (index >= 0)
                    return current.children[index];
            }

            // 見つからなかった
            var child = new AssetChoiceTreeViewItem(id)
            {
                depth = current.depth + 1,
                displayName = name,
                Path = path,
                icon = AssetDatabase.GetCachedIcon(path) as Texture2D
            };
            current.AddChild(child);
            return child;
        }
        
        internal bool Contains(string path)
        {
            return _deletePaths.Contains(path);
        }

        /// <summary>
        /// 削除と削除しないを変更
        /// </summary>
        internal void Change(AssetChoiceTreeViewItem item, bool isAdd)
        {
            Recursive(item, isAdd);
        }
        
        /// <summary>
        /// 追加
        /// </summary>
        private void Add(AssetChoiceTreeViewItem item)
        {
            if (_deletePaths.Contains(item.Path)) 
                return;
            
            _deletePaths.Add(item.Path);
        }

        /// <summary>
        /// 削除
        /// </summary>
        private void Remove(AssetChoiceTreeViewItem item)
        {
            if (!_deletePaths.Contains(item.Path)) 
                return;
            
            _deletePaths.Remove(item.Path);
        }

        /// <summary>
        /// 再帰的に検索
        /// </summary>
        private void Recursive(AssetChoiceTreeViewItem root, bool isAdd)
        {
            if (isAdd)
                Add(root);
            else
                Remove(root);
            
            if (!root.hasChildren)
                return;

            foreach (var child in root.children)
            {
                var item = child as AssetChoiceTreeViewItem;
                Recursive(item, isAdd);
            }
        }
    }
}
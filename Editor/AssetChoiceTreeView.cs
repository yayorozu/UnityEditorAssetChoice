using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Yorozu.EditorTool.AssetChoice
{
    internal class AssetChoiceTreeView : TreeView
    {
        private AssetChoiceTreeState _state => state as AssetChoiceTreeState; 
        
        public AssetChoiceTreeView(TreeViewState state) : base(state)
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            baseIndent += 15f;
            Reload();
            
            // 初期状態でExpandしたいのでここで
            ExpandAll();
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = _state.GetTreeRoot();

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
        
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            // 空対応
            if (!root.hasChildren) 
                root.AddChild(new TreeViewItem(1, 0));
            
            return base.BuildRows(root);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);
            var rect = args.rowRect;
            rect.width = 15f;
            
            var item = args.item as AssetChoiceTreeViewItem;
            if (item == null)
                return;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var isDelete = _state.Contains(item.Path);
                isDelete = EditorGUI.Toggle(rect, isDelete);
                if (check.changed)
                {
                    _state.Change(item, isDelete);
                }
            }
        }
    }

}
using UnityEditor.IMGUI.Controls;

namespace Yorozu.EditorTool.AssetChoice
{
    internal class AssetChoiceTreeViewItem : TreeViewItem
    {
        internal string Path;

        internal AssetChoiceTreeViewItem(int id) : base(id)
        {
        }
    }
}
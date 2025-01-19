using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Gameplay.Controllers;

[CustomEditor(typeof(ReelController))]
public class ReelControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        ReelController controller = (ReelController)target;
        if (controller == null) return;
        if (controller.ReelDataCollection == null) return;
        if (controller.ReelItemsContentHolder == null) return;
        int childCount = controller.ReelItemsContentHolder.transform.childCount;
        if (controller.ReelDataCollection.ItemsCollectionLength < childCount - 1) return;


        var reelData = controller.ReelDataCollection;
        var holder = controller.ReelItemsContentHolder.transform;

        for (int i = 0; i < childCount; i++)
        {
            var child = holder.GetChild(i);
            var img = child.GetComponent<Image>();
            if (img != null && img.sprite == null)
            {

                Sprite newSprite = reelData.GetItemVisual(i);
                img.sprite = newSprite;
                img.SetNativeSize();
            }
        }


    }
}
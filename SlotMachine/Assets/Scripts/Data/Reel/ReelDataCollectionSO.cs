using UnityEngine;
[CreateAssetMenu(fileName = "ReelDataCollection", menuName = "Data/Reel/ReelDataCollection")]
public class ReelDataCollectionSO : ScriptableObject
{
    [SerializeField] private Sprite[] _reelItemsSprite;
    public int ItemsCollectionLength => _reelItemsSprite.Length;
    public Sprite GetItemVisual(int index)
    {
        return _reelItemsSprite[index];
    }

}

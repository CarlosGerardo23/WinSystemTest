using UnityEngine;
namespace Data.ReelData
{
    /// <summary>
    /// Represents a reel item data for a slot machine.
    /// </summary>
    [CreateAssetMenu(fileName = "ReelItemDataSO", menuName = "Scriptable Objects/ReelItemData")]
    public class ReelItemDataSO : ScriptableObject
    {
        [SerializeField] private Sprite _reelItemSprite;
        [SerializeField] private ReelItem _reelItem;
        [SerializeField] private int _baseItemScore;
        public Sprite ReelItemSprite => _reelItemSprite;
        public ReelItem ReelItem => _reelItem;
        public int BaseItemScore => _baseItemScore;
    }
}

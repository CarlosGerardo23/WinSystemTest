using UnityEngine;
namespace Data.ReelData
{
    /// <summary>
    /// Represents a collection of reel data for a slot machine.
    /// </summary>
    [CreateAssetMenu(fileName = "ReelDataCollection", menuName = "Scriptable Objects/ReelDataCollection")]
    public class ReelDataCollectionSO : ScriptableObject
    {
        [Tooltip("The sprites representing the reel items.")]
        [SerializeField] private ReelItemDataSO[] _reelItemsData;

        /// <summary>
        /// Gets the length of the items collection.
        /// </summary>
        public int ItemsCollectionLength => _reelItemsData.Length;

        /// <summary>
        /// Gets the visual representation of an item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The sprite representing the visual item.</returns>
        public ReelItemDataSO GetItemData(int index)
        {
            return _reelItemsData[index];
        }
        /// <summary>
        /// Retrieves the score of a specified reel item.
        /// </summary>
        /// <param name="reelItem">The reel item for which the score is to be retrieved.</param>
        /// <returns>The base score of the specified reel item if found; otherwise, 0.</returns>
        public int GetItemScore(ReelItem reelItem)
        {
            foreach (var item in _reelItemsData)
            {
                if (reelItem == item.ReelItem)
                    return item.BaseItemScore;
            }
            return 0;
        }
    }
    public enum ReelItem
    {
        Bell,
        Cherry,
        Grape,
        Lemon,
        Orange,
        Plum,
        Watermelon
    }

}


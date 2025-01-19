using UnityEngine;
namespace Data.Combination
{
    /// <summary>
    /// Represents a column in the combination matrix.
    /// </summary>
    [System.Serializable]
    public class CombinationColumnData
    {
        public bool[] CombinationValue = new bool[3];
    }
}
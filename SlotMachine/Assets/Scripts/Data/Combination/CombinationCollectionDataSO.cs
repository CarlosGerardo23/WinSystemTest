using Data.ReelData;
using UnityEngine;
using Controllers;
using System.Collections.Generic;
using System;

namespace Data.Combination
{
    /// <summary>
    /// Represents a collection of combination data for a slot machine game.
    /// </summary>
    [CreateAssetMenu(fileName = "CombinationCollectionDataSO", menuName = "Scriptable Objects/CombinationCollectionDataSO")]
    public class CombinationCollectionDataSO : ScriptableObject
    {
        /// <summary>
        /// Array top of combination data.
        /// </summary>
        [SerializeField] private CombinationDataSO[] _topCombinations;
        /// <summary>
        /// Array middle of combination data.
        /// </summary>
        [SerializeField] private CombinationDataSO[] _middleCombinations;
        /// <summary>
        /// Array bottom of combination data.
        /// </summary>
        [SerializeField] private CombinationDataSO[] _bottomCombinations;

        /// <summary>
        /// Checks the provided reel items for any winning combinations of the specified type.
        /// </summary>
        /// <param name="combinationType">The type of combination to check for.</param>
        /// <param name="reelItems">A 2D array of reel item controllers representing the current state of the reels.</param>
        /// <param name="winningLevel">An output parameter that will contain the level of the winning combination, if any.</param>
        /// <param name="winningItems">An output parameter that will contain the list of winning reel item controllers, if any.</param>
        /// <returns>True if a winning combination is found; otherwise, false.</returns>
        public bool CheckCombinations(CombinationType combinationType, ReelItemController[][] reelItems, out int winningLevel, out List<ReelItemController> winningItems)
        {
            CombinationDataSO[] combinations = GetCombinationData(combinationType);
            winningLevel = 0;
            winningItems = new List<ReelItemController>();
            
            foreach (var combination in combinations)
            {
                if (CheckCombination(combination, reelItems, out int newWinningLevel, out var tempWinningItems))
                {
                    if (newWinningLevel > winningLevel)
                    {
                        winningLevel = newWinningLevel;
                        winningItems= new List<ReelItemController>(tempWinningItems);
                        Debug.Log("Winning combination found!");
                    }
                }
            }

            return winningLevel > 0;
        }
        /// <summary>
        /// Retrieves the combination data based on the specified combination type.
        /// </summary>
        /// <param name="combinationType">The type of combination to retrieve.</param>
        /// <returns>An array of <see cref="CombinationDataSO"/> corresponding to the specified combination type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="combinationType"/> is not recognized.
        /// </exception>

        private CombinationDataSO[] GetCombinationData(CombinationType combinationType)
        {
            switch (combinationType)
            {
                case CombinationType.Top:
                    return _topCombinations;
                case CombinationType.Middle:
                    return _middleCombinations;
                case CombinationType.Bottom:
                    return _bottomCombinations;
                default:
                    throw new ArgumentOutOfRangeException(nameof(combinationType), combinationType, null);
            }
        }

        /// <summary>
        /// Checks if the given combination matches the reel items and determines the winning level and items.
        /// </summary>
        /// <param name="combination">The combination data to check against the reel items.</param>
        /// <param name="reelItems">A 2D array of reel item controllers representing the current state of the reels.</param>
        /// <param name="winningLevel">An output parameter that will contain the winning level if the combination matches.</param>
        /// <param name="winningItems">An output parameter that will contain the list of winning reel item controllers if the combination matches.</param>
        /// <returns>True if the combination matches the reel items; otherwise, false.</returns>

        private bool CheckCombination(CombinationDataSO combination, ReelItemController[][] reelItems, out int winningLevel, out List<ReelItemController> winningItems)
        {
            Vector2[] winningPositions = combination.GetWinningPositions();
            ReelItem winningItem = reelItems[(int)winningPositions[0].x][(int)winningPositions[0].y].ReelItem;
            winningLevel = 0;
            winningItems = new List<ReelItemController>();
            for (int i = 0; i < winningPositions.Length; i++)
            {
                int col = (int)winningPositions[i].x;
                int row = (int)winningPositions[i].y;
                if (reelItems[col][row].ReelItem != winningItem)
                    return winningItems.Count >= 2;
                winningLevel++;
                winningItems.Add(reelItems[col][row]);
            }
            return winningItems.Count >= 2;
        }
    }
    /// <summary>
    /// Represents the type of combination in a slot machine game.
    /// </summary>
    public enum CombinationType
    {
        Top,
        Middle,
        Bottom
    }

}
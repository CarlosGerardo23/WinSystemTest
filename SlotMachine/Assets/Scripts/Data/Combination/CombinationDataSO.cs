using UnityEngine;
namespace Data.Combination
{
    /// <summary>
    /// ScriptableObject that holds a combination matrix for a slot machine.
    /// </summary>
    [CreateAssetMenu(fileName = "CombinationDataSO", menuName = "Scriptable Objects/CombinationData")]
    public class CombinationDataSO : ScriptableObject
    {

        /// <summary>
        /// An array of <see cref="CombinationColumnData"/> representing the combination data for the slot machine.
        /// This array is serialized but hidden in the inspector.
        /// </summary>
        [SerializeField, HideInInspector] private CombinationColumnData[] _combination = new CombinationColumnData[5];

        /// <summary>
        /// Gets the number of rows in the combination matrix.
        /// </summary>
        public int Rows => _combination[0].CombinationValue.Length;

        /// <summary>
        /// Gets the number of columns in the combination matrix.
        /// </summary>
        public int Columns => _combination.Length;

        /// <summary>
        /// Sets the value of a specific position in the combination matrix.
        /// </summary>
        /// <param name="columnIndex">The row index of the matrix.</param>
        /// <param name="rowIndex">The column index of the matrix.</param>
        /// <param name="value">The value to set at the specified position.</param>
        public void SetCombination(int rowIndex, int columnIndex, bool value)
        {
            _combination[columnIndex].CombinationValue[rowIndex] = value;
        }

        /// <summary>
        /// Gets the value of a specific position in the combination matrix.
        /// </summary>
        /// <param name="columnIndex">The row index of the matrix.</param>
        /// <param name="rowIndex">The column index of the matrix.</param>
        /// <returns>The value at the specified position.</returns>
        public bool GetCombination(int rowIndex, int columnIndex)
        {
            return _combination[columnIndex].CombinationValue[rowIndex];
        }
        /// <summary>
        /// Gets the positions of all winning combinations in the matrix.
        /// </summary>
        /// <returns>An array of Vector2 representing the positions of all winning combinations.</returns>
        public Vector2[] GetWinningPositions()
        {
            Vector2[] winningPositions = new Vector2[5];
            int index = 0;
            for (int i = 0; i < _combination.Length; i++)
            {
                for (int j = 0; j < _combination[i].CombinationValue.Length; j++)
                {
                    if (_combination[i].CombinationValue[j])
                    {
                        winningPositions[index] = new Vector2(i, j);
                        index++;
                    }
                }
            }
            return winningPositions;
        }
    }

}


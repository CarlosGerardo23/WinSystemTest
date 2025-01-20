using System.Collections;
using System.Collections.Generic;
using Controllers;
using Data.Combination;
using Data.ReelData;
using Gameplay.Controllers;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the slot machine's behavior, including spinning the reels, checking combinations, and updating the score.
/// </summary>
public class SlotMachineController : MonoBehaviour
{

    [Tooltip(" Collection of possible winning combinations.")]
    [SerializeField] private CombinationCollectionDataSO _combinationCollection;
    [Tooltip(" Reference data collection for reel items.")]
    [SerializeField] private ReelDataCollectionSO _referenceDataCollection;
    [Tooltip("Array of reel controllers managing individual reels.")]
    [SerializeField] private ReelController[] _reelControllers;
    [Tooltip("Delay between interactions with reels.")]
    [SerializeField] private float _reelInteractionDelay = 0.5f;
    [Tooltip(" UI element displaying the current score.")]
    [SerializeField] private TMPro.TextMeshProUGUI _scoreText;
    [Tooltip(" Button to start spinning the slot machine.")]
    [SerializeField] private Button _spinButton;

    /// <summary>
    /// Total score accumulated by the player.
    /// </summary>
    private float _totalScore = 0;

    /// <summary>
    /// Initializes the slot machine controller and sets the initial score text.
    /// </summary>
    private void Start()
    {
        _scoreText.text = $"Score: {_totalScore}";
    }

    /// <summary>
    /// Starts the slot machine by disabling the spin button and initiating the spinning process.
    /// </summary>
    public void StartMachine()
    {
        _spinButton.interactable = false;
        StartCoroutine(StartSpinning());
    }

    /// <summary>
    /// Coroutine that handles the spinning process of the slot machine.
    /// </summary>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    public IEnumerator StartSpinning()
    {
        float spinDuration = Random.Range(2f, 4f);

        yield return SetReelState(ReelState.Spinning);
        yield return new WaitForSeconds(spinDuration);
        yield return SetReelState(ReelState.Stopped, ReelState.Finished);

        yield return CheckCombinations();
    }

    /// <summary>
    /// Coroutine that checks for winning combinations and updates the score accordingly.
    /// </summary>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    private IEnumerator CheckCombinations()
    {
        ReelItemController[][] reelItems = new ReelItemController[5][];
        for (int i = 0; i < _reelControllers.Length; i++)
        {
            reelItems[i] = new ReelItemController[3];

            int rowIndex = _reelControllers[i].ChildReelItems.Length - 1;
            ReelItemController[] reelItemControllers = _reelControllers[i].CurrentOrderChildItems;
            for (int j = 1; j < rowIndex; j++)
                reelItems[i][j - 1] = reelItemControllers[j];
        }
        int totalScore = 0;
        for (int i = 0; i < 3; i++)
        {
            if (_combinationCollection.CheckCombinations((CombinationType)i, reelItems, out int newWinningLevel, out List<ReelItemController> winningItems))
            {
                Debug.Log($"New winning level: {newWinningLevel} in line {(CombinationType)i}");
                ReelItem winningReelItem = winningItems[0].ReelItem;
                if (winningReelItem == ReelItem.Lemon && newWinningLevel - 1 == 1)
                    totalScore += 2;
                else
                    totalScore += (newWinningLevel - 1) * _referenceDataCollection.GetItemScore(winningItems[0].ReelItem);
                foreach (var item in winningItems)
                {
                    item.ShowCombinationSuccess(true);
                }
            }
            yield return new WaitForSeconds(1f);
        }
        _totalScore += totalScore;
        Debug.Log($"Total score: {totalScore}");
        _scoreText.text = $"Score: {_totalScore}";
        yield return new WaitForSeconds(0.5f);
        _spinButton.interactable = true;
    }

    /// <summary>
    /// Stops the spinning of the slot machine reels.
    /// </summary>
    [ContextMenu("Stop Spinning")]
    public void StopSpinning()
    {
        StartCoroutine(SetReelState(ReelState.Stopped, ReelState.Finished));
    }

    /// <summary>
    /// Coroutine that sets the state of each reel with a delay between each interaction.
    /// </summary>
    /// <param name="state">The state to set for each reel.</param>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    private IEnumerator SetReelState(ReelState state)
    {
        foreach (var reelController in _reelControllers)
        {
            reelController.SetReelState(state);
            yield return new WaitForSeconds(_reelInteractionDelay);
        }
    }

    /// <summary>
    /// Coroutine that sets the state of each reel and waits until the reel reaches a specified condition state.
    /// </summary>
    /// <param name="state">The state to set for each reel.</param>
    /// <param name="conditionState">The state to wait for each reel to reach.</param>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    private IEnumerator SetReelState(ReelState state, ReelState conditionState)
    {
        foreach (var reelController in _reelControllers)
        {
            reelController.SetReelState(state);

            while (reelController.CurrentReelState != conditionState)
                yield return null;
            yield return new WaitForSeconds(_reelInteractionDelay);
        }
    }
}

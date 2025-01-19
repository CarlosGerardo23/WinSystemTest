using System.Collections;
using System.Collections.Generic;
using Controllers;
using Data.Combination;
using Data.ReelData;
using Gameplay.Controllers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineController : MonoBehaviour
{
    [SerializeField] private CombinationCollectionDataSO _combinationCollection;
    [SerializeField] private ReelDataCollectionSO _referenceDataCollection;
    [SerializeField] private ReelController[] _reelControllers;
    [SerializeField] private float _reelInteractionDelay = 0.5f;
    [SerializeField] private TMPro.TextMeshProUGUI _scoreText;
    [SerializeField] private Button _spinButton;
    private float _totalScore = 0;
    private void Start()
    {
        _scoreText.text = $"Score: {_totalScore}";
    }
    [ContextMenu("Start Spinning")]
    public void StartMachine()
    {
        _spinButton.interactable = false;
        StartCoroutine(StartSpinning());
    }
    [ContextMenu("Start Spinning Infinite")]
    public void StartSpinningTestInfinite()
    {
        StartCoroutine(SetReelSatte(ReelState.Spinning));
    }
    public IEnumerator StartSpinning()
    {
        float spinDuration = Random.Range(2f, 4f);

        yield return SetReelSatte(ReelState.Spinning);
        yield return new WaitForSeconds(spinDuration);
        yield return SetReelState(ReelState.Stopped, ReelState.Finished);

        yield return CheckCombinations();
    }

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

    [ContextMenu("Stop Spinning")]
    public void StopSpinning()
    {
        StartCoroutine(SetReelState(ReelState.Stopped, ReelState.Finished));
    }
    private IEnumerator SetReelSatte(ReelState state)
    {
        foreach (var reelController in _reelControllers)
        {
            reelController.SetReelState(state);
            yield return new WaitForSeconds(_reelInteractionDelay);
        }
    }
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

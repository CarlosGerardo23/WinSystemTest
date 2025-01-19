using System.Collections;
using Gameplay.Controllers;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    [SerializeField] private ReelController[] _reelControllers;
    [SerializeField] private float _reelInteractionDelay = 0.5f;
    [ContextMenu("Start Spinning")]
    public void StartSpinningTest()
    {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameplay.Movement.Reel;
namespace Gameplay.Controllers
{

    /// <summary>
    /// Controls the behavior of the reel in the slot machine game.
    /// </summary>
    public class ReelController : MonoBehaviour
    {
        [Tooltip("The object holder that have the reel items content")]
        [SerializeField] private GameObject _reelItemsContentHolder;
        [Tooltip("The maximum speed at which the reel can spin.")]
        [SerializeField] private float _maxSpeed = 0.5f;

        [Tooltip("The duration it takes for the reel to stop .")]
        [SerializeField] private float _reelStopDelay = 0.2f;
          [Tooltip("The duration it takes for the reel to recenter after stopping.")]
        [SerializeField] private float _reelRecenterDuration = 0.2f;

        private RectTransform[] _childReelItems;
        private bool _isSpinning;
        public ReelState CurrentReelState => _currentReelState;
        /// <summary>
        /// List of ordered RectTransform components representing the runtime reel items.
        /// </summary>
        private readonly List<RectTransform> _orderedRunTimeReelItems = new();

        /// <summary>
        /// The VerticalLayoutGroup component attached to the reel.
        /// </summary>
        private VerticalLayoutGroup _layoutGroup;

        /// <summary>
        /// The ReelMovement component that handles the movement of the reel.
        /// </summary>
        private ReelMovement _reelMovement;
        private ReelState _currentReelState;
        #region  Unity Methods
        private void Awake()
        {
            _layoutGroup = _reelItemsContentHolder.GetComponent<VerticalLayoutGroup>();
            CacheChildReelItems();
            PopulateReelItemsList();
            _reelMovement = new ReelMovement(_childReelItems, _maxSpeed,_reelStopDelay, _reelRecenterDuration);
            SetReelState(ReelState.Ready);
        }

        private void Update()
        {
            if (_isSpinning)
                HandleSpinning();
        }
        #endregion
        #region Initialization
        /// <summary>
        /// Caches the RectTransform components of the child reel items.
        /// </summary>
        private void CacheChildReelItems()
        {
            _childReelItems = new RectTransform[_reelItemsContentHolder.transform.childCount];
            for (int i = 0; i < _reelItemsContentHolder.transform.childCount; i++)
            {
                Transform child = _reelItemsContentHolder.transform.GetChild(i);
                if (child.TryGetComponent(out RectTransform rect))
                    _childReelItems[i] = rect;
            }
        }
        /// <summary>
        /// Populates the ordered list of reel items.
        /// </summary>
        private void PopulateReelItemsList()
        {
            foreach (var item in _childReelItems)
            {
                if (item != null)
                    _orderedRunTimeReelItems.Add(item);
            }
        }
        #endregion
        #region Reel State Methods
        /// <summary>
        /// Sets the state of the reel.
        /// </summary>
        /// <param name="newState">The new state to set.</param>
        public void SetReelState(ReelState newState)
        {
            _currentReelState = newState;
            OnReelStateChanged();
        }

        /// <summary>
        /// Called when the reel state changes to handle state-specific behavior.
        /// </summary>
        private void OnReelStateChanged()
        {
            switch (_currentReelState)
            {
                case ReelState.Ready:
                    _layoutGroup.enabled = false;
                    break;
                case ReelState.Spinning:
                    _reelMovement.StartSpinning();
                    _isSpinning = true;
                    break;
                case ReelState.Stopped:
                    StartCoroutine(StopSpinningSequence());
                    break;
                case ReelState.Finished:
                    break;
            }
        }
        #endregion
        #region Reel Movement Methods
        /// <summary>
        /// Handles the spinning behavior of the reel.
        /// </summary>
        private void HandleSpinning()
        {
            for (int i = 0; i < _childReelItems.Length; i++)
                _childReelItems[i].anchoredPosition += _reelMovement.SpinDelta;

            if (!_reelMovement.ShouldTransition(_orderedRunTimeReelItems[^2].anchoredPosition.y))
                return;

            RotateItems();
            AdjustFirstItemDistance();
        }


        /// <summary>
        /// Rotates the reel items to simulate spinning.
        /// </summary>
        private void RotateItems()
        {
            RectTransform lastItem = _orderedRunTimeReelItems[^1];
            _orderedRunTimeReelItems.RemoveAt(_orderedRunTimeReelItems.Count - 1);
            _orderedRunTimeReelItems.Insert(0, lastItem);
            lastItem.anchoredPosition = _reelMovement.FirstAnchorPosition;
            lastItem.SetAsFirstSibling();
        }

        /// <summary>
        /// Adjusts the distance of the first reel item after rotation.
        /// </summary>

        private void AdjustFirstItemDistance()
        {
            _orderedRunTimeReelItems[0].anchoredPosition =
                _reelMovement.CalculateFirstItemPosition(_orderedRunTimeReelItems[1].anchoredPosition);
        }

        /// <summary>
        /// Coroutine that handles the sequence of stopping the reel.
        /// </summary>
        private IEnumerator StopSpinningSequence()
        {
            Debug.Log("Reel start stopped spinning.");
            yield return _reelMovement.SpinAndStop();
            _isSpinning = false;
            Debug.Log("Reel stopped spinning.");
            yield return CenterReelAfterStop();
            Debug.Log("Reel centered.");
            _layoutGroup.enabled = true;
            SetReelState(ReelState.Finished);
        }

        /// <summary>
        /// Coroutine that centers the reel after it stops spinning.
        /// </summary>
        private IEnumerator CenterReelAfterStop()
        {
            yield return _reelMovement.CenterReelCoroutine(_orderedRunTimeReelItems);
            _layoutGroup.enabled = true;
        }
        #endregion
    }
    /// <summary>
    /// Enum representing the different states of the reel.
    /// </summary>
    public enum ReelState
    {
        Ready,
        Spinning,
        Stopped,
        Finished
    }
}
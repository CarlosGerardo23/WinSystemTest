using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameplay.Movement.Reel;
using Data.ReelData;
using Controllers;
namespace Gameplay.Controllers
{

    /// <summary>
    /// Controls the behavior of the reel in the slot machine game.
    /// </summary>
    public class ReelController : MonoBehaviour
    {
        [Tooltip("The reel data collection asset.")]
        [SerializeField] private ReelDataCollectionSO _reelDataCollection;
        [Tooltip("The object holder that have the reel items content.")]
        [SerializeField] private GameObject _reelItemsContentHolder;
        [Tooltip("The maximum speed at which the reel can spin.")]
        [SerializeField] private float _maxSpeed = 0.5f;

        [Tooltip("The duration it takes for the reel to stop .")]
        [SerializeField] private float _reelStopDelay = 0.2f;
        [Tooltip("The duration it takes for the reel to recenter after stopping.")]
        [SerializeField] private float _reelRecenterDuration = 0.2f;
        private int _currentItemIndex;
        private ReelItemController[] _childReelItems;
        private bool _isSpinning;
        public ReelState CurrentReelState => _currentReelState;
        public ReelDataCollectionSO ReelDataCollection => _reelDataCollection;
        public GameObject ReelItemsContentHolder => _reelItemsContentHolder;
        public ReelItemController[] ChildReelItems => _childReelItems;
        public ReelItemController[] CurrentOrderChildItems => _reelItemsContentHolder.GetComponentsInChildren<ReelItemController>();
        /// <summary>
        /// List of ordered ReelItemController components representing the runtime reel items.
        /// </summary>
        private readonly List<ReelItemController> _orderedRunTimeReelItems = new();

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
            _layoutGroup.enabled = false;
            _childReelItems = _reelItemsContentHolder.GetComponentsInChildren<ReelItemController>();
            PopulateReelItemsList();
            _reelMovement = new ReelMovement(_childReelItems, _maxSpeed, _reelStopDelay, _reelRecenterDuration);
            SetReelState(ReelState.Ready);
            _currentItemIndex = _childReelItems.Length - 1;
            SetInitialSprite();
        }
        private void Update()
        {
            if (_isSpinning)
                HandleSpinning();
        }
        #endregion
        #region Initialization

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
        /// <summary>
        /// Sets the initial sprite for each child reel item by retrieving the corresponding item data
        /// from the reel data collection and setting up the item.
        /// </summary>
        public void SetInitialSprite()
        {
            for (int i = 0; i < _childReelItems.Length; i++)
                _childReelItems[i].GetComponent<ReelItemController>().SetUpItem(_reelDataCollection.GetItemData(i));

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
                    foreach (var item in _childReelItems)
                        item.ShowCombinationSuccess(false);

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
                _childReelItems[i].RectTransform.anchoredPosition += _reelMovement.SpinDelta;

            if (!_reelMovement.ShouldTransition(_orderedRunTimeReelItems[^2].RectTransform.anchoredPosition.y))
                return;

            RotateItems();
            AdjustFirstItemDistance();
        }
        /// <summary>
        /// Rotates the reel items to simulate spinning.
        /// </summary>
        private void RotateItems()
        {
            ReelItemController lastItem = _orderedRunTimeReelItems[^1];
            _orderedRunTimeReelItems.RemoveAt(_orderedRunTimeReelItems.Count - 1);
            _orderedRunTimeReelItems.Insert(0, lastItem);
            lastItem.RectTransform.anchoredPosition = _reelMovement.FirstAnchorPosition;
            lastItem.transform.SetAsFirstSibling();
            UpdateNextItemVisual();
        }
        /// <summary>
        /// Updates the visual representation of the next item in the reel.
        /// </summary>
        /// <remarks>
        /// This method increments the current item index and updates the sprite of the first item
        /// in the ordered runtime reel items collection to the visual representation of the new current item.
        /// </remarks>
        private void UpdateNextItemVisual()
        {
            _currentItemIndex = (_currentItemIndex + 1) % _reelDataCollection.ItemsCollectionLength;
            _orderedRunTimeReelItems[0].SetUpItem(_reelDataCollection.GetItemData(_currentItemIndex));
        }
        /// <summary>
        /// Adjusts the distance of the first reel item after rotation.
        /// </summary>
        private void AdjustFirstItemDistance()
        {
            _orderedRunTimeReelItems[0].RectTransform.anchoredPosition =
                _reelMovement.CalculateFirstItemPosition(_orderedRunTimeReelItems[1].RectTransform.anchoredPosition);
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
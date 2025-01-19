using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Movement.Reel
{


    /// <summary>
    /// The <c>ReelMovement</c> class handles the movement and positioning of reel items in a slot machine game.
    /// </summary>
    public class ReelMovement
    {
        private float _currentSpeed;
        private readonly float _maxSpeed;
        private readonly float _reelRecenterDuration;
        private readonly float _distanceBetweenItems;
        private readonly float _reelStopDelay;
        private readonly Vector2[] _reelItemsAnchorPositionsList;
        public Vector2 SpinDelta => Vector2.down * (_currentSpeed * Time.deltaTime);
        public Vector2 FirstAnchorPosition => _reelItemsAnchorPositionsList[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="ReelMovement"/> class.
        /// </summary>
        /// <param name="reelItems">The array of reel items.</param>
        /// <param name="maxSpeed">The maximum speed of the reel.</param>
        /// <param name="recenterDuration">The duration to recenter the reel.</param>
        public ReelMovement(RectTransform[] reelItems, float maxSpeed, float reelStopDelay,float recenterDuration)
        {
            _reelItemsAnchorPositionsList = new Vector2[reelItems.Length];
            CacheAnchorPositions(reelItems);
            _distanceBetweenItems = Mathf.Abs(reelItems[0].anchoredPosition.y - reelItems[1].anchoredPosition.y);
            _maxSpeed = maxSpeed;
            _reelRecenterDuration = recenterDuration;
            _reelStopDelay = reelStopDelay;
        }

        /// <summary>
        /// Caches the anchor positions of the reel items.
        /// </summary>
        /// <param name="reelItems">The array of reel items.</param>

        private void CacheAnchorPositions(RectTransform[] reelItems)
        {
            for (int i = 0; i < reelItems.Length; i++)
                _reelItemsAnchorPositionsList[i] = reelItems[i].anchoredPosition;
        }

        /// <summary>
        /// Starts spinning the reel by setting the current speed to the maximum speed.
        /// </summary>
        public void StartSpinning()
        {
            _currentSpeed = _maxSpeed;
        }

        /// <summary>
        /// Determines whether the reel should transition based on the given y-position.
        /// </summary>
        /// <param name="yPosition">The y-position to check.</param>
        /// <returns><c>true</c> if the reel should transition; otherwise, <c>false</c>.</returns>
        public bool ShouldTransition(float yPosition)
        {
            return yPosition <= _reelItemsAnchorPositionsList[^1].y;
        }

        /// <summary>
        /// Calculates the position of the first item based on the position of the second item.
        /// </summary>
        /// <param name="secondItemPosition">The position of the second item.</param>
        /// <returns>The calculated position of the first item.</returns>
        public Vector2 CalculateFirstItemPosition(Vector2 secondItemPosition)
        {
            Vector2 newPosition = secondItemPosition;
            newPosition.y += _distanceBetweenItems;
            return newPosition;
        }

        /// <summary>
        /// Spins the reel for a random duration and then gradually stops it.
        /// </summary>
        /// <returns>An enumerator for the spin and stop coroutine.</returns>
        public IEnumerator SpinAndStop()
        {
            float timeReducing = 0f;
            while (timeReducing < _reelStopDelay)
            {
                timeReducing += Time.deltaTime;
                float progress = timeReducing / _reelStopDelay;
                _currentSpeed = Mathf.Lerp(_maxSpeed, 0f, progress);
                Debug.Log($"Reducing speed to {_currentSpeed}.");
                yield return null;
            }

            _currentSpeed = 0f;
        }
        /// <summary>
        /// Recenters the reel items to their original positions over a specified duration.
        /// </summary>
        /// <param name="reelItemsList">The list of reel items to recenter.</param>
        /// <returns>An enumerator for the recentering coroutine.</returns>
        public IEnumerator CenterReelCoroutine(List<RectTransform> reelItemsList)
        {
            float elapsedTime = 0f;
            Vector2[] startPositions = new Vector2[reelItemsList.Count];

            for (int i = 0; i < reelItemsList.Count; i++)
                startPositions[i] = reelItemsList[i].anchoredPosition;

            while (elapsedTime < _reelRecenterDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / _reelRecenterDuration;
                for (int i = 0; i < reelItemsList.Count; i++)
                    reelItemsList[i].anchoredPosition = Vector2.Lerp(startPositions[i], _reelItemsAnchorPositionsList[i], t);
                yield return null;
            }

            for (int i = 0; i < reelItemsList.Count; i++)
                reelItemsList[i].anchoredPosition = _reelItemsAnchorPositionsList[i];
        }
    }
}
using Data.ReelData;
using UnityEngine;
using UnityEngine.UI;
namespace Controllers
{
    /// <summary>
    /// Represents a controller for a reel item in the slot machine.
    /// </summary>
    public class ReelItemController : MonoBehaviour
    {
        /// <summary>
        /// The image component of the reel item.
        /// </summary>
        private Image _itemImage;

        /// <summary>
        /// The image component indicating a successful combination.
        /// </summary>
        private Image _combinationSuccesImage;

        /// <summary>
        /// The reel item associated with this controller.
        /// </summary>
        private ReelItem _reelItem;

        /// <summary>
        /// The RectTransform component of the reel item.
        /// </summary>
        private RectTransform _rectTransform;

        /// <summary>
        /// Gets the RectTransform component of the reel item.
        /// </summary>
        public RectTransform RectTransform => _rectTransform;

        /// <summary>
        /// Gets the reel item associated with this controller.
        /// </summary>
        public ReelItem ReelItem => _reelItem;

        /// <summary>
        /// Initializes the reel item controller.
        /// </summary>
        private void Awake()
        {
            InitItem();
        }

        /// <summary>
        /// Initializes the components of the reel item.
        /// </summary>
        public void InitItem()
        {
            _itemImage = GetComponent<Image>();
            _combinationSuccesImage = transform.GetChild(0).GetComponent<Image>();
            _combinationSuccesImage.gameObject.SetActive(false);
            _rectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Sets up the reel item with the provided data.
        /// </summary>
        /// <param name="reelItemData">The data for the reel item.</param>
        public void SetUpItem(ReelItemDataSO reelItemData)
        {
            _reelItem = reelItemData.ReelItem;
            _itemImage.sprite = reelItemData.ReelItemSprite;
            _itemImage.SetNativeSize();
        }
        /// <summary>
        /// Displays or hides the combination success image based on the provided state.
        /// </summary>
        /// <param name="state">If set to <c>true</c>, the combination success image will be shown; otherwise, it will be hidden.</param>
        public void ShowCombinationSuccess(bool state)
        {
            _combinationSuccesImage.gameObject.SetActive(state);
        }

    }
}

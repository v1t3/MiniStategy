using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
    public class CollectButton : MonoBehaviour
    {
        [SerializeField] private Image loadingImage;
        [SerializeField] private Text collectText;

        public void UpdateCollectText(int collectValue)
        {
            collectText.text = collectValue.ToString();
        }

        public void UpdateAmount(float fillAmount)
        {
            loadingImage.fillAmount = fillAmount;
        }
    }
}
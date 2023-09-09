using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

[RequireComponent(typeof(IAPButton))]
public class IAPButtonDescriptionController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private IAPButton attachedButton;

    private void OnValidate()
    {
        TryGetComponent(out attachedButton);
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        var product = CodelessIAPStoreListener.Instance.GetProduct(attachedButton.productId);

        if (priceText != null)
            priceText.SetText(product.metadata.localizedPriceString);

        if (titleText != null)
            titleText.SetText(product.metadata.localizedTitle);

        if (descriptionText != null)
            descriptionText.SetText(product.metadata.localizedDescription);
    }
}
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager instance;
    private IStoreController m_controller;

    private bool m_purchaseInProgress = false;

    private ITransactionHistoryExtensions m_transactionHistoryExtensions;

    private Action<APIContents> CALLBACK_PURCHASE_COMPLETE;

    private Dictionary<string, InAppData> m_inappItems;
    private Product m_currentPurchasingProduct;

    void Awake()
    {
        instance = this;
        
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        m_inappItems = GameDatabase.Instance.inAppList.inappList;
        foreach(InAppData inapp in m_inappItems.Values)
        {
            builder.AddProduct(inapp.inappIdentifier, ProductType.Consumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public string GetLocalizePrice(string productId)
    {
        string price = "";

        if(string.IsNullOrEmpty(productId) == false)
        {
            Product product = m_controller.products.WithID(productId);
            if(product != null)
                price = product.metadata.localizedPriceString;
            else
                price = "Not Read";            
        }

        return price;
    }

    /// <summary>
    /// This will be called after a call to IAppleExtensions.RestoreTransactions().
    /// </summary>
    private void OnTransactionsRestored(bool success)
    {
        Debug.Log("Transactions restored." + success);
    }

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    ///
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log("Purchase OK: " + purchaseEvent.purchasedProduct.definition.id);
        Debug.Log("Receipt: " + purchaseEvent.purchasedProduct.receipt);
        m_currentPurchasingProduct = purchaseEvent.purchasedProduct;
        
#if UNITY_ANDROID
        UnifiedReceipt ureceipt = JsonFx.Json.JsonReader.Deserialize<UnifiedReceipt>(purchaseEvent.purchasedProduct.receipt);
        Dictionary<string, object> payloadData = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>>(ureceipt.Payload);
        Dictionary<string, object> payLoadJson = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>>((string)payloadData["json"]);
        WebServerAPIs.Instance.VerificationGoogleInAppPurchase(purchaseEvent.purchasedProduct, payLoadJson, CallbackVerifyInAppPurchase);
        return PurchaseProcessingResult.Pending;
#elif UNITY_IOS
        UnifiedReceipt ureceipt = JsonFx.Json.JsonReader.Deserialize<UnifiedReceipt>(purchaseEvent.purchasedProduct.receipt);
        WebServerAPIs.Instance.VerificationIOSInAppPurchase(purchaseEvent.purchasedProduct, ureceipt, CallbackVerifyInAppPurchase);
        return PurchaseProcessingResult.Pending;
#endif

    }

    private void CallbackVerifyInAppPurchase(APIContents contents)
    {
        CM_Singleton<GameData>.instance.m_Info_User.purchased = true;
        //금액갱신..
        // CM_Singleton<GameData>.instance.m_Info_User.nPurchasedTotal =

        Utils.InvokeAction(CALLBACK_PURCHASE_COMPLETE, contents);
        m_controller.ConfirmPendingPurchase(m_currentPurchasingProduct);
        m_currentPurchasingProduct = null;
        CALLBACK_PURCHASE_COMPLETE = null;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("Purchase failed: " + product.definition.id);
        Debug.Log(failureReason);

        // Detailed debugging information
        Debug.Log("Store specific error code: " + m_transactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
        if (m_transactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
        {
            Debug.Log("Purchase failure description message: " +
                      m_transactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
        }

        m_purchaseInProgress = false;
        CALLBACK_PURCHASE_COMPLETE = null;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_controller = controller;
        // foreach(Product product in m_controller.products.all)
        // {
        //     Debug.Log(product.definition.id);
        // }
        // Debug.Log("initialized UnityPurchasing");
    }

    public void BuyProduct(string productID, Action<APIContents> callback = null)
    {
        m_purchaseInProgress = true;
        Debug.Log(productID);
#if UNITY_EDITOR
        WebServerAPIs.Instance.TestVerificationGoogleInAppPurchase(productID, callback);
        m_purchaseInProgress = false;
#else
        Debug.Log("controller initialized checking :: " + m_controller);
        m_controller.InitiatePurchase(m_controller.products.WithID(productID), "developerPayload");
        CALLBACK_PURCHASE_COMPLETE = callback;
#endif
    }
    
    public void BuyShopPackageProduct(string productID, Action<APIContents> callback = null)
    {
        Debug.Log(productID);
        m_purchaseInProgress = true;

#if UNITY_EDITOR
        WebServerAPIs.Instance.TestVerificationGoogleInAppPurchase(productID, callback);
        m_purchaseInProgress = false;
#else
        Debug.Log("controller initialized checking :: " + m_controller);
        m_controller.InitiatePurchase(m_controller.products.WithID(productID), "developerPayload");
        CALLBACK_PURCHASE_COMPLETE = callback;
#endif
    }


#if DELAY_CONFIRMATION
    private HashSet<string> m_PendingProducts = new HashSet<string>();

    private IEnumerator ConfirmPendingPurchaseAfterDelay(Product p)
    {
        m_PendingProducts.Add(p.definition.id);
        Debug.Log("Delaying confirmation of " + p.definition.id + " for 5 seconds.");

		var end = Time.time + 5f;

		while (Time.time < end) {
			yield return null;
			var remaining = Mathf.CeilToInt (end - Time.time);
			// UpdateProductPendingUI (p, remaining);
		}

        if (m_IsGooglePlayStoreSelected)
        {
            Debug.Log("Is " + p.definition.id + " currently owned, according to the Google Play store? "
                      + m_GooglePlayStoreExtensions.IsOwned(p));
        }
        Debug.Log("Confirming purchase of " + p.definition.id);
        m_controller.ConfirmPendingPurchase(p);
        m_PendingProducts.Remove(p.definition.id);
		// UpdateProductUI (p);
    }
#endif
}

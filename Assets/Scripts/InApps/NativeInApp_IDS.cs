using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace NativeInApps
{
    /// <summary>
    /// Native in app identifiers. Here you have to manually add all your IAPPs
    /// </summary>
    public static class NativeInApp_IDS
    {
        /// <summary>
        /// The product ID. Change this to correspond with your IAPP product ID.
        /// </summary>
        public const string NO_ADS_PRODUCT_ID = "com.tedrasoft.unblock.noads";
        /// <summary>
        /// The product Name, this should also correspond with the one under your developer account.
        /// </summary>
        public const string NO_ADS_PRODUCT_NAME = "No Ads";

        public const string BONUS_PACK_PRODUCT_ID = "com.tedrasoft.unblock.bonuspack1";
        public const string BONUS_PACK_PRODUCT_NAME = "Bonus Pack 1";

        /// <summary>
        /// List of available items. They have to be registered here too.
        /// </summary>
        public static NativeInAppItem[] Items =
        {
            new NativeInAppItem(NO_ADS_PRODUCT_NAME, NO_ADS_PRODUCT_ID, ProductType.NonConsumable),
            new NativeInAppItem(BONUS_PACK_PRODUCT_NAME, BONUS_PACK_PRODUCT_ID, ProductType.NonConsumable)
        };

        public static string GetItemName(string sku) {
            for (int i = 0; i < Items.Length; i++)
                if (Items[i].ProductID == sku)
                    return Items[i].Name;
            return null;
        }

        public static string GetItemId(string name) {
            for (int i = 0; i < Items.Length; i++)
                if (Items[i].Name == name)
                    return Items[i].ProductID;
            return null;
        }

    }
}

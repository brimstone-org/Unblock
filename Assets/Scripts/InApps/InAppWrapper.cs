using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NativeInApps{

    /// <summary>
    /// Simple wrapper for Native INAPPS
    /// </summary>
public class InAppWrapper : MonoBehaviour {

        /// <summary>
        /// Restore purchases if Game was reinstalled or installed on another device.
        /// </summary>
    public void Restore(){
        NativeInApp.Instance.RestorePurchases();
    }
        /// <summary>
        /// Buys an item.
        /// </summary>
        /// <param name="sku">ID of the item to be bought.</param>
    public void Buy(string sku){
        NativeInApp.Instance.BuyProductID(sku);
    }
}
}

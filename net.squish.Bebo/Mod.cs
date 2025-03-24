using BepInEx;
using BrutalAPI;
using UnityEngine;

namespace net.squish.Bebo {
    // Version string, used so BepInEx doesn't shit itself
    [BepInPlugin("squished.Bebo", "Bebo the Fool", "0.1")]
    
    public class BeboMod : BaseUnityPlugin {
        public void Awake() {
            Logger.LogInfo("bebo");
            net.squish.Bebo.BeboFool.Add();
            
            // Only called a hairball for marketing purposes
            net.squish.Bebo.Item.Hairball.Add();
        }
    }
}

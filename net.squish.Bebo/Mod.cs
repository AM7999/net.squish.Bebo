using BepInEx;
using BrutalAPI;
using UnityEngine;

namespace net.squish.Bebo {
    // Version string, used so BepInEx doesn't shit itself
    [BepInPlugin("net.squished.Bebo", "Bebo the Fool", "0.1")]
    
    public class BeboMod : BaseUnityPlugin {
        public void Awake() {
            Logger.LogInfo("bebo");
            BeboFool.Add();
        }
    }
}

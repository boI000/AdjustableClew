using BepInEx;
using HarmonyLib;

namespace AdjustableClew
{
    [BepInPlugin(
        "com.bol000.sailwind.adjustableclew",
        "Adjustable Clew",
        "0.1.0"
    )]
    public class AdjustableClewPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony =
                new Harmony("com.bol000.sailwind.adjustableclew");

            harmony.PatchAll();

            Logger.LogInfo(
                "Adjustable Clew 0.1.0 loaded."
            );
        }
    }
}
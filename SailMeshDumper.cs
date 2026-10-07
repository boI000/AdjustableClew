using BepInEx;
using HarmonyLib;

namespace SailMeshDumper
{
    [BepInPlugin(
        "com.sailwind.sailmeshdumper",
        "Sail Mesh Dumper",
        "0.2.0"
    )]
    public class SailMeshDumperPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony =
                new Harmony("com.sailwind.sailmeshdumper");

            harmony.PatchAll();

            Logger.LogInfo(
                "SailMeshDumper 0.2.0 loaded."
            );
        }
    }
}
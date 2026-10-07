using HarmonyLib;

namespace SailMeshDumper
{
    [HarmonyPatch(typeof(SaveableBoatCustomization), "LoadData")]
    internal static class ClewLoadDataPatch
    {
        private static void Postfix(BoatRefs ___refs)
        {
            if (___refs == null)
                return;

            ClewDataManager.LoadClewConfig(___refs);
        }
    }
}
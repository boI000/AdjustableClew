using HarmonyLib;
using UnityEngine;

namespace AdjustableClew
{
    [HarmonyPatch(typeof(ShipyardSailInstaller), "AddNewSail")]
    internal static class SailMeshInstancePatch
    {
        private static void Prefix(GameObject sailObject)
        {
            SailMeshInitializer.Initialize(sailObject);
        }
    }
}
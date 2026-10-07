using HarmonyLib;
using UnityEngine;

namespace SailMeshDumper
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
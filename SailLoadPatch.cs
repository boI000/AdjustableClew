using HarmonyLib;
using UnityEngine;

namespace SailMeshDumper
{
    [HarmonyPatch(typeof(Mast), "LoadSail")]
    internal static class SailLoadPatch
    {
        private static void Postfix(
            Mast __instance,
            SaveSailData sailData)
        {
            // Interesuje nas wyłącznie Brig Jib.
            if (sailData == null || sailData.prefabIndex != 110)
                return;

            if (__instance.sails == null ||
                __instance.sails.Count == 0)
            {
                Debug.LogWarning(
                    "[Clew Load] Mast has no sails after LoadSail."
                );
                return;
            }

            // LoadSail dodaje właśnie utworzony żagiel
            // na koniec mast.sails.
            GameObject sailObject =
                __instance.sails[__instance.sails.Count - 1];

            if (sailObject == null)
            {
                Debug.LogWarning(
                    "[Clew Load] Loaded sail object is null."
                );
                return;
            }

            Sail sail = sailObject.GetComponent<Sail>();

            if (sail == null || sail.prefabIndex != 110)
            {
                Debug.LogWarning(
                    "[Clew Load] Last sail is not Brig Jib."
                );
                return;
            }

            Debug.Log(
                "[Clew Load] Initializing loaded Brig Jib: " +
                sailObject.name
            );

            SailMeshInitializer.Initialize(sailObject);
        }
    }
}
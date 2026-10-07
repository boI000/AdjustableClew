using HarmonyLib;
using UnityEngine;

namespace AdjustableClew
{
    [HarmonyPatch(typeof(SaveableBoatCustomization), "GetData")]
    internal static class ClewSavePatch
    {
        private static void Postfix(BoatRefs ___refs)
        {
            if (___refs == null)
                return;

            // Tak samo jak ShipyardExpansion:
            // zapisujemy konfigurację zakupionego statku.
            PurchasableBoat purchasableBoat =
                ___refs.GetComponent<PurchasableBoat>();

            if (purchasableBoat == null ||
                !purchasableBoat.isPurchased())
            {
                return;
            }

            ClewDataManager.SaveClewConfig(___refs);
        }
    }
}
using HarmonyLib;
using UnityEngine;

namespace AdjustableClew
{
    [HarmonyPatch(typeof(ShipyardUI))]
    internal static class ClewUIPatch
    {
        private static GameObject clewUpButton;
        private static GameObject clewDownButton;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        [HarmonyAfter("com.nandbrew.shipyardexpansion")]
        private static void AwakePostfix()
        {
            CreateButtons();
        }

        [HarmonyPatch("UpdateMoveButtons")]
        [HarmonyPostfix]
        private static void UpdateMoveButtonsPostfix()
        {
            UpdateButtonVisibility();
        }

        private static void CreateButtons()
        {
            // Zabezpieczenie przed podwójnym utworzeniem.
            if (clewUpButton != null ||
                clewDownButton != null)
            {
                return;
            }

            GameObject widthUp =
                GameObject.Find("button scale width up");

            GameObject widthDown =
                GameObject.Find("button scale width down");

            if (widthUp == null || widthDown == null)
            {
                Debug.LogWarning(
                    "[Clew UI] ShipyardExpansion width buttons not found."
                );
                return;
            }

            clewUpButton =
                Object.Instantiate(
                    widthUp,
                    widthUp.transform.parent
                );

            clewDownButton =
                Object.Instantiate(
                    widthDown,
                    widthDown.transform.parent
                );

            clewUpButton.name = "button clew up";
            clewDownButton.name = "button clew down";

            RemoveScaleButton(clewUpButton);
            RemoveScaleButton(clewDownButton);

            ClewButton up =
                clewUpButton.AddComponent<ClewButton>();

            up.increase = true;

            ClewButton down =
                clewDownButton.AddComponent<ClewButton>();

            down.increase = false;

            clewUpButton.transform.localPosition =
                new Vector3(
                    -0.75f,
                    -1.9f,
                    0.0165f
                );

            clewDownButton.transform.localPosition =
                new Vector3(
                    -2.0f,
                    -1.9f,
                    0.0113f
                );

            // Awake może nastąpić bez wybranego żagla.
            clewUpButton.SetActive(false);
            clewDownButton.SetActive(false);

            SetButtonText(clewUpButton, "+\nClew");
            SetButtonText(clewDownButton, "-\nClew");

            Debug.Log("[Clew UI] Buttons created.");
        }

        private static void RemoveScaleButton(
            GameObject button)
        {
            Component oldButton =
                button.GetComponent(
                    "ShipyardExpansion.SailScaleButton"
                );

            if (oldButton == null)
            {
                oldButton =
                    button.GetComponent("SailScaleButton");
            }

            if (oldButton != null)
            {
                Object.DestroyImmediate(oldButton);
            }
        }

        private static void SetButtonText(
            GameObject button,
            string text)
        {
            if (button.transform.childCount == 0)
                return;

            TextMesh textMesh =
                button.transform
                    .GetChild(0)
                    .GetComponent<TextMesh>();

            if (textMesh != null)
            {
                textMesh.text = text;
            }
        }
        private static void UpdateButtonVisibility()
        {
            if (clewUpButton == null ||
                clewDownButton == null)
            {
                // Awaryjnie, gdyby kolejność Awake innych modów
                // była inna niż oczekiwana.
                CreateButtons();

                if (clewUpButton == null ||
                    clewDownButton == null)
                {
                    return;
                }
            }

            bool show = false;

            if (GameState.currentShipyard != null &&
                GameState.currentShipyard.sailInstaller != null)
            {
                Sail sail =
                    GameState.currentShipyard
                        .sailInstaller
                        .GetCurrentSail();

                show =
                    sail != null &&
                    sail.prefabIndex == 110 &&
                    !sail.IsInstalled();
            }

            clewUpButton.SetActive(show);
            clewDownButton.SetActive(show);
        }
    }
}
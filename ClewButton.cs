using UnityEngine;

namespace SailMeshDumper
{
    public class ClewButton : GoPointerButton
    {
        public bool increase;

        private static ClewButton clickHeldButton;
        private static float heldTimer;

        private void HoldButton()
        {
            if (clickHeldButton != this)
            {
                clickHeldButton = this;
                heldTimer = 0.16f;
            }
        }

        public override void ExtraLateUpdate()
        {
            if (clickHeldButton == this && Input.GetMouseButtonUp(0))
            {
                clickHeldButton = null;
            }

            if (clickHeldButton == this)
            {
                heldTimer -= Time.deltaTime;

                if (heldTimer <= 0f)
                {
                    heldTimer = 0.05f;
                    OnActivate();
                }
            }
        }

        public override void OnActivate()
        {
            base.OnActivate();

            UISoundPlayer.instance.PlayUISound(
                UISounds.buttonHover,
                0.33f,
                3f
            );

            Sail currentSail =
                GameState.currentShipyard.sailInstaller.GetCurrentSail();

            if (currentSail == null)
            {
                Debug.LogWarning(
                    "[Sail Mesh Dumper] No sail selected."
                );
                return;
            }

            SkinnedMeshRenderer renderer =
                currentSail.GetComponentInChildren<SkinnedMeshRenderer>();

            if (renderer == null || renderer.sharedMesh == null)
            {
                Debug.LogWarning(
                    "[Sail Mesh Dumper] Sail mesh not found."
                );
                return;
            }

            float step = increase ? 0.25f : -0.25f;

            // Znajdź / utwórz kontroler attachmentu dla tego żagla.
            ClewAttachmentController controller =
                currentSail.GetComponent<ClewAttachmentController>();

            if (controller == null)
            {
                controller =
                    currentSail.gameObject
                        .AddComponent<ClewAttachmentController>();
            }

            controller.ChangeClewOffset(step);

            currentSail.gameObject.SetActive(false);
            currentSail.gameObject.SetActive(true);

            if (ShipyardUI.instance != null)
            {
                ShipyardUI.instance.RefreshButtons();
                ShipyardUI.instance.UpdateDescriptionText();
            }

            HoldButton();
        }
    }
}
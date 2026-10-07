using HarmonyLib;
using UnityEngine;

namespace SailMeshDumper
{
    internal static class SailMeshInitializer
    {
        public static void Initialize(GameObject sailObject)
        {
            if (sailObject == null)
                return;

            Sail sail = sailObject.GetComponent<Sail>();

            // Obsługujemy wyłącznie Brig Jiba.
            if (sail == null || sail.prefabIndex != 110)
                return;

            // Zabezpieczenie przed ponowną inicjalizacją.
            if (sailObject.GetComponent<ClewAttachmentController>() != null)
            {
                Debug.Log(
                    "[Clew Mesh] Sail already initialized: " +
                    sailObject.name
                );
                return;
            }

            SkinnedMeshRenderer renderer =
                sailObject.GetComponentInChildren<SkinnedMeshRenderer>();

            if (renderer == null || renderer.sharedMesh == null)
            {
                Debug.LogWarning(
                    "[Clew Mesh] Renderer/mesh not found."
                );
                return;
            }

            Cloth oldCloth = renderer.GetComponent<Cloth>();
            WindCloth windCloth = renderer.GetComponent<WindCloth>();

            if (oldCloth == null || windCloth == null)
            {
                Debug.LogWarning(
                    "[Clew Mesh] Cloth/WindCloth not found."
                );
                return;
            }

            // Zachowujemy dane starego Cloth.
            ClothSkinningCoefficient[] coefficients =
                oldCloth.coefficients;

            float bendingStiffness = oldCloth.bendingStiffness;
            float stretchingStiffness = oldCloth.stretchingStiffness;
            float damping = oldCloth.damping;
            float friction = oldCloth.friction;
            float sleepThreshold = oldCloth.sleepThreshold;
            float collisionMassScale = oldCloth.collisionMassScale;
            float solverFrequency = oldCloth.clothSolverFrequency;

            bool useGravity = oldCloth.useGravity;
            bool continuousCollision =
                oldCloth.enableContinuousCollision;

            Mesh originalMesh = renderer.sharedMesh;

            Debug.Log(
                "[Clew Mesh] BEFORE | mesh=" +
                originalMesh.GetInstanceID() +
                " | cloth vertices=" +
                oldCloth.vertices.Length +
                " | coefficients=" +
                coefficients.Length
            );

            // Cloth musi zniknąć przed podmianą mesha.
            Object.DestroyImmediate(oldCloth);

            // Prywatna kopia mesha dla konkretnej instancji żagla.
            Mesh privateMesh = Object.Instantiate(originalMesh);
            privateMesh.name =
                originalMesh.name + " [Clew Private]";

            renderer.sharedMesh = privateMesh;

            // Odbudowujemy Cloth już na prywatnym meshu.
            Cloth newCloth =
                renderer.gameObject.AddComponent<Cloth>();

            newCloth.bendingStiffness = bendingStiffness;
            newCloth.stretchingStiffness = stretchingStiffness;
            newCloth.damping = damping;
            newCloth.friction = friction;
            newCloth.sleepThreshold = sleepThreshold;
            newCloth.collisionMassScale = collisionMassScale;
            newCloth.clothSolverFrequency = solverFrequency;
            newCloth.useGravity = useGravity;
            newCloth.enableContinuousCollision =
                continuousCollision;

            newCloth.coefficients = coefficients;

            // Sail przechowuje własną referencję do Cloth.
            sail.cloth = newCloth;

            // WindCloth również przechowuje prywatną referencję.
            AccessTools.Field(typeof(WindCloth), "cloth")
                .SetValue(windCloth, newCloth);

            // Attachment clewa musi być kontrolowany od początku
            // życia runtime'owej instancji.
            sailObject.AddComponent<ClewAttachmentController>();

            Debug.Log(
                "[Clew Mesh] INITIALIZED | sail=" +
                sailObject.name +
                " | mesh=" +
                privateMesh.GetInstanceID() +
                " | cloth vertices=" +
                newCloth.vertices.Length +
                " | coefficients=" +
                newCloth.coefficients.Length
            );
        }
    }
}
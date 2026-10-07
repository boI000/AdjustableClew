using UnityEngine;

namespace SailMeshDumper
{
    public class ClewAttachmentController : MonoBehaviour
    {
        private const int ClewVertexIndex = 167;
        private const int LuffVertexIndex = 229;

        public float ClewOffset { get; private set; } = 0f;

        private Transform attachment;
        private SkinnedMeshRenderer renderer;

        // Oryginalna geometria prywatnego mesha tego żagla.
        private Vector3[] baseVertices;

        // Dane potrzebne do śledzenia clew bez odczytywania
        // mesha w każdej klatce.
        private Transform clewBone;
        private Matrix4x4 clewBindpose;
        private Vector3 clewLocalPosition;

        private void Awake()
        {
            SailConnections connections =
                GetComponent<SailConnections>();

            if (connections == null ||
                connections.angleControllerLeft == null)
            {
                Debug.LogWarning(
                    "[Clew Attachment] SailConnections not found."
                );
                enabled = false;
                return;
            }

            RopeEffect ropeEffect =
                connections.angleControllerLeft.GetComponent<RopeEffect>();

            if (ropeEffect == null ||
                ropeEffect.attachment == null)
            {
                Debug.LogWarning(
                    "[Clew Attachment] Rope attachment not found."
                );
                enabled = false;
                return;
            }

            attachment = ropeEffect.attachment;

            renderer =
                GetComponentInChildren<SkinnedMeshRenderer>();

            if (renderer == null ||
                renderer.sharedMesh == null)
            {
                Debug.LogWarning(
                    "[Clew Attachment] SkinnedMeshRenderer not found."
                );
                enabled = false;
                return;
            }

            Mesh mesh = renderer.sharedMesh;

            if (mesh.vertexCount <= ClewVertexIndex)
            {
                Debug.LogWarning(
                    "[Clew Attachment] Unexpected sail mesh."
                );
                enabled = false;
                return;
            }

            baseVertices = mesh.vertices;

            BoneWeight[] boneWeights = mesh.boneWeights;
            Matrix4x4[] bindposes = mesh.bindposes;

            BoneWeight clewWeight =
                boneWeights[ClewVertexIndex];

            int boneIndex =
                clewWeight.boneIndex0;

            if (boneIndex < 0 ||
                boneIndex >= renderer.bones.Length ||
                boneIndex >= bindposes.Length)
            {
                Debug.LogWarning(
                    "[Clew Attachment] Invalid clew bone."
                );
                enabled = false;
                return;
            }

            clewBone = renderer.bones[boneIndex];
            clewBindpose = bindposes[boneIndex];

            UpdateClewLocalPosition();

            Debug.Log(
                "[Clew Attachment] Controller initialized."
            );
        }

        public void ChangeClewOffset(float amount)
        {
            ClewOffset += amount;

            ApplyClewOffset();

            Debug.Log(
                "[Clew State] Offset changed: " +
                ClewOffset
            );
        }

        public void SetClewOffset(float offset)
        {
            ClewOffset = offset;

            ApplyClewOffset();

            Debug.Log(
                "[Clew State] Offset loaded: " +
                ClewOffset
            );
        }

        public void ApplyClewOffset()
        {
            if (renderer == null ||
                renderer.sharedMesh == null ||
                baseVertices == null)
            {
                return;
            }

            if (baseVertices.Length <= LuffVertexIndex)
            {
                Debug.LogWarning(
                    "[Clew State] Unexpected sail mesh."
                );
                return;
            }

            Mesh mesh = renderer.sharedMesh;

            // Zawsze zaczynamy od bazowego kształtu.
            Vector3[] vertices =
                (Vector3[])baseVertices.Clone();

            float luffX =
                baseVertices[LuffVertexIndex].x;

            float clewX =
                baseVertices[ClewVertexIndex].x;

            for (int i = 0; i < vertices.Length; i++)
            {
                float clewWeight =
                    (baseVertices[i].x - luffX) /
                    (clewX - luffX);

                clewWeight =
                    Mathf.Clamp01(clewWeight);

                vertices[i].z +=
                    ClewOffset * clewWeight;
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            // Clew zmienia lokalną pozycję tylko tutaj,
            // więc cache aktualizujemy tylko po zmianie geometrii.
            UpdateClewLocalPosition();

            Debug.Log(
                "[Clew State] Applied offset: " +
                ClewOffset
            );
        }

        private void UpdateClewLocalPosition()
        {
            if (baseVertices == null ||
                baseVertices.Length <= ClewVertexIndex)
            {
                return;
            }

            clewLocalPosition =
                baseVertices[ClewVertexIndex];

            float luffX =
                baseVertices[LuffVertexIndex].x;

            float clewX =
                baseVertices[ClewVertexIndex].x;

            float clewWeight =
                (baseVertices[ClewVertexIndex].x - luffX) /
                (clewX - luffX);

            clewWeight =
                Mathf.Clamp01(clewWeight);

            clewLocalPosition.z +=
                ClewOffset * clewWeight;
        }

        private void LateUpdate()
        {
            if (attachment == null ||
                clewBone == null)
            {
                return;
            }

            Matrix4x4 skinMatrix =
                clewBone.localToWorldMatrix *
                clewBindpose;

            Vector3 clewWorld =
                skinMatrix.MultiplyPoint3x4(
                    clewLocalPosition
                );

            attachment.position = clewWorld;
        }
    }
}
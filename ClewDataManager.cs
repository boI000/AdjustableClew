using System.Globalization;
using System.Text;
using UnityEngine;

namespace AdjustableClew
{
    internal static class ClewDataManager
    {
        private const string DataKeyPrefix = "ClewSails.";

        public static void SaveClewConfig(BoatRefs refs)
        {
            if (refs == null)
                return;

            SaveableObject saveableObject =
                refs.GetComponent<SaveableObject>();

            if (saveableObject == null)
            {
                Debug.LogWarning(
                    "[Clew Save] SaveableObject not found."
                );
                return;
            }

            string key =
                DataKeyPrefix +
                saveableObject.sceneIndex.ToString();

            StringBuilder data = new StringBuilder();

            for (int mastIndex = 0;
                 mastIndex < refs.masts.Length;
                 mastIndex++)
            {
                Mast mast = refs.masts[mastIndex];

                if (mast == null)
                    continue;

                for (int sailIndex = 0;
                     sailIndex < mast.sails.Count;
                     sailIndex++)
                {
                    GameObject sailObject =
                        mast.sails[sailIndex];

                    if (sailObject == null)
                        continue;

                    Sail sail =
                        sailObject.GetComponent<Sail>();

                    if (sail == null ||
                        sail.prefabIndex != 110)
                    {
                        continue;
                    }

                    ClewAttachmentController controller =
                        sailObject.GetComponent<
                            ClewAttachmentController>();

                    if (controller == null)
                        continue;

                    if (data.Length > 0)
                        data.Append(";");

                    data.Append(mastIndex);
                    data.Append(",");
                    data.Append(sailIndex);
                    data.Append(",");
                    data.Append(
                        controller.ClewOffset.ToString(
                            CultureInfo.InvariantCulture
                        )
                    );
                }
            }

            GameState.modData[key] = data.ToString();

            Debug.Log(
                "[Clew Save] key=" +
                key +
                " | data=" +
                data
            );


        }

        public static void LoadClewConfig(BoatRefs refs)
        {
            if (refs == null)
                return;

            SaveableObject saveableObject =
                refs.GetComponent<SaveableObject>();

            if (saveableObject == null)
            {
                Debug.LogWarning(
                    "[Clew Load Data] SaveableObject not found."
                );
                return;
            }

            string key =
                DataKeyPrefix +
                saveableObject.sceneIndex.ToString();

            if (!GameState.modData.ContainsKey(key))
            {
                Debug.Log(
                    "[Clew Load Data] No saved clew data for key=" +
                    key
                );
                return;
            }

            string data = GameState.modData[key];

            if (string.IsNullOrEmpty(data))
            {
                Debug.Log(
                    "[Clew Load Data] Clew data is empty for key=" +
                    key
                );
                return;
            }

            string[] entries = data.Split(';');

            foreach (string entry in entries)
            {
                string[] parts = entry.Split(',');

                if (parts.Length != 3)
                {
                    Debug.LogWarning(
                        "[Clew Load Data] Invalid entry: " +
                        entry
                    );
                    continue;
                }

                int mastIndex;
                int sailIndex;
                float clewOffset;

                if (!int.TryParse(parts[0], out mastIndex) ||
                    !int.TryParse(parts[1], out sailIndex) ||
                    !float.TryParse(
                        parts[2],
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out clewOffset))
                {
                    Debug.LogWarning(
                        "[Clew Load Data] Could not parse entry: " +
                        entry
                    );
                    continue;
                }

                if (mastIndex < 0 ||
                    mastIndex >= refs.masts.Length)
                {
                    Debug.LogWarning(
                        "[Clew Load Data] Invalid mast index: " +
                        mastIndex
                    );
                    continue;
                }

                Mast mast = refs.masts[mastIndex];

                if (mast == null ||
                    mast.sails == null ||
                    sailIndex < 0 ||
                    sailIndex >= mast.sails.Count)
                {
                    Debug.LogWarning(
                        "[Clew Load Data] Invalid sail index: " +
                        sailIndex
                    );
                    continue;
                }

                GameObject sailObject =
                    mast.sails[sailIndex];

                if (sailObject == null)
                    continue;

                Sail sail =
                    sailObject.GetComponent<Sail>();

                if (sail == null ||
                    sail.prefabIndex != 110)
                {
                    Debug.LogWarning(
                        "[Clew Load Data] Saved sail is not Brig Jib."
                    );
                    continue;
                }

                ClewAttachmentController controller =
                    sailObject.GetComponent<
                        ClewAttachmentController>();

                if (controller == null)
                {
                    Debug.LogWarning(
                        "[Clew Load Data] Controller not found."
                    );
                    continue;
                }

                Debug.Log(
                    "[Clew Load Data] Applying saved offset=" +
                    clewOffset +
                    " | mast=" +
                    mastIndex +
                    " | sail=" +
                    sailIndex
                );

                controller.SetClewOffset(clewOffset);

                // Tak samo jak podczas edycji w Shipyardzie:
                // Cloth musi zostać odświeżony po zmianie mesha.
                sailObject.SetActive(false);
                sailObject.SetActive(true);
            }
        }
    }
}
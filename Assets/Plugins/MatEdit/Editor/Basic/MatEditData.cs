using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace MB.MatEdit
{
    internal class MatEditData : ScriptableObject
    {
        public bool welcomeMessage = false;

        public static MatEditData Data
        {
            get
            {
                MatEditData DATA = Resources.Load<MatEditData>(MatEdit.ROOT_PATH + "/DATA");
                if (DATA == null)
                {
                    DATA = ScriptableObject.CreateInstance<MatEditData>();
                    DATA.name = "DATA";
                    string rootFolder = Content.GetAsset(Content.VERSION_FILE);
                    AssetDatabase.CreateAsset(DATA, rootFolder + "/DATA.asset");
                }
                return DATA;
            }
        }

    }
}
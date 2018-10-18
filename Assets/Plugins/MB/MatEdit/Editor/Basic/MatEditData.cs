// Unity
using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    /// <summary>
    /// A persistant Object to save data which should be done once!
    /// </summary>
    internal class MatEditData : ScriptableObject
    {

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //--------------------------------------< CONTENT >--------------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Content

        /// <summary>
        /// If the user was asked to install CG support
        /// </summary>
        public bool welcomeMessage = false;

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //--------------------------------------< GET SET >--------------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\


        #region Getter Setter

        /// <summary>
        /// The ScripableObject singleton
        /// </summary>
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

        #endregion

    }
}
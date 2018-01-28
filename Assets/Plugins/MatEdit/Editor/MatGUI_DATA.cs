using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.MatEdit
{
    public class MatGUI_DATA : ScriptableObject
    {
        [SerializeField]
        public PropertyDictionary properties = new PropertyDictionary();

    }

    [System.Serializable]
    public class PropertyDictionary : SerializableDictionary<string, bool>
    {
    }
}
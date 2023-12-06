using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using System.Linq;
using UnityEngine;

namespace Utilities.ResourceManager
{
    /// <summary>
    /// This class can be used to instantiate a resource from anywhere by adding a gameObject in the list which automatically
    /// update an enum that you can use as a parameter of the Instantiate method to get any object from the list.
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        [Header("Resources"), SerializeField] private List<GameObject> _gameObjects = new List<GameObject>();
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            string enumName = "ResourceEnum";
            string[] enumEntries = _gameObjects.Select(x => x.name).ToArray();
            string filePathAndName = "Assets/Scripts/Utilities/ResourceManager/" + enumName + ".cs";

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("public enum " + enumName);
                streamWriter.WriteLine("{");
                for (int i = 0; i < enumEntries.Length; i++)
                {
                    streamWriter.WriteLine("	" + enumEntries[i] + ",");
                }

                streamWriter.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
#endif

        protected override void InternalAwake()
        {
        }

        /// <summary>
        /// Instantiate a gameObject from the resource list.
        /// </summary>
        /// <param name="resource">The resource you want to instantiate</param>
        /// <returns>The gameObject that has been instantiated</returns>
        public GameObject InstantiateGameObject(ResourceEnum resource)
        {
            return Instantiate(_gameObjects[(int)resource]);
        }
    }
}
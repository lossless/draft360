/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using System.IO;
#endif

    /**
     * @brief Hold the total infomation of a image data base item.
     */
    [Serializable]
    public struct NRTrackingImageDatabaseEntry
    {
        /// <summary>
        /// The name assigned to the tracked image.
        /// </summary>
        public string Name;

        /// <summary>
        /// The width of the image in meters.
        /// </summary>
        public float Width;

        /// <summary>
        /// The height of the image in meters.
        /// </summary>
        public float Height;

        /// <summary>
        /// The quality of the image.
        /// </summary>
        public string Quality;

        /// <summary>
        /// The Unity GUID for this entry.
        /// </summary>
        public string TextureGUID;

        /// <summary>
        /// Contructs a new Augmented Image database entry.
        /// </summary>
        /// <param name="name">The image name.</param>
        /// <param name="width">The image width in meters or 0 if the width is unknown.</param>
        public NRTrackingImageDatabaseEntry(string name, float width, float height)
        {
            Name = name;
            TextureGUID = string.Empty;
            Width = width;
            Height = height;
            Quality = string.Empty;
            TextureGUID = string.Empty;
        }

#if UNITY_EDITOR
        /// @cond EXCLUDE_FROM_DOXYGEN
        public NRTrackingImageDatabaseEntry(string name, Texture2D texture, float width, float height)
        {
            Name = name;
            TextureGUID = string.Empty;
            Width = width;
            Quality = string.Empty;
            Height = height;
            Texture = texture;
        }

        public NRTrackingImageDatabaseEntry(string name, Texture2D texture)
        {
            Name = name;
            TextureGUID = string.Empty;
            Width = 0;
            Quality = string.Empty;
            Height = 0;
            Texture = texture;
        }

        public NRTrackingImageDatabaseEntry(Texture2D texture)
        {
            Name = "Unnamed";
            TextureGUID = string.Empty;
            Width = 0;
            Quality = string.Empty;
            Height = 0;
            Texture = texture;
        }

        public Texture2D Texture
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(TextureGUID));
            }
            set
            {
                string path = AssetDatabase.GetAssetPath(value);
                TextureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                var fileName = Path.GetFileName(path);
                Name = fileName.Replace(Path.GetExtension(fileName), string.Empty);
            }
        }

        public override string ToString()
        {
            return string.Format("Name:{0} Quality:{1}", Name, Quality);
        }
        /// @endcond
#endif
    }

}

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SpritePixelsPerUnitChanger : AssetPostprocessor {
    void OnPreprocessTexture() {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.spritePixelsPerUnit = 32;
    }
}
#endif
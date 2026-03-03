using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class GeneratePrefabsAutoMaterials
    {
        [MenuItem("Tools/Generate Prefabs with Automatic Materials")]
        public static void GeneratePrefabs()
        {
            // 1️⃣ Carpeta de Sprites
            string spritesFolder = EditorUtility.OpenFolderPanel("Selecciona la carpeta de Sprites", "Assets", "");
            if (string.IsNullOrEmpty(spritesFolder)) return;

            // 2️⃣ Carpeta de Normal Maps
            string normalsFolder = EditorUtility.OpenFolderPanel("Selecciona la carpeta de Normal Maps", "Assets", "");
            if (string.IsNullOrEmpty(normalsFolder)) return;

            // 3️⃣ Carpeta de Prefabs
            string prefabsFolder = EditorUtility.OpenFolderPanel("Selecciona la carpeta destino de Prefabs", "Assets", "");
            if (string.IsNullOrEmpty(prefabsFolder)) return;

            if (!spritesFolder.StartsWith(Application.dataPath) ||
                !normalsFolder.StartsWith(Application.dataPath) ||
                !prefabsFolder.StartsWith(Application.dataPath))
            {
                Debug.LogError("Todas las carpetas deben estar dentro de Assets.");
                return;
            }

            string spritesPath = "Assets" + spritesFolder.Substring(Application.dataPath.Length);
            string normalsPath = "Assets" + normalsFolder.Substring(Application.dataPath.Length);
            string prefabsPath = "Assets" + prefabsFolder.Substring(Application.dataPath.Length);

            // Carpeta de Materials dentro de Prefabs
            string materialsPath = Path.Combine(prefabsPath, "Materials").Replace("\\", "/");
            if (!AssetDatabase.IsValidFolder(materialsPath))
                AssetDatabase.CreateFolder(prefabsPath, "Materials");

            string[] spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { spritesPath });

            bool usingURP = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null;
            string shaderName = usingURP ? "Universal Render Pipeline/2D/Sprite-Lit-Default" : "Sprites/Default";
            Shader shader = Shader.Find(shaderName);

            if (shader == null)
            {
                Debug.LogError("No se encontró el shader: " + shaderName);
                return;
            }

            foreach (string guid in spriteGuids)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (sprite == null) continue;

                string spriteName = sprite.name;

                // Buscar normal map con mismo nombre
                string normalPath = Path.Combine(normalsPath, spriteName + ".png").Replace("\\", "/");
                Texture2D normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);

                if (normalTex == null)
                {
                    Debug.LogWarning($"No se encontró normal map para: {spriteName}");
                }

                // Buscar o crear material
                string materialPath = Path.Combine(materialsPath, spriteName + "_normal.mat").Replace("\\", "/");
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

                if (mat == null)
                {
                    mat = new Material(shader);
                    mat.name = spriteName + "_normal";

                    if (usingURP && normalTex != null)
                        mat.SetTexture("_NormalMap", normalTex);

                    AssetDatabase.CreateAsset(mat, materialPath);
                }

                // Crear GameObject
                GameObject go = new GameObject(spriteName);
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sharedMaterial = mat;

                // Guardar prefab
                string prefabPath = Path.Combine(prefabsPath, spriteName + ".prefab").Replace("\\", "/");
                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

                Object.DestroyImmediate(go);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Prefabs generados automáticamente con materiales y normal maps.");
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public class ChrunityUI : MonoBehaviour
{
    [MenuItem("Chrunity/Open Chrunity")]
    public static void OpenWindow()
    {
        ChrunityWindow window = EditorWindow.GetWindow<ChrunityWindow>();
        window.titleContent = new GUIContent("CHRUNITY");
        window.Show();
    }
}

public class ChrunityWindow : EditorWindow
{
    private string outputPath = "";

    private string globalLightType = "0";
    private string globalTrack = "";
    private string globalMaterial = "";
    
    
    private void OnGUI()
    {
        GUILayout.Label("CHRUNITY", EditorStyles.largeLabel);
        if (AddButton(new GUIContent("Find output folder")))
        {
            string path = EditorUtility.OpenFolderPanel("Select output folder", "", "");
            if (path.Length != 0) outputPath = path;
        }
        
        GUILayout.Label("Output Path: " + outputPath, EditorStyles.label);
        
        if (AddButton(new GUIContent("Export")))
        {
            if (Directory.Exists(outputPath) == false)
            {
                Debug.LogError("Output path does not exist. Please select a valid folder.");
                return;
            }
            // Serialize all ChromaObjects in the scene then write to outputPath / "chroma_objects.json"
            List<ChromaObject> chromaObjects = new List<ChromaObject>(FindObjectsOfType<ChromaObject>());
            List<SerializedChromaObject> serializedObjects = new List<SerializedChromaObject>();
            
            foreach (ChromaObject obj in chromaObjects)
            {
                
                SerializedChromaObject serialized = new SerializedChromaObject
                {
                    Name = string.IsNullOrEmpty(obj.objectNameOverride) ? obj.name : obj.objectNameOverride,
                    MeshType = obj.objectType.ToString(),
                    Material = obj.objectMaterial,
                    Transform = new SerializedTransform(obj.transform),
                    Track = obj.track,
                    LightID = obj.lightID,
                    LightType = obj.lightType
                };
                serializedObjects.Add(serialized);
            }
            
            string json = JsonConvert.SerializeObject(serializedObjects, Formatting.Indented);
            string filePath = Path.Combine(outputPath, "chroma_objects.json");
            File.WriteAllText(filePath, json);
            Debug.Log("Exported " + serializedObjects.Count + " ChromaObjects to " + filePath);
        }
        
        GUILayout.Space(20);
        
        // ----- UTILS -----
        
        GUILayout.Label("Utilities", EditorStyles.boldLabel);
        
        // TRACK SETTER
        GUILayout.Label("Set Track on selected objects", EditorStyles.label);
        
        GUILayout.BeginHorizontal();
        
        globalTrack = GUILayout.TextField(globalTrack);
        if (AddButton(new GUIContent("Set tracks", "Set " + globalTrack + " as the Track for all selected Chroma Objects")))
        {
            if (globalTrack == String.Empty)
            {
                Debug.LogWarning("Track is empty. Please enter a valid track name.");
                return;
            }
            ChromaObject[] chromaObjects = Selection.gameObjects
                .Select(go => go.GetComponent<ChromaObject>())
                .Where(obj => obj != null)
                .ToArray();
            if (chromaObjects.Length == 0) return;
            
            foreach (ChromaObject obj in chromaObjects)
            {
                obj.track = globalTrack;
            }
            
            Debug.Log("Set '" + globalTrack + "' track on " + chromaObjects.Length + " Chroma Objects.");
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        // MATERIAL SETTER
        
        GUILayout.Label("Set Material on selected objects", EditorStyles.label);
        
        GUILayout.BeginHorizontal();
        
        globalMaterial = GUILayout.TextField(globalMaterial);
        if (AddButton(new GUIContent("Set materials", "Set " + globalMaterial + " as the Material for all selected Chroma Objects")))
        {
            if (globalMaterial == String.Empty)
            {
                Debug.LogWarning("Material field is empty. Please enter a valid material name.");
                return;
            }
            ChromaObject[] chromaObjects = Selection.gameObjects
                .Select(go => go.GetComponent<ChromaObject>())
                .Where(obj => obj != null)
                .ToArray();
            if (chromaObjects.Length == 0) return;

            foreach (ChromaObject obj in chromaObjects)
            {
                obj.objectMaterial = globalMaterial;
            }
            Debug.Log("Set '" + globalMaterial + "' material on " + chromaObjects.Length + " Chroma Objects.");
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        // LIGHTTYPE SETTER
        GUILayout.Label("Set LightType on selected objects", EditorStyles.label);
        
        GUILayout.BeginHorizontal();
        
        globalLightType = GUILayout.TextField(globalLightType, GUILayout.Width(50)); // I trust you fuckers to not put some bullshit in here
        if (AddButton(new GUIContent("Set types", "Set " + globalLightType + " as the LightType for all selected Chroma Objects")))
        {
            int type = int.TryParse(globalLightType, out type) ? type : -69;
            if (type == -69)
            {
                Debug.LogError("Invalid LightType. Please enter a valid integer.");
                return;
            }
            ChromaObject[] chromaObjects = Selection.gameObjects
                .Select(go => go.GetComponent<ChromaObject>())
                .Where(obj => obj != null)
                .ToArray();
            if (chromaObjects.Length == 0) return;

            foreach (ChromaObject obj in chromaObjects)
            {
                obj.lightType = type;
            }
            Debug.Log("Set LightType " + type + " on " + chromaObjects.Length + " Chroma Objects.");
        }
        
        GUILayout.EndHorizontal();
    }
    
    public static bool AddButton(GUIContent content = null, string tooltip = null)
    {
        if (content == null) content = new GUIContent("Button");
        if (tooltip != null) content.tooltip = tooltip;
        return GUILayout.Button(content);
    }
}

public class SerializedChromaObject
{
    public string Name = "";
    public string MeshType = "Dummy";
    public string Material = "Dummy";
    public string Track = "";
    public int LightID = 0;
    public int LightType = 0;
    public SerializedTransform Transform;
}

[Serializable]
public class SerializedTransform
{
    public float[] Position;
    public float[] Rotation;
    public float[] LocalRotation;
    public float[] Scale;

    public SerializedTransform(Transform transform)
    {
        Position = new float[3] { transform.position.x, transform.position.y, transform.position.z };
        
        var rotation = transform.rotation.eulerAngles;
        Rotation = new float[3] { rotation.x, rotation.y, rotation.z };
        
        var localRotation = transform.localRotation.eulerAngles;
        LocalRotation = new float[3] { localRotation.x, localRotation.y, localRotation.z };
        
        Scale = new float[3] { transform.localScale.x, transform.localScale.y, transform.localScale.z };
    }
}
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
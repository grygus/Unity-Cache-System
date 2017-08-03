using UnityEngine;
using System.Collections;
using System.Linq;
using Grygus.Utilities.Pool.Unity;
using UnityEditor;


[CustomEditor(typeof(UnityStorage))]
public class UnityStorageInspector : Editor
{

    private UnityStorage _target;
    private GUIStyle background;
    private GUIStyle poolBackground;
    private GUIStyle _toolbarStyle;
    private GUIStyle dropBox;

    private void OnEnable()
    {
        _target = target as UnityStorage;
        background = new GUIStyle();
        poolBackground = new GUIStyle();
        dropBox = new GUIStyle();

        background.normal.background = MakeTex(new Color(0.5f, 0.5f, 0.5f, 0.5f));
        poolBackground.normal.background = MakeTex(new Color(0.3f, 0.3f, 0.3f, 0.2f));
        dropBox.normal.background = MakeTex(new Color(0.2f, 0.5f, 0.2f, 0.5f));

        poolBackground.margin = new RectOffset(2, 2, 2, 2);
        dropBox.margin = new RectOffset(4, 4, 4, 4);

        dropBox.alignment = TextAnchor.MiddleCenter;

        dropBox.fontSize = 14;

        dropBox.normal.textColor = Color.white;

    }

    public override void OnInspectorGUI()
    {
        DropArea();
        base.OnInspectorGUI();
        
    }
    private void DropArea()
    {
        GUILayout.Box("Drop prefabs here", dropBox, GUILayout.ExpandWidth(true), GUILayout.Height(35));

        EventType eventType = Event.current.type;
        bool isAccepted = false;

        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                isAccepted = true;
            }
            Event.current.Use();
        }

        if (isAccepted)
        {
            var pools = DragAndDrop.objectReferences
                .Where(obj => obj.GetType() == typeof(GameObject))
                .Cast<GameObject>()
                .Where(obj => PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab)
                .Except(_target.Pools.Select(_=>_.Prefab).ToList())
                .Select(obj => new UnityPool() {Prefab = obj,Name = obj.name});

            _target.Pools.AddRange(pools);
        }
    }
    private Texture2D MakeTex(Color col)
    {
        Color[] pix = new Color[1 * 1];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        result.hideFlags = HideFlags.HideAndDontSave;
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}


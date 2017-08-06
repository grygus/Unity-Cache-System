
using System.Collections.Generic;
using Grygus.Utilities.Pool;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityStorageLog))]
public class UnityStorageLogDrawer : Editor
{
    const int DEFAULT_FOLDOUT_MARGIN = 11;
    static Dictionary<int, GUIStyle[]> _coloredBoxStyles;

    private UnityStorageLog _target;

    private UnityStorageLogDrawer()
    {
        _coloredBoxStyles = new Dictionary<int, GUIStyle[]>();
    }
    private void OnEnable()
    {
        _target = target as UnityStorageLog;
    }

    public override void OnInspectorGUI()
    {
//        base.OnInspectorGUI();

        if (_target.TypeDic != null)
        {
            int index = 0;
            int count = _target.TypeDic.Count;
            foreach (var kvp in _target.TypeDic)
            {
                
                var boxStyle = getColoredBoxStyle(count, index);
                BeginVerticalBox(boxStyle);
                {
                    EditorGUILayout.LabelField(kvp.Key.ToString());
                    EditorGUILayout.IntField("Count", kvp.Value.Count);
                    foreach (var namedKvp in CacheLog.NamedCacheCounter[kvp.Key])
                    {
                        BeginVerticalBox(boxStyle);
                        {
                            BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(namedKvp.Key);
                                EditorGUILayout.LabelField("Count "+ namedKvp.Value);
                            }
                            EndHorizontal();
                        }
                        EndVertical();
                    }
                }
                EndVertical();
                index++;
            }
        }
    }

    public static bool Foldout(bool foldout, string content, int leftMargin = DEFAULT_FOLDOUT_MARGIN)
    {
        return Foldout(foldout, content, EditorStyles.foldout, leftMargin);
    }

    public static bool Foldout(bool foldout, string content, GUIStyle style, int leftMargin = DEFAULT_FOLDOUT_MARGIN)
    {
        BeginHorizontal();
        GUILayout.Space(leftMargin);
        foldout = EditorGUILayout.Foldout(foldout, content, style);
        EndHorizontal();
        return foldout;
    }

    public static Rect BeginVertical()
    {
        return EditorGUILayout.BeginVertical();
    }

    public static Rect BeginVerticalBox(GUIStyle style = null)
    {
        return EditorGUILayout.BeginVertical(style ?? GUI.skin.box);
    }

    public static void EndVertical()
    {
        EditorGUILayout.EndVertical();
    }

    public static Rect BeginHorizontal()
    {
        return EditorGUILayout.BeginHorizontal();
    }

    public static void EndHorizontal()
    {
        EditorGUILayout.EndHorizontal();
    }
    static GUIStyle getColoredBoxStyle(int totalComponents, int index)
    {
        GUIStyle[] styles;
        if (!_coloredBoxStyles.TryGetValue(totalComponents, out styles))
        {
            styles = new GUIStyle[totalComponents];
            for (int i = 0; i < styles.Length; i++)
            {
                var hue = (float)i / (float)totalComponents;
                var componentColor = Color.HSVToRGB(hue, 0.7f, 1f);
                componentColor.a = 0.15f;
                var style = new GUIStyle(GUI.skin.box);
                style.normal.background = createTexture(2, 2, componentColor);
                styles[i] = style;
            }
            _coloredBoxStyles.Add(totalComponents, styles);
        }

        return styles[index];
    }
    static Texture2D createTexture(int width, int height, Color color)
    {
        var pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; ++i)
        {
            pixels[i] = color;
        }
        var result = new Texture2D(width, height);
        result.SetPixels(pixels);
        result.Apply();
        return result;
    }
}

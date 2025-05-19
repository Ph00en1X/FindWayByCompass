#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IntGrid))]
public class IntGridDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        int h = prop.FindPropertyRelative("Height").intValue;
        return (h + 3) * (EditorGUIUtility.singleLineHeight + 2);
    }

    public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
    {
        var widthProp = prop.FindPropertyRelative("Width");
        var heightProp = prop.FindPropertyRelative("Height");
        var dataProp = prop.FindPropertyRelative("_data");

        EditorGUI.LabelField(Line(ref rect), label, EditorStyles.boldLabel);
        EditorGUI.PropertyField(Line(ref rect), widthProp);
        EditorGUI.PropertyField(Line(ref rect), heightProp);

        int w = Mathf.Max(1, widthProp.intValue);
        int h = Mathf.Max(1, heightProp.intValue);

        float cell = (rect.width - (w - 1) * 2) / w;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var cellRect = new Rect(rect.x + x * (cell + 2), rect.y + y * (cell + 2), cell, cell);
                int index = x + y * w;
                if (index < dataProp.arraySize)
                    EditorGUI.PropertyField(cellRect, dataProp.GetArrayElementAtIndex(index), GUIContent.none);
            }
        }
    }

    private static Rect Line(ref Rect rect)
    {
        var r = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
        rect.y += EditorGUIUtility.singleLineHeight + 2;
        return r;
    }
}
#endif

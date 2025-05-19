using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(OcclusionCulling2D))]
public class OcclusionCulling2DEditor : Editor
{
    private SerializedProperty _updateIntervalProperty;
    private SerializedProperty _globalPaddingProperty;
    private SerializedProperty _skipObjectsWithoutRendererProperty;
    private SerializedProperty _showDebugBoundsProperty;
    private SerializedProperty _debugBoundsColorProperty;

    private bool _showPerformanceSettings = true;
    private bool _showDebugSettings = true;
    private bool _showStatistics = true;

    private void OnEnable()
    {
        _updateIntervalProperty = serializedObject.FindProperty("updateInterval");
        _globalPaddingProperty = serializedObject.FindProperty("globalPadding");
        _skipObjectsWithoutRendererProperty = serializedObject.FindProperty("skipObjectsWithoutRenderer");
        _showDebugBoundsProperty = serializedObject.FindProperty("showDebugBounds");
        _debugBoundsColorProperty = serializedObject.FindProperty("debugBoundsColor");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        OcclusionCulling2D culling = (OcclusionCulling2D)target;

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Occlusion Culling 2D", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Отключает объекты вне поля зрения камеры", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        _showPerformanceSettings = EditorGUILayout.Foldout(_showPerformanceSettings, "Настройки производительности", true);
        if (_showPerformanceSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_updateIntervalProperty, new GUIContent("Интервал обновления", "Как часто обновлять видимость объектов (в секундах)"));
            EditorGUILayout.PropertyField(_globalPaddingProperty, new GUIContent("Глобальный отступ", "Дополнительное расстояние вокруг объектов"));
            EditorGUILayout.PropertyField(_skipObjectsWithoutRendererProperty, new GUIContent("Пропускать без рендерера", "Не обрабатывать объекты без компонента Renderer"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        _showDebugSettings = EditorGUILayout.Foldout(_showDebugSettings, "Настройки отладки", true);
        if (_showDebugSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_showDebugBoundsProperty, new GUIContent("Показать границы", "Отображать границы видимости объектов"));

            if (_showDebugBoundsProperty.boolValue)
            {
                EditorGUILayout.PropertyField(_debugBoundsColorProperty, new GUIContent("Цвет границ", "Цвет для отображения границ объектов"));

                if (GUILayout.Button("Обновить визуализацию"))
                {
                    SceneView.RepaintAll();
                }
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        if (Application.isPlaying)
        {
            _showStatistics = EditorGUILayout.Foldout(_showStatistics, "Статистика", true);
            if (_showStatistics)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Зарегистрировано объектов:", culling.GetRegisteredObjectsCount().ToString());
                EditorGUI.indentLevel--;

                if (GUILayout.Button("Принудительно обновить"))
                {
                    culling.ForceUpdate();
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
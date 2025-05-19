using UnityEngine;
using System.Collections.Generic;

public class OcclusionCulling2D : MonoBehaviour
{
    [Header("Performance Settings")]
    [Range(0.01f, 1.0f)]
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private Vector2 globalPadding = new Vector2(1f, 1f);
    [SerializeField] private bool skipObjectsWithoutRenderer = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugBounds = false;
    [SerializeField] private Color debugBoundsColor = new Color(0, 1, 0, 0.5f);

    private Camera _mainCamera;
    private float _timer;
    private Rect _cameraViewRect = new Rect();
    private readonly List<CullingObject> _cullingObjects = new List<CullingObject>();
    private readonly Dictionary<GameObject, CullingObject> _objectLookup = new Dictionary<GameObject, CullingObject>();

    private class CullingObject
    {
        public GameObject GameObject;
        public Bounds Bounds;
        public Vector2 Padding;
        public bool HasRenderer;
        public bool WasVisible;
    }

    private void Awake()
    {
        _mainCamera = GetComponent<Camera>() ?? Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("OcclusionCulling2D requires a camera");
            enabled = false;
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < updateInterval) return;

        _timer = 0f;
        UpdateCameraViewRect();
        ProcessVisibility();
    }

    private void UpdateCameraViewRect()
    {
        if (_mainCamera.orthographic)
        {
            float height = _mainCamera.orthographicSize * 2f * 1.1f;
            float width = height * _mainCamera.aspect * 1.05f;

            _cameraViewRect.x = _mainCamera.transform.position.x - width * 0.5f;
            _cameraViewRect.y = _mainCamera.transform.position.y - height * 0.5f;
            _cameraViewRect.width = width;
            _cameraViewRect.height = height;
        }
        else
        {
            float distance = 10f;
            float height = 2f * distance * Mathf.Tan(_mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float width = height * _mainCamera.aspect;

            _cameraViewRect.x = _mainCamera.transform.position.x - width * 0.5f;
            _cameraViewRect.y = _mainCamera.transform.position.y - height * 0.5f;
            _cameraViewRect.width = width;
            _cameraViewRect.height = height;
        }
    }

    private void ProcessVisibility()
    {
        for (int i = _cullingObjects.Count - 1; i >= 0; i--)
        {
            CullingObject obj = _cullingObjects[i];

            if (obj.GameObject == null)
            {
                _cullingObjects.RemoveAt(i);
                continue;
            }

            UpdateObjectBounds(obj);
            bool isVisible = IsObjectInView(obj.Bounds);

            if (isVisible != obj.WasVisible)
            {
                obj.GameObject.SetActive(isVisible);
                obj.WasVisible = isVisible;
            }
        }
    }

    public void RegisterObject(GameObject obj, Vector2 padding = default)
    {
        if (obj == null || _objectLookup.ContainsKey(obj)) return;

        bool hasRenderer = obj.TryGetComponent<Renderer>(out _);
        if (skipObjectsWithoutRenderer && !hasRenderer) return;

        CullingObject cullingObj = new CullingObject
        {
            GameObject = obj,
            Padding = padding + globalPadding,
            HasRenderer = hasRenderer,
            WasVisible = obj.activeSelf
        };

        UpdateObjectBounds(cullingObj);
        _cullingObjects.Add(cullingObj);
        _objectLookup.Add(obj, cullingObj);
    }

    public void UnregisterObject(GameObject obj)
    {
        if (obj == null) return;

        if (_objectLookup.TryGetValue(obj, out CullingObject cullingObj))
        {
            _cullingObjects.Remove(cullingObj);
            _objectLookup.Remove(obj);
            obj.SetActive(true);
        }
    }

    private void UpdateObjectBounds(CullingObject obj)
    {
        if (obj.HasRenderer)
        {
            if (obj.GameObject.TryGetComponent<Renderer>(out var renderer))
            {
                obj.Bounds = renderer.bounds;
            }
            else
            {
                obj.HasRenderer = false;
                obj.Bounds = new Bounds(obj.GameObject.transform.position, Vector3.one);
            }
        }
        else
        {
            obj.Bounds = new Bounds(obj.GameObject.transform.position, Vector3.one);
        }

        obj.Bounds.Expand(new Vector3(obj.Padding.x * 2, obj.Padding.y * 2, 0));
    }

    private bool IsObjectInView(Bounds objectBounds)
    {
        return objectBounds.max.x >= _cameraViewRect.x &&
               objectBounds.min.x <= _cameraViewRect.x + _cameraViewRect.width &&
               objectBounds.max.y >= _cameraViewRect.y &&
               objectBounds.min.y <= _cameraViewRect.y + _cameraViewRect.height;
    }

    public void ForceUpdate()
    {
        _timer = updateInterval;
    }

    public bool IsObjectRegistered(GameObject obj)
    {
        return obj != null && _objectLookup.ContainsKey(obj);
    }

    public int GetRegisteredObjectsCount()
    {
        return _cullingObjects.Count;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugBounds || !Application.isPlaying) return;

        Gizmos.color = debugBoundsColor;

        Vector3 cameraCenter = new Vector3(
            _cameraViewRect.x + _cameraViewRect.width * 0.5f,
            _cameraViewRect.y + _cameraViewRect.height * 0.5f,
            0);
        Vector3 cameraSize = new Vector3(_cameraViewRect.width, _cameraViewRect.height, 0.1f);
        Gizmos.DrawWireCube(cameraCenter, cameraSize);

        foreach (CullingObject obj in _cullingObjects)
        {
            if (obj.GameObject != null && obj.GameObject.activeSelf)
            {
                Gizmos.DrawWireCube(obj.Bounds.center, obj.Bounds.size);
            }
        }
    }
}

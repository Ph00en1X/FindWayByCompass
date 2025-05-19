using System.Collections.Generic;
using UnityEngine;

public sealed class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _prefabs;
    [SerializeField] private OcclusionCulling2D _culling;

    private GameController _controller;
    private readonly Dictionary<int, Queue<GameObject>> _pool = new();

    public void Build(LevelData data)
    {
        _controller = FindAnyObjectByType<GameController>();
        ReturnAll();
        var g = data.Grid;

        for (var x = 0; x < g.Width; x++)
            for (var y = 0; y < g.Height; y++)
            {
                var id = g[x, y];
                if (id < 0 || id >= _prefabs.Count) continue;

                var pos = new Vector3(-g.Width / 2f + x, g.Height / 2f - y);

                if (id > 2)
                {
                    var sgo = Get(id, pos);
                    if (id == 4) _controller.RegisterPlayer(sgo);
                    id = 0;
                }
                var go = Get(id, pos);
            }
    }

    private GameObject Get(int id, Vector3 pos)
    {
        if (!_pool.TryGetValue(id, out var q)) _pool[id] = q = new Queue<GameObject>();

        GameObject go;
        if (q.Count > 0)
        {
            go = q.Dequeue();
            go.transform.position = pos;
            go.SetActive(true);
        }
        else
        {
            go = Instantiate(_prefabs[id], pos, Quaternion.identity, transform);
        }

        _culling.RegisterObject(go);
        return go;
    }

    private void ReturnAll()
    {
        foreach (Transform child in transform)
        {
            var go = child.gameObject;
            var id = _prefabs.IndexOf(go.GetComponent<SpriteRenderer>() ? go : null);
            if (id < 0) Destroy(go);
            else
            {
                _culling.UnregisterObject(go);
                go.SetActive(false);
                if (!_pool.ContainsKey(id)) _pool[id] = new Queue<GameObject>();
                _pool[id].Enqueue(go);
            }
        }
    }
}

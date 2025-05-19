using System;
using UnityEngine;

[Serializable]
public sealed class IntGrid : ISerializationCallbackReceiver
{
    [Min(1)] public int Width = 1;
    [Min(1)] public int Height = 1;

    [SerializeField, HideInInspector] private int[] _data;

    public int this[int x, int y]
    {
        get => _data[x + y * Width];
        set => _data[x + y * Width] = value;
    }

    public void OnBeforeSerialize() => ResizeIfNeeded();
    public void OnAfterDeserialize() => ResizeIfNeeded();

    private void ResizeIfNeeded()
    {
        var size = Width * Height;
        if (_data == null || _data.Length != size)
            Array.Resize(ref _data, size);
    }
}

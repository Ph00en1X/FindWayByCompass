using System;

[Serializable]
public sealed class LevelData
{
    public int Index;
    public IntGrid Grid;
    public int Width => Grid.Width;
    public int Height => Grid.Height;
}

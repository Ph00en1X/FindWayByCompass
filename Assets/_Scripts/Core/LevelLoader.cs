using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Core
{
    public sealed class LevelLoader : MonoBehaviour
    {
        [SerializeField] private TextAsset _jsonAsset;
        [SerializeField] private string _fileName = "maps.json";

        public IReadOnlyList<LevelData> Levels { get; private set; }

        public void Initialize()
        {
            var json = _jsonAsset ? _jsonAsset.text : ReadFile();
            Levels = Parse(json);
        }

        private string ReadFile()
        {
            var path = $"{Application.streamingAssetsPath}/{_fileName}";
            return File.Exists(path) ? File.ReadAllText(path) : throw new FileNotFoundException(path);
        }

        private static LevelData[] Parse(string json) =>
            JsonConvert.DeserializeObject<List<MapDto>>(json)
                .Select(ToLevelData)
                .ToArray();

        private static LevelData ToLevelData(MapDto dto)
        {
            var grid = new IntGrid { Width = dto.width, Height = dto.height };
            grid.OnBeforeSerialize();

            for (var y = 0; y < dto.height; y++)
                for (var x = 0; x < dto.width; x++)
                    grid[x, y] = dto.map[y][x];

            return new LevelData { Index = dto.index, Grid = grid };
        }

        [Serializable]
        private struct MapDto
        {
            public int index;
            public int width;
            public int height;
            public int[][] map;
        }
    }
}

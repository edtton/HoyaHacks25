using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;

    private Tile _fireSpawnTile; // Track the tile with the FireSpawn

    private Dictionary<Vector2, Tile> _tiles;

    public Vector2 gridOffset = new Vector2(0, 0);

    private TileType _selectedTileType = TileType.Soil;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x + gridOffset.x, y + gridOffset.y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";


                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public void SelectTileType(TileType type)
    {
        _selectedTileType = type;
        Debug.Log($"Tile type selected: {_selectedTileType}");
    }

    public TileType GetSelectedTileType()
    {
        return _selectedTileType;
    }

    public void SetFireSpawn(Tile tile)
    {
        // Remove the FireSpawn from the previous tile if one exists
        if (_fireSpawnTile != null)
        {
            _fireSpawnTile.RemoveFireSpawn();
        }

        // Set the new FireSpawn tile
        _fireSpawnTile = tile;
        _fireSpawnTile.AddFireSpawn();
    }

    public bool HasFireSpawn()
    {
        return _fireSpawnTile != null;
    }
}
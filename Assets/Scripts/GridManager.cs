using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;

    [SerializeField] private float windSpeed = 8f; // 0 to 20 mph
    [SerializeField] private string windDirection = "W"; // N, E, S, W, NE, NW, SE, SW
    [SerializeField] private float temperature = 70f; // Fahrenheit (32-100 degrees)
    [SerializeField] private float humidity = 30f; // Percentage (0-50%)

    private Tile _fireSpawnTile; // Track the tile with the FireSpawn

    private Dictionary<Vector2, Tile> _tiles;

    public Vector2 gridOffset = new Vector2(0, 0);

    private TileType _selectedTileType = TileType.Soil;

    private Dictionary<string, Vector2> windOffsets = new Dictionary<string, Vector2>
    {
        { "N", new Vector2(0, 1) },
        { "S", new Vector2(0, -1) },
        { "E", new Vector2(1, 0) },
        { "W", new Vector2(-1, 0) },
        { "NE", new Vector2(1, 1) },
        { "NW", new Vector2(-1, 1) },
        { "SE", new Vector2(1, -1) },
        { "SW", new Vector2(-1, -1) }
    };

    private bool _simulationRunning; // Track whether the simulation is active

    private List<Tile> _burningTiles = new List<Tile>();


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

                spawnedTile.SetTileType(TileType.Soil);

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        Vector2Int tilePosition = ParseTileName(tile.name);

        // Define neighbor offsets
        Vector2Int[] offsets = new Vector2Int[]
        {
        new Vector2Int(0, 1),  // Above
        new Vector2Int(0, -1), // Below
        new Vector2Int(1, 0),  // Right
        new Vector2Int(-1, 0), // Left
        new Vector2Int(1, 1),  // Top-right
        new Vector2Int(1, -1), // Bottom-right
        new Vector2Int(-1, 1), // Top-left
        new Vector2Int(-1, -1) // Bottom-left
        };

        foreach (Vector2Int offset in offsets)
        {
            Vector2Int neighborPosition = tilePosition + offset;
            string neighborName = $"Tile {neighborPosition.x} {neighborPosition.y}";

            // Check if the neighbor exists in the grid
            if (_tiles.TryGetValue(neighborPosition, out Tile neighbor))
            {
                neighbors.Add(neighbor);
                Debug.Log($"Neighbor found: {neighbor.name}");
            }
            else
            {
                Debug.Log($"No neighbor at {neighborName}");
            }
        }

        return neighbors;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public void SelectTileType(TileType type)
    {
        _selectedTileType = type;
    }

    public TileType GetSelectedTileType()
    {
        return _selectedTileType;
    }

    public void StartFireSimulation()
    {
        if (!_simulationRunning)
        {
            if (_fireSpawnTile != null)
            {
                _simulationRunning = true;
                Debug.Log($"Starting fire simulation at {_fireSpawnTile.name}");
                int burnSteps = CalculateBurnSteps(_fireSpawnTile); // Calculate burn steps based on terrain
                _fireSpawnTile.Ignite(burnSteps);
                _burningTiles.Add(_fireSpawnTile);
                Debug.Log($"Fire started at {_fireSpawnTile.name}");
                StartCoroutine(SimulationStep());
            }
            else
            {
                Debug.LogWarning("No fire spawn point set. Please set one before starting the simulation.");
            }
        }
    }

    public void StopFireSimulation()
    {
        _simulationRunning = false;
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

    private float CalculateBurnProbability(Tile sourceTile, Tile targetTile)
    {
        // Skip non-flammable tiles
        if (targetTile.FuelLoad == 0f) return 0f;

        // Get distance between tiles (simple grid distance or diagonal adjustment)
        Vector2 direction = targetTile.transform.position - sourceTile.transform.position;
        float distance = direction.magnitude;

        // Calculate the influence of wind
        Vector2 windVector = windOffsets[windDirection];
        float windInfluence = Vector2.Dot(windVector.normalized, direction.normalized);

        // Base probability factors
        float baseProbability = targetTile.FuelLoad * temperature * 0.01f; // Normalize temperature influence
        float adjustedProbability = baseProbability * Mathf.Max(0.5f, 1 + windInfluence * windSpeed * 0.05f); // Add wind effect
        float humidityEffect = Mathf.Clamp01(1 - humidity * 0.02f); // Humidity reduces flammability

        return Mathf.Clamp01(adjustedProbability * humidityEffect / Mathf.Max(1f, distance));
    }

    private IEnumerator SimulationStep()
    {
        while (_simulationRunning)
        {
            // Process each burning tile
            for (int i = _burningTiles.Count - 1; i >= 0; i--)
            {
                _burningTiles[i].ProcessBurningStep();

                if (_burningTiles[i].BurnStepsRemaining == 0)
                {
                    _burningTiles.RemoveAt(i); // Remove burned tiles
                }
                else
                {
                    SpreadFire(_burningTiles[i]); // Check neighbors and spread fire
                }
            }

            // End simulation if no burning tiles remain
            if (_burningTiles.Count == 0)
            {
                _simulationRunning = false;
                Debug.Log("Simulation complete. All tiles have burned out.");
            }

            yield return new WaitForSeconds(1f); // Wait 1 second between steps
        }
    }


    private int CalculateBurnSteps(Tile tile)
    {
        return tile.BedDepth * 2; // Example: Each unit of BedDepth corresponds to 2 steps
    }

    private void SpreadFire(Tile burningTile)
    {
        foreach (Tile neighbor in GetNeighbors(burningTile))
        {
            if (!neighbor.IsBurning && !neighbor.IsBurned) // Skip already burning or burned tiles
            {
                float probability = CalculateBurnProbability(burningTile, neighbor);

                if (UnityEngine.Random.value < probability) // Fire spreads based on probability
                {
                    int burnSteps = CalculateBurnSteps(neighbor);
                    neighbor.Ignite(burnSteps); // Ignite the neighbor
                    _burningTiles.Add(neighbor); // Add it to the burning list
                    Debug.Log($"{neighbor.name} has started burning with a probability of {probability}.");
                }
            }
        }
    }

    private Vector2Int ParseTileName(string tileName)
    {
        // Extract x and y from the tile name (e.g., "Tile 0 1")
        string[] parts = tileName.Split(' ');
        if (parts.Length == 3 && parts[0] == "Tile")
        {
            int x = int.Parse(parts[1]);
            int y = int.Parse(parts[2]);
            return new Vector2Int(x, y);
        }

        throw new Exception($"Invalid tile name: {tileName}");
    }


}
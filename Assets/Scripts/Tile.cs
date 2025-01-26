using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Soil,
    Asphalt,
    Water,
    WoodStruct,
    StoneStruct,
    Grass,
    Shrub,
    Tree,
    FireSpawn
}

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _fireSpawnMarker;
    [SerializeField] private GameObject _burningOverlay; // Assign a red overlay in the prefab
    [SerializeField] private GameObject _burnedOverlay; // Assign a red overlay in the prefab

    [SerializeField] private Sprite _soilSprite;
    [SerializeField] private Sprite _asphaltSprite;
    [SerializeField] private Sprite _waterSprite;
    [SerializeField] private Sprite _woodStructSprite;
    [SerializeField] private Sprite _stoneStructSprite;
    [SerializeField] private Sprite _grassSprite;
    [SerializeField] private Sprite _shrubSprite;
    [SerializeField] private Sprite _treeSprite;

    public float FuelLoad { get; private set; } // Fuel load for the tile
    public int BedDepth { get; private set; }  // Bed depth for the tile

    private int _burnStepsRemaining; // Tracks the number of burning steps

    private TileType _currentType;

    private void Start()
    {
        _currentType = TileType.Soil;
        _renderer.sprite = _soilSprite;

        if (_burningOverlay != null)
        {
            _burningOverlay.SetActive(false); // Burning overlay starts inactive
        }
        if (_burnedOverlay != null)
        {
            _burnedOverlay.SetActive(false); // Burned overlay starts inactive
        }
    }

    public void SetTileType(TileType type, Sprite sprite)
    {
        _currentType = type;
        _renderer.sprite = sprite;
    }

    public TileType GetTileType()
    {
        // Return the current tile type
        return _currentType;
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    public void SetTileType(TileType type)
    {
        _currentType = type;

        switch (type)
        {
            case TileType.Water:
                FuelLoad = 0.0f;
                BedDepth = 0;
                _renderer.sprite = _waterSprite;
                break;
            case TileType.Soil:
                FuelLoad = 0.3f;
                BedDepth = UnityEngine.Random.Range(2, 4); // Random between 2 and 3 inclusive
                _renderer.sprite = _soilSprite;
                break;
            case TileType.Asphalt:
                FuelLoad = 0.0f;
                BedDepth = 0;
                _renderer.sprite = _asphaltSprite;
                break;
            case TileType.WoodStruct:
                FuelLoad = 0.6f;
                BedDepth = UnityEngine.Random.Range(4, 6); // Random between 4 and 5 inclusive
                _renderer.sprite = _woodStructSprite;
                break;
            case TileType.StoneStruct:
                FuelLoad = 0.2f;
                BedDepth = UnityEngine.Random.Range(5, 7); // Random between 5 and 6 inclusive
                _renderer.sprite = _stoneStructSprite;
                break;
            case TileType.Grass:
                FuelLoad = 0.7f;
                BedDepth = UnityEngine.Random.Range(2, 4);
                _renderer.sprite = _grassSprite;
                break;
            case TileType.Shrub:
                FuelLoad = 0.8f;
                BedDepth = UnityEngine.Random.Range(2, 4);
                _renderer.sprite = _shrubSprite;
                break;
            case TileType.Tree:
                FuelLoad = 0.9f;
                BedDepth = UnityEngine.Random.Range(3, 5);
                _renderer.sprite = _treeSprite;
                break;
        }
    }

    public void AddFireSpawn()
    {
        if (_fireSpawnMarker != null)
        {
            _fireSpawnMarker.SetActive(true); // Show the FireSpawn marker
        }
    }

    public void RemoveFireSpawn()
    {
        if (_fireSpawnMarker != null)
        {
            _fireSpawnMarker.SetActive(false); // Hide the FireSpawn marker
        }
    }

    private void OnMouseDown()
    {
        var gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            TileType selectedType = gridManager.GetSelectedTileType();
            // If the currently selected object is FireSpawn, place it on this tile
            if (selectedType == TileType.FireSpawn)
            {
                gridManager.SetFireSpawn(this);
            }
            else if (selectedType != _currentType)
            {
                SetTileType(selectedType);
            }
        }
    }

    public void Ignite(int burnSteps)
    {
        if (_burningOverlay != null)
        {
            _burningOverlay.SetActive(true); // Activate the burning overlay
        }

        _burnStepsRemaining = burnSteps; // Set the burn duration in steps
        Debug.Log($"{name} is now burning for {burnSteps} steps.");
    }

    public void ProcessBurningStep()
    {
        if (_burnStepsRemaining > 0)
        {
            _burnStepsRemaining--;

            if (_burnStepsRemaining == 0)
            {
                TransitionToBurned();
            }
        }
    }

    private void TransitionToBurned()
    {
        if (_burningOverlay != null)
        {
            _burningOverlay.SetActive(false); // Deactivate the burning overlay
        }
        if (_burnedOverlay != null)
        {
            _burnedOverlay.SetActive(true); // Activate the burned overlay
        }

        Debug.Log($"{name} has transitioned to burned.");
    }

    public int BurnStepsRemaining => _burnStepsRemaining;
    public bool IsBurning => _burningOverlay != null && _burningOverlay.activeSelf;
    public bool IsBurned => _burnedOverlay != null && _burnedOverlay.activeSelf;


}

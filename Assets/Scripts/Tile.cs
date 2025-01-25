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

    [SerializeField] private Sprite _soilSprite;
    [SerializeField] private Sprite _asphaltSprite;
    [SerializeField] private Sprite _waterSprite;
    [SerializeField] private Sprite _woodStructSprite;
    [SerializeField] private Sprite _stoneStructSprite;
    [SerializeField] private Sprite _grassSprite;
    [SerializeField] private Sprite _shrubSprite;
    [SerializeField] private Sprite _treeSprite;

    private TileType _currentType;

    private void Start()
    {
        _currentType = TileType.Soil;
        _renderer.sprite = _soilSprite;
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

        // Update the sprite based on the selected type
        switch (type)
        {
            case TileType.Soil:
                _renderer.sprite = _soilSprite;
                break;
            case TileType.Asphalt:
                _renderer.sprite = _asphaltSprite;
                break;
            case TileType.Tree:
                _renderer.sprite = _treeSprite;
                break;
            case TileType.Shrub:
                _renderer.sprite = _shrubSprite;
                break;
            case TileType.Grass:
                _renderer.sprite = _grassSprite;
                break;
            case TileType.Water:
                _renderer.sprite = _waterSprite;
                break;
            case TileType.WoodStruct:
                _renderer.sprite = _woodStructSprite;
                break;
            case TileType.StoneStruct:
                _renderer.sprite = _stoneStructSprite;
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
}

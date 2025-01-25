using UnityEngine;

public class TileButton : MonoBehaviour
{
    [SerializeField] private TileType _tileType; // The type of tile this button represents
    [SerializeField] private Color _hoverColor = Color.gray; // Color for hover
    [SerializeField] private Color _selectedColor = Color.green; // Color for selection
    [SerializeField] private Color _fireSpawnSelectedColor = Color.red; // Special selected color for FireSpawn
    [SerializeField] private Color _defaultColor = Color.white; // Default color

    private SpriteRenderer _renderer;
    private static TileButton _currentlySelectedButton;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.color = _defaultColor;
    }

    private void OnMouseEnter()
    {
        if (_currentlySelectedButton != this)
        {
            _renderer.color = _hoverColor; // Change to hover color when the mouse is over
        }
    }

    private void OnMouseExit()
    {
        if (_currentlySelectedButton != this)
        {
            _renderer.color = _defaultColor; // Revert to default color when the mouse leaves
        }
    }

    private void OnMouseDown()
    {
        // Deselect the previously selected button
        if (_currentlySelectedButton != null)
        {
            _currentlySelectedButton._renderer.color = _defaultColor;
        }

        // Select this button
        _currentlySelectedButton = this;

        // Use red for FireSpawn, green for others
        if (_tileType == TileType.FireSpawn)
        {
            _renderer.color = _fireSpawnSelectedColor;
        }
        else
        {
            _renderer.color = _selectedColor;
        }

        // Notify the GridManager of the selected tile type
        var gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            gridManager.SelectTileType(_tileType);
            Debug.Log($"Selected Tile: {_tileType}");
        }
    }
}

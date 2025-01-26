using UnityEngine;

public class PlayPauseButton : MonoBehaviour
{
    [SerializeField] private Sprite _playSprite; // Assign PlayButton.png in Inspector
    [SerializeField] private Sprite _pauseSprite; // Assign PauseButton.png in Inspector

    [SerializeField] private Color _defaultColor = Color.white; // Default button color
    [SerializeField] private Color _hoverColor = Color.gray; // Hover color

    private SpriteRenderer _renderer;
    private bool _isPlaying = false;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = _playSprite; // Start with the play sprite
        _renderer.color = _defaultColor; // Set default color
    }

    private void OnMouseDown()
    {
        var gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null) return;

        // Check if the fire spawn point has been set
        if (!gridManager.HasFireSpawn())
        {
            Debug.LogWarning("Please set the fire spawn point before starting the simulation.");
            return;
        }

        // Toggle between Play and Pause
        _isPlaying = !_isPlaying;

        if (_isPlaying)
        {
            StartSimulation();
        }
        else
        {
            PauseSimulation();
        }
    }

    // private void StartSimulation()
    // {
    //     Debug.Log("Simulation started!");
    //     _renderer.sprite = _pauseSprite; // Switch to pause button
    //     // Add logic to trigger fire propagation here
    // }

    private void StartSimulation()
    {
        Debug.Log("Simulation started!");
        _renderer.sprite = _pauseSprite; // Switch to pause button

        var gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            gridManager.StartFireSimulation();
        }
    }

    // private void PauseSimulation()
    // {
    //     Debug.Log("Simulation paused!");
    //     _renderer.sprite = _playSprite; // Switch back to play button
    //     // Add logic to pause fire propagation here
    // }

    private void PauseSimulation()
    {
        Debug.Log("Simulation paused!");
        _renderer.sprite = _playSprite; // Switch back to play button

        var gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            gridManager.StopFireSimulation();
        }
    }

    private void OnMouseEnter()
    {
        // Change the color to indicate hover
        _renderer.color = _hoverColor;
    }

    private void OnMouseExit()
    {
        // Revert to the default color when not hovering
        _renderer.color = _defaultColor;
    }
}

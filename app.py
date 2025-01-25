import numpy as np
import matplotlib.pyplot as plt
import random

# Define constants
GRID_SIZE = 10
STEPS = 10

# Define cell types
CELL_TYPES = {
    "burned": {"color": "black", "burn_probability": 0.01, "burn_time": 0},
    "water": {"color": "blue", "burn_probability": 0.0, "burn_time": 0},
    "firebreak": {"color": "gray", "burn_probability": 0.0, "burn_time": 0},
    "dead_brush": {"color": "saddlebrown", "burn_probability": 0.5, "burn_time": 2},
    "live_brush": {"color": "lightgreen", "burn_probability": 0.4, "burn_time": 4},
}

# Initialize grid with random types
def initialize_grid():
    cell_types = list(CELL_TYPES.keys())
    return np.random.choice(cell_types, size=(GRID_SIZE, GRID_SIZE), p=[0.1, 0.1, 0.1, 0.35, 0.35])

# Function to visualize the grid
def plot_grid(grid, burning_grid):
    # Create a colored grid
    color_grid = np.zeros((GRID_SIZE, GRID_SIZE), dtype=object)
    for i in range(GRID_SIZE):
        for j in range(GRID_SIZE):
            cell = grid[i, j]
            if burning_grid[i, j]:  # If the cell is burning, make it red
                color_grid[i, j] = "red"
            else:
                color_grid[i, j] = CELL_TYPES[cell]["color"]
    # Plot the grid
    fig, ax = plt.subplots(figsize=(6, 6))
    ax.imshow([[color for color in row] for row in color_grid], interpolation='nearest')
    ax.set_xticks(np.arange(-0.5, GRID_SIZE, 1), minor=True)
    ax.set_yticks(np.arange(-0.5, GRID_SIZE, 1), minor=True)
    ax.grid(which="minor", color="black", linestyle='-', linewidth=0.5)
    plt.show()

# Initialize the grid
grid = initialize_grid()

# Burn time tracker for each cell
burn_time_grid = np.zeros((GRID_SIZE, GRID_SIZE), dtype=int)

# Burning state tracker
burning_grid = np.zeros((GRID_SIZE, GRID_SIZE), dtype=bool)

# Function to simulate one step
def simulate_step(grid, burning_grid, burn_time_grid):
    new_burning_grid = np.zeros_like(burning_grid, dtype=bool)
    for i in range(GRID_SIZE):
        for j in range(GRID_SIZE):
            # Skip if the cell is water or firebreak
            if grid[i, j] in ["water", "firebreak"]:
                continue
            # If the cell is currently burning
            if burning_grid[i, j]:
                burn_time_grid[i, j] -= 1
                if burn_time_grid[i, j] <= 0:  # If burn time is over
                    grid[i, j] = "burned"
            else:
                # If the cell is not burning, check neighbors for fire spread
                neighbors = [
                    (i - 1, j), (i + 1, j), (i, j - 1), (i, j + 1)
                ]
                for ni, nj in neighbors:
                    if 0 <= ni < GRID_SIZE and 0 <= nj < GRID_SIZE:
                        if burning_grid[ni, nj]:
                            if random.random() < CELL_TYPES[grid[i, j]]["burn_probability"]:
                                new_burning_grid[i, j] = True
                                burn_time_grid[i, j] = CELL_TYPES[grid[i, j]]["burn_time"]
                                break
    # Update burning grid
    burning_grid |= new_burning_grid

# Set an initial fire point
initial_fire_point = (GRID_SIZE // 2, GRID_SIZE // 2)
burning_grid[initial_fire_point] = True
burn_time_grid[initial_fire_point] = CELL_TYPES[grid[initial_fire_point]]["burn_time"]

# Run the simulation
for step in range(STEPS):
    print(f"Step {step + 1}")
    plot_grid(grid, burning_grid)
    simulate_step(grid, burning_grid, burn_time_grid)

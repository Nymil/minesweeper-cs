using System;

namespace Minesweeper.Domain;

public class Cell
{
    public CellPosition Position { get; }
    public bool IsMine { get; private set; }
    public int AdjacentMines { get; private set; }
    public bool IsRevealed { get; private set; }
    public bool IsFlagged { get; private set; }

    public Cell(CellPosition position, bool isMine = false, int adjacentMines = 0)
    {
        Position = position;
        IsMine = isMine;
        AdjacentMines = adjacentMines;
        IsRevealed = false;
    }

    public void PlaceMine() => IsMine = true;

    public void Reveal() => IsRevealed = true;

    public void ToggleFlag() => IsFlagged = !IsFlagged;

    public void IncrementAdjacentMines() => AdjacentMines++;
}

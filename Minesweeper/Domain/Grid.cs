using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace Minesweeper.Domain;

public class Grid
{
    private readonly int _rows;
    private readonly int _cols;
    private readonly int _size;
    private bool _minesPlaced;
    private Cell[,] _cells;
    private int _revealedCount;

    public int MineCount { get; private set; }
    public GameState State { get; private set; }

    public Cell this[int row, int col]
    {
        get
        {
            ValidatePosition(row, col);
            return _cells[row, col];
        }
        set
        {
            ValidatePosition(row, col);
            _cells[row, col] = value;
        }
    }

    public Cell this[CellPosition pos]
    {
        get
        {
            ValidatePosition(pos);
            return _cells[pos.Row, pos.Col];
        }
        set
        {
            ValidatePosition(pos);
            _cells[pos.Row, pos.Col] = value;
        }
    }

    public void Reveal(CellPosition pos)
    {
        ValidatePosition(pos);

        if (State != GameState.Playing)
        {
            return;
        }

        if (!_minesPlaced)
        {
            PlaceRandomMines(pos);
            _minesPlaced = true;
        }

        Cell clickedCell = this[pos];
        if (clickedCell.IsFlagged || clickedCell.IsRevealed)
        {
            return;
        }

        if (clickedCell.IsMine)
        {
            clickedCell.Reveal();
            RevealAllMines();
            State = GameState.Lost;
            return;
        }

        if (clickedCell.AdjacentMines > 0)
        {
            clickedCell.Reveal();
            _revealedCount++;
            CheckVictory();
            return;
        }

        // flood reveal
        int newlyRevealed = FloodReveal(pos);
        _revealedCount += newlyRevealed;
        CheckVictory();
    }

    private int FloodReveal(CellPosition start)
    {
        Queue<CellPosition> queue = new();
        HashSet<CellPosition> visited = new();
        int newlyRevealed = 0;

        visited.Add(start);
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            CellPosition current = queue.Dequeue();
            Cell cell = this[current];

            if (cell.IsFlagged || cell.IsRevealed)
            {
                continue;
            }

            cell.Reveal();
            newlyRevealed++;

            if (cell.AdjacentMines > 0)
            {
                continue;
            }

            foreach (CellPosition neighbour in current.GetNeighborPositions())
            {
                if (visited.Contains(neighbour))
                {
                    continue;
                }

                Cell neighbourCell = this[neighbour];

                if (neighbourCell.IsMine)
                {
                    continue;
                }

                visited.Add(neighbour);
                queue.Enqueue(neighbour);
            }
        }
        
        return newlyRevealed;
    }

    private void CheckVictory()
    {
        int totalToReveal = _rows * _cols - MineCount;
        if (_revealedCount >= totalToReveal)
        {
            State = GameState.Won;
            RevealAllMines();
        }
    }

    private void RevealAllMines()
    {
        foreach (Cell cell in _cells)
        {
            if (cell.IsMine)
            {
                cell.Reveal();
            }
        }
    }

    public void ToggleFlag(CellPosition pos)
    {
        if (State != GameState.Playing)
        {
            return;
        }

        ValidatePosition(pos);
        Cell clickedCell = this[pos];
        if (!clickedCell.IsRevealed)
        {
            clickedCell.ToggleFlag();
        }
    }

    public Grid()
    {
        _rows = Settings.Rows;
        _cols = Settings.Cols;
        _size = checked(_rows * _cols);

        MineCount = Settings.Mines;

        _cells = new Cell[_rows, _cols];

        Reset();
    }

    private void PlaceRandomMines(CellPosition initial)
    {
        HashSet<CellPosition> excluded = BuildExcludedSet(initial);
        List<CellPosition> candidates = BuildCandidates(excluded);

        if (MineCount > candidates.Count)
        {
            throw new InvalidOperationException("Too many mines for available candidate positions.");
        }

        Random rnd = Random.Shared;
        Shuffle(candidates, rnd);

        PlaceMinesFromCandidates(candidates);
    }

    private HashSet<CellPosition> BuildExcludedSet(CellPosition initial)
    {
        HashSet<CellPosition> excluded = [initial, .. initial.GetNeighborPositions()];
        return excluded;
    }

    private List<CellPosition> BuildCandidates(HashSet<CellPosition> excluded)
    {
        List<CellPosition> candidates = new(_size);

        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                CellPosition pos = new(r, c);
                if (!excluded.Contains(pos))
                {
                    candidates.Add(pos);
                }
            }
        }

        return candidates;
    }

    private void Shuffle(List<CellPosition> list, Random rnd)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            CellPosition tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    private void PlaceMinesFromCandidates(List<CellPosition> candidates)
    {
        List<CellPosition> placedMines = new(MineCount);

        for (int i = 0; i < MineCount; i++)
        {
            CellPosition minePos = candidates[i];
            this[minePos].PlaceMine();
            placedMines.Add(minePos);
        }

        UpdateAdjacentCounts(placedMines);
    }

    private void UpdateAdjacentCounts(List<CellPosition> mines)
    {
        foreach (CellPosition mine in mines)
        {
            foreach (CellPosition neighbour in mine.GetNeighborPositions())
            {
                Cell cell = this[neighbour];
                if (!cell.IsMine)
                {
                    cell.IncrementAdjacentMines();
                }
            }
        }
    }

    public void Reset()
    {
        _minesPlaced = false;
        _revealedCount = 0;
        State = GameState.Playing;

        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                this[r, c] = new(new CellPosition(r, c));
            }
        }
    }

    private void ValidatePosition(int row, int col)
    {
        if (row < 0 || row >= _rows) throw new ArgumentOutOfRangeException(nameof(row));
        if (col < 0 || col >= _cols) throw new ArgumentOutOfRangeException(nameof(col));
    }

    private void ValidatePosition(CellPosition pos)
    {
        var (row, col) = pos;
        if (row < 0 || row >= _rows || col < 0 || col >= _cols)
            throw new ArgumentOutOfRangeException(nameof(pos), pos.ToString());
    }
}

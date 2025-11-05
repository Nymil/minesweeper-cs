using System;

namespace Minesweeper.Domain;

public readonly struct CellPosition : IEquatable<CellPosition>
{
    public int Row { get; }
    public int Col { get; }

    public CellPosition(int row, int col)
    {
        if (row < 0 || row >= Settings.Rows) throw new ArgumentOutOfRangeException(nameof(row));
        if (col < 0 || col >= Settings.Cols) throw new ArgumentOutOfRangeException(nameof(col));

        Row = row;
        Col = col;
    }

    public void Deconstruct(out int row, out int col) => (row, col) = (Row, Col);

    public bool Equals(CellPosition other) => Row == other.Row && Col == other.Col;

    public override bool Equals(object? obj) => obj is CellPosition other && Equals(other);

    public IEnumerable<CellPosition> GetNeighborPositions()
    {
        int rows = Settings.Rows;
        int cols = Settings.Cols;

        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;

                int r = Row + dr;
                int c = Col + dc;

                if (r < 0 || r >= rows) continue;
                if (c < 0 || c >= cols) continue;

                yield return new CellPosition(r, c);
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(Row, Col);

    public static bool operator ==(CellPosition left, CellPosition right) => left.Equals(right);

    public static bool operator !=(CellPosition left, CellPosition right) => !left.Equals(right);

    public override string ToString() => $"({Row}, {Col})";
}
using System.Numerics;
using Raylib_cs;

namespace Minesweeper.Domain;

public class BoardRenderer
{
    private readonly int _cellSize;
    private readonly int _rows;
    private readonly int _cols;

    public BoardRenderer()
    {
        _cellSize = Settings.CellSize;
        _rows = Settings.Rows;
        _cols = Settings.Cols;
    }

    public void Draw(Grid grid)
    {
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                DrawCell(grid[r, c]);
            }
        }
    }

    private bool TryGetCellAtScreen(float screenX, float screenY, out CellPosition pos)
    {
        int col = (int)(screenX / _cellSize);
        int row = (int)(screenY / _cellSize);

        if (row < 0 || row >= _rows || col < 0 || col >= _cols)
        {
            pos = default;
            return false;
        }

        pos = new CellPosition(row, col);
        return true;
    }

    public bool TryGetCellFromMouse(out CellPosition pos)
    {
        float mouseX = Raylib.GetMouseX();
        float mouseY = Raylib.GetMouseY();
        return TryGetCellAtScreen(mouseX, mouseY, out pos);
    }

    public void DrawOverlay(Grid grid)
    {
        if (grid.State == GameState.Playing)
        {
            return;
        }

        // dim background
        Color overlay = new(0, 0, 0, 160);
        Raylib.DrawRectangle(0, 0, Settings.WindowWidth, Settings.WindowHeight, overlay);

        string title = grid.State == GameState.Won ? "You Win!" : "Game Over";
        int titleSize = 48;
        int titleWidth = Raylib.MeasureText(title, titleSize);
        int titleX = (Settings.WindowWidth - titleWidth) / 2;
        int titleY = (Settings.WindowHeight - titleSize) / 2 - 20;

        Raylib.DrawText(title, titleX, titleY, titleSize, Color.White);

        string hint = "Press R to restart";
        int hintSize = 20;
        int hintWidth = Raylib.MeasureText(hint, hintSize);
        int hintX = (Settings.WindowWidth - hintWidth) / 2;
        int hintY = titleY + titleSize + 16;

        Raylib.DrawText(hint, hintX, hintY, hintSize, Color.LightGray);
    }

    private void DrawCell(Cell cell)
    {
        int x = cell.Position.Col * _cellSize;
        int y = cell.Position.Row * _cellSize;
        Rectangle rect = new(x, y, _cellSize, _cellSize);

        // background
        Color bg = cell.IsRevealed ? new(200, 200, 200, 255) : new(70, 70, 70, 255);
        Raylib.DrawRectangleRec(rect, bg);

        // cell border
        int lineThickness = 1;
        Raylib.DrawRectangleLinesEx(rect, lineThickness, Color.DarkGray);

        if (cell.IsRevealed)
        {
            if (cell.IsMine)
            {
                float cx = x + _cellSize * 0.5f;
                float cy = y + _cellSize * 0.5f;
                float radius = _cellSize * 0.22f;
                Raylib.DrawCircle((int)cx, (int)cy, radius, Color.Black);
            }
            else if (cell.AdjacentMines > 0)
            {
                string txt = cell.AdjacentMines.ToString();
                int fontSize = Math.Max(12, _cellSize / 2);
                int txtWidth = Raylib.MeasureText(txt, fontSize);
                int tx = x + (_cellSize - txtWidth) / 2;
                int ty = y + (_cellSize - fontSize) / 2;
                Raylib.DrawText(txt, tx, ty, fontSize, NumberColor(cell.AdjacentMines));
            }
        }
        else
        {
            if (cell.IsFlagged)
            {
                int pad = Math.Max(4, _cellSize / 8);
                Vector2 p1 = new(x + pad, y + pad);
                Vector2 p2 = new(x + pad, y + _cellSize - pad);
                Vector2 p3 = new(x + _cellSize - pad, y + _cellSize / 2);
                Raylib.DrawTriangle(p1, p2, p3, Color.Red);
            }
        }
    }

    private Color NumberColor(int n) => n switch
    {
        1 => Color.Blue,
        2 => Color.Green,
        3 => Color.Red,
        4 => new Color(0, 0, 128, 255),
        5 => new Color(128, 0, 0, 255),
        6 => new Color(64, 64, 64, 255),
        _ => Color.Black,
    };
}

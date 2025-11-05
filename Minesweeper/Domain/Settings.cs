using System;

namespace Minesweeper.Domain;

public static class Settings
{
    public const string Title = "Minesweeper";
    public const int Rows = 16;
    public const int Cols = 20;
    public const int CellSize = 60;
    public const int Mines = 45;

    public static int WindowWidth => Cols * CellSize;
    public static int WindowHeight => Rows * CellSize;
}

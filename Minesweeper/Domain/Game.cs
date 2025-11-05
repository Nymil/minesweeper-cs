using System;
using System.Data;
using Raylib_cs;

namespace Minesweeper.Domain;

public class Game
{
    private Grid _grid = null!;
    private BoardRenderer _renderer = null!;

    public void Run()
    {
        Init();
        StartMainLoop();
        // if main loop ends, quit program
        Destroy();
    }

    void Destroy()
    {
        Raylib.CloseWindow();
    }

    void Init()
    {
        _grid = new();
        _renderer = new();

        Raylib.InitWindow(Settings.WindowWidth, Settings.WindowHeight, Settings.Title);
    }

    void StartMainLoop()
    {
        while (!Raylib.WindowShouldClose())
        {
            Update();
            Draw();
        }
    }

    void Update()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.R))
        {
            _grid.Reset();
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            CellPosition clicked;
            if (_renderer.TryGetCellFromMouse(out clicked))
            {
                _grid.Reveal(clicked);
            }
        }
        else if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            CellPosition clicked;
            if (_renderer.TryGetCellFromMouse(out clicked))
            {
                _grid.ToggleFlag(clicked);
            }
        }
    }
    
    void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        _renderer.Draw(_grid);
        _renderer.DrawOverlay(_grid);

        Raylib.EndDrawing();
    }
}

using System.Numerics;

namespace LevelSolver;

public enum GameColor {
    White,
    Red,
    Blue,
    Yellow,
    Orange,
    Purple,
    Green,
}

public enum EnemyType
{
    Normal,
    Passthrough,
    Shield,
    ColorAbsorbers,
    ColorShifters,
    ColorShiftersPassthrough,
    SuicideBombers,
    BlockerToPassthroughSwitcher,
    PassthroughToBlockerSwitcher,
    TripleColorShifter,
    Boss
}

[Serializable]
public class EnemyData
{
    public GameColor color;
    public GameColor secondaryColor;
    public GameColor tertiaryColor;
    public EnemyType enemyType;
    
        
}
public enum GridTile
{
    Empty,
    Red,
    Blue,
    Yellow,
    White,
    Portal1,
    Blocker,
    ColorBlocker,
    StartPosition,
    Portal2,
    Portal3,
    Portal4,
    PowerUpTile,
    HallOfHueTile,
    MoveLeft,
    MoveRight,
    MoveUp,
    MoveDown
}
[Serializable]
public class LevelData {
    
    public string levelId;
    public string levelName;
    
    public EnemyData[,] enemiesToSpawn;
        
    public GridTile[,] playerGrid;

    public void YFlipCauseOdin() {
        FlipYAxis(enemiesToSpawn);
        FlipYAxis(playerGrid);
    }
    
    private  void FlipYAxis<T>(T[,] array)
    {
        int columns = array.GetLength(0); // This is X (Width)
        int rows = array.GetLength(1);    // This is Y (Height)

        // Loop through all columns
        for (int x = 0; x < columns; x++)
        {
            // Loop through only the top half of the Y rows
            for (int y = 0; y < rows / 2; y++)
            {
                int targetY = rows - 1 - y;

                // Swap using C# tuple syntax, targeting the second index [x, y]
                (array[x, y], array[x, targetY]) = (array[x, targetY], array[x, y]);
            }
        }
    }
}

public readonly record struct LevelStateKey(
    int playerX,
    int playerY,
    int phase,
    GameColor playerColor,
    ulong enemyMask
);

public class LevelState {

    public LevelState(int startX, int startY, int enemyRowCount, int enemyColumnCount) {
        playerX = startX;
        playerY = startY;
        enemydead = new bool[enemyColumnCount, enemyRowCount];
    }

    public LevelState(LevelState target) {
        playerX = target.playerX;
        playerY = target.playerY;
        
        moveCount  = target.moveCount;
        playerColor  = target.playerColor;
        
        enemydead = (bool[,])target.enemydead.Clone();
        
        
    }

    public LevelState Clone() => new LevelState(this);

    public LevelStateKey CreateHashKey() {
        ulong mask = 0;
        int bit = 0;

        for (int y = 0; y < enemydead.GetLength(1); y++) {
            for (int i = 0; i < enemydead.GetLength(0); i++) {
                if (!enemydead[i, y]) {
                    mask |= 1UL << bit;
                }
                bit++;
            }
        }
        return new LevelStateKey(playerX, playerY, moveCount % 6, playerColor, mask);
    }
    
    public int playerX, playerY;
    public GameColor playerColor = GameColor.White;

    public int moveCount = 0;

    public bool[,] enemydead;

    
    
}

public enum Direction{
    Left,
    Right,
    Up,
    Down
}
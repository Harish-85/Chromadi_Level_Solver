using System.Text;

namespace LevelSolver;

public class AsciiRenderer {
    public void Render(LevelData level,LevelState state) {
        
        
        StringBuilder sb = new();
        
        sb.AppendLine($"Moves: {state.moveCount}");
        sb.AppendLine($"Player Color: {state.playerColor}");
        sb.AppendLine();

        // Enemy rows
        sb.AppendLine("----------------------------------------");
        
        for (int y = level.enemiesToSpawn.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < level.enemiesToSpawn.GetLength(0); x++)
            {
                if (state.enemydead[x, y])
                {
                    sb.Append(" .  ");
                    continue;
                }

                var color = GetEnemyColorAtTile( x, y,state.moveCount,level);
                sb.Append($"{color}  ");
            }

            sb.AppendLine();
        }
        
        sb.AppendLine();
        
        for (int y = level.playerGrid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < level.playerGrid.GetLength(0); x++)
            {
                if (state.playerX == x && state.playerY == y)
                {
                    sb.Append(" P" + GameColorToChar(state.playerColor) + " ");
                    continue;
                }

                sb.Append($" {TileToChar(level.playerGrid[x, y])}  ");
            }

            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }

    public GameColor GetEnemyColorAtTile(int x, int y,int moveCount,LevelData data) {
        var enemyAtIndex = data.enemiesToSpawn[x, y];

        if (enemyAtIndex.enemyType is EnemyType.Normal or EnemyType.BlockerToPassthroughSwitcher or EnemyType.PassthroughToBlockerSwitcher or EnemyType.Passthrough) {
            return enemyAtIndex.color;
        }

        if (enemyAtIndex.enemyType is EnemyType.ColorShifters) {
            //get the current color
            if (moveCount % 2 == 0) {
                return enemyAtIndex.color;
            } else {
                return enemyAtIndex.secondaryColor;
            }
        }

        if (enemyAtIndex.enemyType is EnemyType.TripleColorShifter) {
            if (moveCount % 3 == 0) {
                return enemyAtIndex.color;
            }
            if (moveCount % 3 == 1) {
                return enemyAtIndex.secondaryColor;
            }
            return enemyAtIndex.tertiaryColor;
        }

        throw new Exception($"Unhandled enemy {enemyAtIndex.enemyType.ToString()}");
    }
    
    private static char GameColorToChar(GameColor col) {
        return col.ToString()[0];
    }
    
    private static char TileToChar(GridTile tile)
    {
        return tile switch
        {
            GridTile.Empty => '.',

            GridTile.Red => 'R',
            GridTile.Blue => 'B',
            GridTile.Yellow => 'Y',
            GridTile.White => 'W',

            GridTile.Blocker => '#',

            GridTile.Portal1 => '1',
            GridTile.Portal2 => '2',
            GridTile.Portal3 => '3',
            GridTile.Portal4 => '4',

            GridTile.MoveLeft => '<',
            GridTile.MoveRight => '>',
            GridTile.MoveUp => '^',
            GridTile.MoveDown => 'v',

            GridTile.StartPosition => 'S',

            GridTile.PowerUpTile => '+',
            GridTile.HallOfHueTile => 'H',

            _ => '?'
        };
    }
}

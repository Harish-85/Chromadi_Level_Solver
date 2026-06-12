using System.Diagnostics;

namespace LevelSolver;

public class GameSimulator {

    LevelData _levelData;
    
    private int gridSizeX, gridSizeY;
    
    public GameSimulator(ILevelDataProvider levelDataProvider,bool flipY = true) {
        levelDataProvider.ParseLevelData();
        _levelData = levelDataProvider.GetLevelData();
        _levelData.YFlipCauseOdin();
        
        gridSizeX = _levelData.playerGrid.GetLength(0);
        gridSizeY = _levelData.playerGrid.GetLength(1);

        
        
    }
    
    public LevelData GetLevelData() => _levelData;

    public LevelState InitialLevelState() {
        int startX = -1, startY = -1;
        for (int i = 0; i < gridSizeX; i++) {
            for (int j = 0; j < gridSizeY; j++) {
                if (_levelData.playerGrid[i, j] == GridTile.StartPosition) {
                    startX = i;
                    startY = j;
                }
            }
        }

        if (startY == -1) {
            startY = gridSizeY /2;
            startX = gridSizeX /2;
        }
        
        var res = new LevelState(startX, startY, _levelData.enemiesToSpawn.GetLength(1), _levelData.enemiesToSpawn.GetLength(0));
        AutoKillEnemies(res);
        return res;
    }
    
    public LevelState NextState(LevelState ls, Direction dir) {
        LevelState  nextState = ls.Clone();
        AutoKillEnemies(nextState);
        
        nextState.moveCount++;
        ApplyMovement( nextState, dir );
        UpdateBoardState(nextState);
        AutoKillEnemies(nextState);
        
        return nextState;
    }

    private int prevPlayerX,prevPlayerY;
    private void ApplyMovement(LevelState _levelState, Direction dir) {
        prevPlayerX = _levelState.playerX;
        prevPlayerY = _levelState.playerY;
        
        switch (dir) {

            case Direction.Left:
                _levelState.playerX--;
                break;
            case Direction.Right:
                _levelState.playerX++;
                break;
            case Direction.Up:
                _levelState.playerY++;
                break;
            case Direction.Down:
                _levelState.playerY--;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
        //teleport
        if(_levelState.playerX<0) _levelState.playerX = _levelData.playerGrid.GetLength(0) - 1;
        if (_levelState.playerY<0) _levelState.playerY =  _levelData.playerGrid.GetLength(1) - 1;
        
        
        _levelState.playerX %= _levelData.playerGrid.GetLength(0);
        _levelState.playerY %= _levelData.playerGrid.GetLength(1);
        
        
        
    }

    #region Board State update

    private void UpdateBoardState(LevelState state) {
        int iterCount = 0;
        while (ApplyTileProperties(state)) {
            iterCount++;
            if (iterCount > 1000) {
                throw new Exception("Infinte loop in board state update");
            }
        }
        
    }
    
    private bool ApplyTileProperties(LevelState _levelState) {
        GridTile tile = GetTileAtPosition(_levelState.playerX, _levelState.playerY);
        bool isStateDirty = false;
        switch (tile) {

            case GridTile.Red or GridTile.Blue or GridTile.Yellow or GridTile.White:
                HandleColorTile(_levelState,tile);
                break;
            
            case GridTile.Blocker:
                HandleBlockerTile(_levelState);
                Console.WriteLine("blocker");
                isStateDirty = true;
                break;
            
            
            case GridTile.Portal1 or GridTile.Portal2 or GridTile.Portal3 or GridTile.Portal4:
                isStateDirty = HandlePortalTile(_levelState,tile);
                Console.WriteLine("portal");
                break;
          
            
            case GridTile.MoveLeft or GridTile.MoveRight or GridTile.MoveUp or GridTile.MoveDown:
                isStateDirty = true;
                HandleMoverTile(_levelState, tile);
                Console.WriteLine("mover");
                break;
        }
        return isStateDirty;
    }

    private void HandleColorTile(LevelState _levelState,GridTile tile) {
        Dictionary<GridTile, GameColor> colorTiles = new Dictionary<GridTile, GameColor>()
        {
            { GridTile.Red, GameColor.Red },
            { GridTile.Blue, GameColor.Blue },
            { GridTile.Yellow, GameColor.Yellow },
            { GridTile.White, GameColor.White },
        };

        void UpdatePlayerColor(GameColor colorTile) {
            if (colorTile == GameColor.White) {
                _levelState.playerColor = GameColor.White;
                return;
            }
            GameColor playerColor = _levelState.playerColor;

            if (playerColor == GameColor.White) {
                _levelState.playerColor = colorTile;
                return;
            }
            
            if (colorTile == GameColor.Red) {
              
                if (playerColor == GameColor.Yellow) {
                    _levelState.playerColor = GameColor.Orange;
                    return;
                }
                if (playerColor == GameColor.Blue) {
                    _levelState.playerColor = GameColor.Purple;
                    return;
                }
                return;
            }
            if (colorTile == GameColor.Blue) {
               
                if (playerColor == GameColor.Yellow) {
                    _levelState.playerColor = GameColor.Green;
                    return;
                }
                if (playerColor == GameColor.Red) {
                    _levelState.playerColor = GameColor.Purple;
                    return;
                }
                return;
            }
            if (colorTile == GameColor.Yellow) {
                if (playerColor == GameColor.Red) {
                    _levelState.playerColor = GameColor.Orange;
                    return;
                }
                if (playerColor == GameColor.Blue) {
                    _levelState.playerColor = GameColor.Green;
                }
            }
        }
        
        UpdatePlayerColor( colorTiles[tile]);
        
    }

    private void HandleBlockerTile(LevelState _levelState) {
        _levelState.playerX  = prevPlayerX;
        _levelState.playerY = prevPlayerY;
    }
    
    private void HandleMoverTile(LevelState _levelState,GridTile tile) {
        prevPlayerX = _levelState.playerX;
        prevPlayerY = _levelState.playerY;
        
        if (tile == GridTile.MoveLeft) {
            _levelState.playerX--;
        }
        if (tile == GridTile.MoveRight) {
            _levelState.playerX++;
        }
        if (tile == GridTile.MoveUp) {
            _levelState.playerY++;
        }
        if (tile == GridTile.MoveDown) {
            _levelState.playerY--;
        }

        if (_levelState.playerX < 0) _levelState.playerX = _levelData.playerGrid.GetLength(0) - 1;
        if (_levelState.playerY < 0) _levelState.playerY = _levelData.playerGrid.GetLength(1) - 1;
        if (_levelState.playerX >= _levelData.playerGrid.GetLength(0)) _levelState.playerX = 0;
        if( _levelState.playerY >= _levelData.playerGrid.GetLength(1)) _levelState.playerY = 0;
        
    }
    
    private bool HandlePortalTile(LevelState _levelState,GridTile tile) {
        for (int i = 0; i < _levelData.playerGrid.GetLength(0); i++) {
            for (int j = 0; j < _levelData.playerGrid.GetLength(1);  j++) {
                if(i == _levelState.playerX && j == _levelState.playerY) continue;
                
                if (_levelData.playerGrid[i, j] == tile) {
                    
                    prevPlayerX = _levelState.playerX;
                    prevPlayerY = _levelState.playerY;
                    
                    _levelState.playerX = i;
                    _levelState.playerY = j;
                    return false;
                }
            }
        }
        Console.WriteLine("not mathing portal tile found");
        return false;
    }
    
    #endregion


    #region UpdateEnemies

    void AutoKillEnemies(LevelState _levelState) {
        for (int i = 0; i < _levelData.enemiesToSpawn.GetLength(1); i++) {
            if (_levelState.enemydead[_levelState.playerX, i]) {
                continue;   
            }
            
            bool isPassthrough = IsEnemyPassthrough(_levelState.playerX, i,_levelState.moveCount);
            GameColor color = GetEnemyColorAtTile(_levelState.playerX, i,_levelState.moveCount);

            if (color == _levelState.playerColor) {
                _levelState.enemydead[_levelState.playerX, i] = true;
                continue;
            }

            if (!isPassthrough) {
                break;
            }
        }
        
    }

    private bool IsEnemyPassthrough(int x, int y , int moveCount) {
        var enemyAtIndex = _levelData.enemiesToSpawn[x, y];
        
        if(enemyAtIndex.enemyType is EnemyType.Passthrough) {
            return true;
        }
        if(enemyAtIndex.enemyType is EnemyType.BlockerToPassthroughSwitcher ) {
            return moveCount % 2 == 1;
        }
        if (enemyAtIndex.enemyType is EnemyType.PassthroughToBlockerSwitcher) {
            return moveCount % 2 == 0;
        }
        
        return false;
    }

    public GameColor GetEnemyColorAtTile(int x, int y,int moveCount) {
        var enemyAtIndex = _levelData.enemiesToSpawn[x, y];

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
        
    #endregion
    
    #region Helpers
    GridTile GetTileAtPosition(int x, int y) {
        return _levelData.playerGrid[x, y];
    }
    #endregion
}

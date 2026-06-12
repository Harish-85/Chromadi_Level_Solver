namespace LevelSolver;

public class SearchNode {
    public LevelState state;
    public SearchNode? parent;
    public Direction? move;
}

public class BFSSolver {

    bool IsSolved(LevelState state) {
        for (int i = 0; i < state.enemydead.GetLength(1); i++) {
            for (int j = 0; j < state.enemydead.GetLength(0); j++) {
                if (!state.enemydead[j, i]) {
                    return false;
                }
            }
        }
        return true;
    }

    public List<Direction> BuildPath(SearchNode goal) {
        List<Direction> result = new();
        
        SearchNode? current = goal;

        while (current?.move != null) {
            result.Add(current.move.Value);
            current = current.parent;
        }
        
        result.Reverse();
        return result;
    }
    
    public SearchNode? Solve(GameSimulator sim, LevelState startState) {
        Queue<SearchNode> queue = new();
        HashSet<LevelStateKey> visited = new();

        SearchNode start = new()
        {
            state = startState,
        };
        
        queue.Enqueue(start);
        visited.Add(startState.CreateHashKey());

        var  expanded = 0;
        while (queue.Count > 0) {
            SearchNode current =  queue.Dequeue();
            if (IsSolved(current.state)) {
                return current;
            }
            
            Console.WriteLine(
                $"Expanded={expanded} Queue={queue.Count} Visited={visited.Count}");
            
            
            foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
                LevelState nextState = sim.NextState(current.state,dir);

                bool added = visited.Add(nextState.CreateHashKey());
                
                Console.WriteLine(
                    $"{dir} -> ({nextState.playerX},{nextState.playerY}) " +
                    $"phase={nextState.moveCount % 6} " +
                    $"added={added}");
                
                if (!added) {
                    continue;
                }
                
                queue.Enqueue(new SearchNode()
                {
                    state = nextState,
                    parent = current,
                    move = dir
                });
            }
        }
        return null;
    }
    
}

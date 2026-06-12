namespace LevelSolver;

public class SearchNode {
    public LevelState state;
    public SearchNode? parent;
    public Direction? move;

    public int g;
    public int h;
    public int f => g + h;
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
        PriorityQueue<SearchNode,int> queue = new();
        Dictionary<LevelStateKey,int> visited = new();

        SearchNode start = new()
        {
            state = startState,
            g =0,
            h = Heuristic(startState)
        };
        
        queue.Enqueue(start,start.f);
        visited[startState.CreateHashKey()] = 0;

        var  expanded = 0;
        while (queue.Count > 0) {
            expanded++;
            SearchNode current =  queue.Dequeue();
            if (IsSolved(current.state)) {
                return current;
            }
            
            Console.WriteLine(
                $"Expanded={expanded} Queue={queue.Count} Visited={visited.Count}");
            
            
            foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
                LevelState nextState = sim.NextState(current.state,dir);

                int newG = current.g + 1;
                
               var nextStatekey =nextState.CreateHashKey();

               if (visited.TryGetValue(nextStatekey, out int oldG) && oldG <= newG) {
                   continue;
               }
               visited[nextStatekey] = newG;


               var node = new SearchNode
               {
                   state = nextState,
                   parent = current,
                   move = dir,
                   g = newG,
                   h = Heuristic(nextState)
               };

               queue.Enqueue(node ,node.f);
            }
        }
        return null;
    }
    int Heuristic(LevelState state) {
        
        int alive = 0;

        for (int y = 0; y < state.enemydead.GetLength(1); y++) {
            bool anyAlive = false;
            for (int x = 0; x < state.enemydead.GetLength(0); x++) {
                if (!state.enemydead[x, y]) {
                    anyAlive = true;
                    break;
                }
            }
            if(anyAlive) alive++;
        }

        return alive;
    }
    
}

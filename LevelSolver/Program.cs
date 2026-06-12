using System.Xml;
using LevelSolver;


//Dependencies
//ILevelDataProvider levelDataProvider = new JsonFileLevelDataProvider("testLevel.json");
Console.WriteLine("Press Q to perform previous input list. Press anything else to go to level solver");
ConsoleKey choice = Console.ReadKey().Key;
if (choice == ConsoleKey.Q) {
    //try to load previous inputs
    if (File.Exists("last_solution.txt")) {
        string readText = File.ReadAllText("last_solution.txt");
        //split it where ever there aercommas
        string[] entries = readText.Split(",");
        //convert this into a list of Directions
        List<Direction> inputs = new List<Direction>();
        foreach (var entry in entries) {
            inputs.Add(Enum.Parse<Direction>(entry));
            foreach (var direction in inputs) {
                Console.WriteLine();
                Console.Write(direction.ToString() + " ");
            }
        }
        new PerformInput().PerformInputs(inputs);

    } else {
        Console.WriteLine("No last solution found");
        return;
    }
    
}

ILevelDataProvider levelDataProvider = new ConsoleLevelDataProvider();

GameSimulator gameSimulator = new GameSimulator(levelDataProvider);

AsciiRenderer asciiRenderer = new AsciiRenderer();

BFSSolver solver = new BFSSolver();

bool showTest = false;
if (showTest) {
    LevelState testState = gameSimulator.InitialLevelState();
    asciiRenderer.Render(gameSimulator.GetLevelData(),testState);
    while (true) {
        ConsoleKey key = Console.ReadKey().Key;
        Console.Clear();
        Dictionary<ConsoleKey, Direction> directions = new Dictionary<ConsoleKey, Direction>()
        {
            { ConsoleKey.UpArrow, Direction.Up },
            { ConsoleKey.DownArrow, Direction.Down },
            { ConsoleKey.LeftArrow, Direction.Left },
            { ConsoleKey.RightArrow, Direction.Right },
        };
        testState = gameSimulator.NextState(testState,directions[key]);
        asciiRenderer.Render(gameSimulator.GetLevelData(),testState);

    }

}

var goal = solver.Solve(gameSimulator,gameSimulator.InitialLevelState());

if (goal != null) {
    var path = solver.BuildPath(goal);
    
    Console.WriteLine("Solution Found, saving it...... ");

    string res = "";
    foreach (var direction in path) {
        res += direction.ToString()+",";
    }
    //remove the last comma
    res = res.Remove(res.Length - 1);
    
    File.WriteAllText( "last_solution.txt", res);
    
    Console.WriteLine("Move count: " +  path.Count);
    foreach (var line in path) {
        Console.WriteLine(line);
    }
    Console.WriteLine("Press q to start auto key sequence :");
    if (Console.ReadKey().Key == ConsoleKey.Q) {
        new PerformInput().PerformInputs(path);
    }
} else {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("NO solution found");
    Console.ResetColor();
}


using System.Diagnostics;

namespace LevelSolver;

public class PerformInput {
    static void SendKey(int keycode) {
        Process.Start(new ProcessStartInfo
        {
            FileName = "ydotool",
            Arguments = $"key {keycode}:1 {keycode}:0",
            UseShellExecute = false,
        })?.WaitForExit();
    }
    
    public static void Shoot()
    {
        SendKey(57);
    }

    public void PerformInputs(List<Direction> path) {
        Console.WriteLine("Starting in 3 seconds");
        Thread.Sleep(3000);
        Console.WriteLine("Started");
        foreach (var dir in path) {
            for (int i = 0; i < 10; i++) {
                Shoot();
                Thread.Sleep(50);
            }
            Thread.Sleep(1400);
            Console.WriteLine("Moving" + dir.ToString());
            Move(dir);
            Thread.Sleep(300);
        }
    
    }
    
    public static void Move(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                SendKey(17);
                break;

            case Direction.Left:
                SendKey(30);
                break;

            case Direction.Down:
                SendKey(31);
                break;

            case Direction.Right:
                SendKey(32);
                break;
        }
    }
}

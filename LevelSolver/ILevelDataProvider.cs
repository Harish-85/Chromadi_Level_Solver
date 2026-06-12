using Newtonsoft.Json;

namespace LevelSolver;

public interface ILevelDataProvider {
    bool ParseLevelData();
    LevelData GetLevelData();
}
class JsonFileLevelDataProvider  : ILevelDataProvider {

    string jsonPath;
    LevelData? _levelData; 
    
    public JsonFileLevelDataProvider(string jsonPath) {
        this.jsonPath = jsonPath;
    }
    
    public  bool ParseLevelData() {
        string dataString = File.ReadAllText( jsonPath );
        if (string.IsNullOrEmpty(dataString)) {
            throw  new Exception(" Invalid json file");
        }
        
        _levelData = JsonConvert.DeserializeObject<LevelData>( dataString);
        if (_levelData == null) {
            throw  new Exception(" _level failed deserialization.");
            return false;
        }
        
        
        return true;
    }
    
    public LevelData GetLevelData() {

        if (_levelData == null) {
            throw new Exception(" _level data not yet deserialized. Make sure you call parse level data");
        }
        
        return _levelData;
    }
}

class ConsoleLevelDataProvider : ILevelDataProvider {
    LevelData _levelData;

    public bool ParseLevelData() {
        //get the input from terminal
        Console.WriteLine("Enter the json for the level you want to solve : ");
        string dataString = Console.In.ReadToEnd() ;
        if (string.IsNullOrEmpty(dataString)) {
            throw  new Exception(" Invalid json file");
        }
        _levelData = JsonConvert.DeserializeObject<LevelData>(dataString);
        if (_levelData == null) {
            throw  new Exception(" _level data failed deserialization.");
        }
        return true;
        
    }
    public LevelData GetLevelData() {
        return _levelData;
    }
}
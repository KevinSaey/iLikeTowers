using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{

    //Variables for gamestate --> JSON
    public int _xAmount, _yAmount;// JSON
    public List<IniNode> _iniNodes = new List<IniNode>();// JSON
    public string _LevelInfo;// JSON
    public int _maxTurns; // JSON
    public int _maxBeams = 100; // JSON
    public Vector3 _position; //JSON

    public Level()
    {
    }

    public Level(int xAmount, int yAmount, List<IniNode> iniNodes, string levelInfo, int maxTurns, int maxBeams, Vector3 position)
    {
        _xAmount = xAmount;
        _yAmount = yAmount;
        _iniNodes = iniNodes;
        _LevelInfo = levelInfo;
        _maxBeams = maxBeams;
        _position = position;

    }
}

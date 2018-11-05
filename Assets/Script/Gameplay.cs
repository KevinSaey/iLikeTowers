using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Gameplay : MonoBehaviour
{
    public GameObject _goNode;
    public float _length;
    public int _xAmount, _yAmount;
    public TriGrid _triGrid;
    public Material _matBeam, _matLine;
    public float _breakForce;
    public GUISkin _skin;
    public Level _level;
    public List<IniNode> _iniNodes = new List<IniNode>();

    private Ray _ray;
    private RaycastHit _hit;

    private string gameDataFileName = "data.json";

    private void SaveGameData()
    {
        Debug.Log("Saving");


    }


    private void LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
        }
        else
        {
            Debug.Log("File not found");
        }

    }


    void Start()
    {
        _iniNodes.Add(new IniNode(1, 0, true));
        _iniNodes.Add(new IniNode(2, 0, true));
        _iniNodes.Add(new IniNode(5, 0, true));
        _iniNodes.Add(new IniNode(6, 0, true));
        _iniNodes.Add(new IniNode(8, 0, true));
        _iniNodes.Add(new IniNode(4, 6, new Vector3(10000, 20, 0)));

        _level = new Level(_xAmount, _yAmount, _iniNodes, "Hold the force", 10, 100, Vector3.zero);
        Physics.autoSimulation = false;
        _triGrid = new TriGrid(_goNode, _length, _matBeam, _matLine, _breakForce, _level);
    }

    void Update()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            SwitchBeam();
        }

        // switcht between build and simulation mode
        if (Input.GetButtonDown("Turn"))
        {
            if (!Physics.autoSimulation)
            {
                //Enter into simulation mode
                foreach (Beam beam in _triGrid._beams)
                {
                    if (!beam._exists) beam.Destruct();
                    else beam.ResetCollider();

                }
                for (int j = 0; j < _triGrid._nodes.Count; j++)
                {
                    for (int i = 0; i < _triGrid._nodes[i].Count; i++)
                    {
                        if (_triGrid._nodes[j][i]._connectedBeams == 0
                            && !_triGrid._nodes[j][i]._hasForce
                            && !_triGrid._nodes[j][i]._kinematic) _triGrid._nodes[j][i].Destruct();
                    }
                }

                Physics.autoSimulation = true;
            }
            else
            {
                //re-enter into building mode
                foreach (Beam beam in _triGrid._beams)
                {
                    beam.Destruct();
                }
                foreach (var list in _triGrid._nodes)
                {
                    foreach (var node in list)
                    {
                        node.Destruct();
                    }
                }
                _triGrid.Clear();
                _triGrid.MakeGrid();

                Physics.autoSimulation = false;
            }
        }
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        _triGrid.Update();

    }

    void OnMouseDrag()
    {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        SwitchBeam();
    }

    private void OnGUI()
    {
        GUI.skin = _skin;
        GUI.Label(new Rect(10, 10, 200, 50), $"# Beams {_triGrid._numberOfBeams.ToString()}"); // show the amount of used beams
        //GUI.Label(new Rect(10, 50, 200, 50), $"# Beams left {(_triGrid._maxBeams- _triGrid._numberOfBeams).ToString()}"); // show the amount beams left
        GUI.Label(new Rect(Screen.width - 110, 10, 100, 50), $"Turn {_triGrid._turn.ToString()}"); // show the amount of turns


        if (_triGrid._broken)
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200, 200), $"You Broke It!");
        }

        /*//enable|dissable physics and collision
        if (GUI.Button(new Rect(20, 20, 50, 40), "Enable physics"))
        {
            Physics.autoSimulation = !Physics.autoSimulation;
        }*/
    }

    void SwitchBeam()
    {
        if (Physics.Raycast(_ray, out _hit, 1000))
        {
            GameObject goBeam = _hit.transform.gameObject;
            if (goBeam.layer == 9) //check if the selected object is a beam
            {
                Beam beam = _triGrid.GetBeam(goBeam);
                beam._exists = !beam._exists;
                if (beam._exists)
                {
                    // this number will be used to detemine if a beam should exist
                    beam._startNode._connectedBeams++;
                    beam._endNode._connectedBeams++;

                    _triGrid._numberOfBeams++;
                }
                else
                {
                    _triGrid._numberOfBeams--;
                }
                _triGrid._beamExists[beam._index] = !_triGrid._beamExists[beam._index];

                beam.SwitchBeams();
            }
        }
    }
}

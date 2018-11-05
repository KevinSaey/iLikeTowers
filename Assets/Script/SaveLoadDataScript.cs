using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoadDataScript : MonoBehaviour
{

    // Get the number from the Lenght of List
    public int numberOfSaves = 0;
    // get from the button number, or log
    public int chosenNumber = 0;

    /// 
    List<GameData> savedGameData = new List<GameData>();
    string jsonlog = "No Information was loaded...";

    // Info from game /Debug
    public bool beamInf = true;
   


    void Start()
    {

    }

    void Update()
    {

        /////Debug Change Bool
        if (Input.GetKeyDown("b"))
        {
            beamInf = !beamInf;
            //DebugList.Add(Random.Range(-10.0f, 10.0f));
         //DebugList[Random.Range(1, 3)] = Random.Range(-10.0f, 10.0f);
        }
        /////Choose the input here, change it on buttons later//////
        if (Input.GetKeyDown("l"))
        {
            LoadData();
        }


        if (Input.GetKeyDown("k"))
        {
            SaveData();
        }
    }


    // Choosing number from console
    public void GetInput(string inputNumber)
    {

        Debug.Log("You have chosen" + inputNumber + "... ");
        chosenNumber = 0;
        int.TryParse(inputNumber, out chosenNumber);
        Debug.Log("Parsed int = " + chosenNumber + "... ");

        // numberOfSaves += 1;
    }


    // Save Load Script Methods
    void LoadData()
    {
        /* if (chosenSaveFile < numberOfSaves)
         {*/
        Debug.Log("Loading " + chosenNumber + "_file...");
        string jsonlog = File.ReadAllText(Application.dataPath + "savefile_" + chosenNumber + ".json");
        GameData LoadedGame = JsonUtility.FromJson<GameData>(jsonlog);
        Debug.Log("Beam exists ? =" + LoadedGame._beamexist);
        Debug.Log("Beam Data =" + LoadedGame._debugList);

        string jsonlog2 = LoadedGame.ToString();
        Debug.Log(jsonlog2);
        /* }
         else
         {
             Debug.Log("Loading " + chosenSaveFile + "_file failed ! " + chosenSaveFile + "_file can not be found or does not exist.");
         }*/
    }


    void SaveData()
    {

        Debug.Log("Saving " + chosenNumber + "_file...");
        GameData InstantInfo = new GameData();
        InstantInfo._beamexist = beamInf;
        InstantInfo._debugList = new List<float> { 0.2f, 11, 2, 0.5f };
        //savedGameData.Add(InstantInfo);
        Debug.Log("Do Beams exist ?");

        string jsonlog = JsonUtility.ToJson(InstantInfo);
        Debug.Log(jsonlog);

        File.WriteAllText(Application.dataPath + "savefile_" + chosenNumber + ".json", jsonlog);
        // savedGameData.Add(new GameData());
    }

    private class GameData
    {
        public bool _beamexist;

        public List<float> _debugList ;


    }

}

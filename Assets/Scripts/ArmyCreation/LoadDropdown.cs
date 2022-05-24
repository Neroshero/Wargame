using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadDropdown : MonoBehaviour
{

    public Dropdown currFac;
    public Dropdown currArmy;
    private List<string> armies;

    // Start is called before the first frame update
    void Start()
    {
        armies = new List<string>();
        DirectoryInfo armiesPath = new DirectoryInfo(Application.streamingAssetsPath + "\\Armies\\Army");
        armies.Add("please select an army");
        foreach (FileInfo i in armiesPath.GetFiles("*.txt"))
        {
            armies.Add(i.Name.Replace(".txt", ""));
        }
        currArmy.AddOptions(armies);
        armies = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeValues()
    {
        currArmy.ClearOptions();
        armies.Add("please select an army");
        string toSet = Application.streamingAssetsPath + "\\Armies\\" + currFac.options[currFac.value].text;
        DirectoryInfo armiesPath;
        try
        {
            armiesPath = new DirectoryInfo(toSet);
        }catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory(toSet);
            armiesPath = new DirectoryInfo(toSet);
        }
        foreach (FileInfo i in armiesPath.GetFiles("*.txt"))
        {
            armies.Add(i.Name.Replace(".txt", ""));
        }
        currArmy.AddOptions(armies);
        armies = new List<string>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    public echoAR echo;
    public Text textAPIKey ;
    private string APIKey;


    public void Download()
    {
        APIKey = textAPIKey.text;
        echo.APIKey = APIKey;



    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

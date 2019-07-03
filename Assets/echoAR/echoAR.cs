using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;
using System.IO;
using System.Collections.Generic;
using AsImpL;
using System.Globalization;

public class echoAR : MonoBehaviour
{

    // Your echoAR API key
    public string APIKey = "<YOUR_API_KEY>";
    private string serverURL;

    // echoAR Database
    static public Database dbObject;

    void Start()
    {
        // The echoAR server details
        serverURL = "https://console.echoar.xyz/query?key=" + APIKey;

        // Run the database query subroutine followed by assets download subroutine
        try
        {
            StartCoroutine(QueryDatabase(serverURL));
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

    }

    IEnumerator QueryDatabase(string serverURL)
    {
        // Create a new request
        UnityWebRequest www = UnityWebRequest.Get(serverURL);

        Debug.Log("Querying database...");

        // Yield for the request
        yield return www.Send();

        // Wait for the request to finish
        while (!www.isDone)
        {
            yield return null;
        }

        string json = "not found";
        // Check for errors
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Parse response
            json = www.downloadHandler.text;

            // Handle repsonse
            Debug.Log("Query Response:");
            print(json);

            // Parse Database
            ParseDatabase(json);
        }

        // Cleanup
        www.disposeDownloadHandlerOnDispose = true;
        www.disposeUploadHandlerOnDispose = true;
        www.Dispose();
        www = null;

        // Check for valid response
        if (!json.ToLower().Contains("not found"))
        {
            // Start Websocket client
            StartCoroutine(WebsocketClient());
            // Download Assets
            StartCoroutine(DownloadAssets(serverURL));
        }
    }

    public void ParseDatabase(string json)
    {
        // Parse database
        if (!json.ToLower().Contains("not found"))
        {
            Debug.Log("Parsing database...");
            // Parse JSON
            var parsedJSON = JSON.Parse(json);
            // Create a Database object with and API key
            dbObject = new Database(parsedJSON["apiKey"].Value);
            // Get entries
            int i = 0;
            var entry = parsedJSON["db"][i];
            while (entry != null)
            {
                // Parse Entry
                ParseEntry(entry);
                // Continue to next entry
                entry = parsedJSON["db"][++i];
            }
            Debug.Log("Database parsed.");
        }
    }

    public Entry ParseEntry(JSONNode entry)
    {
        // Create entry
        Entry entryObject = new Entry();
        entryObject.setId(entry["id"]);

        // Create target
        Target targetObject;
        var target = entry["target"];
        string targetType = target["type"];
        switch (targetType)
        {
            case "IMAGE_TARGET":
                ImageTarget imageTargetObject = new ImageTarget();
                imageTargetObject.setFilename(target["filename"]);
                imageTargetObject.setStorageID(target["storageID"]);
                imageTargetObject.setId(target["id"]);
                imageTargetObject.setType(Target.targetType.IMAGE_TARGET);
                targetObject = imageTargetObject;
                break;
            case "GEOLOCATION_TARGET":
                GeolocationTarget geolocationTargetObject = new GeolocationTarget();
                geolocationTargetObject.setCity(target["city"]);
                geolocationTargetObject.setContinent(target["continent"]);
                geolocationTargetObject.setCountry(target["country"]);
                geolocationTargetObject.setId(target["id"]);
                geolocationTargetObject.setLatitude(target["latitude"]);
                geolocationTargetObject.setLongitude(target["longitude"]);
                geolocationTargetObject.setPlace(target["place"]);
                geolocationTargetObject.setType(Target.targetType.GEOLOCATION_TARGET);
                targetObject = geolocationTargetObject;
                break;
            case "BRICK_TARGET":
                BrickTarget brickTargetObject = new BrickTarget();
                brickTargetObject.setId(target["id"]);
                brickTargetObject.setType(Target.targetType.BRICK_TARGET);
                targetObject = brickTargetObject;
                break;
            default:
                targetObject = new Target();
                break;
        }
        List<string> hologramsListObject = new List<string>();
        int j = 0;
        var hologramID = target["holograms"][j];
        while (hologramID != null)
        {
            hologramsListObject.Add(hologramID);
            hologramID = target["holograms"][++j];
        }
        targetObject.setHolograms(hologramsListObject);
        entryObject.setTarget(targetObject);

        // Create Hologram
        Hologram hologramObject;
        var hologram = entry["hologram"];
        string hologramType = hologram["type"];
        switch (hologramType)
        {
            case "VIDEO_HOLOGRAM":
                VideoHologram videoHologramObject = new VideoHologram();
                videoHologramObject.setFilename(hologram["filename"]);
                videoHologramObject.setId(hologram["id"]);
                videoHologramObject.setStorageID(hologram["storageID"]);
                videoHologramObject.setTargetID(hologram["targetID"]);
                videoHologramObject.setType(Hologram.hologramType.VIDEO_HOLOGRAM);
                videoHologramObject.setTarget(targetObject);
                hologramObject = videoHologramObject;
                break;
            case "ECHO_HOLOGRAM":
                EchoHologram echoHologramObject = new EchoHologram();
                echoHologramObject.setFilename(hologram["filename"]);
                echoHologramObject.setId(hologram["id"]);
                echoHologramObject.setEncodedEcho(hologram["encodedEcho"]);
                echoHologramObject.setTextureFilename(hologram["textureFilename"]);
                echoHologramObject.setTargetID(hologram["targetID"]);
                echoHologramObject.setType(Hologram.hologramType.ECHO_HOLOGRAM);
                echoHologramObject.setTarget(targetObject);
                List<string> videosListObject = new List<string>();

                j = 0;
                var videoID = hologram["vidoes"][j];
                while (videoID != null)
                {
                    videosListObject.Add(videoID);
                    hologramID = hologram["vidoes"][++j];
                }
                echoHologramObject.setVidoes(videosListObject);

                hologramObject = echoHologramObject;
                break;
            case "MODEL_HOLOGRAM":
                ModelHologram modelHologramObject = new ModelHologram();
                modelHologramObject.setEncodedFile(hologram["encodedFile"]);
                modelHologramObject.setFilename(hologram["filename"]);
                modelHologramObject.setId(hologram["id"]);
                modelHologramObject.setMaterialFilename(hologram["materialFilename"]);
                modelHologramObject.setMaterialStorageID(hologram["materialStorageID"]);
                modelHologramObject.setStorageID(hologram["storageID"]);
                modelHologramObject.setTargetID(hologram["targetID"]);
                modelHologramObject.setTextureFilename(hologram["textureFilename"]);
                modelHologramObject.setTextureStorageID(hologram["textureStorageID"]);
                modelHologramObject.setType(Hologram.hologramType.MODEL_HOLOGRAM);
                modelHologramObject.setTarget(targetObject);
                hologramObject = modelHologramObject;
                break;
            default:
                hologramObject = new Hologram();
                break;
        }
        entryObject.setHologram(hologramObject);

        // Create SDKs array
        bool[] sdksObject = new bool[9];
        var sdks = entry["sdks"].AsArray;
        for (j = 0; j < 9; j++)
        {
            sdksObject[j] = sdks[j];
        }
        entryObject.setSupportedSDKs(sdksObject);

        // Create Additional Data
        var additionalData = entry["additionalData"];
        foreach (var data in additionalData)
        {
            entryObject.addAdditionalData(data.Key, data.Value);
        }

        // Add entry to database
        dbObject.addEntry(entryObject);

        // Return
        return entryObject;
    }

    IEnumerator DownloadAssets(string serverURL)
    {
        Debug.Log("Downloading assets...");

        // Iterate over all database entries
        foreach (Entry entry in dbObject.getEntries())
        {
            DownloadEntryAssets(entry, serverURL);

        }
        Debug.Log("Assets downloaded.");
        yield return null;
    }

    public void DownloadEntryAssets(Entry entry, string serverURL)
    {
        // Check if Unity is supported
        //if (entry.getSupportedSDKs()[Entry.SDKs.UNITY.ordinal()])
        if (true)
        {
            // Get model hologram
            if (entry.getHologram().getType().Equals(Hologram.hologramType.MODEL_HOLOGRAM))
            {
                // Get model names and ID
                ModelHologram modelHologram = (ModelHologram)entry.getHologram();
                string[] filenames = { modelHologram.getFilename(), modelHologram.getTextureFilename(), modelHologram.getMaterialFilename() };
                string[] fileStorageIDs = { modelHologram.getStorageID(), modelHologram.getTextureStorageID(), modelHologram.getMaterialStorageID() };

                // Import Options
                ImportOptions importOptions = ParseAdditionalData(entry.getAdditionalData());

                // Download model files
                StartCoroutine(DownloadFiles(entry, serverURL, filenames, fileStorageIDs, importOptions));
            }
        }
    }

    ImportOptions ParseAdditionalData(Dictionary<string, string> additionalData)
    {
        ImportOptions importOptions = new ImportOptions();
        // Set position
        string x, y, z;
        float xFloat = 0, yFloat = 0, zFloat = 0;
        if (additionalData.TryGetValue("x", out x)) xFloat = float.Parse(x, CultureInfo.InvariantCulture);
        if (additionalData.TryGetValue("y", out y)) yFloat = float.Parse(y, CultureInfo.InvariantCulture);
        if (additionalData.TryGetValue("z", out z)) zFloat = float.Parse(z, CultureInfo.InvariantCulture);
        importOptions.localPosition = new Vector3(xFloat, yFloat, zFloat);
        // Set scale
        string scale;
        float scaleFloat = 1;
        if (additionalData.TryGetValue("scale", out scale)) scaleFloat = float.Parse(scale, CultureInfo.InvariantCulture);
        importOptions.localScale = Vector3.one * scaleFloat;
        // Set rotation
        string xAngle, yAngle, zAngle;
        float xAngleFloat = 0, yAngleFloat = 0, zAngleFloat = 0;
        if (additionalData.TryGetValue("xAngle", out xAngle)) xAngleFloat = float.Parse(xAngle, CultureInfo.InvariantCulture);
        if (additionalData.TryGetValue("yAngle", out yAngle)) yAngleFloat = float.Parse(yAngle, CultureInfo.InvariantCulture);
        if (additionalData.TryGetValue("zAngle", out zAngle)) zAngleFloat = float.Parse(zAngle, CultureInfo.InvariantCulture);
        importOptions.localEulerAngles = new Vector3(90 + xAngleFloat, 180 + yAngleFloat, 0 + zAngleFloat);
        // Return
        return importOptions;
    }

    IEnumerator DownloadFiles(Entry entry, string serverURL, string[] filenames, string[] fileStorageIDs, ImportOptions importOptions)
    {

        for (int i = 0; i < filenames.Length; ++i)
        {
            // Check for invalid files
            if (string.IsNullOrEmpty(filenames[i]) || string.IsNullOrEmpty(fileStorageIDs[i])) continue;

            // Create a new request
            UnityWebRequest www = UnityWebRequest.Get(serverURL + "&file=" + fileStorageIDs[i]);

            Debug.Log("Downloading file " + filenames[i] + " (" + fileStorageIDs[i] + ")...");

            // Yield for the request
            yield return www.Send();

            // Wait for the request to finish
            while (!www.isDone)
            {
                yield return null;
            }

            // Check for errors
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Parse response
                byte[] bytes = www.downloadHandler.data;

                // Handle repsonse
                System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + filenames[i], bytes);

                Debug.Log("File " + filenames[i] + " (" + fileStorageIDs[i] + ") downloaded and stored in " + Application.persistentDataPath);
            }

            // Cleanup
            www.disposeDownloadHandlerOnDispose = true;
            www.disposeUploadHandlerOnDispose = true;
            www.Dispose();
            www = null;
        }

        // Instantiate model
        StartCoroutine(InstantiateModel(entry, filenames, importOptions));
        yield return null;
    }

    IEnumerator InstantiateModel(Entry entry, string[] filenames, ImportOptions importOptions)
    {
        Debug.Log("Instantiating model...");

        // Refresh assets in editor
#if UNITY_EDITOR
        if (Application.isEditor) UnityEditor.AssetDatabase.Refresh();
#endif

        // Import model
        gameObject.AddComponent<ObjectImporter>().ImportModelAsync(entry, Path.GetFileNameWithoutExtension(filenames[0]), Application.persistentDataPath + "/" + filenames[0], filenames[1], null, importOptions);
        yield return null;
    }

    IEnumerator WebsocketClient()
    {
        // Instantiate a websocket
        this.gameObject.AddComponent<WClient>();
        // Set up listeners
        WClient.On(WClient.EventType.CONNECTED_TO_WS.ToString(), (string arg0) => {
            WClient.Emit(WClient.EventType.KEY.ToString(), APIKey);
        });
        WClient.On(WClient.EventType.CONNECTION_LOST.ToString(), (string arg0) => {
            
        });
        WClient.On(WClient.EventType.ADD_ENTRY.ToString(), (string message) => {
            // Parse new entry
            Entry entry = ParseEntry(JSON.Parse(message));
            // Download and instantiate content
            DownloadEntryAssets(entry, serverURL);
        });
        WClient.On(WClient.EventType.DELETE_ENTRY.ToString(), (string message) => {
            // Parse data
            string[] messageArray = message.Split('|');
            string eventType = messageArray[0];
            string entryID = messageArray[1];
            GameObject gameObjectToDestroy = null;
            // Find entry and destroy content
            foreach (CustomBehaviour cb in FindObjectsOfType<CustomBehaviour>())
            {
                if (cb.entry.getId().Equals(entryID))
                {
                    // Remove entry
                    dbObject.getEntries().Remove(cb.entry);
                    // Destroy game object
                    gameObjectToDestroy = cb.gameObject;
                    break;
                }
            }
            if (gameObjectToDestroy != null) Destroy(gameObjectToDestroy);
        });
        WClient.On(WClient.EventType.DATA_POST_ALL.ToString(), (string message) => {
            // Parse data
            string[] messageArray = message.Split('|');
            string eventType = messageArray[0];
            string dataKey = messageArray[1];
            string dataValue = messageArray[2];
            // Find entry
            foreach (Entry entry in dbObject.getEntries())
            {
                // Update or add key
                entry.removeAdditionalData(dataKey);
                entry.addAdditionalData(dataKey, dataValue);
            }
        });
        WClient.On(WClient.EventType.DATA_POST_ENTRY.ToString(), (string message) => {
            // Parse data
            string[] messageArray = message.Split('|');
            string eventType = messageArray[0];
            string entryID = messageArray[1];
            string dataKey = messageArray[2];
            string dataValue = messageArray[3];
            // Find entry
            foreach (Entry entry in dbObject.getEntries())
            {
                if (entry.getId().Equals(entryID))
                {
                    // Update key
                    entry.removeAdditionalData(dataKey);
                    entry.addAdditionalData(dataKey, dataValue);
                    break;
                }
            }
        });
        WClient.On(WClient.EventType.DATA_REMOVE_ALL.ToString(), (string message) => {
            // Parse data
            string[] messageArray = message.Split('|');
            string eventType = messageArray[0];
            string dataKey = messageArray[1];
            // Find entry
            foreach (Entry entry in dbObject.getEntries())
            {
                // Remove key
                entry.removeAdditionalData(dataKey);
            }
        });
        WClient.On(WClient.EventType.DATA_REMOVE_ENTRY.ToString(), (string message) => {
            // Parse data
            string[] messageArray = message.Split('|');
            string eventType = messageArray[0];
            string entryID = messageArray[1];
            string dataKey = messageArray[2];
            // Find entry
            foreach (Entry entry in dbObject.getEntries())
            {
                if (entry.getId().Equals(entryID))
                {
                    // Remove key
                    entry.removeAdditionalData(dataKey);
                    break;
                }
            }
        });
        WClient.On(WClient.EventType.SESSION_INFO.ToString(), (string session) => {
            Debug.Log("Session is: " + session);
        });
        yield return null;
    }
} 
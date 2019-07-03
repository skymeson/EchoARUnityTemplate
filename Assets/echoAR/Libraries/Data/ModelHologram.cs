[System.Serializable]
public class ModelHologram : Hologram
{

    private string filename;
    private string storageID;
    private string textureFilename;
    private string textureStorageID;
    private string materialFilename;
    private string materialStorageID;
    private string encodedFile;

    public ModelHologram() : base()
    {
        setType(hologramType.MODEL_HOLOGRAM);
    }

    public string getFilename()
    {
        return filename;
    }

    public void setFilename(string filename)
    {
        this.filename = filename;
    }

    public string getStorageID()
    {
        return storageID;
    }

    public void setStorageID(string storageID)
    {
        this.storageID = storageID;
    }

    public string getTextureFilename()
    {
        return textureFilename;
    }

    public void setTextureFilename(string textureFilename)
    {
        this.textureFilename = textureFilename;
    }

    public string getTextureStorageID()
    {
        return textureStorageID;
    }

    public void setTextureStorageID(string textureStorageID)
    {
        this.textureStorageID = textureStorageID;
    }

    public string getMaterialFilename()
    {
        return materialFilename;
    }

    public void setMaterialFilename(string materialFilename)
    {
        this.materialFilename = materialFilename;
    }

    public string getMaterialStorageID()
    {
        return materialStorageID;
    }

    public void setMaterialStorageID(string materialStorageID)
    {
        this.materialStorageID = materialStorageID;
    }

    public string getEncodedFile()
    {
        return encodedFile;
    }

    public void setEncodedFile(string encodedFile)
    {
        this.encodedFile = encodedFile;
    }
}

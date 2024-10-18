namespace EncryptionTrainer.UnitTest.Helpers;

public static class ImageLoading
{
    public static byte[] LoadImage(string fileName)
    {
        string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Faces", fileName);
        
        if (!File.Exists(path))
            throw new FileNotFoundException();

        byte[] bytes = File.ReadAllBytes(path);

        return bytes;
    }
}
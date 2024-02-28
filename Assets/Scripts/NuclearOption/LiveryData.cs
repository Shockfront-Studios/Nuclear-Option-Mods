public class LiveryData : UnityEngine.ScriptableObject
{
    public UnityEngine.Texture2D Texture;
    public System.Single Glossiness;
    public LiveryData.TextureColor[] Colors;
    [System.Serializable]
    public struct TextureColor
    {
        public UnityEngine.Color32 Color;
        public System.Int32 Count;
    }
}

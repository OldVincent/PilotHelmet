using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using UnityEngine.UI;

public class ViewSynchronizer : MonoBehaviour
{
    /// <summary>
    /// Name of the topic for CompressedImage.
    /// </summary>
    public string targetTopic;

    /// <summary>
    /// RawImage component to display the image.
    /// </summary>
    public RawImage targetImage;
    
    public ROSConnection connection;
    
    void Start()
    {
        connection.Subscribe<CompressedImageMsg>(targetTopic, UpdateImage);
    }
    
    void UpdateImage(CompressedImageMsg message)
    {
        if (targetImage.texture is not Texture2D)
        {
            targetImage.texture = new Texture2D(1, 1);
        }
        ((Texture2D)targetImage.texture).LoadImage(message.data);
        Debug.Log($"[CyberDiver] View updated.");
    }
}

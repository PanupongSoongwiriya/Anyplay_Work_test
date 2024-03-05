using UnityEngine;
using TMPro;

//Show device detail and FPS
public class ShowDeviceDetail : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI UIDetail;
    private string deviceDetail;

    private int lastFrameIndex;
    private float[] frameDeltaTimeArray;

    private void Awake()
    {
        frameDeltaTimeArray = new float[50];
    }

    private void Start()
    {
        deviceDetail += SetTextColor(SystemInfo.graphicsDeviceName, "orange") + "\n";
        deviceDetail += SetTextColor(SystemInfo.graphicsMemorySize.ToString(), "orange") + " MB VRAM\n";
        deviceDetail += SetTextColor(SystemInfo.processorType, "blue") + "\n";
        deviceDetail += SetTextColor(SystemInfo.systemMemorySize.ToString(), "blue") + " MB RAM\n";
        deviceDetail += SetTextColor(SystemInfo.operatingSystem, "yellow");
    }

    private void Update()
    {
        frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;

        UIDetail.text = "";
        UIDetail.text += SetTextColor(CalclateFPS().ToString("F1"), "blue") + " FPS " + SetTextColor((1000 / CalclateFPS()).ToString("F1"), "blue") + " ms\n";
        UIDetail.text += deviceDetail;
    }

    //Set text color with Rich Text
    private string SetTextColor(string text, string color)
    {
        return "<color=\"" + color + "\">" + text + "</color>";
    }

    private float CalclateFPS()
    {
        float total = 0;
        foreach (float deltaTime in frameDeltaTimeArray)
        {
            total += deltaTime;
        }

        return frameDeltaTimeArray.Length / total;
    }
}

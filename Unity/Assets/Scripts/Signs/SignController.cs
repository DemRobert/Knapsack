using TMPro;
using UnityEngine;

public class SignController : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI weightText;

    public int Id = -1;

    public void SetValue(int value)
    {
        valueText.text = value.ToString();
    }

    public void SetWeight(int weight)
    {
        weightText.text = weight.ToString();
    }
}

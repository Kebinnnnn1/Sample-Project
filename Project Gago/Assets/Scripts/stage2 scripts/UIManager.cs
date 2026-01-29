using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject lavaWarningText;
    public GameObject relicUnlockedText;
    public TextMeshProUGUI countdownText;

    public void ShowRelicUnlocked()
    {
        relicUnlockedText.SetActive(true);
    }

    public void ShowLavaWarning()
    {
        lavaWarningText.SetActive(true);
    }

    public void HideLavaWarning()
    {
        lavaWarningText.SetActive(false);
    }

    public IEnumerator Countdown(int startNumber)
    {
        countdownText.gameObject.SetActive(true);

        for (int i = startNumber; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.gameObject.SetActive(false);
    }
}

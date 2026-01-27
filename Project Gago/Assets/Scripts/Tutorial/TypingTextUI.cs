using UnityEngine;
using TMPro;
using System.Collections;

public class TypingTextUI : MonoBehaviour
{
    public TextMeshProUGUI textUI;

    Coroutine typingRoutine;
    public bool IsTyping { get; private set; }

    public void PlayText(string text, float typingSpeed)
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeRoutine(text, typingSpeed));
    }

    IEnumerator TypeRoutine(string text, float typingSpeed)
    {
        IsTyping = true;
        textUI.text = "";

        foreach (char c in text)
        {
            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
    }
}

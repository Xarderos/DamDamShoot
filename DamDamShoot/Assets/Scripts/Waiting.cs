using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class PanelFadeController : MonoBehaviour
{
    public Image panelImage;
    public Text waitingText;  // Referencia al texto de "Waiting"

    public float fadeDuration = 1f;

    private float minAlphaPanel = 80f / 255f;
    private float maxAlphaPanel = 133f / 255f;
    public float dotAnimationSpeed = 1.25f;

    private Color originalPanelColor;
    private Coroutine animateTextCoroutine;

    void Start()
    {
        originalPanelColor = panelImage.color;
        StartCoroutine(AnimateText());
        StartCoroutine(FadePanel());
    }

    private IEnumerator FadePanel()
    {
        while (true)
        {
            yield return StartCoroutine(FadeToAlpha(minAlphaPanel));
            yield return StartCoroutine(FadeToAlpha(maxAlphaPanel));
        }
    }

    private IEnumerator FadeToAlpha(float targetAlphaPanel)
    {
        float lerpTime = 0f;
        float startAlphaPanel = panelImage.color.a;

        while (lerpTime < fadeDuration)
        {
            float lerpedAlphaPanel = Mathf.Lerp(startAlphaPanel, targetAlphaPanel, lerpTime / fadeDuration);
            Color panelColor = originalPanelColor;
            panelColor.a = lerpedAlphaPanel;
            panelImage.color = panelColor;

            lerpTime += Time.deltaTime;
            yield return null;
        }

        Color finalPanelColor = originalPanelColor;
        finalPanelColor.a = targetAlphaPanel;
        panelImage.color = finalPanelColor;
    }

    private IEnumerator AnimateText()
    {
        while (true)
        {
            waitingText.text = "Waiting.";
            yield return new WaitForSeconds(dotAnimationSpeed);

            waitingText.text = "Waiting..";
            yield return new WaitForSeconds(dotAnimationSpeed);

            waitingText.text = "Waiting...";
            yield return new WaitForSeconds(dotAnimationSpeed);
        }
    }

    public void StopTextAnimation()
    {
        if (animateTextCoroutine != null)
        {
            StopCoroutine(animateTextCoroutine);  // Detener la corrutina
            waitingText.text = "Waiting...";  // Dejar los 3 puntos visibles
        }
    }
}


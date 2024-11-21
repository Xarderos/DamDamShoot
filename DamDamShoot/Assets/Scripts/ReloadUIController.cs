using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReloadUIController : MonoBehaviour
{
    public Image reloadImageP1;
    public Image reloadImageP2;
    public Sprite[] reloadSprites; 
    public float frameDuration = 0.2f; 

    private Coroutine reloadCoroutineP1 = null;
    private Coroutine reloadCoroutineP2 = null;

    public void StartReloadAnimation(bool isP1)
    {
        if (isP1)
        {
            if (reloadCoroutineP1 != null) StopCoroutine(reloadCoroutineP1);
            reloadCoroutineP1 = StartCoroutine(RunReloadAnimation(reloadImageP1));
        }
        else
        {
            if (reloadCoroutineP2 != null) StopCoroutine(reloadCoroutineP2);
            reloadCoroutineP2 = StartCoroutine(RunReloadAnimation(reloadImageP2));
        }
    }

    private IEnumerator RunReloadAnimation(Image targetImage)
    {
        targetImage.gameObject.SetActive(true); 

        for (int i = 0; i < reloadSprites.Length; i++)
        {
            targetImage.sprite = reloadSprites[i]; 
            yield return new WaitForSeconds(frameDuration); 
        }

        targetImage.gameObject.SetActive(false); 
    }
}


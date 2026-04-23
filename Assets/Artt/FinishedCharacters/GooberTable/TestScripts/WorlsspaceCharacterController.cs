using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WorldspaceCharacterController : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    public RectTransform characterImage;
    public Image imageComponent;
    public TextMeshProUGUI dialogueText;
    public CanvasGroup dialogueCanvasGroup;
    public AudioSource audioSource;
    public AudioLowPassFilter lowPassFilter;
    public CanvasGroup screenFade;

    [Header("Character")]
    public CharacterData currentCharacter;

    [Header("Breathing")]
    public float breatheSpeed = 2f;
    public float breatheAmount = 0.05f;
    public float floatAmount = 5f;

    private Vector3 baseScale;
    private Vector3 basePos;

    [Header("Timing")]
    public Vector2 dialogueInterval = new Vector2(30f, 40f);
    public Vector2 spriteInterval = new Vector2(5f, 12f);

    [Header("Dialogue")]
    public float textSpeed = 0.03f;
    public float visibleTime = 3f;
    public float fadeDuration = 1f;

    [Header("Sprite Bounce")]
    public float bounceDuration = 0.15f;
    public float squashAmount = 0.2f;
    public float stretchAmount = 0.15f;

    [Header("End Sequence FX")]
    public float shakeIntensity = 10f;
    public float shakeDuration = 2f;
    public float screenFadeDuration = 2f;

    [Header("End Degradation")]
    public float endMinPitch = 0.3f;
    public float endMaxTextSpeed = 0.15f;

    [Header("Glitch FX")]
    public float glitchChance = 0.15f;
    public float glitchDuration = 0.05f;

    [Header("Distortion FX")]
    public float distortionAmount = 0.2f;
    public float distortionSpeed = 40f;

    [Header("Audio FX")]
    public float muffledCutoff = 500f;

    private Coroutine typingCoroutine;
    private Coroutine fadeCoroutine;

    private bool isBouncing = false;
    private bool isEnding = false;

    void Start()
    {
        baseScale = characterImage.localScale;
        basePos = characterImage.localPosition;

        if (playerCamera == null)
            playerCamera = Camera.main.transform;

        ApplyCharacter(currentCharacter);
    }

    void Update()
    {
        FaceCamera();
        Breathe();
    }


    [ContextMenu("Test End Sequence")]
    void DebugEndSequence()
    {
        if (Application.isPlaying)
            PlayEndSequence();
        else
            Debug.LogWarning("Enter Play Mode first.");
    }

    public void ApplyCharacter(CharacterData newCharacter)
    {
        if (newCharacter == null) return;

        currentCharacter = newCharacter;

        StopAllCoroutines();

        if (currentCharacter.sprites.Count > 0)
            StartCoroutine(BounceSprite(currentCharacter.sprites[Random.Range(0, currentCharacter.sprites.Count)]));

        if (currentCharacter.dialogueLines.Count > 0)
            PlayDialogue(0);

        StartCoroutine(DialogueLoop());
        StartCoroutine(SpriteLoop());
    }

    void FaceCamera()
    {
        if (!playerCamera) return;
        transform.forward = playerCamera.forward;
    }

    void Breathe()
    {
        if (isBouncing || isEnding) return;

        float t = Time.time * breatheSpeed;

        float scale = Mathf.Sin(t) * breatheAmount;
        float floatY = Mathf.Sin(t * 0.8f) * floatAmount;

        characterImage.localScale = baseScale + Vector3.one * scale;
        characterImage.localPosition = basePos + new Vector3(0, floatY, 0);
    }

    IEnumerator SpriteLoop()
    {
        while (!isEnding)
        {
            yield return new WaitForSeconds(Random.Range(spriteInterval.x, spriteInterval.y));

            if (currentCharacter.sprites.Count == 0) continue;

            StartCoroutine(BounceSprite(currentCharacter.sprites[Random.Range(0, currentCharacter.sprites.Count)]));
        }
    }

    IEnumerator BounceSprite(Sprite newSprite)
    {
        isBouncing = true;

        Vector3 originalScale = baseScale;
        float t = 0;

        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            float squash = Mathf.Lerp(0, squashAmount, t / bounceDuration);
            characterImage.localScale = originalScale + new Vector3(squash, -squash, 0);
            yield return null;
        }

        imageComponent.sprite = newSprite;

        t = 0;

        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            float stretch = Mathf.Lerp(stretchAmount, 0, t / bounceDuration);
            characterImage.localScale = originalScale + new Vector3(-stretch, stretch, 0);
            yield return null;
        }

        characterImage.localScale = originalScale;
        isBouncing = false;
    }

    IEnumerator DialogueLoop()
    {
        yield return new WaitForSeconds(Random.Range(dialogueInterval.x, dialogueInterval.y));

        while (!isEnding)
        {
            if (currentCharacter.dialogueLines.Count > 1)
            {
                int index = Random.Range(1, currentCharacter.dialogueLines.Count);
                PlayDialogue(index);
            }

            yield return new WaitForSeconds(Random.Range(dialogueInterval.x, dialogueInterval.y));
        }
    }

    public void PlayDialogue(int index)
    {
        if (currentCharacter == null) return;
        if (index < 0 || index >= currentCharacter.dialogueLines.Count) return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(currentCharacter.dialogueLines[index]));
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        dialogueCanvasGroup.alpha = 1;

        foreach (char c in line)
        {
            dialogueText.text += c;

            if (audioSource && currentCharacter.blipSound && c != ' ')
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(currentCharacter.blipSound);
            }

            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(visibleTime);
        fadeCoroutine = StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            dialogueCanvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        dialogueCanvasGroup.alpha = 0;
    }

    // END SEQUENCE HERE THIS IS THE THING TO TRIGGER LIANA <3
  
    public void PlayEndSequence()
    {
        StopAllCoroutines();
        StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        isEnding = true;

        if (currentCharacter == null || string.IsNullOrEmpty(currentCharacter.finalDialogue))
            yield break;

        yield return StartCoroutine(TypeFinalLine(currentCharacter.finalDialogue));

        StartCoroutine(ShakeCharacter());
        yield return StartCoroutine(FadeCharacterOut());

        yield return StartCoroutine(FadeToBlack());
    }

    IEnumerator TypeFinalLine(string line)
    {
        dialogueText.text = "";
        dialogueCanvasGroup.alpha = 1;

        int totalChars = line.Length;

        for (int i = 0; i < totalChars; i++)
        {
            float progress = (float)i / totalChars;

            char c = line[i];
            dialogueText.text += c;

            if (Random.value < glitchChance && c != ' ')
                StartCoroutine(GlitchText(line, i));

            float pitch = Mathf.Lerp(currentCharacter.finalPitch, endMinPitch, progress);
            float delay = Mathf.Lerp(currentCharacter.finalTextSpeed, endMaxTextSpeed, progress);

            if (lowPassFilter)
                lowPassFilter.cutoffFrequency = Mathf.Lerp(22000f, muffledCutoff, progress);

            if (audioSource && currentCharacter.blipSound && c != ' ')
            {
                audioSource.pitch = Random.Range(pitch - 0.03f, pitch + 0.03f);
                audioSource.PlayOneShot(currentCharacter.blipSound);
            }

            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator GlitchText(string original, int index)
    {
        char[] chars = original.ToCharArray();
        chars[index] = (char)Random.Range(33, 126);

        dialogueText.text = new string(chars);

        yield return new WaitForSeconds(glitchDuration);

        dialogueText.text = original.Substring(0, index + 1);
    }

    IEnumerator ShakeCharacter()
    {
        float t = 0;
        Vector3 originalPos = characterImage.localPosition;
        Vector3 originalScale = baseScale;

        while (t < shakeDuration)
        {
            t += Time.deltaTime;

            float strength = Mathf.Lerp(shakeIntensity, 0, t / shakeDuration);

            characterImage.localPosition = originalPos + new Vector3(
                Random.Range(-1f, 1f) * strength,
                Random.Range(-1f, 1f) * strength,
                0
            );

            float dx = Mathf.Sin(Time.time * distortionSpeed) * distortionAmount;
            float dy = Mathf.Cos(Time.time * distortionSpeed) * distortionAmount;

            characterImage.localScale = originalScale + new Vector3(dx, dy, 0);

            yield return null;
        }

        characterImage.localPosition = originalPos;
        characterImage.localScale = originalScale;
    }

    IEnumerator FadeCharacterOut()
    {
        float t = 0;

        Color img = imageComponent.color;
        Color txt = dialogueText.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1, 0, t / fadeDuration);

            img.a = a;
            txt.a = a;

            imageComponent.color = img;
            dialogueText.color = txt;

            yield return null;
        }
    }

    IEnumerator FadeToBlack()
    {
        float t = 0;

        while (t < screenFadeDuration)
        {
            t += Time.deltaTime;
            screenFade.alpha = Mathf.Lerp(0, 1, t / screenFadeDuration);
            yield return null;
        }

        screenFade.alpha = 1;
    }
}
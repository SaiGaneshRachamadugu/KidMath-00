using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI wrongansText;
    [SerializeField] private TextMeshProUGUI wrightText;
    [SerializeField] private GameObject[] optionButtons;
    [SerializeField] private TextMeshProUGUI starfishCountText;
    [SerializeField] private GameObject multiplty;
    [SerializeField] private GameObject hudI;

    [Header("Audio")]
    [SerializeField] private AudioClip collectedAudio;
    [SerializeField] private AudioClip wrongAnswerAudio;
    public AudioSource newAudiosource;
    // [SerializeField] private AudioClip startup;

    private AudioSource audioSource;

    [Header("Starfish Reward")]
    [SerializeField] private GameObject starfishPrefab;
    [SerializeField] private Transform starfishTarget;

    [Header("Shop Link")]
    [SerializeField] private ShopManager shopManager;
    [Header("Particles")]
    [SerializeField] private ParticleSystem confetti;
    [SerializeField] private GameObject help;
    [SerializeField] private GameObject yaay;


    private int starfishCount = 0;
    private int correctAnswer, a, b, correctIndex;
    private Coroutine hintCoroutine;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();

    public Image[] inputIndex1;
    public Image[] inputIndex2;

    public static GameManager Instance;
    private Dictionary<Button, Color> originalButtonColors = new Dictionary<Button, Color>();

    [Header("UI Reference")]
    //public TextMeshProUGUI tipText;

    [Header("Tip Settings")]
    public float minDelayBetweenTips = 2f;
    public float maxDelayBetweenTips = 3f;
    public float tipDisplayDuration = 6f;

    private List<string> tipsList = new List<string>
    {
        "You can count the items to find the answer!",
        "Groups mean the same number together. Like 2 + 2 + 2!",
        "Try saying the numbers out loud!",
        "Look closely at the groups — they are clues!",
        "Multiplying is like adding again and again.",
        "You’re doing amazing!",
        "Smart kids keep trying!",
        "Catch the correct fish and win a star!",
        "Practice makes math fun!",
        "Starfish love smart players!"
    };

    private Coroutine tipsCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
    }
    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        GenerateQuestion();

        if (shopManager.CurrentSelectedEnvironment == "ClassRoom")
        {
            multiplty.SetActive(false);
            hudI.SetActive(false);
        }
        else
        {
            multiplty.SetActive(true);
            hudI.SetActive(true);
        }

        foreach (var btnObj in optionButtons)
        {
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null && btn.image != null)
            {
                originalButtonColors[btn] = btn.image.color;
            }
        }
        tipsCoroutine = StartCoroutine(ShowTipsRoutine());

    }

    public void OnEnvironmentChange()
    {
        questionText.text = "";

        if (shopManager.CurrentSelectedEnvironment == "ClassRoom")
        {
            multiplty.SetActive(false);
            hudI.SetActive(false);
        }
        else
        {
            multiplty.SetActive(true);
            hudI.SetActive(true);
        }
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        feedbackText.text = "";
        a = Random.Range(1, 5);
        b = Random.Range(1, 5);
        correctAnswer = a * b;
        correctIndex = Random.Range(0, optionButtons.Length);

        ResetImages();
        if (shopManager.CurrentSelectedEnvironment != "ClassRoom")
        {
            questionText.text = "?";
            AssignIcons(a, b);
        }
        else
        {
            questionText.text = $"{a} × {b} = ?";
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int answer = (i == correctIndex) ? correctAnswer : GetUniqueWrongAnswer();
            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answer.ToString();

            Button btn = optionButtons[i].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            AddButtonListener(btn, answer);
        }

        foreach (var btnObj in optionButtons)
        {
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null && btn.image != null && originalButtonColors.ContainsKey(btn))
            {
                btn.image.color = originalButtonColors[btn];
            }
        }
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
        }
        hintCoroutine = StartCoroutine(ShowHintAfterDelay(6f));
    }

    void AddButtonListener(Button btn, int answer)
    {
        btn.onClick.AddListener(() => CheckAnswer(answer, btn.transform));
    }

    void AssignIcons(int inputA, int inputB)
    {
        for (int i = 0; i < inputIndex1.Length; i++)
        {
            inputIndex1[i].gameObject.SetActive(i < inputA);
            if (i < inputA && shopManager.currentSelectedIcon != null)
            {
                inputIndex1[i].sprite = shopManager.currentSelectedIcon;

                // Reset scale for safe repeat animation
                inputIndex1[i].transform.localScale = Vector3.zero;

                // Animate scale bounce with delay for a fun staggered effect
                inputIndex1[i].transform.DOScale(1f, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * 0.1f);
                //audioSource.PlayOneShot(startup); // Play startup sound when icons are assigned
            }
        }

        for (int i = 0; i < inputIndex2.Length; i++)
        {
            inputIndex2[i].gameObject.SetActive(i < inputB);
            if (i < inputB && shopManager.currentSelectedIcon != null)
            {
                inputIndex2[i].sprite = shopManager.currentSelectedIcon;

                inputIndex2[i].transform.localScale = Vector3.zero;

                inputIndex2[i].transform.DOScale(1f, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * 0.1f + 0.3f);
                //audioSource.PlayOneShot(startup); //Play startup sound when icons are assigned

            }
        }
    }


    int GetUniqueWrongAnswer()
    {
        int wrong;
        do { wrong = Random.Range(1, 35); }
        while (wrong == correctAnswer);
        return wrong;
    }

    void CheckAnswer(int selected, Transform clickedBtn)
    {
        if (selected == correctAnswer)
        {
            feedbackText.text = "Correct!";
            wrightText.text = "Well done! You got it right!";
            if (shopManager.CurrentSelectedEnvironment != "ClassRoom")
            {
                questionText.text = correctAnswer.ToString();
            }
            else
            {
                questionText.text = $"{a} × {b} = {correctAnswer}";
            }
            OnCorrectAnswer();

            // Stop any existing hint coroutine
            if (hintCoroutine != null)
            {
                StopCoroutine(hintCoroutine);
                hintCoroutine = null;
            }

            Coroutine starfishCoroutine = StartCoroutine(AnimateStarfish(clickedBtn));
            activeCoroutines.Add(starfishCoroutine);
            StartCoroutine(HideYaay(2f));
        }
        else
        {
            help.gameObject.SetActive(true);
            help.gameObject.GetComponent<DOTweenAnimation>().enabled = true;
            help.gameObject.GetComponent<DOTweenAnimation>().DORestart();
            help.gameObject.GetComponentInChildren<DOTweenAnimation>().enabled = true;
            help.gameObject.GetComponentInChildren<DOTweenAnimation>().DORestart();
            audioSource.PlayOneShot(wrongAnswerAudio);
            string[] wrongTips = new string[]
     {
    "You're a clever crab! Try again!",
    "Count one group and multiply by how many groups!"
    };

            wrongansText.text = wrongTips[Random.Range(0, wrongTips.Length)];
            StartCoroutine(Hidehelp(2f));
        }
    }

    IEnumerator Hidehelp(float delay)
    {
        yield return new WaitForSeconds(delay);
        help.gameObject.SetActive(false);
        help.gameObject.GetComponent<DOTweenAnimation>().DOComplete();
        help.gameObject.GetComponentInChildren<DOTweenAnimation>().DOComplete();


    }
   
    IEnumerator HideYaay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        yaay.gameObject.SetActive(false);
        yaay.GetComponent<DOTweenAnimation>().DOComplete();
        yaay.GetComponentInChildren<DOTweenAnimation>().DOComplete();
    }

    void ResetImages()
    {
       
        for (int i = 0; i < inputIndex1.Length; i++)
        {
            inputIndex1[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < inputIndex2.Length; i++)
        {
            inputIndex2[i].gameObject.SetActive(false);
        }
    }
    IEnumerator ShowTipsRoutine()
    {
        while (true)
        {
            // Pick a random tip
            string randomTip = tipsList[Random.Range(0, tipsList.Count)];

           // tipText.text = randomTip;
           // tipText.gameObject.SetActive(true);

            yield return new WaitForSeconds(tipDisplayDuration);

            // Hide the tip
           // tipText.gameObject.SetActive(false);

            // Wait for a random delay before showing the next tip
            float delay = Random.Range(minDelayBetweenTips, maxDelayBetweenTips);
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator AnimateStarfish(Transform startTransform)
    {
        GameObject starfish = Instantiate(starfishPrefab, startTransform.position, Quaternion.identity, startTransform.root);
        RectTransform starfishRect = starfish.GetComponent<RectTransform>();

        Vector3 startPos = starfishRect.position;
        Vector3 endPos = starfishTarget.position;

        if (audioSource != null && collectedAudio != null)
            audioSource.PlayOneShot(collectedAudio);
        yaay.SetActive(true);
        yaay.GetComponent<DOTweenAnimation>().enabled = true;
        yaay.GetComponent<DOTweenAnimation>().DORestart();
        yaay.GetComponentInChildren<DOTweenAnimation>().enabled = true;

        confetti.Play();
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            starfishRect.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        if (starfish != null)
            Destroy(starfish);

        // starfishCount++;
        //UpdateStarfishCount();
        ResetImages();
        GenerateQuestion();
    }

    IEnumerator ShowHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (correctIndex < optionButtons.Length)
        {
            GameObject correctBtn = optionButtons[correctIndex];
            Image img = correctBtn.GetComponent<Image>();
            if (img != null && originalButtonColors.ContainsKey(correctBtn.GetComponent<Button>()))
            {
                Color originalColor = originalButtonColors[correctBtn.GetComponent<Button>()];

                for (int i = 0; i < 3; i++)
                {
                    img.color = Color.yellow;
                    yield return new WaitForSeconds(0.5f);
                    img.color = originalColor;
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }


    void OnCorrectAnswer()
    {
        if (shopManager != null)
            shopManager.AddCoins(1); //reset to value 1 after testing//
    }

    void OnDisable()
    {
        // Stop all active coroutines
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }

        foreach (var coroutine in activeCoroutines)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        activeCoroutines.Clear();

        // Clean up button listeners
        foreach (var btn in optionButtons)
        {
            if (btn != null)
            {
                Button button = btn.GetComponent<Button>();
                if (button != null)
                    button.onClick.RemoveAllListeners();
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    void OnDestroy()
    {
        OnDisable(); // Ensure cleanup happens on destroy too
    }

    void UpdateStarfishCount()
    {
        if (starfishCountText != null)
            starfishCountText.text = starfishCount.ToString();
    }
}
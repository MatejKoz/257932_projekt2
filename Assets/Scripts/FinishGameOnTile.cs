using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public sealed class FinishGameOnTile : MonoBehaviour
{
    [SerializeField] private Transform startSurface;
    [SerializeField] private Transform finishSurface;
    [SerializeField] private Vector2 surfaceHalfExtents = new Vector2(0.5f, 0.5f);

    private Rigidbody playerRigidbody;
    private Vector3 finishPosition;
    private bool gameFinished;
    private GameObject instructionsObject;
    private GameObject gameOverObject;

    private void Awake()
    {
        if (startSurface == null || finishSurface == null)
        {
            Debug.LogError("Start and finish surfaces must be assigned.", this);
            enabled = false;
            return;
        }

        playerRigidbody = GetComponent<Rigidbody>();
        CreateMessages();
    }

    private void LateUpdate()
    {
        if (gameFinished)
        {
            LockPlayerAtFinish();
            return;
        }

        Vector3 position = transform.position;
        bool isOnStart =
            Mathf.Abs(position.x - startSurface.position.x) <= surfaceHalfExtents.x &&
            Mathf.Abs(position.z - startSurface.position.z) <= surfaceHalfExtents.y;
        instructionsObject.SetActive(isOnStart);

        if (Mathf.Abs(position.x - finishSurface.position.x) > surfaceHalfExtents.x ||
            Mathf.Abs(position.z - finishSurface.position.z) > surfaceHalfExtents.y)
        {
            return;
        }

        gameFinished = true;
        finishPosition = position;

        ScriptMachine movementMachine = GetComponent<ScriptMachine>();
        if (movementMachine != null)
        {
            movementMachine.enabled = false;
        }

        instructionsObject.SetActive(false);
        gameOverObject.SetActive(true);
        LockPlayerAtFinish();
    }

    private void LockPlayerAtFinish()
    {
        transform.position = finishPosition;

        if (playerRigidbody != null)
        {
            playerRigidbody.position = finishPosition;
        }
    }

    private void CreateMessages()
    {
        GameObject canvasObject = new GameObject(
            "Game Messages Canvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        instructionsObject = CreateMessage(
            canvasObject.transform,
            "Movement Instructions",
            "POHYB DOPŘEDU W\nOTÁČENÍ R",
            54f);
        gameOverObject = CreateMessage(
            canvasObject.transform,
            "Game Over Text",
            "KONEC HRY",
            72f);
        gameOverObject.SetActive(false);
    }

    private static GameObject CreateMessage(
        Transform parent,
        string objectName,
        string message,
        float fontSize)
    {
        GameObject textObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = message;
        text.color = Color.green;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;

        return textObject;
    }
}

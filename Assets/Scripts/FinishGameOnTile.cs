using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public sealed class FinishGameOnTile : MonoBehaviour
{
    [SerializeField] private Transform finishSurface;
    [SerializeField] private Vector2 surfaceHalfExtents = new Vector2(0.5f, 0.5f);

    private Rigidbody playerRigidbody;
    private Vector3 finishPosition;
    private bool gameFinished;

    private void Awake()
    {
        if (finishSurface == null)
        {
            Debug.LogError("Finish surface is not assigned.", this);
            enabled = false;
            return;
        }

        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (gameFinished)
        {
            LockPlayerAtFinish();
            return;
        }

        Vector3 position = transform.position;
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

        ShowGameOverMessage();
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

    private static void ShowGameOverMessage()
    {
        GameObject canvasObject = new GameObject(
            "Game Over Canvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject textObject = new GameObject(
            "Game Over Text",
            typeof(RectTransform),
            typeof(TextMeshProUGUI));
        textObject.transform.SetParent(canvasObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = "KONEC HRY";
        text.color = Color.green;
        text.fontSize = 72f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
    }
}

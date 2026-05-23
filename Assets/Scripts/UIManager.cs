// ============================================================
// UIManager.cs
// Smart Relaxation Room — SE4051 Individual Assignment
// Author: Ishara Kumarage
//
// Manages the overall HUD — shows/hides the mood panel,
// and handles the ESC hint so the player knows to press
// Escape before clicking UI buttons.
//
// Attach to the Canvas or a dedicated UIManager object.
// ============================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD Panels")]
    [Tooltip("The panel containing mood selection buttons")]
    public GameObject moodPanel;

    [Header("Instruction & Hint Text")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI escHintText;

    [Header("Toggle Mood Panel Button (optional)")]
    public Button toggleMoodPanelButton;

    private bool moodPanelVisible = true;

    void Start()
    {
        if (instructionText != null)
            instructionText.text = "Use WASD to move. Use mouse to look around.\nSelect your current mood or walk into a zone.";

        if (escHintText != null)
            escHintText.text = "Press ESC to show cursor and interact with buttons.";

        if (toggleMoodPanelButton != null)
            toggleMoodPanelButton.onClick.AddListener(ToggleMoodPanel);

        // Make sure mood panel starts visible
        if (moodPanel != null)
            moodPanel.SetActive(true);
    }

    void Update()
    {
        // Show/hide ESC hint based on cursor visibility
        if (escHintText != null)
        {
            bool cursorVisible = Cursor.visible;
            escHintText.text = cursorVisible
                ? "Click a button above to change mood. Press ESC to re-lock mouse."
                : "Press ESC to show cursor and interact with buttons.";
        }
    }

    public void ToggleMoodPanel()
    {
        if (moodPanel == null) return;
        moodPanelVisible = !moodPanelVisible;
        moodPanel.SetActive(moodPanelVisible);
    }
}

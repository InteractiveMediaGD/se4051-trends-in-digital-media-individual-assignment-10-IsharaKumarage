// ============================================================
// MoodButtonController.cs
// Smart Relaxation Room — SE4051 Individual Assignment
// Author: Ishara Kumarage
//
// Connects the Canvas UI mood buttons to the EnvironmentManager.
// Attach this script to the Canvas (or a UIManager object).
// Wire up the EnvironmentManager reference in the Inspector.
// Then assign each button's onClick event in the Inspector to
// call the corresponding method on this script.
// ============================================================

using UnityEngine;
using UnityEngine.UI;

public class MoodButtonController : MonoBehaviour
{
    // ── Inspector References ──────────────────────────────────────
    [Header("Environment Manager")]
    [Tooltip("Drag the EnvironmentManager GameObject here")]
    public EnvironmentManager envManager;

    [Header("Mood Buttons (assign in Inspector)")]
    public Button stressedButton;   // Stressed → Calm Mode
    public Button tiredButton;      // Tired    → Energy Mode
    public Button unfocusedButton;  // Unfocused→ Focus Mode
    public Button reduceButton;     // Accessibility reduce effects

    // ── Button highlight colors ───────────────────────────────────
    [Header("Button Highlight Colors")]
    public Color calmHighlight   = new Color(0.4f, 0.6f, 1.0f);
    public Color energyHighlight = new Color(1.0f, 0.65f, 0.2f);
    public Color focusHighlight  = new Color(0.9f, 0.95f, 1.0f);

    // ── Unity lifecycle ──────────────────────────────────────────
    void Start()
    {
        // Register button listeners programmatically as a backup
        // (the Inspector onClick can also be used — both are safe)
        if (stressedButton  != null) stressedButton.onClick.AddListener(OnStressedClicked);
        if (tiredButton     != null) tiredButton.onClick.AddListener(OnTiredClicked);
        if (unfocusedButton != null) unfocusedButton.onClick.AddListener(OnUnfocusedClicked);
        if (reduceButton    != null) reduceButton.onClick.AddListener(OnReduceEffectsClicked);

        if (envManager == null)
            Debug.LogError("[MoodButtonController] EnvironmentManager is not assigned!");
    }

    // ── Button handlers ──────────────────────────────────────────

    /// <summary>Stressed button → activates Calm Mode</summary>
    public void OnStressedClicked()
    {
        if (envManager == null) return;
        envManager.SetCalmMode();
        HighlightButton(stressedButton, calmHighlight);
        ResetOtherButtons(stressedButton);
        Debug.Log("[MoodButtonController] Stressed → Calm Mode.");
    }

    /// <summary>Tired button → activates Energy Mode</summary>
    public void OnTiredClicked()
    {
        if (envManager == null) return;
        envManager.SetEnergyMode();
        HighlightButton(tiredButton, energyHighlight);
        ResetOtherButtons(tiredButton);
        Debug.Log("[MoodButtonController] Tired → Energy Mode.");
    }

    /// <summary>Unfocused button → activates Focus Mode</summary>
    public void OnUnfocusedClicked()
    {
        if (envManager == null) return;
        envManager.SetFocusMode();
        HighlightButton(unfocusedButton, focusHighlight);
        ResetOtherButtons(unfocusedButton);
        Debug.Log("[MoodButtonController] Unfocused → Focus Mode.");
    }

    private bool isReducedActive = false;

    /// <summary>Reduce Effects button → accessibility feature</summary>
    public void OnReduceEffectsClicked()
    {
        if (envManager == null) return;
        envManager.ReduceEffects();

        isReducedActive = !isReducedActive;

        // Visual feedback for the accessibility toggle
        if (reduceButton != null)
        {
            var image = reduceButton.GetComponent<Image>();
            if (image != null)
            {
                // Highlight with a softer, dimmer color when active
                image.color = isReducedActive ? new Color(0.3f, 0.4f, 0.3f) : new Color(0.2f, 0.2f, 0.25f);
            }
        }

        Debug.Log("[MoodButtonController] Reduce Effects toggled.");
    }

    // ── Visual Feedback Helpers ───────────────────────────────────

    private void HighlightButton(Button active, Color color)
    {
        if (active == null) return;
        var image = active.GetComponent<Image>();
        if (image != null) image.color = color;
    }

    private void ResetOtherButtons(Button except)
    {
        Button[] allMoodButtons = { stressedButton, tiredButton, unfocusedButton };
        Color defaultColor = new Color(0.2f, 0.2f, 0.25f); // Dark grey default

        foreach (var btn in allMoodButtons)
        {
            if (btn == null || btn == except) continue;
            var image = btn.GetComponent<Image>();
            if (image != null) image.color = defaultColor;
        }
    }
}

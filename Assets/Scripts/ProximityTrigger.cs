// ============================================================
// ProximityTrigger.cs
// Smart Relaxation Room — SE4051 Individual Assignment
// Author: Ishara Kumarage
//
// Detects when the Player enters a trigger zone and calls
// the corresponding method on EnvironmentManager.
//
// HOW TO USE:
//   1. Add a BoxCollider (or SphereCollider) to each trigger zone.
//   2. Enable "Is Trigger" on that collider.
//   3. Attach this script to the same GameObject.
//   4. Set the TriggerType in the Inspector.
//   5. Drag the EnvironmentManager object into the "envManager" slot.
// ============================================================

using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
    // ── Trigger Type Enum ─────────────────────────────────────────
    public enum TriggerType
    {
        Calm,    // RelaxTrigger  → activates Calm Mode
        Focus,   // FocusTrigger  → activates Focus Mode
        Energy,  // EnergyTrigger → activates Energy Mode
        Lamp,    // LampTrigger   → turns lamp light on
        Plant    // PlantTrigger  → shows encouraging message
    }

    // ── Inspector Settings ────────────────────────────────────────
    [Header("Trigger Configuration")]
    [Tooltip("What this trigger does when the player enters it")]
    public TriggerType triggerType = TriggerType.Calm;

    [Tooltip("Reference to the scene's EnvironmentManager")]
    public EnvironmentManager envManager;

    // ── Unity Trigger Callbacks ──────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        // Only react to the Player
        if (!other.CompareTag("Player")) return;

        if (envManager == null)
        {
            Debug.LogWarning($"[ProximityTrigger] EnvironmentManager is not assigned on {gameObject.name}!");
            return;
        }

        // Dispatch to the correct EnvironmentManager method
        switch (triggerType)
        {
            case TriggerType.Calm:
                envManager.SetCalmMode();
                Debug.Log($"[ProximityTrigger] Player entered Calm zone: {gameObject.name}");
                break;

            case TriggerType.Focus:
                envManager.SetFocusMode();
                Debug.Log($"[ProximityTrigger] Player entered Focus zone: {gameObject.name}");
                break;

            case TriggerType.Energy:
                envManager.SetEnergyMode();
                Debug.Log($"[ProximityTrigger] Player entered Energy zone: {gameObject.name}");
                break;

            case TriggerType.Lamp:
                envManager.ActivateLamp();
                Debug.Log($"[ProximityTrigger] Player entered Lamp zone: {gameObject.name}");
                break;

            case TriggerType.Plant:
                envManager.ShowPlantMessage();
                Debug.Log($"[ProximityTrigger] Player entered Plant zone: {gameObject.name}");
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // When the player leaves the Lamp zone, turn the lamp off
        if (triggerType == TriggerType.Lamp && envManager != null)
        {
            envManager.DeactivateLamp();
            Debug.Log($"[ProximityTrigger] Player exited Lamp zone: {gameObject.name}");
        }
    }

    // ── Editor Helper — draws a colored gizmo in Scene view ──────
    private void OnDrawGizmos()
    {
        Color gizmoColor = triggerType switch
        {
            TriggerType.Calm   => new Color(0.4f, 0.6f, 1.0f, 0.35f),
            TriggerType.Focus  => new Color(1.0f, 1.0f, 1.0f, 0.35f),
            TriggerType.Energy => new Color(1.0f, 0.65f, 0.2f, 0.35f),
            TriggerType.Lamp   => new Color(1.0f, 1.0f, 0.4f, 0.35f),
            TriggerType.Plant  => new Color(0.4f, 0.9f, 0.4f, 0.35f),
            _                  => Color.white
        };

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}

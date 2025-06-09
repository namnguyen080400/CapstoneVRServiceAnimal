using System.Collections;
using UnityEngine;

public class EnvironmentColliderDebugger : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Wait 1–2 seconds to ensure MRUK anchors are initialized
        yield return new WaitForSeconds(2.0f);

        var allColliders = FindObjectsOfType<Collider>();
        Debug.Log($"Nam11: Found {allColliders.Length} colliders in dog scene");

        foreach (var col in allColliders)
        {
            GameObject go = col.gameObject;
            var meshFilter = go.GetComponent<MeshFilter>();
            var meshRenderer = go.GetComponent<MeshRenderer>();

            Debug.Log($"Nam11 EnvironmentColliderDebugger Collider on {go.name}");
            Debug.Log($"Nam11 EnvironmentColliderDebugger isTrigger: {col.isTrigger}");
            Debug.Log($"Nam11 EnvironmentColliderDebugger enabled: {col.enabled}");
            Debug.Log($"Nam11 EnvironmentColliderDebugger layer: {go.layer} ({LayerMask.LayerToName(go.layer)})");
            Debug.Log($"Nam11 EnvironmentColliderDebugger has MeshFilter: {meshFilter != null}");
            Debug.Log($"Nam11 EnvironmentColliderDebugger has MeshRenderer: {meshRenderer != null}");
            Debug.Log($"Nam11 EnvironmentColliderDebugger has MeshCollider: {go.GetComponent<MeshCollider>() != null}");
            Debug.Log($"Nam11 EnvironmentColliderDebugger bounds: {col.bounds}");
        }
    }
}

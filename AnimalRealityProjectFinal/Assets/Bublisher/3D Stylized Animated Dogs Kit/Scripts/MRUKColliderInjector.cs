using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections;

public class MRUKColliderInjector : MonoBehaviour
{
    IEnumerator Start()
    {
        while (MRUK.Instance == null || MRUK.Instance.GetCurrentRoom().Anchors.Count == 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(3.0f); // Wait for anchors to load

        var anchors = FindObjectsOfType<MRUKAnchor>();
        Debug.Log($"Nam11: MRUKColliderInjector Found {anchors.Length} MRUK anchors");

        foreach (var anchor in anchors)
        {
            if (!anchor.TryGetComponent<MeshCollider>(out _))
            {
                var mc = anchor.gameObject.AddComponent<MeshCollider>();
                mc.convex = false; // Set false for static environment
                Debug.Log("Nam11: MRUKColliderInjector MeshCollider added to: " + anchor.name);
            }
        }
    }
}

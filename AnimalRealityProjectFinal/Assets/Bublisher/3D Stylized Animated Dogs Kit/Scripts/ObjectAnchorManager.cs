using UnityEngine;
using Meta.XR.MRUtilityKit;

public class ObjectAnchorManager : MonoBehaviour
{
    public GameObject curryPlate;
    public GameObject tennisBall;
    public GameObject dog;

    private void Start()
    {
        if (MRUK.Instance != null)
        {
            Debug.Log("Nam11 ObjectAnchorManager Start");
            MRUK.Instance.RoomCreatedEvent.AddListener(OnRoomCreated);
        }
    }

    private void OnRoomCreated(MRUKRoom room)
    {
        Debug.Log("Nam11 Room created, anchoring objects...");

        // Parent all objects to the room
        curryPlate.transform.SetParent(room.transform, false);
        curryPlate.transform.localPosition = new Vector3(1.5f, 0.0f, 2.0f);

        tennisBall.transform.SetParent(room.transform, false);
        tennisBall.transform.localPosition = new Vector3(1.5f, 0.0f, 2.5f);

        dog.transform.SetParent(room.transform, false);
        dog.transform.localPosition = new Vector3(1.0f, 0.0f, 3.0f);
    }
}

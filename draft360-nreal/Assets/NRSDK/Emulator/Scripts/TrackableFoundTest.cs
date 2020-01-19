using UnityEngine;

public class TrackableFoundTest : MonoBehaviour
{
    public TrackableObserver Observer;
    public GameObject Obj;
    void Start()
    {
#if !UNITY_EDITOR
        Destroy(GameObject.Find("EmulatorRoom"));
#endif
        Obj.SetActive(false);
        Observer.FoundEvent += Found;
        Observer.LostEvnet += Lost;
    }

    private void Found(Vector3 pos, Quaternion qua)
    {
        Obj.transform.position = pos;
        Obj.transform.rotation = qua;
        Obj.SetActive(true);
    }
    private void Lost()
    {
        Obj.SetActive(false);
    }
}

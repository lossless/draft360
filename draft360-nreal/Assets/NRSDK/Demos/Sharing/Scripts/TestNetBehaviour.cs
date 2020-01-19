namespace NRKernal.NRExamples
{
    using NRToolkit.Sharing;
    using NRToolkit.Sharing.AutoGenerate;
    using UnityEngine;

    public class TestNetBehaviour : NetworkBehaviour
    {
        public SynTransform SynTransform;
        public SynInt SynInt;
        public SynVector2 SynVector2;
        public SynVector3 SynVector3;
        public SynQuaternion SynQuaternion;

        private void Awake()
        {
            SynTransform = new SynTransform();
            SynInt = new SynInt();
            SynVector2 = new SynVector2();
            SynVector3 = new SynVector3();
            SynQuaternion = new SynQuaternion();

            SynTransform.position = Random.insideUnitSphere * 3;
        }

        public override void Initialize(NetObjectInfo info)
        {
            base.Initialize(info);
            gameObject.GetComponent<MeshRenderer>().material.color = IsOwner ? Color.red : Color.green;
        }

        void Update()
        {
            if (IsOwner)
            {
                SynTransform.rotation = Quaternion.Euler(transform.eulerAngles + Time.deltaTime * 50 * Vector3.up);
                SynInt.value += 1;
                SynVector2.value += Vector2.one;
                SynVector3.value += Vector3.one;
                SynQuaternion.value = SynTransform.rotation;
            }

            transform.position = SynTransform.position;
            transform.rotation = SynTransform.rotation;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.RPC("Hello");
            }
        }

        public void Hello()
        {
            Debug.Log("Hello world!");
        }
    }
}

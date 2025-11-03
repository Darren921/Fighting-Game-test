using System.Net.NetworkInformation;
using UnityEngine;

public class CameraDistance : MonoBehaviour
{
    
        public Transform fighter1;
        public Transform fighter2;
        public float zoomSpeed = 2f;
        public float minZoom = 5f;
        public float maxZoom = 15f;
        public float margin = 2f;
        public float scale;

        private Camera cam;
        [SerializeField ]Vector3 playerView;
    
    

    void Start()
        {
            cam = GetComponent<Camera>();
        }

        void Update()
        {
            float dis = Vector3.Distance(fighter1.position, fighter2.position) * scale;
            if (dis < minZoom)
            {
                cam.fieldOfView = minZoom;
            }
            else if (dis > maxZoom)

            {
                cam.fieldOfView = maxZoom;
            }
            else if (dis < maxZoom && dis > minZoom)
            {
                cam.fieldOfView = dis;
            }
            playerView = cam.WorldToViewportPoint(fighter1.position);
            
        }
    
}

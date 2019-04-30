using System.Linq;
using UnityEngine;

namespace Tanaka
{
    public enum TouchInfo
    {
        None = 99,

        // UnityEngine.TouchPhaseに対応
        Began = 0,
        Moved,
        Stationary,
        Ended,
        Canceled,
    }

    public static class UtilTouch
    {
        private static Vector3 TouchPosition = Vector3.zero;

        public static TouchInfo GetTouch()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0)) { return TouchInfo.Began; }
            if (Input.GetMouseButton(0)) { return TouchInfo.Moved; }
            if (Input.GetMouseButtonUp(0)) { return TouchInfo.Ended; }
#else
        if (Input.touchCount > 0)
        {
            return (TouchInfo)Input.GetTouch(0).phase;
        }
#endif
            return TouchInfo.None;
        }

        /// <summary>
        /// タッチ(クリック)座標取得。
        /// タッチしていない場合、xyzすべて-10000。
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetTouchPosition()
        {
            if (GetTouch() == TouchInfo.None)
                return -Vector3.one * 10000;

            TouchPosition = GetPosition();

            //Debug.Log("タッチ座標：" + TouchPosition);
            return TouchPosition;
        }

        /// <summary>
        /// Orthカメラでのタッチ座標のワールド変換。
        /// タッチしていない場合、xyzすべて-1。
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static Vector3 GetTouchWorldPositionOnOrth(Camera cam)
        {
            if (GetTouch() == TouchInfo.None)
                return -Vector3.one;

            var pos = GetPosition();
            // Perspectiveカメラの場合、Z座標が必要になってくるのでカメラZを代入
            pos.z = 7.6f;

            TouchPosition = cam.ScreenToWorldPoint(pos);
            //Debug.Log("タッチワールド座標：" + TouchPosition);
            return TouchPosition;
        }
        /// <summary>
        /// Persカメラでのタッチ座標のワールド変換。
        /// タッチしていない場合、xyzすべて-1。
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static Vector3 GetTouchWorldPositionOnPers(Camera cam, GameObject obj)
        {
            if (GetTouch() == TouchInfo.None)
                return -Vector3.one;

            var pos = GetPosition();
            // Perspectiveカメラの場合、Z座標が必要になってくるので
            // 目標のオブジェクトとカメラのZを計算して代入
            pos.z = obj.transform.position.z - cam.transform.position.z;

            TouchPosition = cam.ScreenToWorldPoint(pos);
            //Debug.Log("タッチワールド座標：" + TouchPosition);
            return TouchPosition;
        }

        /// <summary>
        /// タッチ座標の画面比率。
        /// タッチしていない場合、xyzすべて-1。
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetTouchPosRatio()
        {
            if (GetTouch() == TouchInfo.None)
                return -Vector3.one;

            TouchPosition = GetPosition();

            var ratio = new Vector3(TouchPosition.x / Screen.width, TouchPosition.y / Screen.height);
            //Debug.Log("タッチ座標比率：" + ratio);
            return ratio;
        }

        private static Vector3 GetPosition()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.mousePosition;
#else
            var touch = Input.GetTouch(0);
            return touch.position;
#endif
        }

        /// <summary>
        /// タッチ座標上にオブジェクトがあったらすべて取得
        /// </summary>
        /// <returns></returns>
        public static Type[] GetTouchObjects<Type>()
            where Type : class
        {
            if (GetTouch() != TouchInfo.Began)
                return null;

            var ray = Camera.main.ScreenPointToRay(GetPosition());
            var hits = Physics.RaycastAll(ray.origin, ray.direction);
            if (hits.Length == 0)
                return null;
            return hits.Select(h => h.collider.GetComponent<Type>()).ToArray();
        }

        /// <summary>
        /// 指定のオブジェクトがタッチされたかどうか
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsTouchObject(GameObject obj)
        {
            if (GetTouch() != TouchInfo.Began)
                return false;
            var hit = new RaycastHit();
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //マウスクリックした場所からRayを飛ばし、オブジェクトがあればtrue 
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.GetInstanceID() == obj.GetInstanceID())
                    return true;
            }
            return false;
        }
    }
}
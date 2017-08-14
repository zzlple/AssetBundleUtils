using UnityEngine;
using System.Collections;
using UnityEngine.UI;



#if UNITY_EDITOR
using UnityEditor;
#endif

	/// <summary>
	/// AR camera config.
	/// </summary>
	public class ARCameraConfig : MonoBehaviour
	{


		static float sWidth = 0;
		static float sHeight = 0;
		/// <summary>
		/// The time.
		/// </summary>
		private float time;
		/// <summary>
		/// The has fous.
		/// </summary>
		private bool hasFous;
		/// <summary>
		/// The first time.
		/// </summary>
		[HideInInspector]
		public	bool openOfflineSlam = false;
		/// <summary>
		/// The found.
		/// </summary>
		private bool found;
		/// <summary>
		/// The current distance.
		/// </summary>
		private float currentDistance;
		/// <summary>
		/// The stand distance.
		/// </summary>
		private float standDistance;
		/// <summary>
		/// The start position.
		/// </summary>
		public Vector3 startPosition = new Vector3 (0f, 250f, 0f);
		/// <summary>
		/// The model total position.
		/// 公共开始位置
		/// </summary>
		public Vector3 modelTotalPosition = new Vector3 (0f, 50f, -150f);
		private Vector3 tempTotalPosition = new Vector3 (0f, 0f, 0f);
		/// <summary>
		/// The video total position.
		/// </summary>
		public Vector3 videoTotalPosition = new Vector3 (0f, 250f, 0f);
		/// <summary>
		/// The cost time.
		/// </summary>
		public float costTime = 1.5f;

		void Start ()
		{
			ToSlamViewPort ();
		
		}

		bool play=false;


		public void ToSlamViewPort ()
		{
	
	
			sWidth = Screen.width;
			sHeight = Screen.height;

			#if UNITY_EDITOR
			EditorApplication.isPlaying=(play=!play);
			#endif

			transform.localPosition = modelTotalPosition;
			transform.localEulerAngles = Vector3.zero;
			transform.localScale = Vector3.one;
			AdapterScreen ();
		
		}


		public void AdapterScreen ()
		{
			float width;
			float height;
			Vector3[] screenPoints = new Vector3[4];

		GameObject pointobj = GameObject.Find (Constant.MODEL_TAG_POINTS);
			RectTransform rt = null;
			if (null != pointobj) {
				rt = pointobj.GetComponent<RectTransform> ();
			}

			if (null == rt) {
				Debug.LogError ("Adapter Scale Mode Failed！");
				return;
			} else {

				Vector3[] points = new Vector3[4];
				rt.GetWorldCorners (points);

				for (int i = 0; i < screenPoints.Length; i++) {
					screenPoints [i] = Camera.main.WorldToScreenPoint (points [i]);
				}
				width = Mathf.Abs (screenPoints [1].x - screenPoints [2].x);
				height = Mathf.Abs (screenPoints [0].y - screenPoints [1].y);

			
				float scaleRate;
				if (width > height) {
					if (sWidth != 0) {
						scaleRate = sWidth / width;
					} else {
						scaleRate = Screen.width / width;
					}

				} else {
					if (sHeight != 0) {
						scaleRate = sHeight / height;
					
					} else {

						scaleRate = Screen.height / height;
					}


				}
				Debug.Log (Screen.width + ":::::" + Screen.height);

				float scaleDistance=0f;



			GameObject h=GameObject.Find (Constant.MODEL_TAG_DIRECT_H);
			GameObject v=GameObject.Find (Constant.MODEL_TAG_DIRECT_V);

				if (null != h) {
					Debug.Log ("open H Adapter Mode");
					scaleDistance= videoTotalPosition.y / scaleRate;
					Camera.main.transform.localPosition = new Vector3 (rt.position.x, scaleDistance, rt.position.z);
					Camera.main.transform.localEulerAngles = new Vector3 (90f, 0f, 0f);
				}

				if (null != v) {
				
				Debug.Log ("open V Adapter Mode");
					scaleDistance= modelTotalPosition.z / scaleRate;
					Camera.main.transform.localPosition = new Vector3 (rt.position.x, rt.position.y, scaleDistance);
				}



			}





		}






	}

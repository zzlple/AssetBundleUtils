using UnityEngine;
using System.Collections;

public class ScreenAdapter : MonoBehaviour {



	public Transform mainCamera;

	/// <summary>
	/// The model total position.模型目标位置为了计算正对屏幕的缩放
	/// </summary>
	public Vector3 modelTotalPosition = new Vector3 (0f, 50f, -150f);
	/// <summary>
	/// The video total position.视频目标位置为了计算正对屏幕的缩放
	/// </summary>
	public Vector3 videoTotalPosition = new Vector3 (0f, 250f, 0f);

	public Vector3 tempAdapterPosition;

	public bool isBackCamera = true;

	// Use this for initialization
	void Start () {
	
	}
	

	/// <summary>
	/// Adapters the screen.
	/// 自动匹配屏幕，根据模块类型是否开启陀螺仪
	/// </summary>
	public void AdapterScreen ()
	{

		float width;
		float height;
		GameObject pointobj = null;
		RectTransform rt = null;
		GameObject h = null; 
		GameObject v = null; 

		pointobj = GameObject.Find (Constant.MODEL_TAG_POINTS);
		if (null != pointobj) {
			h = GameObject.Find (Constant.MODEL_TAG_DIRECT_H);
			v = GameObject.Find (Constant.MODEL_TAG_DIRECT_V);
			rt = pointobj.GetComponent<RectTransform> ();
		}


		if (null == rt) {
			Debug.Log("匹配缩放模式失败！缺少缩放标记点...");
			return;
		} else {

			if (h == null && v == null) {
				Debug.Log ("匹配缩放模式失败！缺少方向标记点...");
				return;
			}

			if (h != null && v != null) {
				Debug.Log("匹配缩放模式失败！多余的方向标记点...");
				return;
			}
			#if UNITY_IOS
			//特殊位置相机正对地面，只有这样才能正确计算出标记点与屏幕的比例关系
			if (null != h) {


			mainCamera.localEulerAngles = new Vector3 (90f, 0f, 0f);
			mainCamera.localPosition = videoTotalPosition;
			}
			//特殊位置相机正对前方，只有这样才能正确计算出标记点与屏幕的比例关系
			if (null != v) {
			mainCamera.localEulerAngles = new Vector3 (0f, 0f, 0f);
			mainCamera.localPosition = modelTotalPosition;
			}

			//针对安卓适配
			#elif UNITY_ANDROID
			if (isBackCamera) {
			//特殊位置相机正对地面，只有这样才能正确计算出标记点与屏幕的比例关系
			if (null != h) {
			mainCamera.localEulerAngles = new Vector3 (90f, 0f, 0f);
			mainCamera.localPosition = videoTotalPosition;
			}
			//特殊位置相机正对前方，只有这样才能正确计算出标记点与屏幕的比例关系
			if (null != v) {
			mainCamera.localEulerAngles = new Vector3 (0f, 0f, 0f);
			mainCamera.localPosition = modelTotalPosition;
			}

			} else {

			//特殊位置相机正对地面，只有这样才能正确计算出标记点与屏幕的比例关系
			if (null != h) {
			mainCamera.localEulerAngles = new Vector3 (90f, 180f, 0f);
			mainCamera.localPosition = videoTotalPosition;
			}
			//特殊位置相机正对前方，只有这样才能正确计算出标记点与屏幕的比例关系
			if (null != v) {
			mainCamera.localEulerAngles = new Vector3 (0f, 0f, 180f);
			mainCamera.localPosition = modelTotalPosition;
			}

			}

			#elif UNITY_EDITOR
			//特殊位置相机正对地面，只有这样才能正确计算出标记点与屏幕的比例关系
			if (null != h) {
				mainCamera.localEulerAngles = new Vector3 (90f, 0f, 0f);
				mainCamera.localPosition = videoTotalPosition;
			}
			//特殊位置相机正对前方，只有这样才能正确计算出标记点与屏幕的比例关系
			if (null != v) {
				mainCamera.localEulerAngles = new Vector3 (0f, 0f, 0f);
				mainCamera.localPosition = modelTotalPosition;
			}
			#endif
			//计算缩放比例关系，根据bundle标记点算出与屏幕的缩放比，如果标记点的宽大于高，那么取宽度作为缩放比率，反之亦然
			Vector3[] points = new Vector3[4];
			Vector3[] screenPoints = new Vector3[4];
			rt.GetWorldCorners (points);

			for (int i = 0; i < screenPoints.Length; i++) {
				screenPoints [i] = Camera.main.WorldToScreenPoint (points [i]);
			}
			width = Mathf.Abs (screenPoints [1].x - screenPoints [2].x);
			height = Mathf.Abs (screenPoints [0].y - screenPoints [1].y);
			float scaleRate;
			if (width > height) {
				scaleRate = Screen.width / width;
			} else {
				scaleRate = Screen.height / height;
			}
			float scaleDistance = 0f;
			if (null != h) {
				//根据Bundle类型重新定向type类型以获取最佳缩放比，水平方向的片状模型统一视做视频处理
				//当相机俯视时候我们取坐标系Y轴向作为相机和模型距离，相机的坐标保证位于四个标记点中心保证相机正对模型
				scaleDistance = videoTotalPosition.y / scaleRate;
				mainCamera.localPosition = new Vector3 (rt.position.x, scaleDistance, rt.position.z);
			}

			if (null != v) {
				//根据Bundle类型重新定向type类型以获取最佳缩放比
				//当相机前视时候我们取坐标系Z轴向作为相机和模型距离，相机的坐标保证位于四个标记点中心保证相机正对模型
				scaleDistance = modelTotalPosition.z / scaleRate;
				mainCamera.localPosition = new Vector3 (rt.position.x, rt.position.y, scaleDistance);
			}
		}

		//记录适配后最佳位置到ARCameraConfig类中供陀螺仪再次reset使用
		tempAdapterPosition =mainCamera.localPosition;
	

	}




}

#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class CreateStandModel : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
	}

	public void OnDrawGizmos()
	{

	


	}    

	void OnGUI(){

		Handles.color = Color.yellow;
		Handles.ArrowCap(0, Vector3.zero, Quaternion.Euler(Vector3.zero),10);

	}




	public static string tempPath;
	public static Texture2D textImage;
	public static GameObject gRootObject;
	public static GameObject gCamera;
	public static GameObject gTarget;
	public static GameObject gPointHolder;
	public static RectTransform gPointHolderRect;
	public static GameObject gDirectTag;
	public static MeshFilter mFilter;
	public static MeshRenderer mRen;
	public static Vector3[] vertices;
	public static int index = 0;
	public static	int[] triangles;
	public static	Vector3[] normals;
	public static	Vector2[] uvs;
	public static Mesh mesh;
	//清空场景所有物体
	public static void DestroyAllObjectInScene(){

		Transform[] rr = GameObject.FindObjectsOfType<Transform> ();

		if (null != rr) {
		
			for (int i = 0; i < rr.Length; i++) {
			
			
				DestroyImmediate (rr [i].gameObject);
			}
		}
	}


	[MenuItem (Constant.MENU_NAME+ "/Create StandModel")]
	public static void StandModel ()
	{


		//DestroyAllObjectInScene ();


		Tools.current = Tool.Rect;
		tempPath = Utils.OpenFileLocalPath (new string[]{ ".jpg", ".png" });

		if (string.IsNullOrEmpty (tempPath)) {
			Debug.LogError ("error:can not create tag object,image format error，please reselect！");
			return;
		}
		textImage = Utils.getLocalImage (tempPath);
		if (null == textImage) {
			Debug.LogError ("error:can not create tag object,load image cause error，please reselect！");
			return;
		}

		ReFindObject ();

		destoryAllGameObject ();

		createCamera ();
		createImageTarget ();
		createMaskPoints();
	}


	/// <summary>
	/// Alerts the mask points disection.
	/// </summary>
	/// <param name="v">If set to <c>true</c> v=垂直.</param>
	public static void alertMaskPointsDisection(bool v){
		ReFindObject ();

		if (v) {
	


				Debug.Log ("open the V Adapter Mode");

	

			gPointHolderRect.transform.localEulerAngles = new Vector3 (0, 0, 0);
			gPointHolderRect.sizeDelta = new Vector2 (150,200);
			gPointHolderRect.anchoredPosition3D = new Vector3 (0,100,0);
			gDirectTag.name = Constant.MODEL_TAG_DIRECT_V;
		
		} else {
	

			Debug.Log ("open the H Adapter Mode");



			gPointHolderRect.transform.localEulerAngles = new Vector3 (90, 0, 0);
			gPointHolderRect.sizeDelta = new Vector2 (200,200);
			gPointHolderRect.anchoredPosition3D = new Vector3 (0,0,0);
			gDirectTag.name =Constant.MODEL_TAG_DIRECT_H;
		
		}




	}


	//当从运行模式回到编辑模式重新注入对象引用
	public static void ReFindObject(){
		gCamera = GameObject.Find (Constant.MODEL_TAG_ARCAMERA);
		gTarget = GameObject.Find (Constant.MODEL_TAG_IMAGETARGET);
		gRootObject = GameObject.Find (Constant.MODEL_TAG_ROOT_OBJECT);
		gPointHolder = GameObject.Find (Constant.MODEL_TAG_POINTS);
		gDirectTag = (GameObject.Find (Constant.MODEL_TAG_DIRECT_H) != null) ? GameObject.Find (Constant.MODEL_TAG_DIRECT_H) : GameObject.Find (Constant.MODEL_TAG_DIRECT_V);

		if (null != gPointHolder) {
			gPointHolderRect=gPointHolder.GetComponent<RectTransform>();
		}
	}


	public static void destoryAllGameObject(){

		try {
			DestroyImmediate (gRootObject);
			DestroyImmediate (gPointHolder);
			DestroyImmediate (gDirectTag);


			if(null==FindObjectOfType(typeof(ARCameraConfig))){

				DestroyImmediate(gCamera);
			}


		} catch (Exception ex) {
			
		}

	}




	//创建Camera并且标记为主相机
	public static void createCamera(){

		Camera mainCamera = ResetArCamera ();

		//create arcamera;
		if (null == mainCamera) {

			GameObject tempCamera = new GameObject (Constant.MODEL_TAG_ARCAMERA);


			
				tempCamera.AddComponent<ARCameraConfig> ();
			

		

			Camera cm =	tempCamera.AddComponent<Camera> ();
			cm.tag = "MainCamera";
			ResetArCamera ();


		} else {

			if (null == mainCamera.gameObject.GetComponent<ARCameraConfig> ()) {
			
				mainCamera.gameObject.AddComponent<ARCameraConfig> ();
			}
	

		}

	}





	//创建标记RectTransform；
	public static void createMaskPoints ()
	{

		if (null != gRootObject) {


			DestroyImmediate (gRootObject);
		}


		gRootObject = new GameObject (Constant.MODEL_TAG_ROOT_OBJECT);
		gRootObject.transform.localPosition = Vector3.zero;
		gRootObject.transform.localEulerAngles = Vector3.zero;
		gRootObject.transform.localScale = Vector3.one;


		gPointHolder = new GameObject (Constant.MODEL_TAG_POINTS);
		gPointHolderRect=	gPointHolder.AddComponent<RectTransform> ();
		gPointHolder.transform.parent = gRootObject.transform;

		gDirectTag = new GameObject (Constant.MODEL_TAG_DIRECT_V);
		gDirectTag.transform.parent = gPointHolder.transform;
		gDirectTag.transform.localPosition = Vector3.zero;
		gDirectTag.transform.localEulerAngles = Vector3.zero;
		gDirectTag.transform.localScale = Vector3.one;
		alertMaskPointsDisection (true);
	
	}


	//创建标记ImageTarget；
	public static void createImageTarget ()
	{
		if (null != gTarget) {
			DestroyImmediate (gTarget);
		}
		//careate imagetarget
		gTarget = new GameObject (Constant.MODEL_TAG_IMAGETARGET);
		
		int width;
		int height;
		float rote;
		
		
		width = textImage.width;
		height = textImage.height;

		if (width > height) {
			
			rote = width / (float)height;
		} else {
			
			rote = height / (float)(width);

		}
		gTarget.transform.localScale = new Vector3 (100, 100, 100);
		
		mFilter = gTarget.AddComponent<MeshFilter> ();
		
		mRen = gTarget.AddComponent<MeshRenderer> ();

		mRen.receiveShadows = false;

		mRen.castShadows = false;
		
		gTarget.transform.GetComponent<Renderer> ().sharedMaterial = new Material (Shader.Find ("Unlit/Texture"));
		try {
			gTarget.transform.GetComponent<Renderer> ().sharedMaterial.mainTexture = textImage;
		} catch (Exception ex) {
			
		}
		
		//矩形的四个顶点坐标

		vertices = new Vector3[4];

		if (width > height) {
			vertices [0] = new Vector3 (-0.5f * rote, 0, -0.5f);
			vertices [1] = new Vector3 (0.5f * rote, 0, -0.5f);
			vertices [2] = new Vector3 (0.5f * rote, 0, 0.5f);
			vertices [3] = new Vector3 (-0.5f * rote, 0, 0.5f);
			
		} else {
			
			vertices [0] = new Vector3 (-0.5f, 0, -0.5f * rote);
			vertices [1] = new Vector3 (0.5f, 0, -0.5f * rote);
			vertices [2] = new Vector3 (0.5f, 0, 0.5f * rote);
			vertices [3] = new Vector3 (-0.5f, 0, 0.5f * rote);
			
			
		}
		
		
		//三角形顶点索引
		
		triangles = new int[6] { 0, 3, 2, 0, 2, 1 };
		//反向绘制
		// int[] triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		
		//每个顶点的法线    
		
		normals = new Vector3[4];
		normals [0] = new Vector3 (0, 1, 0);
		normals [1] = new Vector3 (0, 1, 0);
		normals [2] = new Vector3 (0, 1, 0);
		normals [3] = new Vector3 (0, 1, 0);
		
		//UV贴图
		uvs = new Vector2[4];
		uvs [0] = new Vector2 (0, 0);
		uvs [1] = new Vector2 (1, 0);
		uvs [2] = new Vector2 (1, 1);
		uvs [3] = new Vector2 (0, 1);         
		mesh = new Mesh ();
		mesh.hideFlags = HideFlags.None;
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.normals = normals;
		mFilter.mesh = mesh;
	
	}




	
	//重置Camera
	static Camera ResetArCamera ()
	{

		if (null == Camera.main) {

			return null;
		}

		Camera.main.farClipPlane = 5000f;
		Camera.main.transform.localPosition = new Vector3 (0f, 250f, 0f);
		Camera.main.transform.localEulerAngles = new Vector3 (90f, 0f, 0f);
		Camera.main.transform.localScale = new Vector3 (1f, 1f, 1f);
		Camera.main.fieldOfView = 51f;
		Camera.main.clearFlags = CameraClearFlags.SolidColor;
		Camera.main.depth = -1;
		Camera.main.nearClipPlane = 0f;
		Camera.main.farClipPlane = 5000f;
		Camera.main.name = "ARCamera";





		return Camera.main;


	}


	
}

#endif
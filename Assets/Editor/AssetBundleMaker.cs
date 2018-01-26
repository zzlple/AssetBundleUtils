using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleMaker:Editor
{
	public const string TARGET_DIR = "_AssetBunldes";
	public const string TARGET_FORMAT = "assetbunldes";


	/// <summary>
	/// Gets the selection path.
	/// </summary>
	/// <returns>The selection path.</returns>
	static string getSelectionPath ()
	{
		UnityEngine.Object[] arr = Selection.GetFiltered (typeof(UnityEngine.Object), SelectionMode.TopLevel);
		return Application.dataPath.Substring (0, Application.dataPath.LastIndexOf ('/')) + "/" + AssetDatabase.GetAssetPath (arr [0]);
	}

	/// <summary>
	/// Gets the project path.
	/// </summary>
	/// <returns>The project path.</returns>
	static string getProjectPath ()
	{
	
		return Application.dataPath.Substring (0, Application.dataPath.LastIndexOf ('/'));
	
	}

	/// <summary>
	/// Gets the selection asset path.
	/// </summary>
	/// <returns>The selection asset path.</returns>
	static string getSelectionAssetPath ()
	{
		UnityEngine.Object[] arr = Selection.GetFiltered (typeof(UnityEngine.Object), SelectionMode.TopLevel);
		return  AssetDatabase.GetAssetPath (arr [0]);
	}

	/// <summary>
	/// Clears the name of the asset bundles.
	/// </summary>
	static void ClearAssetBundlesName ()
	{  
		int length = AssetDatabase.GetAllAssetBundleNames ().Length;  
		Debug.Log (length);  
		string[] oldAssetBundleNames = new string[length];  
		for (int i = 0; i < length; i++) {  
			oldAssetBundleNames [i] = AssetDatabase.GetAllAssetBundleNames () [i];  
		}  

		for (int j = 0; j < oldAssetBundleNames.Length; j++) {  
			AssetDatabase.RemoveAssetBundleName (oldAssetBundleNames [j], true);  
		}  
		length = AssetDatabase.GetAllAssetBundleNames ().Length;  
		Debug.Log (length);  

	
		//EditorUtility.DisplayCancelableProgressBar ("", "", 1);

	}





	[MenuItem (Constant.MENU_NAME+"/Build Adapter Bundle")]
	static void BuildAdapterBundle ()
	{
		try {


	
			Object[] SelectedAsset = Selection.GetFiltered(typeof (Object), SelectionMode.DeepAssets);



			foreach(Object obj in SelectedAsset){



				if (CheckObject (obj as GameObject)==false) {


					Debug.LogError ("package failed");
					return;

				}

			}

			string selectionPath = getSelectionAssetPath ();
			Debug.Log (selectionPath);
			AssetImporter assImporter =	AssetImporter.GetAtPath (selectionPath);
			Debug.Log (assImporter.name + ":" + assImporter.assetBundleName + ":" + assImporter.GetInstanceID ());
			assImporter.assetBundleName = System.DateTime.Now.ToString ("yyyy_MM_dd_hh_mm_ss");
			assImporter.assetBundleVariant = TARGET_FORMAT;
			assImporter.SaveAndReimport ();
			BuildAssetBundles (BuildTarget.iOS, "iOS");
			BuildAssetBundles (BuildTarget.Android, "Android");
		} catch (System.Exception ex) {

			Debug.Log (ex.Message);
			EditorUtility.DisplayDialog ("error", ex.Message + " please select the real perfabs", "sure");

		}
	}



    [MenuItem(Constant.MENU_NAME + "/Build WebGL")]
    static void BuildWebGLBundle()
    {
        try
        {


            string selectionPath = getSelectionAssetPath();
            Debug.Log(selectionPath);
            AssetImporter assImporter = AssetImporter.GetAtPath(selectionPath);
            Debug.Log(assImporter.name + ":" + assImporter.assetBundleName + ":" + assImporter.GetInstanceID());
            assImporter.assetBundleName = System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
            assImporter.assetBundleVariant = TARGET_FORMAT;
            assImporter.SaveAndReimport();
            BuildAssetBundles(BuildTarget.WebGL, "WebGL");
        }
        catch (System.Exception ex)
        {

            Debug.Log(ex.Message);
            EditorUtility.DisplayDialog("error", ex.Message + " please select the real perfabs", "sure");

        }


    }



	[MenuItem (Constant.MENU_NAME+"/Build iOS Android")]
	static void BuildBundle ()
	{
		try {

		
			string selectionPath = getSelectionAssetPath ();
			Debug.Log (selectionPath);
			AssetImporter assImporter =	AssetImporter.GetAtPath (selectionPath);
			Debug.Log (assImporter.name + ":" + assImporter.assetBundleName + ":" + assImporter.GetInstanceID ());
			assImporter.assetBundleName = System.DateTime.Now.ToString ("yyyy_MM_dd_hh_mm_ss");
			assImporter.assetBundleVariant = TARGET_FORMAT;
			assImporter.SaveAndReimport ();
			BuildAssetBundles (BuildTarget.iOS, "iOS");
			BuildAssetBundles (BuildTarget.Android, "Android");
		} catch (System.Exception ex) {

			Debug.Log (ex.Message);
			EditorUtility.DisplayDialog ("error", ex.Message + " please select the real perfabs", "sure");
			
		}
	}




	[MenuItem (Constant.MENU_NAME+"/Load Bundle")]
	static void LoadBundle ()
	{
		//EditorApplication.isPlaying = true;
		string bundleFile = Utils.OpenFileLocalPath (new string[]{ TARGET_FORMAT });
		Debug.Log (bundleFile);
		if (string.IsNullOrEmpty (bundleFile)) {
			EditorUtility.DisplayDialog ("error", "your selection is error," + "please select the real assetbunldes file", "sure");
		} else {



			EditorCoroutineRunner.StartEditorCoroutine (LoadBundle (bundleFile));
		}

	}

	[MenuItem (Constant.MENU_NAME+"/Clear Data")]
	static void ClearData ()
	{
		//清除bundle名字
		ClearAssetBundlesName ();

		//清除生成文件夹
		string file = getProjectPath () + "/" + TARGET_DIR;
		if (Directory.Exists (file)) {
			Directory.Delete (file, true);

		}

		Caching.CleanCache ();
		AssetDatabase.Refresh ();

	}

	public static Object tempObject;

	static IEnumerator LoadBundle (string file)
	{
		WWW www = new WWW ("file://" + file);
		yield return www;
		tempObject =	Instantiate (www.assetBundle.LoadAsset (www.assetBundle.GetAllAssetNames () [0]));
		yield return tempObject;
		www.assetBundle.Unload (false);
	}




	/// <summary>
	/// Builds the asset bundles.
	/// </summary>
	/// <param name="buildTarget">Build target.</param>
	/// <param name="platform">Platform.</param>

	static void BuildAssetBundles (BuildTarget buildTarget, string platform)
	{
		Caching.CleanCache ();
		string targetPath = TARGET_DIR + Path.DirectorySeparatorChar + platform;
		Debug.Log (targetPath);
		if (!File.Exists (targetPath)) {
			Directory.CreateDirectory (targetPath);
		} 
		// Put the bundles in a folder called "ABs" within the Assets folder.
		BuildPipeline.BuildAssetBundles (targetPath, 
			BuildAssetBundleOptions.CollectDependencies |
			BuildAssetBundleOptions.CompleteAssets |
			BuildAssetBundleOptions.DisableWriteTypeTree |
			BuildAssetBundleOptions.DeterministicAssetBundle |
			BuildAssetBundleOptions.ForceRebuildAssetBundle |
			BuildAssetBundleOptions.ChunkBasedCompression

			, buildTarget);
	AssetDatabase.Refresh ();
	}



	/// <summary>
	/// Checks the object.
	/// </summary>
	/// <returns><c>true</c>, if object was checked, <c>false</c> otherwise.</returns>
	/// <param name="obj">Object.</param>
	static bool CheckObject (GameObject obj)
	{
		int model_root_object = 0;
		int model_tag_points = 0;
		int model_tag_direct_v = 0;
		int model_tag_direct_h = 0;
		int model_tag_recttransform = 0;
		Transform[] t = obj.GetComponentsInChildren<Transform> ();
		for (int i = 0; i < t.Length; i++) {
			if (!t [0].gameObject.name.Equals (Constant.MODEL_TAG_ROOT_OBJECT)) {
				Debug.LogError ("your bundle root object name error,the correct name is:" + Constant.MODEL_TAG_ROOT_OBJECT);
				return false;
			}

			if (t [i].gameObject.name.Equals (Constant.MODEL_TAG_ROOT_OBJECT)) {

				model_root_object++;
			}
			if (t [i].gameObject.name.Equals (Constant.MODEL_TAG_POINTS)) {

				if (null != t [i].gameObject.GetComponent<RectTransform> ()) {

					model_tag_recttransform++;
				}
				model_tag_points++;
			}
			if (t [i].gameObject.name.Equals (Constant.MODEL_TAG_DIRECT_H)) {

				model_tag_direct_h++;
			}
			if (t [i].gameObject.name.Equals (Constant.MODEL_TAG_DIRECT_V)) {

				model_tag_direct_v++;
			}

		}





		if (model_root_object == 0) {

			Debug.LogError ("your bundle lost root object:" + Constant.MODEL_TAG_ROOT_OBJECT);

			return false;

		} 
		if (model_tag_points == 0) {


			Debug.LogError ("your bundle lost tag object:" + Constant.MODEL_TAG_POINTS);
			return false;

		}

		if (model_tag_recttransform == 0) {

			Debug.LogError ("your bundle lost tag object:" + "RectTransform");
			return false;

		}

		if (model_tag_direct_h + model_tag_direct_v == 0) {

			Debug.LogError ("your bundle lost direction tag object:" + Constant.MODEL_TAG_DIRECT_V + "&&" + Constant.MODEL_TAG_DIRECT_V);
			return false;

		}



		if (model_root_object > 1) {

			Debug.LogError ("your bundle exist extra root object:" + Constant.MODEL_TAG_ROOT_OBJECT);

			return false;

		} 
		if (model_tag_points > 1) {


			Debug.LogError ("your bundle exist extra root object:" + Constant.MODEL_TAG_POINTS);
			return false;

		}

		if (model_tag_recttransform > 1) {

			Debug.LogError ("your bundle exist extra root object:" + "RectTransform");
			return false;

		}

		if (model_tag_direct_h + model_tag_direct_v > 1) {

			Debug.LogError ("your bundle exist extra direction tag object:" + Constant.MODEL_TAG_DIRECT_H + "&&" + Constant.MODEL_TAG_DIRECT_V);
			return false;

		}




		return true;
	}






}

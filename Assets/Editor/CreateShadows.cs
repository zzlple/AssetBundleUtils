using UnityEngine;
using System.Collections;
using UnityEditor;

#if UNITY_EDITOR
public class CreateShadows : Editor {

	public static int i=0;

	private static GameObject  plane;
	[MenuItem(Constant.MENU_NAME+"/Create Shadow")]
	public static void Shadow(){
		
		Debug.Log ("Must open the QualitySetting的Shadows->Hard and Soft Shadows selection for Shadows work job");
		if (null == plane) {
			string name="AlphaShadowReceiver";
			if(GameObject.Find(name)!=null){
	
			}else{
				plane=GameObject.CreatePrimitive(PrimitiveType.Plane);
				plane.name=name;
				plane.transform.localPosition=new Vector3(0f,1f,0f);
				plane.transform.localEulerAngles=new Vector3(0f,0f,0f);
				plane.transform.localScale=new Vector3(15f,15f,15f);
				MeshRenderer mr=plane.GetComponent<MeshRenderer>();
				mr.castShadows=false;
				mr.receiveShadows=true;
		
				plane.GetComponent<Renderer>().sharedMaterial =	Resources.Load<Material> ("Shadow");
			
			

			}





		}





		GameObject light = new GameObject ("Shadows Light "+i++);

		light.transform.localPosition = new Vector3 (0f, 300f, 0f);

		light.transform.localEulerAngles = new Vector3 (50f, -30f, 0f);

	    Light l=light.AddComponent<Light> ();

		l.type = LightType.Directional;

		l.intensity = 1.0f;
		l.shadows = LightShadows.Hard;
		l.shadowStrength = 0.7f;
		l.renderMode = LightRenderMode.Auto;


		try {
			l.cullingMask = Camera.main.cullingMask;
		} catch (System.Exception ex) {
			l.cullingMask =-1;
		}


		l.alreadyLightmapped = true;



	}
}
#endif
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
[CustomEditor (typeof(ARCameraConfig))]
public class ARCameraEditor : Editor
{
	
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		ARCameraConfig config = (ARCameraConfig)target;
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("SimSlamViewPort", new GUILayoutOption[]{ GUILayout.Width (200), GUILayout.Height (100) })) {
		
				config.ToSlamViewPort ();


			}

			GUILayout.BeginVertical ();

			if (GUILayout.Button ("H Tag", new GUILayoutOption[]{ GUILayout.Width (100), GUILayout.Height (50) })) {

				CreateStandModel.alertMaskPointsDisection (false);


			}

			if (GUILayout.Button ("V Tag", new GUILayoutOption[]{ GUILayout.Width (100), GUILayout.Height (50) })) {

				CreateStandModel.alertMaskPointsDisection (true);

			}


			GUILayout.EndVertical ();

		GUILayout.EndHorizontal ();


	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Detonator))]
public class DetonatorCustomInspector : Editor {

	//Serialized Vars
	//Basic
	SerializedProperty sizeVar;
    SerializedProperty colorVar;
    SerializedProperty explodeOnStarVar;
    SerializedProperty durationVar;
    SerializedProperty destroyTimeVar;
    SerializedProperty detailVar;

	//Direction
	SerializedProperty worldSpaceVar;
    SerializedProperty upwardsBiasVar;
    SerializedProperty directionVar;

	//Materials
	SerializedProperty fireballAVar;
	SerializedProperty isFireballAFlipbookVar;
	SerializedProperty fireballAFlipbookSizeVar;
    SerializedProperty fireballBVar;
	SerializedProperty isFireballBFlipbookVar;
	SerializedProperty fireballBFlipbookSizeVar;
    SerializedProperty smokeAVar;
	SerializedProperty isSmokeAFlipbookVar;
	SerializedProperty smokeAFlipbookSizeVar;
	SerializedProperty smokeBVar;
	SerializedProperty isSmokeBFlipbookVar;
	SerializedProperty smokeBFlipbookSizeVar;
    SerializedProperty shockwaveVar;
	SerializedProperty isShockwaveFlipbookVar;
	SerializedProperty shockwaveFlipbookSizeVar;
	SerializedProperty sparksVar;
	SerializedProperty isSparksFlipbookVar;
	SerializedProperty sparksFlipbookSizeVar;
	SerializedProperty glowVar;
	SerializedProperty isGlowFlipbookVar;
	SerializedProperty glowFlipbookSizeVar;
	SerializedProperty heatwaveVar;
	
	//Auto Create
	SerializedProperty autoCreateFireballVar;
	SerializedProperty autoCreateSmokeVar;
	SerializedProperty autoCreateShockwaveVar;
	SerializedProperty autoCreateSparksVar;
	SerializedProperty autoCreateGlowVar;
	SerializedProperty autoCreateHeatwaveVar;
	SerializedProperty autoCreateLightVar;
	SerializedProperty autoCreateForceVar;

	void OnEnable()
    {
		//Basic
        sizeVar = serializedObject.FindProperty("size");
        colorVar = serializedObject.FindProperty("color");
        explodeOnStarVar = serializedObject.FindProperty("explodeOnStart");
        durationVar = serializedObject.FindProperty("duration");
        destroyTimeVar = serializedObject.FindProperty("destroyTime");
        detailVar = serializedObject.FindProperty("detail");

		//Direction
		worldSpaceVar = serializedObject.FindProperty("useWorldSpace");
        upwardsBiasVar = serializedObject.FindProperty("upwardsBias");
        directionVar = serializedObject.FindProperty("direction");

		//Materials
		fireballAVar = serializedObject.FindProperty("fireballAMaterial");
		isFireballAFlipbookVar = serializedObject.FindProperty("isFireballAFlipbook");
		fireballAFlipbookSizeVar = serializedObject.FindProperty("fireballAFlipbookSize");
		fireballBVar = serializedObject.FindProperty("fireballBMaterial");
		isFireballBFlipbookVar = serializedObject.FindProperty("isFireballBFlipbook");
		fireballBFlipbookSizeVar = serializedObject.FindProperty("fireballBFlipbookSize");
		smokeAVar = serializedObject.FindProperty("smokeAMaterial");
		isSmokeAFlipbookVar = serializedObject.FindProperty("isSmokeAFlipbook");
		smokeAFlipbookSizeVar = serializedObject.FindProperty("smokeAFlipbookSize");
		smokeBVar = serializedObject.FindProperty("smokeBMaterial");
		isSmokeBFlipbookVar = serializedObject.FindProperty("isSmokeBFlipbook");
		smokeBFlipbookSizeVar = serializedObject.FindProperty("smokeBFlipbookSize");
		shockwaveVar = serializedObject.FindProperty("shockwaveMaterial");
		isShockwaveFlipbookVar = serializedObject.FindProperty("isShockwaveFlipbook");
		shockwaveFlipbookSizeVar = serializedObject.FindProperty("shockwaveFlipbookSize");
		sparksVar = serializedObject.FindProperty("sparksMaterial");
		isSparksFlipbookVar = serializedObject.FindProperty("isSparksFlipbook");
		sparksFlipbookSizeVar = serializedObject.FindProperty("sparksFlipbookSize");
		glowVar = serializedObject.FindProperty("glowMaterial");
		isGlowFlipbookVar = serializedObject.FindProperty("isGlowFlipbook");
		glowFlipbookSizeVar = serializedObject.FindProperty("glowFlipbookSize");
		heatwaveVar = serializedObject.FindProperty("heatwaveMaterial");

		//Auto Create
		autoCreateFireballVar = serializedObject.FindProperty("autoCreateFireball");
		autoCreateSmokeVar = serializedObject.FindProperty("autoCreateSmoke");
		autoCreateShockwaveVar = serializedObject.FindProperty("autoCreateShockwave");
		autoCreateSparksVar = serializedObject.FindProperty("autoCreateSparks");
		autoCreateGlowVar = serializedObject.FindProperty("autoCreateGlow");
		autoCreateHeatwaveVar = serializedObject.FindProperty("autoCreateHeatwave");
		autoCreateLightVar = serializedObject.FindProperty("autoCreateLight");
		autoCreateForceVar = serializedObject.FindProperty("autoCreateForce");
    }

	override public void OnInspectorGUI()
   {
    var detonatorScript = target as Detonator;

	//Draw the basic stuff
	EditorGUILayout.LabelField("Basic Variables", EditorStyles.boldLabel);
	EditorGUILayout.PropertyField(sizeVar, new GUIContent("Size"));
	EditorGUILayout.PropertyField(colorVar, new GUIContent("Color"));
	EditorGUILayout.PropertyField(explodeOnStarVar, new GUIContent("Explode On Start"));
	EditorGUILayout.PropertyField(durationVar, new GUIContent("Duration"));
	EditorGUILayout.PropertyField(destroyTimeVar, new GUIContent("Seconds Until Destroy"));
	EditorGUILayout.PropertyField(detailVar, new GUIContent("Detail Level"));
	
	//Direction
	EditorGUILayout.Space();
	EditorGUILayout.LabelField("Direction Variables", EditorStyles.boldLabel);
	EditorGUILayout.PropertyField(worldSpaceVar, new GUIContent("Use World Space"));
	EditorGUILayout.PropertyField(upwardsBiasVar, new GUIContent("Upwards Bias"));
	EditorGUILayout.PropertyField(directionVar, new GUIContent("Direction"));
    
	//Materials
	EditorGUILayout.Space();
	EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
	//Fireball A
	EditorGUILayout.PropertyField(fireballAVar, new GUIContent("Fireball A Material"));
	if(detonatorScript.fireballAMaterial)
	{
		EditorGUILayout.PropertyField(isFireballAFlipbookVar, new GUIContent("   Is this a flipbook?"));

		if (detonatorScript.isFireballAFlipbook)
			EditorGUILayout.PropertyField(fireballAFlipbookSizeVar, new GUIContent("   Flipbook size"));
	}
	else
	{
		detonatorScript.isFireballAFlipbook = false;
	}
	//Fireball B
	EditorGUILayout.PropertyField(fireballBVar, new GUIContent("Fireball B Material"));
	if(detonatorScript.fireballBMaterial)
	{
		EditorGUILayout.PropertyField(isFireballBFlipbookVar, new GUIContent("   Is this a flipbook?"));

		if (detonatorScript.isFireballBFlipbook)
			EditorGUILayout.PropertyField(fireballBFlipbookSizeVar, new GUIContent("   Flipbook size"));
	}
	else
	{
		detonatorScript.isFireballBFlipbook = false;
	}
	//Smoke A
	EditorGUILayout.PropertyField(smokeAVar, new GUIContent("Smoke A Material"));
	if(detonatorScript.smokeAMaterial)
	{
		EditorGUILayout.PropertyField(isSmokeAFlipbookVar, new GUIContent("   Is this a flipbook?"));

		if (detonatorScript.isSmokeAFlipbook)
			EditorGUILayout.PropertyField(smokeAFlipbookSizeVar, new GUIContent("   Flipbook size"));
	}
	else
	{
		detonatorScript.isSmokeAFlipbook = false;
	}
	//Smoke B
	EditorGUILayout.PropertyField(smokeBVar, new GUIContent("Smoke B Material"));
	if(detonatorScript.smokeBMaterial)
	{
		EditorGUILayout.PropertyField(isSmokeBFlipbookVar, new GUIContent("   Is this a flipbook?"));

		if (detonatorScript.isSmokeBFlipbook)
			EditorGUILayout.PropertyField(smokeBFlipbookSizeVar, new GUIContent("   Flipbook size"));
	}
	else
	{
		detonatorScript.isSmokeBFlipbook = false;
	}
	//Shockwave
	EditorGUILayout.PropertyField(shockwaveVar, new GUIContent("Shockwave Material"));
	if(detonatorScript.shockwaveMaterial)
	{
		EditorGUILayout.PropertyField(isShockwaveFlipbookVar, new GUIContent("   Is this a flipbook?"));

		if (detonatorScript.isShockwaveFlipbook)
			EditorGUILayout.PropertyField(shockwaveFlipbookSizeVar, new GUIContent("   Flipbook size"));
	}
	else
	{
		detonatorScript.isShockwaveFlipbook = false;
	}
	//Sparks
	EditorGUILayout.PropertyField(sparksVar, new GUIContent("Sparks Material"));
	if(detonatorScript.sparksMaterial)
	{
		EditorGUILayout.PropertyField(isSparksFlipbookVar, new GUIContent("   Is this a flipbook?"));

		if (detonatorScript.isSparksFlipbook)
			EditorGUILayout.PropertyField(sparksFlipbookSizeVar, new GUIContent("   Flipbook size"));
	}
	else
	{
		detonatorScript.isSparksFlipbook = false;
	}
	//Glow
	EditorGUILayout.PropertyField(glowVar, new GUIContent("Glow Material"));
	if(detonatorScript.glowMaterial)
	{
		EditorGUILayout.PropertyField(isGlowFlipbookVar, new GUIContent("   Is this a flipbook?"));

		if (detonatorScript.isGlowFlipbook)
			EditorGUILayout.PropertyField(glowFlipbookSizeVar, new GUIContent("   Flipbook size"));
	}
	else
	{
		detonatorScript.isGlowFlipbook = false;
	}
	//Heatwave
	EditorGUILayout.PropertyField(heatwaveVar, new GUIContent("Heatwave Material"));

	//Auto Create
	EditorGUILayout.Space();
	EditorGUILayout.LabelField("Auto Create Components", EditorStyles.boldLabel);
	EditorGUILayout.PropertyField(autoCreateFireballVar, new GUIContent("Auto Create Fireball?"));
	EditorGUILayout.PropertyField(autoCreateSmokeVar, new GUIContent("Auto Create Smoke?"));
	EditorGUILayout.PropertyField(autoCreateShockwaveVar, new GUIContent("Auto Create Shockwave?"));
	EditorGUILayout.PropertyField(autoCreateSparksVar, new GUIContent("Auto Create Sparks?"));
	EditorGUILayout.PropertyField(autoCreateGlowVar, new GUIContent("Auto Create Glow?"));
	EditorGUILayout.PropertyField(autoCreateHeatwaveVar, new GUIContent("Auto Create Heatwave?"));
	EditorGUILayout.PropertyField(autoCreateLightVar, new GUIContent("Auto Create Light?"));
	EditorGUILayout.PropertyField(autoCreateForceVar, new GUIContent("Auto Create Force?"));

	//Save values
	serializedObject.ApplyModifiedProperties();
   }
}

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System;
using System.Reflection;

public class InspectorPlusFiler
{
	string writeString;
	SerializedObject so;
	string name;
	InspectorPlusTracker tracker;
	string fileName;
	string q = "\"";

	string Quot(string toQuout)
	{
		return q + toQuout + q;
	}

	string BuildArgs(params object[] args)
	{
		string ret = "";

		if (args.Length == 0)
			return "";

		foreach (object s in args)
		{
			if (s == null)
				continue;

			string add = s.ToString();

			if (s.GetType() == typeof(string))
			{
				add = add.Replace("\"", "\\\"");
				add = "\"" + add + "\"";
			}

			if (s.GetType() == typeof(Vector3))
			{
				Vector3 val = (Vector3)s;
				add = "new Vector3(" + val.x + "f," + val.y + "f," + val.z + "f)";
			}

			if (s.GetType() == typeof(InspectorPlusVar.LimitType))
				add = "InspectorPlusVar.LimitType." + add;

			if (s.GetType() == typeof(InspectorPlusVar.VectorDrawType))
				add = "InspectorPlusVar.VectorDrawType." + add;

			if (s.GetType().IsArray)
			{
				Array arr = s as Array;
				add = "new " + add + "{";
				for (int i = 0; i < ((Array)s).Length; i += 1)
				{
					add += BuildArgs(arr.GetValue(i));
					if (i + 1 < ((Array)s).Length)
						add += ",";
				}
				add += "}";
			}

			if (s.GetType() == typeof(bool))
				add = add.ToLower();

			ret += "," + add;
		}

		return ret.Substring(1);
	}


	string BuildVar(InspectorPlusVar v)
	{

		return @"
        vars.Add(new InspectorPlusVar(" + BuildArgs(v.limitType, v.min, v.max, v.progressBar, v.iMin, v.iMax, v.active, v.type, v.name, v.dispName, v.vectorDrawType, v.relative, v.scale,
									   v.space, v.labelEnabled, v.label, v.labelBold, v.labelItalic, v.labelAlign, v.buttonEnabled, v.buttonText, v.buttonCallback, v.buttonCondense, v.numSpace, v.classType,
									   v.offset, v.QuaternionHandle, v.canWrite, v.tooltip, v.hasTooltip, v.toggleStart, v.toggleSize, v.toggleLevel, v.largeTexture, v.textureSize, v.textFieldDefault, v.textArea) + "));";
	}

	string BuildVars()
	{
		string ret = "";
		foreach (InspectorPlusVar v in tracker.GetVars())
		{
			ret += BuildVar(v);
		}
		return ret;
	}

	bool hasInspector;
	bool hasFloat;
	bool hasInt;
	bool hasProgressBar;
	bool hasArray;
	bool hasVector2;
	bool hasQuaternion;
	bool hasBool;

	bool hasScene;
	bool hasVectorScene;
	bool hasQuaternionScene;

	bool hasSpace;
	bool hasTooltip;

	bool hasTexture;

	bool hasText;

	void CheckExists()
	{
		hasInspector = false;
		hasFloat = false;
		hasInt = false;
		hasProgressBar = false;
		hasArray = false;
		hasVector2 = false;
		hasQuaternion = false;
		hasBool = false;

		hasScene = false;
		hasVectorScene = false;
		hasQuaternionScene = false;

		hasSpace = false;
		hasTooltip = false;

		foreach (InspectorPlusVar v in tracker.GetVars())
		{
			if (!v.active)
				continue;
			hasInspector = true;
			if (v.type == typeof(float).Name) hasFloat = true;
			if (v.type == typeof(int).Name) hasInt = true;
			if (hasFloat || hasInt) hasProgressBar = true;
			if (v.isArray) hasArray = true;
			if (v.type == typeof(Vector2).Name) hasVector2 = true;
			if (v.type == typeof(Quaternion).Name) hasQuaternion = true;
			if (v.type == typeof(bool).Name) hasBool = true;

			if (v.vectorDrawType != InspectorPlusVar.VectorDrawType.None || v.QuaternionHandle) hasScene = true;
			if (v.vectorDrawType != InspectorPlusVar.VectorDrawType.None) hasVectorScene = true;
			if (v.QuaternionHandle) hasQuaternionScene = true;
			if (v.hasTooltip) hasTooltip = true;
			if (v.space != 0.0f) hasSpace = true;
			if (v.largeTexture) hasTexture = true;
			if (v.textFieldDefault != "" || v.textArea == true) hasText = true;
		}
	}

	void Write()
	{
		CheckExists();
		InspectorPlusFilerStrings s = new InspectorPlusFilerStrings(name, name + "Inspector", hasInspector);
		writeString += s.header;
		if (hasInspector || hasScene)
		{
			writeString += s.inspectorPlusVar;
			writeString += s.vars;
			writeString += BuildVars();
			writeString += s.onEnable;
			if (hasProgressBar) writeString += s.progressBar;
			if (hasArray) writeString += s.arrayGUI;
			if (hasVector2) writeString += s.vector2Field;
			if (hasFloat) writeString += s.floatField;
			if (hasInt) writeString += s.intField;
			if (hasQuaternion) writeString += s.quaternionField;
			if (hasBool) writeString += s.boolField;
			if (hasTexture) writeString += s.textureGUI;
			if (hasText) writeString += s.textGUI;
			/*if (hasProperty)*/
			writeString += s.propertyField;
			if (hasInspector) writeString += s.GetOnGUI(hasFloat, hasInt, hasProgressBar, hasArray, hasVector2, hasQuaternion, hasBool, hasTooltip, hasSpace, hasTexture, hasText);
			if (hasInspector) writeString += s.watermark;
			if (hasVectorScene) writeString += s.vectorScene;
			if (hasQuaternionScene) writeString += s.quaternionScene;
			if (hasScene) writeString += s.GetSceneString(hasScene, hasVectorScene, hasQuaternionScene);
		}
		else
			writeString += @"
    public override void OnInspectorGUI (){}";

		writeString += @"
}";
	}

	public void WriteToFile(string _name, InspectorPlusTracker _tracker, string folder)
	{
		_tracker.UpdateFields();
		name = _name;
		fileName = name + "Inspector";
		tracker = _tracker;
		writeString = "";
		Write();

		writeString = writeString.Trim();


		if (!Directory.Exists(Application.dataPath + "/" + folder + "/"))
			Directory.CreateDirectory(Application.dataPath + "/" + folder + "/");

		File.WriteAllText(Application.dataPath + "/" + folder + "/" + fileName + ".cs", writeString);
	}
}


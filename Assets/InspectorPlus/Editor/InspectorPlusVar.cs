#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class InspectorPlusVar {
	public enum LimitType {
		None,
		Max,
		Min,
		Range
	};

	public enum VectorDrawType {
		None,
		Direction,
		Point,
		PositionHandle,
		Scale,
		Rotation
	};

	public LimitType limitType = LimitType.None;
	public float min = -0.0f;
	public float max = -0.0f;

	public bool progressBar;

	public int iMin = -0;
	public int iMax = -0;

	public bool active = true;

	public string type;
	public string name;
	bool setName;

	public string dispName {
		get {
			if (!setName) {
				setName = true;
				SetDispName();
			}
			return _dispName;
		}
		set { _dispName = value; }
	}

	string _dispName = "";
	public VectorDrawType vectorDrawType;
	public bool relative = false;
	public bool scale = false;

	public float space = 0.0f;

	public bool[] labelEnabled = new bool[4];
	public string[] label = new string[4];

	public bool[] buttonEnabled = new bool[16];
	public string[] buttonText = new string[16];
	public string[] buttonCallback = new string[16];
	public bool[] buttonCondense = new bool[4];


	public int numSpace = 0;
	public string classType;
	public bool isArray;

	public Vector3 offset = new Vector3(0.5f, 0.5f);

	public bool QuaternionHandle;

	Vector2 startpos;
	bool pressed;

	public bool canWrite = true;
	public string tooltip = "Tooltip";
	public bool hasTooltip = false;
	public bool fixedTip = false;

	public bool toggleStart = false;
	public int toggleSize = 0;
	public int toggleLevel = 0;

	public int index;
	public int maxSize;

	public bool largeTexture;
	public float textureSize = 70;

	public string textFieldDefault;
	public bool textArea;

	public InspectorPlusVar() {
		for (int i = 0; i < 4; i += 1) {
			labelEnabled[i] = false;
			label[i] = "";
			for (int j = 0; j < 4; j += 1) {
				buttonEnabled[i * 4 + j] = false;
				buttonText[i * 4 + j] = "";
				buttonCallback[i * 4 + j] = "";
			}
		}
	}


	public void SetDispName() {
		string output = "";

		foreach (char letter in name) {
			if (Char.IsUpper(letter) && output.Length > 0) {
				output += " " + letter;
			}
			else if (letter == '_') {
				output += " ";
			}
			else {
				output += letter;
			}
		}

		_dispName = output;
		_dispName = char.ToUpper(_dispName[0]) + _dispName.Substring(1);
		setName = true;
	}

	public void DrawFieldGUI(InspectorPlusTracker gui) {
		if (canWrite) {
			gui.GetRect(30.0f);

			if (type == typeof(float).Name) {
				GUI.Label(gui.GetRect(80.0f), "Limit: ");
				limitType = (LimitType) EditorGUI.EnumPopup(gui.GetRect(80.0f), limitType);

				bool oldEnabled = GUI.enabled;

				GUI.enabled = limitType == LimitType.Min || limitType == LimitType.Range;
				min = EditorGUI.FloatField(gui.GetRect(80.0f), min);

				GUI.enabled = limitType == LimitType.Max || limitType == LimitType.Range;
				max = EditorGUI.FloatField(gui.GetRect(80.0f), max);

				if (limitType == LimitType.Range) {
					GUI.Label(gui.GetRect(80.0f), "ProgressBar: ");
					progressBar = GUI.Toggle(gui.GetRect(80.0f), progressBar, "");
				}

				GUI.enabled = oldEnabled;
			}
			else if (type == typeof(int).Name) {
				GUI.Label(gui.GetRect(80.0f), "Limit: ");
				limitType = (LimitType) EditorGUI.EnumPopup(gui.GetRect(80.0f), limitType);

				bool oldEnabled = GUI.enabled;

				GUI.enabled = limitType == LimitType.Min || limitType == LimitType.Range;
				iMin = EditorGUI.IntField(gui.GetRect(80.0f), iMin);

				GUI.enabled = limitType == LimitType.Max || limitType == LimitType.Range;
				iMax = EditorGUI.IntField(gui.GetRect(80.0f), iMax);

				if (limitType == LimitType.Range) {
					GUI.Label(gui.GetRect(80.0f), "ProgressBar: ");
					progressBar = GUI.Toggle(gui.GetRect(80.0f), progressBar, "");
				}

				GUI.enabled = oldEnabled;
			}
			else if (type == typeof(Vector3).Name || type == typeof(Vector2).Name) {
				GUI.Label(gui.GetRect(80.0f), "Draw: ");
				vectorDrawType = (VectorDrawType) EditorGUI.EnumPopup(gui.GetRect(80.0f), vectorDrawType);

				bool oldEnabled = GUI.enabled;

				GUI.enabled = vectorDrawType != VectorDrawType.None;
				GUI.Label(gui.GetRect(80.0f), "Relative: ");
				relative = GUI.Toggle(gui.GetRect(80.0f), relative, "");

				if (vectorDrawType == VectorDrawType.Direction) {
					GUI.Label(gui.GetRect(80.0f), "Scale: ");
					scale = GUI.Toggle(gui.GetRect(80.0f), scale, "");
				}

				if (vectorDrawType == VectorDrawType.Scale || vectorDrawType == VectorDrawType.Rotation) {
					GUI.Label(gui.GetRect(80.0f), "Offset: ");
					offset.x = EditorGUI.FloatField(gui.GetRect(40.0f), offset.x);
					offset.y = EditorGUI.FloatField(gui.GetRect(40.0f), offset.y);
					offset.z = EditorGUI.FloatField(gui.GetRect(40.0f), offset.z);
				}

				GUI.enabled = oldEnabled;
			}
			else if (type == typeof(Quaternion).Name) {
				QuaternionHandle = GUI.Toggle(gui.GetRect(80.0f), QuaternionHandle, new GUIContent("handle"));
				gui.GetRect(20.0f);

				GUI.enabled = QuaternionHandle;
				GUI.Label(gui.GetRect(80.0f), "Offset: ");
				offset.x = EditorGUI.FloatField(gui.GetRect(40.0f), offset.x);
				offset.y = EditorGUI.FloatField(gui.GetRect(40.0f), offset.y);
				offset.z = EditorGUI.FloatField(gui.GetRect(40.0f), offset.z);
				GUI.enabled = true;
			}
			else if (type == typeof(bool).Name) {
				toggleStart = GUI.Toggle(gui.GetRect(150.0f), toggleStart, "Toggle group");
				GUI.enabled = toggleStart;
				toggleSize = EditorGUI.IntSlider(gui.GetRect(120.0f), toggleSize, 1,
					Mathf.Max(1, (maxSize - index) - 1));
				GUI.enabled = true;
			}
			else if (type == typeof(Texture).Name || type == typeof(Texture2D).Name) {
				largeTexture = GUI.Toggle(gui.GetRect(120.0f), largeTexture, new GUIContent("large preview"));
				GUI.enabled = largeTexture;
				textureSize = EditorGUI.Slider(gui.GetRect(80.0f), textureSize, 35.0f, 300.0f);
				GUI.enabled = true;
			}
			else if (type == typeof(string).Name) {
				GUI.Label(gui.GetRect(80.0f), "Default text");
				textFieldDefault = GUI.TextField(gui.GetRect(180.0f), textFieldDefault);

				GUI.Label(gui.GetRect(80.0f), "Text area: ");
				textArea = GUI.Toggle(gui.GetRect(80.0f), textArea, "");
			}
		}
		else {
			GUI.Label(gui.GetRect(80.0f), "Read only");
		}
	}

	public void DrawDragBox(InspectorPlusTracker gui) {
		gui.Line(EditorGUIUtility.singleLineHeight + 5.0f);
		var boxRect = gui.GetRect(900.0f);
		boxRect.height = space;
		GUI.Box(boxRect, "");


		gui.Line(0);

		numSpace = Mathf.FloorToInt(space / 25.0f);
		bool guiEnabled = GUI.enabled;

		if (numSpace > 0) {
			gui.Line((space - numSpace * 25.0f) / 2.0f);
		}

		for (int i = 0; i < numSpace; i += 1) {
			GUI.enabled = true;

			gui.GetRect(10.0f);
			labelEnabled[i] = GUI.Toggle(gui.GetRect(40.0f), labelEnabled[i], "");

			GUI.enabled = labelEnabled[i];
			GUI.Label(gui.GetRect(80.0f), "Label ");
			label[i] = EditorGUI.TextField(gui.GetRect(40.0f), "", label[i]);
			GUI.enabled = true;

			for (int j = 0; j < 4; j += 1) {
				GUI.enabled = true;
				//button
				buttonEnabled[i * 4 + j] = GUI.Toggle(gui.GetRect(40.0f), buttonEnabled[i * 4 + j], "");

				GUI.enabled = buttonEnabled[i * 4 + j];
				GUI.Label(gui.GetRect(90.0f), "Button ");
				buttonText[i * 4 + j] = EditorGUI.TextField(gui.GetRect(50.0f), "", buttonText[i * 4 + j]);

				GUI.Label(gui.GetRect(90.0f), "Callback ");
				buttonCallback[i * 4 + j] = EditorGUI.TextField(gui.GetRect(50.0f), "", buttonCallback[i * 4 + j]);

				bool buttonToCome = false;
				for (int k = j; k < 4; k += 1)
					if (buttonEnabled[i * 4 + k])
						buttonToCome = true;

				if (!buttonToCome || j == 3) {
					GUI.enabled = true;
					if (j > 1) {
						buttonCondense[i] = GUI.Toggle(gui.GetRect(40.0f), buttonCondense[i], new GUIContent("Condense"));
					}
					break;
				}
			}

			gui.Line(EditorGUIUtility.singleLineHeight);
		}

		if (numSpace > 0) {
			gui.Line(-(space - numSpace * 25.0f) / 2.0f);
		} else {
			gui.Line(space);
		}

		GUI.enabled = guiEnabled;

		boxRect.y += boxRect.height;
		boxRect.height = 5.0f;

		EditorGUIUtility.AddCursorRect(boxRect, MouseCursor.ResizeVertical);

		if (Event.current.type == EventType.mouseDown) {
			pressed = false;
			if (boxRect.Contains(Event.current.mousePosition)) {
				startpos = Event.current.mousePosition;
				pressed = true;
			}
		}

		if (Event.current.type == EventType.mouseDrag && pressed) {
			space += (Event.current.mousePosition - startpos).y;
			startpos = Event.current.mousePosition;

			EditorWindow.GetWindow(typeof(InspectorPlusWindow)).Repaint();
			space = Mathf.Clamp(space, 0.0f, 4 * 25.0f);

			GUI.changed = true;

			Event.current.Use();
		}

		if (Event.current.type == EventType.mouseUp) {
			pressed = false;
		}
	}
}
#endif
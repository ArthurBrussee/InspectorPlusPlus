#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class InspectorPlusFilerStrings
{


	static string q = "\"";

	static string Quot(string toQuout)
	{
		return q + toQuout + q;
	}
	public string header;
	#region inspectorPlusVar
	public string inspectorPlusVar = @"
    public class InspectorPlusVar
    {
        public enum LimitType { None, Max, Min, Range };
        public enum VectorDrawType { None, Direction, Point, PositionHandle, Scale, Rotation };
        public LimitType limitType = LimitType.None;
        public float min = -0.0f;
        public float max = -0.0f;
        public bool progressBar;
        public int iMin = -0;
        public int iMax = -0;
        public bool active = true;
        public string type;
        public string name;
        public string dispName;
        public VectorDrawType vectorDrawType;
        public bool relative = false;
        public bool scale = false;
        public float space = 0.0f;
        public bool[] labelEnabled = new bool[4];
        public string[] label = new string[4];
        public bool[] labelBold = new bool[4];
        public bool[] labelItalic = new bool[4];
        public int[] labelAlign = new int[4];
        public bool[] buttonEnabled = new bool[16];
        public string[] buttonText = new string[16];
        public string[] buttonCallback = new string[16];
        public bool[] buttonCondense = new bool[4];

        public int numSpace = 0;
        public string classType;
        public Vector3 offset = new Vector3(0.5f, 0.5f);
        public bool QuaternionHandle;
	    public bool canWrite = true;
	    public string tooltip;
	    public bool hasTooltip = false;
        public bool toggleStart = false;
        public int toggleSize = 0;
        public int toggleLevel = 0;
        public bool largeTexture;
        public float textureSize;

		public string textFieldDefault;
		public bool textArea;

    public InspectorPlusVar(LimitType _limitType, float _min, float _max, bool _progressBar, int _iMin, int _iMax, bool _active, string _type, string _name, string _dispName,
                        VectorDrawType _vectorDrawType, bool _relative, bool _scale, float _space, bool[] _labelEnabled, string[] _label, bool[] _labelBold, bool[] _labelItalic, int[] _labelAlign, bool[] _buttonEnabled, string[] _buttonText,
                        string[] _buttonCallback, bool[] buttonCondense, int _numSpace, string _classType, Vector3 _offset, bool _QuaternionHandle, bool _canWrite, string _tooltip, bool _hasTooltip,
                        bool _toggleStart, int _toggleSize, int _toggleLevel, bool _largeTexture, float _textureSize, string _textFieldDefault, bool _textArea)
    {
        limitType = _limitType;
        min = _min;
        max = _max;
        progressBar = _progressBar;
        iMax = _iMax;
        iMin = _iMin;
        active = _active;
        type = _type;
        name = _name;
        dispName = _dispName;
        vectorDrawType = _vectorDrawType;
        relative = _relative;
        scale = _scale;
        space = _space;
        labelEnabled = _labelEnabled;
        label = _label;
        labelBold = _labelBold;
        labelItalic = _labelItalic;
        labelAlign = _labelAlign;
        buttonEnabled = _buttonEnabled;
        buttonText = _buttonText;
        buttonCallback = _buttonCallback;
        numSpace = _numSpace;
        classType = _classType;
        offset = _offset;
        QuaternionHandle = _QuaternionHandle;
        canWrite = _canWrite;
        tooltip = _tooltip;
        hasTooltip = _hasTooltip;
        toggleStart = _toggleStart;
        toggleSize = _toggleSize;
        toggleLevel = _toggleLevel;
        largeTexture = _largeTexture;
        textureSize = _textureSize;
		textFieldDefault = _textFieldDefault;
		textArea = _textArea;
    }
    }";
	#endregion
	#region vars
	public string vars = @"	
    SerializedObject so;
	SerializedProperty[] properties;
	new string name;
    string dispName;
	Rect tooltipRect;	
	List<InspectorPlusVar> vars;
	void RefreshVars(){for (int i = 0; i < vars.Count; i += 1) properties[i] = so.FindProperty (vars[i].name);}
	void OnEnable ()
	{
        vars = new List<InspectorPlusVar>();
        so = serializedObject;";
	#endregion
	#region onEnable
	public string onEnable = @"	
		int count = vars.Count;
		properties = new SerializedProperty[count];
	}
    ";
	#endregion
	#region progressBar
	public string progressBar = @"
	void ProgressBar (float value, string label)
	{
		GUILayout.Space (3.0f);
		Rect rect = GUILayoutUtility.GetRect (18, 18, " + Quot("TextField") + @");
		EditorGUI.ProgressBar (rect, value, label);
		GUILayout.Space (3.0f);
	}";
	#endregion
	#region propertyField
	public string propertyField = @"
	void PropertyField (SerializedProperty sp, string name)
	{
		if (sp.hasChildren) {
            GUILayout.BeginVertical();
			while (true) {
				if (sp.propertyPath != name && !sp.propertyPath.StartsWith (name + " + Quot(".") + @"))
					break;

				EditorGUI.indentLevel = sp.depth;
                bool child = false;

                if (sp.depth == 0)
                    child = EditorGUILayout.PropertyField(sp, new GUIContent(dispName));
                else
                    child = EditorGUILayout.PropertyField(sp);

				if (!sp.NextVisible (child))
					break;
			}
            EditorGUI.indentLevel = 0;
            GUILayout.EndVertical();
		} else EditorGUILayout.PropertyField(sp, new GUIContent(dispName));
	}";
	#endregion
	#region arrayGUI
	public string arrayGUI = @"
	void ArrayGUI (SerializedProperty sp, string name)
	{
		EditorGUIUtility.LookLikeControls (120.0f, 40.0f);
		GUILayout.Space (4.0f);
		EditorGUILayout.BeginVertical (" + Quot("box") + @", GUILayout.MaxWidth(Screen.width));

		int i = 0;
		int del = -1;

		SerializedProperty array = sp.Copy ();
		SerializedProperty size = null;
		bool first = true;

		while (true) {
			if (sp.propertyPath != name && !sp.propertyPath.StartsWith (name + " + Quot(".") + @"))
				break;

			bool child;
            EditorGUI.indentLevel = sp.depth;

			if (sp.depth == 1 && !first) {
				EditorGUILayout.BeginHorizontal ();

				if (GUILayout.Button (" + Quot("") + ", " + Quot("OL Minus") + @", GUILayout.Width (24.0f)))
					del = i;

				child = EditorGUILayout.PropertyField (sp);

				GUI.enabled = i > 0;

				if (GUILayout.Button (" + Quot("U") + @", " + Quot("ButtonLeft") + @", GUILayout.Width (24.0f), GUILayout.Height(18.0f)))
					array.MoveArrayElement (i - 1, i);

				GUI.enabled = i < array.arraySize - 1;
                if (GUILayout.Button(" + Quot("D") + @", " + Quot("ButtonRight") + @", GUILayout.Width(24.0f), GUILayout.Height(18.0f)))
					array.MoveArrayElement (i + 1, i);

				++i;

				GUI.enabled = true;
				EditorGUILayout.EndHorizontal ();
			} else if (sp.depth == 1) {
				first = false;
				size = sp.Copy ();

				EditorGUILayout.BeginHorizontal ();

                if (!size.hasMultipleDifferentValues && GUILayout.Button(" + Quot("") + @", " + Quot("OL Plus") + @", GUILayout.Width(24.0f)))
					array.arraySize += 1;


				child = EditorGUILayout.PropertyField (sp);

				EditorGUILayout.EndHorizontal ();
			} else {
                child = EditorGUILayout.PropertyField(sp);
			}

			if (!sp.NextVisible (child))
				break;
		}

		sp.Reset ();

		if (del != -1)
			array.DeleteArrayElementAtIndex (del);

		if (array.isExpanded && !size.hasMultipleDifferentValues) {
			EditorGUILayout.BeginHorizontal ();

            if (GUILayout.Button(" + Quot("") + @", " + Quot("OL Plus") + @", GUILayout.Width(24.0f)))
				array.arraySize += 1;

			GUI.enabled = false;
			EditorGUILayout.PropertyField (array.GetArrayElementAtIndex (array.arraySize - 1), new GUIContent (" + Quot("") + @" + array.arraySize));
			GUI.enabled = true;

			EditorGUILayout.EndHorizontal ();
		}


        EditorGUI.indentLevel = 0;
		EditorGUILayout.EndVertical ();
		EditorGUIUtility.LookLikeControls (170.0f, 80.0f);
	}";
	#endregion
	#region vector2Field
	public string vector2Field = @"
	void Vector2Field(SerializedProperty sp)
	{
        EditorGUI.BeginProperty(new Rect(0.0f, 0.0f, 0.0f, 0.0f), new GUIContent(), sp);
		EditorGUI.BeginChangeCheck ();
		var newValue = EditorGUILayout.Vector2Field (dispName, sp.vector2Value);

		if (EditorGUI.EndChangeCheck ())
			sp.vector2Value = newValue;
		
		EditorGUI.EndProperty ();
	}";
	#endregion
	#region floatField
	public string floatField = @"
	void FloatField(SerializedProperty sp, InspectorPlusVar v)
	{
		if (v.limitType == InspectorPlusVar.LimitType.Min && !sp.hasMultipleDifferentValues)
			sp.floatValue = Mathf.Max (v.min, sp.floatValue);
		else if (v.limitType == InspectorPlusVar.LimitType.Max && !sp.hasMultipleDifferentValues)
			sp.floatValue = Mathf.Min (v.max, sp.floatValue);
		
		if (v.limitType == InspectorPlusVar.LimitType.Range) {
			if (!v.progressBar)
				EditorGUILayout.Slider (sp, v.min, v.max);
			else {
				if (!sp.hasMultipleDifferentValues) {
					sp.floatValue = Mathf.Clamp (sp.floatValue, v.min, v.max);
					ProgressBar ((sp.floatValue - v.min) / v.max, dispName);
				} else
					ProgressBar ((sp.floatValue - v.min) / v.max, dispName);
			}
		}
        else EditorGUILayout.PropertyField(sp, new GUIContent(dispName));
	}";
	#endregion
	#region intField
	public string intField = @"
	void IntField(SerializedProperty sp, InspectorPlusVar v)
	{
		if (v.limitType == InspectorPlusVar.LimitType.Min && !sp.hasMultipleDifferentValues)
			sp.intValue = Mathf.Max (v.iMin, sp.intValue);
		else if (v.limitType == InspectorPlusVar.LimitType.Max && !sp.hasMultipleDifferentValues)
			sp.intValue = Mathf.Min (v.iMax, sp.intValue);
		
		if (v.limitType == InspectorPlusVar.LimitType.Range)
		{
			if (!v.progressBar)
			{
                EditorGUI.BeginProperty(new Rect(0.0f, 0.0f, 0.0f, 0.0f), new GUIContent(), sp);
				EditorGUI.BeginChangeCheck ();

                var newValue = EditorGUI.IntSlider(GUILayoutUtility.GetRect(18.0f, 18.0f), new GUIContent(dispName), sp.intValue, v.iMin, v.iMax);
				
				if (EditorGUI.EndChangeCheck ())
					sp.intValue = newValue;
				EditorGUI.EndProperty ();
			}
			else {
				if (!sp.hasMultipleDifferentValues) {
					sp.intValue = Mathf.Clamp (sp.intValue, v.iMin, v.iMax);
					ProgressBar ((float)(sp.intValue - v.iMin) / v.iMax, dispName);
				} else
					ProgressBar ((float)(sp.intValue - v.iMin) / v.iMax, dispName);
			}
		}
        else EditorGUILayout.PropertyField(sp, new GUIContent(dispName));
	}";
	#endregion
	#region quaternionField
	public string quaternionField = @"
	void QuaternionField(SerializedProperty sp)
	{
        EditorGUI.BeginProperty(new Rect(0.0f, 0.0f, 0.0f, 0.0f), new GUIContent(), sp);

		EditorGUI.BeginChangeCheck ();
		SerializedProperty x = sp.FindPropertyRelative (" + Quot("x") + @");
		SerializedProperty y = sp.FindPropertyRelative (" + Quot("y") + @");
		SerializedProperty z = sp.FindPropertyRelative (" + Quot("z") + @");
		SerializedProperty w = sp.FindPropertyRelative (" + Quot("w") + @");

		Quaternion q = new Quaternion (x.floatValue, y.floatValue, z.floatValue, w.floatValue);
                   
		var newValue = EditorGUILayout.Vector3Field (dispName, q.eulerAngles);

		if (EditorGUI.EndChangeCheck ()) {
			Quaternion r = Quaternion.Euler (newValue);
			x.floatValue = r.x;
			y.floatValue = r.y;
			z.floatValue = r.z;
			w.floatValue = r.w;
		}

		EditorGUI.EndProperty ();
	}";
	#endregion

	public string boolField = @"
    int BoolField(SerializedProperty sp, InspectorPlusVar v)
    {
        if (v.toggleStart)
        {
            EditorGUI.BeginProperty(new Rect(0.0f, 0.0f, 0.0f, 0.0f), new GUIContent(), sp);

            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUILayout.Toggle(dispName, sp.boolValue);
            
            if (EditorGUI.EndChangeCheck())
                sp.boolValue = newValue;
            
            EditorGUI.EndProperty();

            if (!sp.boolValue)
                return v.toggleSize;
        }
        else EditorGUILayout.PropertyField(sp, new GUIContent(dispName));

        return 0;
    }";

	public string textureGUI = @"
    void TextureGUI(SerializedProperty sp, InspectorPlusVar v)
    {
        if (!v.largeTexture)
            PropertyField(sp, name);
        else
        {
           GUILayout.Label(dispName, GUILayout.Width(145.0f));
    
            EditorGUI.BeginProperty(new Rect(0.0f, 0.0f, 0.0f, 0.0f), new GUIContent(), sp);

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var newValue = EditorGUILayout.ObjectField(sp.objectReferenceValue, typeof(Texture2D), false, GUILayout.Width(v.textureSize), GUILayout.Height(v.textureSize));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
                sp.objectReferenceValue = newValue;

            EditorGUI.EndProperty();
        }
    }";

	public string textGUI = @"
	void TextGUI(SerializedProperty sp, InspectorPlusVar v)
	{
		if (v.textFieldDefault == " + Quot("") + @")
		{
			PropertyField(sp, name);
			return;
		}

		string focusName = " + Quot("_focusTextField") + @"+ v.name;

		GUI.SetNextControlName(focusName);

		EditorGUI.BeginProperty(new Rect(0.0f, 0.0f, 0.0f, 0.0f), new GUIContent(), sp);

		EditorGUI.BeginChangeCheck();

		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(dispName);

		string newValue = " + Quot("") + @";

		if (!v.textArea)
			newValue = EditorGUILayout.TextField(" + Quot("") + @", sp.stringValue, GUILayout.Width(Screen.width));
		else
			newValue = EditorGUILayout.TextArea(sp.stringValue, GUILayout.Width(Screen.width));

		if (GUI.GetNameOfFocusedControl() != focusName && !sp.hasMultipleDifferentValues && sp.stringValue == " + Quot("") + @")
		{
			GUI.color = new Color(0.7f, 0.7f, 0.7f);
			GUI.Label(GUILayoutUtility.GetLastRect(), v.textFieldDefault);
			GUI.color = Color.white;
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		if (EditorGUI.EndChangeCheck())
			sp.stringValue = newValue;

		EditorGUI.EndProperty();
	}";

	public string GetOnGUI(bool hasFloat, bool hasInt, bool hasProgressBar, bool hasArray, bool hasVector2, bool hasQuaternion, bool hasBool,
						   bool hasTooltip, bool hasSpace, bool hasTexture, bool hasText)
	{
		string ret = "";
		ret += @"
	public override void OnInspectorGUI ()
	{	
		so.Update ();
		RefreshVars();
		
		EditorGUIUtility.LookLikeControls (135.0f, 50.0f);

		for (int i = 0; i < properties.Length; i += 1) 
		{
			InspectorPlusVar v = vars[i];
			
			if (v.active && properties[i] != null) 
			{
				SerializedProperty sp = properties [i];";

		bool any = hasFloat || hasInt || hasProgressBar || hasArray || hasVector2 || hasQuaternion || hasBool || hasSpace || hasTexture || hasText;

		if (any)
		{
			ret += @"string s = v.type;
							 bool skip = false;";
		}

		ret += @"
				name = v.name;
                dispName = v.dispName;

				GUI.enabled = v.canWrite;

                GUILayout.BeginHorizontal();

                if (v.toggleLevel != 0)
                   GUILayout.Space(v.toggleLevel * 10.0f);
                ";

		if (hasVector2)
			ret += @"
                if (s == typeof(Vector2).Name){
                    Vector2Field(sp);
                    skip = true;
                }";
		if (hasFloat)
			ret += @"
                if (s == typeof(float).Name){
                    FloatField(sp, v);
                    skip = true;
                }";
		if (hasInt)
			ret += @"
                if (s == typeof(int).Name){
                    IntField(sp, v);
                    skip = true;
                }";
		if (hasQuaternion)
			ret += @"
                if (s == typeof(Quaternion).Name){
                    QuaternionField(sp);
                    skip = true;
                }";
		if (hasBool)
			ret += @"
                if (s == typeof(bool).Name){
                    i += BoolField(sp, v);
                    skip = true;
                }";
		if (hasTexture)
			ret += @"
                if (s == typeof(Texture2D).Name || s == typeof(Texture).Name){
                    TextureGUI(sp, v);
                    skip = true;
                }";

		if (hasText)
			ret += @"
                if (s == typeof(string).Name){
                    TextGUI(sp, v);
                    skip = true;
                }";

		if (hasArray)
			ret += @"
                if (sp.isArray && s != typeof(string).Name){
                    ArrayGUI(sp, name);
                    skip = true;
                }";


		if (hasVector2 || hasFloat || hasInt || hasQuaternion || hasBool || hasArray)
			ret += @"
                if (!skip)";
		ret += @"
                    PropertyField(sp, name);
                GUILayout.EndHorizontal();
                GUI.enabled = true;";

		if (hasTooltip)
			ret += @"
				if (v.hasTooltip)
				{
	                Rect last = GUILayoutUtility.GetLastRect();
	                GUI.Label(last, new GUIContent(" + Quot("") + @", v.tooltip));

                    GUIStyle style = new GUIStyle();
                    style.fixedWidth = 250.0f;
                    style.wordWrap = true;

					Vector2 size = new GUIStyle().CalcSize(new GUIContent(GUI.tooltip));
					tooltipRect = new Rect(Event.current.mousePosition.x + 4.0f, Event.current.mousePosition.y + 12.0f, 28.0f + size.x, 9.0f + size.y);

                    if (tooltipRect.width > 250.0f)
                    {
                        float delt = (tooltipRect.width - 250.0f);
                        tooltipRect.width -= delt;
                        tooltipRect.height += size.y * Mathf.CeilToInt(delt / 250.0f);
                    }
				}";
		ret += @"
			}";
		if (hasSpace)
			ret += @"
			if (v.space == 0.0f)
				continue;
			float usedSpace = 0.0f;
			for (int j = 0; j < v.numSpace; j += 1) {
				if (v.labelEnabled [j] || v.buttonEnabled [j])
					usedSpace += 18.0f;
			}
			if (v.space == 0.0f)
				continue;
			float space = Mathf.Max (0.0f, (v.space - usedSpace) / 2.0f);
			GUILayout.Space (space);
			for (int j = 0; j < v.numSpace; j += 1) {
                bool buttonLine = false;
                for (int k = 0; k < 4; k += 1) if (v.buttonEnabled[j * 4 + k]) buttonLine = true;
                if (!v.labelEnabled[j] && !buttonLine)
                    continue;


                GUILayout.BeginHorizontal();
                if (v.labelEnabled[j])
                {
                    GUIStyle boldItalic = new GUIStyle();
                    boldItalic.margin = new RectOffset(5, 5, 5, 5);
                    
                    if (v.labelAlign[j] == 0)
                        boldItalic.alignment = TextAnchor.MiddleLeft;
                    else if (v.labelAlign[j] == 1)
                        boldItalic.alignment = TextAnchor.MiddleCenter;
                    else if (v.labelAlign[j] == 2)
                        boldItalic.alignment = TextAnchor.MiddleRight;
                    
                    if (v.labelBold[j] && v.labelItalic[j])
                        boldItalic.fontStyle = FontStyle.BoldAndItalic;
                    else if (v.labelBold[j])
                        boldItalic.fontStyle = FontStyle.Bold;
                    else if (v.labelItalic[j])
                        boldItalic.fontStyle = FontStyle.Italic;

                    GUILayout.Label(v.label[j], boldItalic);
                    boldItalic.alignment = TextAnchor.MiddleLeft;
                }
                bool alignRight = (v.labelEnabled[j] && buttonLine);

                if (!alignRight)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }

                GUILayout.FlexibleSpace();
                for (int k = 0; k < 4; k += 1)
                {
                    if (v.buttonEnabled[j * 4 + k])
                    {
                        if (!v.buttonCondense[j] && !alignRight)
                            GUILayout.FlexibleSpace();

                        string style = " + Quot("Button") + @";
                        if (v.buttonCondense[j])
                        {
                            bool hasLeft = false;
                            bool hasRight = false;
                            for(int p = k - 1; p >= 0; p -= 1)
                                if (v.buttonEnabled[j * 4 + p])
                                    hasLeft = true;
                            for (int p = k + 1; p < 4; p += 1)
                                if (v.buttonEnabled[j * 4 + p])
                                    hasRight = true;

                            if (!hasLeft && hasRight)
                                style = " + Quot("ButtonLeft") + @";
                            else if (hasLeft && hasRight)
                                style = " + Quot("ButtonMid") + @";
                            else if (hasLeft && !hasRight)
                                style = " + Quot("ButtonRight") + @";
                            else if (!hasLeft && !hasRight)
                                style = " + Quot("Button") + @";
                        }

                        if (GUILayout.Button(v.buttonText[j * 4 + k], style, GUILayout.MinWidth(60.0f)))
                        {
                            foreach (object t in targets)
                            {
                                MethodInfo m = t.GetType().GetMethod(v.buttonCallback[j * 4 + k], BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
                                if (m != null)
                                    m.Invoke(target, null);
                            }
                        }

                        if (!v.buttonCondense[j] && !alignRight)
                            GUILayout.FlexibleSpace();

                        
                    }
                }
                GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal ();
			}
			GUILayout.Space (space);";
		ret += @"
		}
        so.ApplyModifiedProperties (); ";
		if (hasTooltip)
			ret += @"
		if (!string.IsNullOrEmpty(GUI.tooltip))
        {
            GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            GUI.Box(tooltipRect, new GUIContent());
			EditorGUI.HelpBox(tooltipRect, GUI.tooltip, MessageType.Info);
			Repaint();
		}
		GUI.tooltip = " + Quot("") + ";";


		return ret;
	}


	public string watermark = @"
        //NOTE NOTE NOTE: WATERMARK HERE
        //You are free to remove this
        //START REMOVE HERE
        GUILayout.BeginHorizontal();
        GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        GUILayout.FlexibleSpace();
        GUILayout.Label(" + Quot("Created with") + @");
        GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.6f);
        if (GUILayout.Button(" + Quot("Inspector++") + @"))
            Application.OpenURL(" + Quot("http://forum.unity3d.com/threads/136727-Inspector-Meh-to-WOW-inspectors") + @");
        GUI.color = new Color(1.0f, 1.0f, 1.0f);
		GUILayout.EndHorizontal();
        //END REMOVE HERE
    }";


	public string vectorScene = @"
	void VectorScene(InspectorPlusVar v, string s, Transform t)
	{
		Vector3 val;
		
		if (s == typeof(Vector3).Name)
			val = (Vector3)GetTargetField(name);
		else 
			val = (Vector3)((Vector2)GetTargetField(name));
		
		Vector3 newVal = Vector3.zero;
		Vector3 curVal = Vector3.zero;
		bool setVal = false;
		bool relative = v.relative;
		bool scale = v.scale;

		switch (v.vectorDrawType) {
		case InspectorPlusVar.VectorDrawType.Direction:
			curVal = relative ? val:val - t.position;
            float size = scale ? Mathf.Min(2.0f, Mathf.Sqrt(curVal.magnitude) / 2.0f) : 1.0f;
            size *= HandleUtility.GetHandleSize(t.position);
			Handles.ArrowCap (0, t.position, curVal != Vector3.zero ? Quaternion.LookRotation (val.normalized) : Quaternion.identity, size);
			break;

		case InspectorPlusVar.VectorDrawType.Point:
			curVal = relative ? val:t.position + val;
			Handles.SphereCap (0, curVal, Quaternion.identity, 0.1f);
			break;

		case InspectorPlusVar.VectorDrawType.PositionHandle:
			curVal = relative ? t.position + val:val;
			setVal = true;
			newVal = Handles.PositionHandle (curVal, Quaternion.identity) - (relative ? t.position : Vector3.zero);
			break;

		case InspectorPlusVar.VectorDrawType.Scale:
			setVal = true;
            curVal = relative ? t.localScale + val :val;
            newVal = Handles.ScaleHandle(curVal, t.position + v.offset, t.rotation, HandleUtility.GetHandleSize(t.position + v.offset)) - (relative ? t.localScale : Vector3.zero);
			break;
			
		case InspectorPlusVar.VectorDrawType.Rotation:
			setVal = true;
            curVal = relative ? val + t.rotation.eulerAngles : val;
			newVal = Handles.RotationHandle(Quaternion.Euler(curVal), t.position + v.offset).eulerAngles - (relative?t.rotation.eulerAngles:Vector3.zero);
			break;
		}
	
		if (setVal)
		{
			object newObjectVal = newVal;
			
			if (s==typeof(Vector2).Name)
				newObjectVal = (Vector2)newVal;
			else if (s == typeof(Quaternion).Name)
				newObjectVal = Quaternion.Euler(newVal);
						
			SetTargetField(name, newObjectVal);
		}
	}";

	public string quaternionScene = @"	
	void QuaternionScene(Transform t, Vector3 offset)
	{
		Quaternion val = (Quaternion)GetTargetField(name);
		SetTargetField(name, Handles.RotationHandle (val, t.position + offset));
	}";


	public string GetSceneString(bool hasScene, bool hasVectorScene, bool hasQuaternionScene)
	{
		string ret = "";
		ret += @"
	object GetTargetField(string name){return target.GetType ().GetField (name).GetValue (target);}
	void SetTargetField(string name, object value){target.GetType ().GetField (name).SetValue (target, value);}
	//some magic to draw the handles
	public void OnSceneGUI ()
	{
		Transform t = ((MonoBehaviour)target).transform;
		
		foreach (InspectorPlusVar v in vars) {
			if (!v.active)
				continue;

			string s = v.type;
			name = v.name;";

		if (hasVectorScene)
			ret += @"
			if (s == typeof(Vector3).Name || s == typeof(Vector2).Name) 
				VectorScene(v, s, t);";
		if (hasQuaternionScene)
			ret += @"
			if (s == typeof(Quaternion).Name && v.QuaternionHandle) 
				QuaternionScene(t, v.offset);";

		ret += @"
	    }
    }";
		return ret;
	}



	public InspectorPlusFilerStrings(string name, string fileName, bool hasInspector)
	{
		header = @"
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;
using Object = UnityEngine.Object;";
		if (hasInspector)
			header += @"
using InspectorPlusVar = " + fileName + @".InspectorPlusVar;";
		header += @"

[CanEditMultipleObjects]
[CustomEditor(typeof(" + name + @"))]
public class " + fileName + @" : Editor {";
	}
}
#endif
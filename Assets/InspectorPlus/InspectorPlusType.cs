#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class InspectorPlusType {
	static AppDomain app = AppDomain.CurrentDomain;
	
    public static Type Get(string name)
    {
		foreach(Assembly a in app.GetAssemblies()) {
			var types = a.GetTypes();

			foreach (var type in types) {
				if (type.Name.ToLower() == name.ToLower()) {
					return type;
				}
			}
		}
		
		return null;
    }
}
#endif
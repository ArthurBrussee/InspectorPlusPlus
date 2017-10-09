using UnityEngine;
using System;
using System.Collections.Generic;



namespace TestSpace
{
	[Serializable]
	public class MyTestClass2
	{
		public int x;
		public float y;
		public bool z;
	}


	[Serializable]
	public class MyTestClass
	{
		public int x;
		public float y;
		public bool z;

		public MyTestClass2 tc;
	}


	public class InspectorPlusTestScript : MonoBehaviour
	{
		public Transform target;
		public Vector3 lookAtPoint;
		public Vector3 direction;

		public Vector2 screenPos;

		public float maxSpeed;
		public float health;

		public int MyInt;

		public int level;
		public int myAwesomeInt;

		public Transform[] myTransforms = new Transform[5];


		/// <summary>
		/// This tooltip is a summary
		/// </summary>
		[SerializeField] private float dontHideMe;

		public string stringTest;

		/// <summary>
		/// Rotation (with handle in scene)
		/// </summary>
		public Quaternion rotation;

		public bool toggleField = false;
		public bool toggle1 = true;
		public float toggleStr = 0.0f;

		public float my_ugly_var;

		public Texture myTextureWithLargePreview;

		public Texture textureTest;
		public MyTestClass myTestClass;
		public List<MyTestClass> intList;

		public Rect test;


		// Use this for initialization
		private void Start()
		{

		}

		// Update is called once per frame
		private void Update()
		{

		}



		private void OnButton()
		{
			Debug.Log("Button!");
		}

	}
}

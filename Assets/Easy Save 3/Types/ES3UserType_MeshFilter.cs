using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("sharedMesh", "mesh", "name")]
	public class ES3UserType_MeshFilter : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_MeshFilter() : base(typeof(UnityEngine.MeshFilter)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.MeshFilter)obj;
			
			writer.WritePropertyByRef("sharedMesh", instance.sharedMesh);
			writer.WritePropertyByRef("mesh", instance.mesh);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.MeshFilter)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "sharedMesh":
						instance.sharedMesh = reader.Read<UnityEngine.Mesh>(ES3Type_Mesh.Instance);
						break;
					case "mesh":
						instance.mesh = reader.Read<UnityEngine.Mesh>(ES3Type_Mesh.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_MeshFilterArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_MeshFilterArray() : base(typeof(UnityEngine.MeshFilter[]), ES3UserType_MeshFilter.Instance)
		{
			Instance = this;
		}
	}
}
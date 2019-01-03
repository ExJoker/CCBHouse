﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
#endif

namespace Coffee.UIExtensions
{
	/// <summary>
	/// UIEffect.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(Graphic))]
	[DisallowMultipleComponent]
	public class UIEffect : BaseMeshEffect
#if UNITY_EDITOR
		, ISerializationCallbackReceiver
#endif
	{
		/// <summary>
		/// Additional shadow.
		/// </summary>
		[System.Serializable]
		public class AdditionalShadow
		{
			/// <summary>
			/// How far is the blurring shadow from the graphic.
			/// </summary>
			[Range(0, 1)] public float shadowBlur = 0.25f;

			/// <summary>
			/// Shadow effect mode.
			/// </summary>
			public ShadowStyle shadowMode = ShadowStyle.Shadow;

			/// <summary>
			/// Color for the shadow effect.
			/// </summary>
			public Color shadowColor = Color.black;

			/// <summary>
			/// How far is the shadow from the graphic.
			/// </summary>
			public Vector2 effectDistance = new Vector2(1f, -1f);

			/// <summary>
			/// Should the shadow inherit the alpha from the graphic?
			/// </summary>
			public bool useGraphicAlpha = true;
		}

		//################################
		// Constant or Static Members.
		//################################
		public const string shaderName = "UI/Hidden/UI-Effect";

		/// <summary>
		/// Tone effect mode.
		/// </summary>
		public enum ToneMode
		{
			None = 0,
			Grayscale,
			Sepia,
			Nega,
			Pixel,
			Mono,
			Cutoff,
			Hue,
		}

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public enum ColorMode
		{
			None = 0,
			Set,
			Add,
			Sub,
		}

		/// <summary>
		/// Shadow effect style.
		/// </summary>
		public enum ShadowStyle
		{
			None = 0,
			Shadow,
			Outline,
			Outline8,
			Shadow3,
		}

		/// <summary>
		/// Blur effect mode.
		/// </summary>
		public enum BlurMode
		{
			None = 0,
			Fast,
			Medium,
			Detail,
		}


		//################################
		// Serialize Members.
		//################################
		[SerializeField][Range(0, 1)] float m_ToneLevel = 1;
		[SerializeField][Range(0, 1)] float m_Blur = 0.25f;
		[SerializeField][Range(0, 1)] float m_ShadowBlur = 0.25f;
		[SerializeField] ShadowStyle m_ShadowStyle;
		[SerializeField] ToneMode m_ToneMode;
		[SerializeField] ColorMode m_ColorMode;
		[SerializeField] BlurMode m_BlurMode;
		[SerializeField] Color m_ShadowColor = Color.black;
		[SerializeField] Vector2 m_EffectDistance = new Vector2(1f, -1f);
		[SerializeField] bool m_UseGraphicAlpha = true;
		[SerializeField] Color m_EffectColor = Color.white;
		[SerializeField] Material m_EffectMaterial;
		[SerializeField] List<AdditionalShadow> m_AdditionalShadows = new List<AdditionalShadow>();


		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Graphic affected by the UIEffect.
		/// </summary>
		new public Graphic graphic { get { return base.graphic; } }

		/// <summary>
		/// Tone effect level between 0(no effect) and 1(complete effect).
		/// </summary>
		public float toneLevel{ get { return m_ToneLevel; } set { m_ToneLevel = Mathf.Clamp(value, 0, 1); _SetDirty(); } }

		/// <summary>
		/// How far is the blurring from the graphic.
		/// </summary>
		public float blur { get { return m_Blur; } set { m_Blur = Mathf.Clamp(value, 0, 2); _SetDirty(); } }

		/// <summary>
		/// How far is the blurring shadow from the graphic.
		/// </summary>
		public float shadowBlur { get { return m_ShadowBlur; } set { m_ShadowBlur = Mathf.Clamp(value, 0, 2); _SetDirty(); } }

		/// <summary>
		/// Shadow effect mode.
		/// </summary>
		public ShadowStyle shadowStyle { get { return m_ShadowStyle; } set { m_ShadowStyle = value; _SetDirty(); } }

		/// <summary>
		/// Tone effect mode.
		/// </summary>
		public ToneMode toneMode { get { return m_ToneMode; } }

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public ColorMode colorMode { get { return m_ColorMode; } }

		/// <summary>
		/// Blur effect mode.
		/// </summary>
		public BlurMode blurMode { get { return m_BlurMode; } }

		/// <summary>
		/// Color for the shadow effect.
		/// </summary>
		public Color shadowColor { get { return m_ShadowColor; } set { m_ShadowColor = value; _SetDirty(); } }

		/// <summary>
		/// How far is the shadow from the graphic.
		/// </summary>
		public Vector2 effectDistance { get { return m_EffectDistance; } set { m_EffectDistance = value; _SetDirty(); } }

		/// <summary>
		/// Should the shadow inherit the alpha from the graphic?
		/// </summary>
		public bool useGraphicAlpha { get { return m_UseGraphicAlpha; } set { m_UseGraphicAlpha = value; _SetDirty(); } }

		/// <summary>
		/// Color for the color effect.
		/// </summary>
		public Color effectColor { get { return m_EffectColor; } set { m_EffectColor = value; _SetDirty(); } }

		/// <summary>
		/// Effect material.
		/// </summary>
		public virtual Material effectMaterial { get { return m_EffectMaterial; } }

		/// <summary>
		/// Additional Shadows.
		/// </summary>
		public List<AdditionalShadow> additionalShadows { get { return m_AdditionalShadows; } }

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			graphic.material = effectMaterial;
			base.OnEnable();
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled () or inactive.
		/// </summary>
		protected override void OnDisable()
		{
			graphic.material = null;
			base.OnDisable();
		}

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		public override void ModifyMesh(VertexHelper vh)
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			UIVertex vt;
			vh.GetUIVertexStream(s_Verts);

			//================================
			// Effect modify original vertices.
			//================================
			{
				// Pack some effect factors to 1 float.
				Vector2 factor = new Vector2(
									_PackToFloat(toneLevel, 0, blur, 0),
									_PackToFloat(effectColor.r, effectColor.g, effectColor.b, effectColor.a)
								 );

				for (int i = 0; i < s_Verts.Count; i++)
				{
					vt = s_Verts[i];

					// Set UIEffect prameters to vertex.
					vt.uv1 = factor;
					s_Verts[i] = vt;
				}
			}

			//================================
			// Append shadow vertices.
			//================================
			{
				var inputVertCount = s_Verts.Count;
				var start = 0;
				var end = inputVertCount;

				// Additional Shadows.
				for (int i = additionalShadows.Count - 1; 0 <= i; i--)
				{
					AdditionalShadow shadow = additionalShadows[i];
					_ApplyShadow(s_Verts, ref start, ref end, shadow.shadowMode, toneLevel, shadow.shadowBlur, shadow.effectDistance, shadow.shadowColor, shadow.useGraphicAlpha);
				}

				// Shadow.
				_ApplyShadow(s_Verts, ref start, ref end, shadowStyle, toneLevel, shadowBlur, effectDistance, shadowColor, useGraphicAlpha);
			}

			vh.Clear();
			vh.AddUIVertexTriangleStream(s_Verts);

			s_Verts.Clear();
		}

#if UNITY_EDITOR

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			var obj = this;
			EditorApplication.delayCall += () =>
			{
				if (Application.isPlaying || !obj)
					return;

				var mat = (0 == toneMode) && (0 == colorMode) && (0 == blurMode)
						? null
						: GetOrGenerateMaterialVariant(Shader.Find(shaderName), toneMode, colorMode, blurMode);

				if(m_EffectMaterial == mat && graphic.material == mat)
					return;
					
				graphic.material = m_EffectMaterial = mat;
				EditorUtility.SetDirty(this);
				EditorUtility.SetDirty(graphic);
				EditorApplication.delayCall +=AssetDatabase.SaveAssets;
			};
		}
		
		public static Material GetMaterial(Shader shader, ToneMode tone, ColorMode color, BlurMode blur)
		{
			string variantName = GetVariantName(shader, tone, color, blur);
			return AssetDatabase.FindAssets("t:Material " + Path.GetFileName(shader.name))
				.Select(x => AssetDatabase.GUIDToAssetPath(x))
				.SelectMany(x => AssetDatabase.LoadAllAssetsAtPath(x))
				.OfType<Material>()
				.FirstOrDefault(x => x.name == variantName);
		}


		public static Material GetOrGenerateMaterialVariant(Shader shader, ToneMode tone, ColorMode color, BlurMode blur)
		{
			if (!shader)
				return null;

			Material mat = GetMaterial(shader, tone, color, blur);

			if (!mat)
			{
				Debug.Log("Generate material : " + GetVariantName(shader, tone, color, blur));
				mat = new Material(shader);

				if (0 < tone)
					mat.EnableKeyword("UI_TONE_" + tone.ToString().ToUpper());
				if (0 < color)
					mat.EnableKeyword("UI_COLOR_" + color.ToString().ToUpper());
				if (0 < blur)
					mat.EnableKeyword("UI_BLUR_" + blur.ToString().ToUpper());

				mat.name = GetVariantName(shader, tone, color, blur);
				mat.hideFlags |= HideFlags.NotEditable;

#if UIEFFECT_SEPARATE
				bool isMainAsset = true;
				string dir = Path.GetDirectoryName(GetDefaultMaterialPath (shader));
				string materialPath = Path.Combine(Path.Combine(dir, "Separated"), mat.name + ".mat");
#else
				bool isMainAsset = (0 == tone) && (0 == color) && (0 == blur);
				string materialPath = GetDefaultMaterialPath (shader);
#endif
				if (isMainAsset)
				{
					Directory.CreateDirectory(Path.GetDirectoryName(materialPath));
					AssetDatabase.CreateAsset(mat, materialPath);
					AssetDatabase.SaveAssets();
				}
				else
				{
					mat.hideFlags |= HideFlags.HideInHierarchy;
					AssetDatabase.AddObjectToAsset(mat, materialPath);
				}
			}
			return mat;
		}

		public static string GetDefaultMaterialPath(Shader shader)
		{
			var name = Path.GetFileName (shader.name);
			return AssetDatabase.FindAssets("t:Material " + name)
				.Select(x => AssetDatabase.GUIDToAssetPath(x))
				.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == name)
				?? ("Assets/UIEffect/Materials/" + name + ".mat");
		}

		public static string GetVariantName(Shader shader, ToneMode tone, ColorMode color, BlurMode blur)
		{
			return
#if UIEFFECT_SEPARATE
				"[Separated] " + Path.GetFileName(shader.name)
#else
				Path.GetFileName(shader.name)
#endif
				+ (0 < tone ? "-" + tone : "")
				+ (0 < color ? "-" + color : "")
				+ (0 < blur ? "-" + blur : "");
		}
#endif

		//################################
		// Private Members.
		//################################
		static readonly List<UIVertex> s_Verts = new List<UIVertex>();

		/// <summary>
		/// Append shadow vertices.
		/// * It is similar to Shadow component implementation.
		/// </summary>
		static void _ApplyShadow(List<UIVertex> verts, ref int start, ref int end, ShadowStyle mode, float toneLevel, float blur, Vector2 effectDistance, Color color, bool useGraphicAlpha)
		{
			if (ShadowStyle.None == mode)
				return;

			var factor = new Vector2(
							 _PackToFloat(toneLevel, 0, blur, 0),
							 _PackToFloat(color.r, color.g, color.b, 1)
						 );

			// Append Shadow.
			_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, effectDistance.x, effectDistance.y, factor, color, useGraphicAlpha);

			// Append Shadow3.
			if (ShadowStyle.Shadow3 == mode)
			{
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, effectDistance.x, 0, factor, color, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, 0, effectDistance.y, factor, color, useGraphicAlpha);
			}

			// Append Outline.
			else if (ShadowStyle.Outline == mode)
			{
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, effectDistance.x, -effectDistance.y, factor, color, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -effectDistance.x, effectDistance.y, factor, color, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -effectDistance.x, -effectDistance.y, factor, color, useGraphicAlpha);
			}

			// Append Outline8.
			else if (ShadowStyle.Outline8 == mode)
			{
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, effectDistance.x, -effectDistance.y, factor, color, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -effectDistance.x, effectDistance.y, factor, color, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -effectDistance.x, -effectDistance.y, factor, color, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, -effectDistance.x, 0, factor, color, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, 0, -effectDistance.y, factor, color, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, effectDistance.x, 0, factor, color, useGraphicAlpha);
				_ApplyShadowZeroAlloc(s_Verts, ref start, ref end, 0, effectDistance.y, factor, color, useGraphicAlpha);
			}
		}

		/// <summary>
		/// Append shadow vertices.
		/// * It is similar to Shadow component implementation.
		/// </summary>
		static void _ApplyShadowZeroAlloc(List<UIVertex> verts, ref int start, ref int end, float x, float y, Vector2 factor, Color color, bool useGraphicAlpha)
		{
			// Check list capacity.
			var neededCapacity = verts.Count + end - start;
			if (verts.Capacity < neededCapacity)
				verts.Capacity = neededCapacity;

			// Append shadow vertices to the front of list.
			// * The original vertex is pushed backward.
			UIVertex vt;
			for (int i = start; i < end; ++i)
			{
				vt = verts[i];
				verts.Add(vt);

				Vector3 v = vt.position;
				vt.position.Set(v.x + x, v.y + y, v.z);

				Color vertColor = color;
				vertColor.a = useGraphicAlpha ? color.a * vt.color.a / 255 : color.a;
				vt.color = vertColor;

				// Set UIEffect prameters to vertex.
				vt.uv1 = factor;
				verts[i] = vt;
			}

			// Update next shadow offset.
			start = end;
			end = verts.Count;
		}

		/// <summary>
		/// Mark the UIEffect as dirty.
		/// </summary>
		void _SetDirty()
		{
			if(graphic)
				graphic.SetVerticesDirty();
		}

		/// <summary>
		/// Pack 4 low-precision [0-1] floats values to a float.
		/// Each value [0-1] has 64 steps(6 bits).
		/// </summary>
		static float _PackToFloat(float x, float y, float z, float w)
		{
			const int PRECISION = (1 << 6) - 1;
			return (Mathf.FloorToInt(w * PRECISION) << 18)
			+ (Mathf.FloorToInt(z * PRECISION) << 12)
			+ (Mathf.FloorToInt(y * PRECISION) << 6)
			+ Mathf.FloorToInt(x * PRECISION);
		}
	}
}

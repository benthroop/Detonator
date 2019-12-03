using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

	Detonator - A parametric explosion system for Unity
	Created by Ben Throop in August 2009 for the Unity Summer of Code
	
	Simplest use case:
	
	1) Use a prefab
	
	OR
	
	1) Attach a Detonator to a GameObject, either through code or the Unity UI
	2) Either set the Detonator's ExplodeOnStart = true or call Explode() yourself when the time is right
	3) View explosion :)
	
	Medium Complexity Use Case:
	
	1) Attach a Detonator as above 
	2) Change parameters, add your own materials, etc
	3) Explode()
	4) View Explosion
	
	Super Fun Use Case:
	
	1) Attach a Detonator as above
	2) Drag one or more DetonatorComponents to that same GameObject
	3) Tweak those parameters
	4) Explode()
	5) View Explosion
	
	Better documentation is included as a PDF with this package, or is available online. Check the Unity site for a link
	or visit my site, listed below.
	
	Ben Throop
	@ben_throop
*/

//This script uses the custom inspector "DetonatorCustomInspector.cs"
//Any public values added/removed here would also need to be replicated in that file
[AddComponentMenu("Detonator/Detonator")]
public class Detonator : MonoBehaviour {

	private static float _baseSize = 30f;
	private static Color _baseColor = new Color(1f, .423f, 0f, .5f);
	private static float _baseDuration = 3f;

	/*
		_baseSize reflects the size that DetonatorComponents at size 1 match. Yes, this is really big (30m)
		size below is the default Detonator size, which is more reasonable for typical useage. 
		It wasn't my intention for them to be different, and I may change that, but for now, that's how it is.
	*/
	public float size = 10f; 
	public Color color = Detonator._baseColor;
	public bool explodeOnStart = true;
	public float duration = Detonator._baseDuration;
	public float detail = 1f; 
	public float upwardsBias = 0f;
	
	public float destroyTime = 7f; //sorry this is not auto calculated... yet.
	public bool useWorldSpace = true;
	public Vector3 direction = Vector3.zero;
	
	public Material fireballAMaterial;
	public bool isFireballAFlipbook;
	public Vector2 fireballAFlipbookSize = Vector2.one;
	public Material fireballBMaterial;
	public bool isFireballBFlipbook;
	public Vector2 fireballBFlipbookSize = Vector2.one;
	public Material smokeAMaterial;
	public bool isSmokeAFlipbook;
	public Vector2 smokeAFlipbookSize = Vector2.one;
	public Material smokeBMaterial;
	public bool isSmokeBFlipbook;
	public Vector2 smokeBFlipbookSize = Vector2.one;
	public Material shockwaveMaterial;
	public bool isShockwaveFlipbook;
	public Vector2 shockwaveFlipbookSize = Vector2.one;
	public Material sparksMaterial;
	public bool isSparksFlipbook;
	public Vector2 sparksFlipbookSize = Vector2.one;
	public Material glowMaterial;
	public bool isGlowFlipbook;
	public Vector2 glowFlipbookSize = Vector2.one;
	public Material heatwaveMaterial;
		
    private Component[] components;

	private List<DetonatorFireball> _fireball = new List<DetonatorFireball>();
	private List<DetonatorSmoke> _smoke = new List<DetonatorSmoke>();
	private List<DetonatorShockwave> _shockwave = new List<DetonatorShockwave>();
	private List<DetonatorSparks> _sparks = new List<DetonatorSparks>();
	private List<DetonatorGlow> _glow = new List<DetonatorGlow>();
	private List<DetonatorHeatwave> _heatwave = new List<DetonatorHeatwave>();
	private List<DetonatorLight> _light = new List<DetonatorLight>();
	private List<DetonatorForce> _force = new List<DetonatorForce>();
	public bool autoCreateFireball = true;
	public bool autoCreateSparks = true;
	public bool autoCreateShockwave = true;
	public bool autoCreateSmoke = true;
	public bool autoCreateGlow = true;
	public bool autoCreateLight = true;
	public bool autoCreateForce = true;
	public bool autoCreateHeatwave = false;
	
	void Awake() 
	{
		FillDefaultMaterials();
		
        components = this.GetComponents(typeof(DetonatorComponent));
		foreach (DetonatorComponent dc in components)
		{
			if (dc is DetonatorFireball)
			{
				_fireball.Add(dc as DetonatorFireball);
			}
			if (dc is DetonatorSparks)
			{
				_sparks.Add(dc as DetonatorSparks);
			}
			if (dc is DetonatorShockwave)
			{
				_shockwave.Add(dc as DetonatorShockwave);
			}
			if (dc is DetonatorSmoke)
			{
				_smoke.Add(dc as DetonatorSmoke);
			}
			if (dc is DetonatorGlow)
			{
				_glow.Add(dc as DetonatorGlow);
			}
			if (dc is DetonatorLight)
			{
				_light.Add(dc as DetonatorLight);
			}
			if (dc is DetonatorForce)
			{
				_force.Add(dc as DetonatorForce);
			}
			if (dc is DetonatorHeatwave)
			{
				_heatwave.Add(dc as DetonatorHeatwave);
			}
		}
		
		if (_fireball.Count == 0 && autoCreateFireball)
		{
			_fireball.Add(gameObject.AddComponent<DetonatorFireball>() as DetonatorFireball);
			_fireball[0].Reset();
		}
		
		if (_smoke.Count == 0 && autoCreateSmoke)
		{
			_smoke.Add(gameObject.AddComponent<DetonatorSmoke>() as DetonatorSmoke);
			_smoke[0].Reset();
		}
		
		if (_sparks.Count == 0 && autoCreateSparks)
		{
			_sparks.Add(gameObject.AddComponent<DetonatorSparks>() as DetonatorSparks);
			_sparks[0].Reset();
		}
		
		if (_shockwave.Count == 0 && autoCreateShockwave)
		{
			_shockwave.Add(gameObject.AddComponent<DetonatorShockwave>() as DetonatorShockwave);
			_shockwave[0].Reset();
		}
		
		if (_glow.Count == 0 && autoCreateGlow)
		{
			_glow.Add(gameObject.AddComponent<DetonatorGlow>() as DetonatorGlow);
			_glow[0].Reset();
		}
		
		if (_light.Count == 0 && autoCreateLight)
		{
			_light.Add(gameObject.AddComponent<DetonatorLight>() as DetonatorLight);
			_light[0].Reset();
		}
		
		if (_force.Count == 0 && autoCreateForce)
		{
			_force.Add(gameObject.AddComponent<DetonatorForce>() as DetonatorForce);
			_force[0].Reset();
		}

        if (_heatwave.Count == 0 && autoCreateHeatwave && SystemInfo.supportsImageEffects)
		{
			_heatwave.Add(gameObject.AddComponent<DetonatorHeatwave>() as DetonatorHeatwave);
			_heatwave[0].Reset();
		}
		
		components = this.GetComponents(typeof(DetonatorComponent));

		SetUpFlipbookSupport();
	}
	
	void FillDefaultMaterials()
	{
		if (!fireballAMaterial) fireballAMaterial = DefaultFireballAMaterial();
		if (!fireballBMaterial) fireballBMaterial = DefaultFireballBMaterial();
		if (!smokeAMaterial) smokeAMaterial = DefaultSmokeAMaterial();
		if (!smokeBMaterial) smokeBMaterial = DefaultSmokeBMaterial();
		if (!shockwaveMaterial) shockwaveMaterial = DefaultShockwaveMaterial();
		if (!sparksMaterial) sparksMaterial = DefaultSparksMaterial();
		if (!glowMaterial) glowMaterial = DefaultGlowMaterial();
		if (!heatwaveMaterial) heatwaveMaterial = DefaultHeatwaveMaterial();
	}

	void SetUpFlipbookSupport()
	{
		if (_fireball.Count > 0) 
		{
			foreach (DetonatorFireball dc in _fireball)
			{
				dc.SetFlipbookAStatus(isFireballAFlipbook, fireballAFlipbookSize);
				dc.SetFlipbookBStatus(isFireballBFlipbook, fireballBFlipbookSize);
			}
		}
		if (_smoke.Count > 0) 
		{
			foreach (DetonatorSmoke dc in _smoke)
			{
				dc.SetFlipbookAStatus(isSmokeAFlipbook, smokeAFlipbookSize);
				dc.SetFlipbookBStatus(isSmokeBFlipbook, smokeBFlipbookSize);
			}
		}
		if (_shockwave.Count > 0)
		{
			foreach (DetonatorShockwave dc in _shockwave)
				dc.SetFlipbookStatus(isShockwaveFlipbook, shockwaveFlipbookSize);
		}
		if (_sparks.Count > 0)
		{
			foreach (DetonatorSparks dc in _sparks)
				dc.SetFlipbookStatus(isSparksFlipbook, sparksFlipbookSize);
		}
		if (_glow.Count > 0)
		{
			foreach (DetonatorGlow dc in _glow)
				dc.SetFlipbookStatus(isGlowFlipbook, glowFlipbookSize);
		}
	}
	
	void Start()
	{
		if (explodeOnStart)
		{
			UpdateComponents();
			this.Explode();
		}
	}
	
	private float _lastExplosionTime = 1000f;
	void Update () 
    {
		if (destroyTime > 0f)
		{
			if (_lastExplosionTime + destroyTime <= Time.time)
			{
				Destroy(gameObject);
			}
		}
	}
	
	private bool _firstComponentUpdate = true;

	void UpdateComponents()
	{
		if (_firstComponentUpdate)
		{
			foreach (DetonatorComponent component in components)
			{
				component.Init();
				component.SetStartValues();
			}
			_firstComponentUpdate = false;
		}
		
		if (!_firstComponentUpdate)
		{
			float s = size / _baseSize;
			
			Vector3 sdir = new Vector3(direction.x * s, direction.y * s, direction.z * s);
			
			float d = duration / _baseDuration;
			
			foreach (DetonatorComponent component in components)
			{
				if (component.detonatorControlled)
				{
					component.size = component.startSize * s;
					component.timeScale = d;
					component.detail = component.startDetail * detail;
					component.force = new Vector3(component.startForce.x * s + sdir.x, component.startForce.y * s + sdir.y, component.startForce.z * s + sdir.z );
					component.velocity = new Vector3(component.startVelocity.x * s + sdir.x, component.startVelocity.y * s + sdir.y, component.startVelocity.z * s + sdir.z );
					
					//take the alpha of detonator color and consider it a weight - 1=use all detonator, 0=use all components
					component.color = Color.Lerp(component.startColor, color, color.a);
				}
			}
		}
	}
	
	private Component[] _subDetonators;
	
	 public void Explode() 
	{
		_lastExplosionTime = Time.time;
	
		foreach (DetonatorComponent component in components)
		{
			UpdateComponents();
			component.Explode();
		}
	}
	
	public void Reset() 
	{
		size = 10f; //this is hardcoded because _baseSize up top is not really the default as much as what we match to
		color = _baseColor;
		duration = _baseDuration;
		FillDefaultMaterials();
	}
	

	//Default Materials
	//The statics are so that even if there are multiple Detonators in the world, they
	//don't each create their own default materials. Theoretically this will reduce draw calls, but I haven't really
	//tested that.
	public static Material defaultFireballAMaterial;
	public static Material defaultFireballBMaterial;
	public static Material defaultSmokeAMaterial;
	public static Material defaultSmokeBMaterial;
	public static Material defaultShockwaveMaterial;
	public static Material defaultSparksMaterial;
	public static Material defaultGlowMaterial;
	public static Material defaultHeatwaveMaterial;
	
	public static Material DefaultFireballAMaterial()
	{
		if (defaultFireballAMaterial != null) return defaultFireballAMaterial;

        //2019+ counts Additive particles as legacy, this conditional makes sure it 
		//doesn't break the game when run on either version
		if (Shader.Find("Particles/Additive"))
			defaultFireballAMaterial = new Material(Shader.Find("Particles/Additive"));
		else if (Shader.Find("Legacy Shaders/Particles/Additive"))
			defaultFireballAMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));


		defaultFireballAMaterial.name = "FireballA-Default";
        Texture2D tex = Resources.Load("Detonator/Textures/Fireball") as Texture2D;
		defaultFireballAMaterial.SetColor("_TintColor", Color.white);
		defaultFireballAMaterial.mainTexture = tex;
		defaultFireballAMaterial.mainTextureScale = new Vector2(0.5f, 1f);
		return defaultFireballAMaterial;
	}

	public static Material DefaultFireballBMaterial()
	{
		if (defaultFireballBMaterial != null) return defaultFireballBMaterial;
        
		//2019+ counts Additive particles as legacy, this conditional makes sure it 
		//doesn't break the game when run on either version
		if (Shader.Find("Particles/Additive"))
			defaultFireballBMaterial = new Material(Shader.Find("Particles/Additive"));
		else if (Shader.Find("Legacy Shaders/Particles/Additive"))
			defaultFireballBMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));

		defaultFireballBMaterial.name = "FireballB-Default";
        Texture2D tex = Resources.Load("Detonator/Textures/Fireball") as Texture2D;
		defaultFireballBMaterial.SetColor("_TintColor", Color.white);
		defaultFireballBMaterial.mainTexture = tex;
		defaultFireballBMaterial.mainTextureScale = new Vector2(0.5f, 1f);
		defaultFireballBMaterial.mainTextureOffset = new Vector2(0.5f, 0f);
		return defaultFireballBMaterial;
	}
	
	public static Material DefaultSmokeAMaterial()
	{
		if (defaultSmokeAMaterial != null) return defaultSmokeAMaterial;
        
		//2019+ counts Alpha Blended particles as legacy, this conditional makes sure it 
		//doesn't break the game when run on either version
		if (Shader.Find("Particles/Alpha Blended"))
			defaultSmokeAMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
		else if (Shader.Find("Legacy Shaders/Particles/Alpha Blended"))
			defaultSmokeAMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));

		defaultSmokeAMaterial.name = "SmokeA-Default";
        Texture2D tex = Resources.Load("Detonator/Textures/Smoke") as Texture2D;
		defaultSmokeAMaterial.SetColor("_TintColor", Color.white);
		defaultSmokeAMaterial.mainTexture = tex;
		defaultSmokeAMaterial.mainTextureScale = new Vector2(0.5f, 1f);
		return defaultSmokeAMaterial;
	}
		
	public static Material DefaultSmokeBMaterial()
	{
		if (defaultSmokeBMaterial != null) return defaultSmokeBMaterial;
        
		//2019+ counts Alpha Blended particles as legacy, this conditional makes sure it 
		//doesn't break the game when run on either version
		if (Shader.Find("Particles/Alpha Blended"))
			defaultSmokeBMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
		else if (Shader.Find("Legacy Shaders/Particles/Alpha Blended"))
			defaultSmokeBMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
		
		defaultSmokeBMaterial.name = "SmokeB-Default";
        Texture2D tex = Resources.Load("Detonator/Textures/Smoke") as Texture2D;
		defaultSmokeBMaterial.SetColor("_TintColor", Color.white);
		defaultSmokeBMaterial.mainTexture = tex;
		defaultSmokeBMaterial.mainTextureScale = new Vector2(0.5f, 1f);
		defaultSmokeBMaterial.mainTextureOffset = new Vector2(0.5f, 0f);
		return defaultSmokeBMaterial;
	}
	
	public static Material DefaultSparksMaterial()
	{
		if (defaultSparksMaterial != null) return defaultSparksMaterial;
        
		//2019+ counts Additive particles as legacy, this conditional makes sure it 
		//doesn't break the game when run on either version
		if (Shader.Find("Particles/Additive"))
			defaultSparksMaterial = new Material(Shader.Find("Particles/Additive"));
		else if (Shader.Find("Legacy Shaders/Particles/Additive"))
			defaultSparksMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));

		defaultSparksMaterial.name = "Sparks-Default";
        Texture2D tex = Resources.Load("Detonator/Textures/GlowDot") as Texture2D;
		defaultSparksMaterial.SetColor("_TintColor", Color.white);
		defaultSparksMaterial.mainTexture = tex;
		return defaultSparksMaterial;
	}
	
	public static Material DefaultShockwaveMaterial()
	{	
		if (defaultShockwaveMaterial != null) return defaultShockwaveMaterial;
        
		//2019+ counts Additive particles as legacy, this conditional makes sure it 
		//doesn't break the game when run on either version
		if (Shader.Find("Particles/Additive"))
			defaultShockwaveMaterial = new Material(Shader.Find("Particles/Additive"));
		else if (Shader.Find("Legacy Shaders/Particles/Additive"))
			defaultShockwaveMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));

		defaultShockwaveMaterial.name = "Shockwave-Default";
        Texture2D tex = Resources.Load("Detonator/Textures/Shockwave") as Texture2D;
		defaultShockwaveMaterial.SetColor("_TintColor", new Color(0.1f,0.1f,0.1f,1f));
		defaultShockwaveMaterial.mainTexture = tex;
		return defaultShockwaveMaterial;
	}
	
	public static Material DefaultGlowMaterial()
	{
		if (defaultGlowMaterial != null) return defaultGlowMaterial;
        
		//2019+ counts Additive particles as legacy, this conditional makes sure it 
		//doesn't break the game when run on either version
		if (Shader.Find("Particles/Additive"))
			defaultGlowMaterial = new Material(Shader.Find("Particles/Additive"));
		else if (Shader.Find("Legacy Shaders/Particles/Additive"))
			defaultGlowMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
		
		defaultGlowMaterial.name = "Glow-Default";
        Texture2D tex = Resources.Load("Detonator/Textures/Glow") as Texture2D;
		defaultGlowMaterial.SetColor("_TintColor", Color.white);
		defaultGlowMaterial.mainTexture = tex;
		return defaultGlowMaterial;
	}
	
	public static Material DefaultHeatwaveMaterial()
	{
        //Unity Pro Only
        if (SystemInfo.supportsImageEffects)
        {
            if (defaultHeatwaveMaterial != null) return defaultHeatwaveMaterial;
            defaultHeatwaveMaterial = new Material(Shader.Find("HeatDistort"));
            defaultHeatwaveMaterial.name = "Heatwave-Default";
            Texture2D tex = Resources.Load("Detonator/Textures/Heatwave") as Texture2D;
            defaultHeatwaveMaterial.SetTexture("_BumpMap", tex);
            return defaultHeatwaveMaterial;
        }
        else
        {
            return null;
        }
	}
}

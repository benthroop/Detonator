using UnityEngine;
using System.Collections;

/*
	DetonatorBurstEmitter is an interface for DetonatorComponents to use to create particles
	
	- Handles common tasks for Detonator... almost every DetonatorComponent uses this for particles
	- Builds the gameobject with emitter, animator, renderer
	- Everything incoming is automatically scaled by size, timeScale, color
	- Enable oneShot functionality

	You probably don't want to use this directly... though you certainly can.
*/

public class DetonatorBurstEmitter : DetonatorComponent
{
    private ParticleSystem _particleSystem;
    //private ParticleEmitter _particleEmitter;
    //private ParticleRenderer _particleRenderer;
    //private ParticleAnimator _particleAnimator;

	private float _baseDamping = 0.1300004f;
	private float _baseSize = 1f;
	private Color _baseColor = Color.white;

    //The below modules are needed to update various modules to 2018.3+ -Z
    private ParticleSystem.LimitVelocityOverLifetimeModule _limitVelOverLifetime;
    private ParticleSystem.EmissionModule _emission;
    private ParticleSystem.SizeOverLifetimeModule _sizeOverLifetime;
    private ParticleSystem.MainModule _particleMain;
    private ParticleSystem.ColorOverLifetimeModule _colorOverLifetime;
    private ParticleSystem.ForceOverLifetimeModule _forceOverLifetime;
    private ParticleSystem.EmitParams _emitParams;
    private ParticleSystem.ShapeModule _psShape;

    public float damping = 1f;
	public float startRadius = 1f;
	public float maxScreenSize = 2f;
	public bool explodeOnAwake = false;
	public bool oneShot = true;
	public float sizeVariation = 0f;
	public float particleSize = 1f;
	public float count = 1;
	public float sizeGrow = 20f;
	public bool exponentialGrowth = true;
	public float durationVariation = 0f;
	public bool useWorldSpace = true;
	public float upwardsBias = 0f;
	public float angularVelocity = 20f;
	public bool randomRotation = true;

    public ParticleSystemRenderer renderMode;

    //TODO make this based on some var
    /*
	_sparksRenderer.particleRenderMode = ParticleRenderMode.Stretch;
	_sparksRenderer.lengthScale = 0f;
	_sparksRenderer.velocityScale = 0.7f;
	*/

    public Gradient colorGradient = new Gradient();
	public Color[] colorAnimation = new Color[5];
	public bool useExplicitColorAnimation = false;
	
	private bool _delayedExplosionStarted = false;
	private float _explodeDelay;
	
	public Material material;
	
	//unused
	override public void Init() 
	{
		print ("UNUSED");
	} 
	
    public void Awake()
    {
        _particleSystem = gameObject.AddComponent<ParticleSystem>();
        _particleSystem.Stop();
        _particleMain = _particleSystem.main;
        _particleMain.startLifetime = duration;
        renderMode = gameObject.GetComponent<ParticleSystemRenderer>();
        _psShape = _particleSystem.shape;
        //-Z
        //OR do I need 
        //_particleSystem = gameObject.AddComponent<ParticleSystem>() as ParticleSystem;

        //_particleEmitter = (gameObject.AddComponent<EllipsoidParticleEmitter>()) as ParticleEmitter;
        //_particleRenderer = (gameObject.AddComponent<ParticleRenderer>()) as ParticleRenderer;
        //_particleAnimator = (gameObject.AddComponent<ParticleAnimator>()) as ParticleAnimator;

        //-Z
        //Documentation: https://docs.unity3d.com/ScriptReference/Object-hideFlags.html
        //Objects created as hide and don't save must be explicitly destroyed by the owner of the object
        //Do I need a DestroyImmediate(_particleSystem); ?
        _particleSystem.hideFlags = HideFlags.HideAndDontSave;

        //_particleEmitter.hideFlags = HideFlags.HideAndDontSave;
        //_particleRenderer.hideFlags = HideFlags.HideAndDontSave;
        //_particleAnimator.hideFlags = HideFlags.HideAndDontSave;

        //-Z
        //This doesn't have a direct copy. Old documentation: https://docs.unity3d.com/Manual/class-ParticleAnimator.html
        //Damping can be used to decelerate or accelerate without changing their direction:
        //A value of 1 means no Damping is applied, the particles will not slow down or accelerate.
        //A value of 0 means particles will stop immediately.
        //A value of 2 means particles will double their speed every second.
        //New: https://gamedev.stackexchange.com/questions/102318/damping-in-unity-shuriken-particle-system
        //Limit Velocity Over Lifetime->Dampen in the Editor.
        //From Unity Docs: "(0-1) value that controls how much the exceeding velocity should be dampened. For example, a value of 0.5 will dampen exceeding velocity by 50%."
        //For best results set Speed under Limit Velocity Over Lifetime to a Curve that shows how you want the particles velocity to react over its lifetime then use Dampen to fine tune it to your liking.
        //This might not be the best way to do it now

        _limitVelOverLifetime = _particleSystem.limitVelocityOverLifetime;
        _limitVelOverLifetime.enabled = true;
        _limitVelOverLifetime.dampen = _baseDamping;

        //_particleAnimator.damping = _baseDamping;

        //I don't know if the emission part is needed, it might be on by default
        _emission = _particleSystem.emission;
        _emission.enabled = true;

        //How does the _renderMode know what it renders to? -Z
        //renderMode.renderMode = ParticleSystemRenderMode.Billboard;
        renderMode.maxParticleSize = maxScreenSize;
        renderMode.material = material;
        renderMode.material.color = Color.white; //Does this still need to be set as so? -Z

        //-Z
        //sizeGrow is replaced by sizeOverLifetime. This does not have a numberical value (replaced by curve), so we'll have to allow for the three different options
        //See https://docs.unity3d.com/2018.3/Documentation/ScriptReference/ParticleSystem-sizeOverLifetime.html for details
        _sizeOverLifetime = _particleSystem.sizeOverLifetime;
        _sizeOverLifetime.enabled = true;
        
        //-Z
        //Does this work?
        _sizeOverLifetime.sizeMultiplier = sizeGrow;
        //Else need to do this:
        //AnimationCurve curve = new AnimationCurve();
        //curve.AddKey(0.0f, 0.1f);
        //curve.AddKey(.75f, 1.0f);
        //_sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(sizeGrow, curve);
        
        //_particleEmitter.emit = false;
        //_particleRenderer.maxParticleSize = maxScreenSize;
        //_particleRenderer.material = material;
        //_particleRenderer.material.color = Color.white; //workaround for this not being settable elsewhere
        //_particleAnimator.sizeGrow = sizeGrow;

        if (explodeOnAwake)
		{
			Explode();
		}
    }
	
	private float _emitTime;
	private float speed = 3.0f;
	private float initFraction = 0.1f;

    //Check to see how growth is done in new system (SizeOverLifetime) -Z
    static float epsilon = 0.01f;
	
	void Update () 
	{
		//do exponential particle scaling once emitted
		if (exponentialGrowth)
		{
			//float elapsed = Time.time - _emitTime;
			//float oldSize = SizeFunction(elapsed - epsilon);
			//float newSize = SizeFunction(elapsed);
			//float growth = ((newSize / oldSize) - 1) / epsilon;

            //Copy info from above if this doesn't work -Z
            _sizeOverLifetime.sizeMultiplier = sizeGrow;

            //_particleAnimator.sizeGrow = growth;
		}
		else
		{
            //Copy info from above if this doesn't work -Z
            _sizeOverLifetime.sizeMultiplier = sizeGrow;

            //_particleAnimator.sizeGrow = sizeGrow;
		}
		
		//delayed explosion
		if (_delayedExplosionStarted)
		{
			_explodeDelay = (_explodeDelay - Time.deltaTime);
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}
	
	private float SizeFunction (float elapsedTime) 
	{
		float divided = 1 - (1 / (1 + elapsedTime * speed));
		return initFraction + (1 - initFraction) * divided;
	}
	
    public void Reset()
    {
		size = _baseSize;
		color = _baseColor;
		damping = _baseDamping;
    }

	
	private float _tmpParticleSize; //calculated particle size... particleSize * randomized size (by sizeVariation)
	private Vector3 _tmpPos; //calculated position... randomized inside sphere of incoming radius * size
	private Vector3 _tmpDir; //calculated velocity - randomized inside sphere - incoming velocity * size
	private Vector3 _thisPos; //handle on this gameobject's position, set inside
	private float _tmpDuration; //calculated duration... incoming duration * incoming timescale
	private float _tmpCount; //calculated count... incoming count * incoming detail
	private float _scaledDuration; //calculated duration... duration * timescale
	private float _scaledDurationVariation; 
	private float _scaledStartRadius; 
	private float _scaledColor; //color with alpha adjusted according to detail and duration
	private float _randomizedRotation;
	private float _tmpAngularVelocity; //random angular velocity from -angularVelocity to +angularVelocity, if randomRotation is true;
	
    override public void Explode()
    {
		if (on)
		{
            _particleSystem.Stop();
            _particleMain = _particleSystem.main;
            _particleMain.startLifetime = duration;
            // -Z
            //This is needed but can't work while playing (even though I Stop() it beforehand????) and also it sometimes works anyway
            //Update: It has to do with the second set of Fireballs created. Perhaps since there was already a first set, the second set 
            //is overwritting the component values and sending the error. Wouldn't explain why the error only happens sometimes though...
            print("Starting " + gameObject.name + " for a duration of " + duration);
            _particleMain.duration = duration;
            print("Ending " + gameObject.name + " with a duration of " + _particleMain.duration);

            _particleMain.loop = false;
            _particleMain.simulationSpace = ParticleSystemSimulationSpace.World;
			//_particleEmitter.useWorldSpace = useWorldSpace;
			
			_scaledDuration = timeScale * duration;
			_scaledDurationVariation = timeScale * durationVariation;
			_scaledStartRadius = size * startRadius;

            //I don't know if this is still needed (and if so what is it needed for) -Z
			//_particleRenderer.particleRenderMode = renderMode;
			
            //Is this accessed by anything? -Z
			if (!_delayedExplosionStarted)
			{
				_explodeDelay = explodeDelayMin + (Random.value * (explodeDelayMax - explodeDelayMin));
			}
			if (_explodeDelay <= 0) 
			{
                _colorOverLifetime = _particleSystem.colorOverLifetime;
                _colorOverLifetime.enabled = true;
                //Color[] modifiedColors = _particleAnimator.colorAnimation;
				
                //Where does the user access this? -Z
				if (useExplicitColorAnimation)
				{
                    GradientColorKey[] gradColorKey = new GradientColorKey[5];
                    GradientAlphaKey[] gradAlphaKey = new GradientAlphaKey[5];

                    float tmpTime = 0;
                    for (int i = 0; i < colorAnimation.Length; i++)
                    {
                        Color col = colorAnimation[i];

                        gradColorKey[i].color.r = col.r;
                        gradColorKey[i].color.g = col.g;
                        gradColorKey[i].color.b = col.b;
                        gradColorKey[i].time = tmpTime;

                        gradAlphaKey[i].alpha = col.a;
                        gradAlphaKey[i].time = tmpTime;

                        tmpTime += .2f;
                    }
                    //modifiedColors[0] = colorAnimation[0];
                    //modifiedColors[1] = colorAnimation[1];
                    //modifiedColors[2] = colorAnimation[2];
                    //modifiedColors[3] = colorAnimation[3];
                    //modifiedColors[4] = colorAnimation[4];

                    colorGradient.SetKeys(gradColorKey, gradAlphaKey);
                    _colorOverLifetime.color = colorGradient;
                }
                else //auto fade
				{
                    //sets the default color gradient -Z
                    colorGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
                    _colorOverLifetime.color = colorGradient;
                }
                //_particleAnimator.colorAnimation = modifiedColors;

                if (renderMode == null)
                    renderMode = gameObject.GetComponent<ParticleSystemRenderer>();
                renderMode.material = material;
                //_particleRenderer.material = material;

                _forceOverLifetime = _particleSystem.forceOverLifetime;
                _forceOverLifetime.enabled = true;
                _forceOverLifetime.x = force.x;
                _forceOverLifetime.y = force.y;
                _forceOverLifetime.z = force.z; //Is there a better way to do this? -Z
                //_particleAnimator.force = force;
				_tmpCount = count * detail;
				if (_tmpCount < 1) _tmpCount = 1;
				
				if (_particleMain.simulationSpace == ParticleSystemSimulationSpace.World)
				{
					_thisPos = this.gameObject.transform.position;
				}
				else
				{
					_thisPos = new Vector3(0,0,0);
				}

				for (int i = 1; i <= _tmpCount; i++)
				{
					_tmpPos =  Vector3.Scale(Random.insideUnitSphere, new Vector3(_scaledStartRadius, _scaledStartRadius, _scaledStartRadius)); 
					_tmpPos = _thisPos + _tmpPos;
									
					_tmpDir = Vector3.Scale(Random.insideUnitSphere, new Vector3(velocity.x, velocity.y, velocity.z)); 
					_tmpDir.y = (_tmpDir.y + (2 * (Mathf.Abs(_tmpDir.y) * upwardsBias)));
					
					if (randomRotation == true)
					{
						_randomizedRotation = Random.Range(-1f,1f);
						_tmpAngularVelocity = Random.Range(-1f,1f) * angularVelocity;
						
					}
					else
					{
						_randomizedRotation = 0f;
						_tmpAngularVelocity = angularVelocity;
					}
					
					_tmpDir = Vector3.Scale(_tmpDir, new Vector3(size, size, size));
					
					 _tmpParticleSize = size * (particleSize + (Random.value * sizeVariation));
					
					//_tmpDuration = _scaledDuration + (Random.value * _scaledDurationVariation);
                    _emitParams = new ParticleSystem.EmitParams();
                    _emitParams.position = _tmpPos;
                    _emitParams.velocity = _tmpDir;
                    _emitParams.startSize = _tmpParticleSize;
                    _emitParams.startColor = color;
                    _emitParams.rotation = _randomizedRotation;
                    _emitParams.angularVelocity = _tmpAngularVelocity;

                    _particleSystem.Emit(_emitParams, 1);
                    
					//_particleEmitter.Emit(_tmpPos, _tmpDir, _tmpParticleSize, _tmpDuration, color, _randomizedRotation, _tmpAngularVelocity);
				}
					
				_emitTime = Time.time;
				_delayedExplosionStarted = false;
				_explodeDelay = 0f;
			}
			else
			{
				//tell update to start reducing the start delay and call explode again when it's zero
				_delayedExplosionStarted = true;
			}

            //To allow for edits -Z
            _particleSystem.Play();
        }
    }

}

namespace Softsplit;

/// <summary>
/// Basic tool functionality
/// </summary>
public abstract class ToolComponent : InputWeaponComponent
{
	public Beam beam {get;set;}
	public bool AlwaysEnabledMenu { get; set; }
	public string ToolName { get; set; } = "";
	public string ToolDes { get; set; } = "";

	public Ray WeaponRay => Equipment.Owner.AimRay;

	private ToolGunHandler toolGunHandler;



	protected override void OnStart()
	{
		InputActions.Add( "Attack2" );
		InputActions.Add( "ToolGunMenu" );

		beam = Components.Get<Beam>();
		

		

		Start();
	}

	[Sync, Property] public float RayActive { get; set; }
	[Property] public float RayTime { get; set; } = 0.2f;

	protected override void OnUpdate()
	{
		Update();

		RayActive -= Time.Delta;

		beam.enabled = RayActive > 0;

		if(Equipment.Owner.CharacterController.Velocity.Length < 1)
		{
			beam.Base = Effector.Muzzle.Transform.Position;
		}
	}


	protected override void OnInputUpdate()
	{
		if ( !toolGunHandler.IsValid() )
			toolGunHandler = Components.Get<ToolGunHandler>();

		if ( Input.Pressed( "Attack1" ) )
			PrimaryAction();

		if ( Input.Pressed( "Attack2" ) )
			SecondaryAction();

		if ( AlwaysEnabledMenu )
			toolGunHandler.ActiveToolMenu.Enabled = Equipment.IsDeployed;
		else if ( Input.Pressed( "ToolGunMenu" ) )
			toolGunHandler.ActiveToolMenu.Enabled = !toolGunHandler.ActiveToolMenu.Enabled;
	}

	protected virtual void Start()
	{

	}

	protected virtual void Update()
	{
		
	}

	protected virtual void PrimaryAction()
	{

	}

	protected virtual void SecondaryAction()
	{

	}


	protected IEquipment Effector
	{
		get
		{
			if ( IsProxy || !Equipment.ViewModel.IsValid() )
				return Equipment;

			return Equipment.ViewModel;
		}
	}

	private GameObject p1;
	private GameObject p2;

	protected SceneTraceResult Trace()
	{
		return Scene.Trace.Ray( WeaponRay.Position, WeaponRay.Position + WeaponRay.Forward * 500 )
		.UseHitboxes()
		.IgnoreGameObjectHierarchy( GameObject.Root )
		.WithoutTags( "trigger", "invis", "ragdoll", "movement", "player_clip", "player" )
		.Run();
	}

	[Broadcast]
	public void Recoil( Vector3 effectPoint )
	{
		if ( p1.IsValid() )
		{
			p1.Destroy();
			p2.Destroy();
		}

		RayActive = RayTime;
		beam.CreateEffect(Effector.Muzzle.Transform.Position, effectPoint, Effector.Muzzle.Transform.Position);

		Sound.Play( "sounds/guns/gun_dryfire.sound", Transform.Position );

		if ( Equipment.Owner.IsValid() && Equipment.Owner.BodyRenderer.IsValid() )
			Equipment.Owner.BodyRenderer.Set( "b_attack", true );

		if ( Equipment.ViewModel.IsValid() )
			Equipment.ViewModel.ModelRenderer.Set( "b_attack", true );
	}
	
}

using Sandbox;


[Library( "dm_pistol", Title = "Shit" )]
[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
partial class ShitterPistol : BaseDmWeapon
{ 
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public override AmmoType AmmoType => AmmoType.Shit;
	public override int ClipSize => 69;
	public override float ReloadTime => 3.0f;

	public override int Bucket => 1;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
		AmmoClip = 100;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = -0.5f;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}


		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "shoot" );

		//
		// Shoot the bullets
		//
		//ShootBullet( 0.05f, 1.5f, 9.0f, 3.0f );
		if (IsClient) return;
		ShootShit();

	}

	public override void AttackSecondary()
    {
		TimeSinceSecondaryAttack = -5f;

		if (!TakeAmmo (5))
        {
			DryFire();
			return;
        }

		ShootEffects();
		PlaySound ( "shoot_big");

		if (IsClient) return;
		ShootShitBig();
    }
	
	void ShootShit()
	{
		var ent = new Prop
		{
			Position = Owner.EyePos + Owner.EyeRot.Forward * 40,
			Rotation = Owner.EyeRot
		};

		ent.SetModel("models/poopemoji/poopemoji.vmdl");
		ent.Velocity = Owner.EyeRot.Forward * 10000;
	}

		void ShootShitBig()
	{
		var ent = new Prop
		{
			Position = Owner.EyePos + Owner.EyeRot.Forward * 70,
			Rotation = Owner.EyeRot
		};

		ent.SetModel("models/poopemoji/poopemoji.vmdl");
		ent.Scale = 3;
		ent.Velocity = Owner.EyeRot.Forward * 10000;
	}
}

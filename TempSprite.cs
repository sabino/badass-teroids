using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Badass_Teroids
{
	class TempSprite : Sprite
	{
		public TempSprite(Texture2D texture) : base(texture)
		{
			this.Texture = texture;

			Rotation = 0;
			Velocity = Vector2.Zero;
			Position = new Vector2(BadassGame.Instance.ScreenWidth / 2, BadassGame.Instance.ScreenHeight / 2);
			this.Create();
		}


		public override void Draw(SpriteBatch spriteBatch)
		{
			//if (!IsDead)
			//	base.Draw(spriteBatch);
		}

		public override void Update()
		{

		}

	}
}

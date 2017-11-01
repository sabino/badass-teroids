using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badass_Teroids
{
	static class Elements
	{
		public static Texture2D Ship { get; private set; }
		public static Texture2D Bullet { get; private set; }
		public static Texture2D Turret { get; private set; }
		public static Texture2D Destroyer { get; private set; }
		public static Texture2D Walker { get; private set; }
		public static Texture2D Portal { get; private set; }

		public static SpriteFont Font { get; private set; }

		public static List<Texture2D> asteroidTextures { get; private set; }

		public static void Load(ContentManager content)
		{
			Font = content.Load<SpriteFont>("myFont");

			Ship = content.Load<Texture2D>("Sprites/ship");
			Bullet = content.Load<Texture2D>("Sprites/bullet");

			for (int i = 1; i < 4; i++)
				asteroidTextures.Add(content.Load<Texture2D>("Sprites/large" + i.ToString()));

			for (int i = 1; i < 4; i++)
				asteroidTextures.Add(content.Load<Texture2D>("Sprites/medium" + i.ToString()));

			for (int i = 1; i < 4; i++)
				asteroidTextures.Add(content.Load<Texture2D>("Sprites/small" + i.ToString()));
		}
	}
}
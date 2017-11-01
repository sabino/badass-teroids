﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Badass_Teroids
{
	abstract class Sprite
	{
		Texture2D texture;

		Vector2 position;
		Vector2 center;
		Vector2 velocity;

		float rotation;
		float scale;

		bool alive;

		int index;

		Color color = Color.White;

		public Sprite(Texture2D texture)
		{
			this.texture = texture;

			position = Vector2.Zero;
			center = new Vector2(texture.Width / 2, texture.Height / 2);
			velocity = Vector2.Zero;

			Rotation = 0.0f;
			Scale = 1.0f;

			alive = false;

			index = 0;
		}

		public Sprite()
		{

		}

		public Texture2D Texture
		{
			get { return texture; }
			set { texture = value; }
		}

		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public Vector2 Center
		{
			get { return center; }
		}

		public Vector2 Velocity
		{
			get { return velocity; }
			set { velocity = value; }
		}

		public float Rotation
		{
			get { return rotation; }
			set
			{
				rotation = value;
				if (rotation < -MathHelper.TwoPi)
					rotation = MathHelper.TwoPi;
				if (rotation > MathHelper.TwoPi)
					rotation = -MathHelper.TwoPi;
			}
		}

		public float Scale
		{
			get { return scale; }
			set { scale = value; }
		}

		public bool Alive
		{
			get { return alive; }
		}

		public int Index
		{
			get { return index; }
			set { index = value; }
		}

		public int Width
		{
			get { return texture.Width; }
		}

		public int Height
		{
			get { return texture.Height; }
		}

		public void Create()
		{
			alive = true;
		}

		public void Kill()
		{
			alive = false;
		}

		public abstract void Update();

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(this.Texture, this.Position, null, Color.White, this.Rotation, this.Center, this.Scale, SpriteEffects.None, 1.0f);
		}
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Badass_Teroids
{
	public class BadassGame : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Ship ship;

		public int ScreenWidth, ScreenHeight;
		public static BadassGame Instance { get; private set; }
		int level = 0;
		int score = 0;
		int lives;

		KeyboardState oldState;

		Sprite bullet;
		List<Sprite> bullets = new List<Sprite>();

		List<Texture2D> asteroidTextures = new List<Texture2D>();
		List<Sprite> asteroids = new List<Sprite>();

		SpriteFont font;

		float distance = 0.0f;

		Random random = new Random();

		public bool GameOver = true;

		public BadassGame()
		{
			Instance = this;
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.PreferredBackBufferHeight = 800;
			graphics.PreferredBackBufferWidth = 800;
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetupGame();
		}


		private void SetupGame()
		{
			ScreenHeight = graphics.GraphicsDevice.Viewport.Height;
			ScreenWidth = graphics.GraphicsDevice.Viewport.Width;
			SetupShip();
		}

		private void SetupShip()
		{
			ship.Rotation = 0;
			ship.Velocity = Vector2.Zero;
			bullets.Clear();
			ship.Position = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
			ship.Create();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			ship = new Ship(Content.Load<Texture2D>("Sprites/ship"));
			bullet = new Bullet(Content.Load<Texture2D>("Sprites/bullet"));

			for (int i = 1; i < 4; i++)
				asteroidTextures.Add(Content.Load<Texture2D>("Sprites/large" + i.ToString()));

			for (int i = 1; i < 4; i++)
				asteroidTextures.Add(Content.Load<Texture2D>("Sprites/medium" + i.ToString()));

			for (int i = 1; i < 4; i++)
				asteroidTextures.Add(Content.Load<Texture2D>("Sprites/small" + i.ToString()));

			font = Content.Load<SpriteFont>("font");

		}

		protected override void UnloadContent()
		{
			// Unload non ContentManager content
		}

		protected override void Update(GameTime gameTime)
		{
			KeyboardState newState = Keyboard.GetState();

			if (newState.IsKeyDown(Keys.Escape))
				this.Exit();

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			if (GameOver)
			{
				asteroids.Clear();
				ship.Kill();

				if (newState.IsKeyDown(Keys.Enter))
				{
					level = 0;
					score = 0;
					lives = 3;
					SetupGame();
					CreateAsteroids();
					GameOver = false;
				}
				else
					return;
			}

			if (newState.IsKeyDown(Keys.Left))
				ship.Rotation -= 0.05f;

			if (newState.IsKeyDown(Keys.Right))
				ship.Rotation += 0.05f;

			if (newState.IsKeyUp(Keys.Space) && oldState.IsKeyDown(Keys.Space))
				FireBullet();

			if (newState.IsKeyDown(Keys.Up))
				AccelerateShip();
			else if (newState.IsKeyUp(Keys.Up))
				DecelerateShip();

			if (newState.IsKeyUp(Keys.LeftControl) && oldState.IsKeyDown(Keys.LeftControl))
				HyperSpace();

			if (newState.IsKeyUp(Keys.RightControl) && oldState.IsKeyDown(Keys.RightControl))
				HyperSpace();

			oldState = newState;

			UpdateShip();
			UpdateAsteroids();
			UpdateBullets();
			AllDead();

			base.Update(gameTime);
		}

		private void AllDead()
		{
			bool allDead = true;

			foreach (Sprite s in asteroids)
			{
				if (s.Alive)
					allDead = false;
			}

			if (allDead)
			{
				SetupGame();
				level++;
				asteroids.Clear();
				CreateAsteroids();
			}

		}

		private void HyperSpace()
		{
			int positionX;
			int positionY;

			positionX = random.Next(ship.Width, ScreenWidth - ship.Width);
			positionY = random.Next(ship.Height, ScreenHeight - ship.Height);

			ship.Position = new Vector2(positionX, positionY);

			ship.Velocity = Vector2.Zero;
		}

		private void AccelerateShip()
		{
			ship.Velocity += new Vector2(
				(float)(Math.Cos(ship.Rotation - MathHelper.PiOver2) * 0.05f),
				(float)((Math.Sin(ship.Rotation - MathHelper.PiOver2) * 0.05f)));

			if (ship.Velocity.X > 5.0f)
				ship.Velocity = new Vector2(5.0f, ship.Velocity.Y);
			if (ship.Velocity.X < -5.0f)
				ship.Velocity = new Vector2(-5.0f, ship.Velocity.Y);
			if (ship.Velocity.Y > 5.0f)
				ship.Velocity = new Vector2(ship.Velocity.X, 5.0f);
			if (ship.Velocity.Y < -5.0f)
				ship.Velocity = new Vector2(ship.Velocity.X, -5.0f);
		}

		private void DecelerateShip()
		{
			if (ship.Velocity.X < 0)
				ship.Velocity = new Vector2(ship.Velocity.X + 0.02f, ship.Velocity.Y);
			if (ship.Velocity.X > 0)
				ship.Velocity = new Vector2(ship.Velocity.X - 0.02f, ship.Velocity.Y);
			if (ship.Velocity.Y < 0)
				ship.Velocity = new Vector2(ship.Velocity.X, ship.Velocity.Y + 0.02f);
			if (ship.Velocity.Y > 0)
				ship.Velocity = new Vector2(ship.Velocity.X, ship.Velocity.Y - 0.02f);
		}

		public void UpdateShip()
		{
			ship.Position += ship.Velocity;

			if (ship.Position.X + ship.Width < 0)
				ship.Position = new Vector2(ScreenWidth, ship.Position.Y);
			if (ship.Position.X - ship.Width > ScreenWidth)
				ship.Position = new Vector2(0, ship.Position.Y);
			if (ship.Position.Y + ship.Height < 0)
				ship.Position = new Vector2(ship.Position.X, ScreenHeight);
			if (ship.Position.Y - ship.Height > ScreenHeight)
				ship.Position = new Vector2(ship.Position.X, 0);
		}

		private void CreateAsteroids()
		{
			int value;

			for (int i = 0; i < 4 + level; i++)
			{
				int index = random.Next(0, 3);

				Sprite tempSprite = new TempSprite(asteroidTextures[index]);
				asteroids.Add(tempSprite);
				asteroids[i].Index = index;

				double xPos = 0;
				double yPos = 0;

				value = random.Next(0, 8);

				switch (value)
				{
					case 0:
					case 1:
						xPos = asteroids[i].Width + random.NextDouble() * 40;
						yPos = random.NextDouble() * ScreenHeight;
						break;
					case 2:
					case 3:
						xPos = ScreenWidth - random.NextDouble() * 40;
						yPos = random.NextDouble() * ScreenHeight;
						break;
					case 4:
					case 5:
						xPos = random.NextDouble() * ScreenWidth;
						yPos = asteroids[i].Height + random.NextDouble() * 40;
						break;
					default:
						xPos = random.NextDouble() * ScreenWidth;
						yPos = ScreenHeight - random.NextDouble() * 40;
						break;
				}

				asteroids[i].Position = new Vector2((float)xPos, (float)yPos);
				asteroids[i].Velocity = RandomVelocity();
				asteroids[i].Rotation = (float)random.NextDouble() * MathHelper.Pi * 4 - MathHelper.Pi * 2;
				asteroids[i].Create();
			}
		}

		private void UpdateAsteroids()
		{
			foreach (Sprite a in asteroids)
			{
				a.Position += a.Velocity;

				if (a.Position.X + a.Width < 0)
					a.Position = new Vector2(ScreenWidth, a.Position.Y);
				if (a.Position.Y + a.Height < 0)
					a.Position = new Vector2(a.Position.X, ScreenHeight);
				if (a.Position.X - a.Width > ScreenWidth)
					a.Position = new Vector2(0, a.Position.Y);
				if (a.Position.Y - a.Height > ScreenHeight)
					a.Position = new Vector2(a.Position.X, 0);

				if (a.Alive && CheckShipCollision(a))
				{
					a.Kill();
					lives--;
					SetupShip();
					if (lives < 1)
						GameOver = true;
				}
			}

		}

		private bool CheckShipCollision(Sprite asteroid)
		{
			Vector2 position1 = asteroid.Position;
			Vector2 position2 = ship.Position;

			float Catetos1 = Math.Abs(position1.X - position2.X);
			float Catetos2 = Math.Abs(position1.Y - position2.Y);

			Catetos1 *= Catetos1;
			Catetos2 *= Catetos2;

			distance = (float)Math.Sqrt(Catetos1 + Catetos2);

			if ((int)distance < ship.Width)
				return true;

			return false;
		}

		private bool CheckAsteroidCollision(Sprite asteroid, Sprite bullet)
		{
			Vector2 position1 = asteroid.Position;
			Vector2 position2 = bullet.Position;

			float Catetos1 = Math.Abs(position1.X - position2.X);
			float Catetos2 = Math.Abs(position1.Y - position2.Y);

			Catetos1 *= Catetos1;
			Catetos2 *= Catetos2;

			distance = (float)Math.Sqrt(Catetos1 + Catetos2);

			if ((int)distance < asteroid.Width)
				return true;

			return false;
		}

		private void UpdateBullets()
		{
			List<Sprite> destroyed = new List<Sprite>();

			foreach (Sprite b in bullets)
			{
				b.Position += b.Velocity;
				foreach (Sprite a in asteroids)
				{
					if (a.Alive && CheckAsteroidCollision(a, b))
					{
						if (a.Index < 3)
							score += 25;
						else if (a.Index < 6)
							score += 50;
						else
							score += 100;

						a.Kill();
						destroyed.Add(a);
						b.Kill();
					}
				}
				if (b.Position.X < 0)
					b.Kill();
				else if (b.Position.Y < 0)
					b.Kill();
				else if (b.Position.X > ScreenWidth)
					b.Kill();
				else if (b.Position.Y > ScreenHeight)
					b.Kill();
			}

			for (int i = 0; i < bullets.Count; i++)
			{
				if (!bullets[i].Alive)
				{
					bullets.RemoveAt(i);
					i--;
				}
			}

			foreach (Sprite a in destroyed)
				SplitAsteroid(a);
		}

		private void SplitAsteroid(Sprite a)
		{
			if (a.Index < 3)
			{
				for (int i = 0; i < 2; i++)
				{
					int index = random.Next(3, 6);
					NewAsteroid(a, index);
				}
			}
			else if (a.Index < 6)
			{
				for (int i = 0; i < 2; i++)
				{
					int index = random.Next(6, 9);
					NewAsteroid(a, index);
				}
			}
		}

		private void NewAsteroid(Sprite a, int index)
		{
			Sprite tempSprite = new TempSprite(asteroidTextures[index]);

			tempSprite.Index = index;
			tempSprite.Position = a.Position;
			tempSprite.Velocity = RandomVelocity();

			tempSprite.Rotation = (float)random.NextDouble() *
				MathHelper.Pi * 4 - MathHelper.Pi * 2;

			tempSprite.Create();
			asteroids.Add(tempSprite);
		}

		private Vector2 RandomVelocity()
		{
			float xVelocity = (float)(random.NextDouble() * 2 + .5);
			float yVelocity = (float)(random.NextDouble() * 2 + .5);

			if (random.Next(2) == 1)
				xVelocity *= -1.0f;

			if (random.Next(2) == 1)
				yVelocity *= -1.0f;

			return new Vector2(xVelocity, yVelocity);
		}

		private void FireBullet()
		{
			Sprite newBullet = new Bullet(bullet.Texture);

			Vector2 velocity = new Vector2((float)Math.Cos(ship.Rotation - (float)MathHelper.PiOver2), (float)Math.Sin(ship.Rotation - (float)MathHelper.PiOver2));

			velocity.Normalize();
			velocity *= 6.0f;

			newBullet.Velocity = velocity;

			newBullet.Position = ship.Position + newBullet.Velocity;
			newBullet.Create();

			bullets.Add(newBullet);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			if (GameOver)
			{
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, null);

				Vector2 position2 = new Vector2(0.0f, 20.0f);

				string text = "BADASS_TEROIDS";

				Vector2 size = font.MeasureString(text);

				position2 = new Vector2((ScreenWidth / 2) - (size.X / 2), (ScreenHeight / 2) - (size.Y * 2));

				spriteBatch.DrawString(font, text, position2, Color.White);

				text = "PRESS <ENTER> TO START";
				size = font.MeasureString(text);

				position2 = new Vector2((ScreenWidth / 2) - (size.X / 2), (ScreenHeight / 2) + (size.Y * 2));

				spriteBatch.DrawString(font, text, position2, Color.White);

				spriteBatch.End();

				return;
			}

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, null);

			Vector2 position = new Vector2(10, 10);
			spriteBatch.DrawString(font, "Score = " + score.ToString(), position, Color.White);

			Rectangle shipRect;

			for (int i = 0; i < lives; i++)
			{
				shipRect = new Rectangle(i * ship.Width + 10, 40, ship.Width, ship.Height);
				spriteBatch.Draw(ship.Texture, shipRect, Color.White);
			}

			spriteBatch.Draw(ship.Texture, ship.Position, null, Color.White, ship.Rotation, ship.Center, ship.Scale, SpriteEffects.None, 1.0f);
			foreach (Sprite b in bullets)
				if (b.Alive)
					spriteBatch.Draw(b.Texture, b.Position, null, Color.White, b.Rotation, b.Center, b.Scale, SpriteEffects.None, 1.0f);

			foreach (Sprite a in asteroids)
				if (a.Alive)
					spriteBatch.Draw(a.Texture, a.Position, null, Color.White, a.Rotation, a.Center, a.Scale, SpriteEffects.None, 1.0f);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}

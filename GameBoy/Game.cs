using System;

namespace GameBoy
{
	[Flags]
	public enum Vector
	{
		None = 0b000,
		Up = 0b1000,
		Down = 0b0010,
		Left = 0b0001,
		Right = 0b0100
	}

	[Flags]
	public enum GameKey
	{
		None = 0x0,
		Up = 0x1,
		Down = 0x2,
		Left = 0x4,
		Right = 0x8,
		ButtonA = 0xf,
		ButtonB = 0x10
	}

	public class GameClick
	{
		public int X { get; set; }
		public int Y { get; set; }
	}

	public class GameBlock
	{
		public int X { get; set; }
		public int Y { get; set; }
		public Vector Vector { get; set; }
		public GameColor Color { get; set; }

		public bool Equals( GameBlock block ) => X == block.X && Y == block.Y;

		public GameBlock Copy() => new GameBlock() { X = X, Y = Y, Vector = Vector, Color = Color };

		public void Move()
		{
			switch( Vector )
			{
				case Vector.Up:				     Y--;      break;
				case Vector.Up | Vector.Right:   Y--; X++; break;
				case Vector.Right:			     X++;      break;
				case Vector.Right | Vector.Down: Y++; X++; break;
				case Vector.Down:                Y++;      break;
				case Vector.Down | Vector.Left:  Y++; X--; break;
				case Vector.Left:                X--;      break;
				case Vector.Left | Vector.Up:    Y--; X--; break;
			}
		}
	}

	public class Controller
	{
		public GameKey GameKey { get; set; }
		public GameClick GameClick { get; set; }
	}

	public enum StatusGame
	{
		Preview,
		Play,
		Over
	}

	public class Game
	{
		public Field Field { get; set; }
		public delegate void EmptyMethod();
		public delegate void UpdateMethod( Controller controller );
		public event EmptyMethod OnPreview;
		public event EmptyMethod OnNewGame;
		public event UpdateMethod OnUpdatePreview;
		public event UpdateMethod OnUpdateGame;
		public event UpdateMethod OnUpdateOver;
		public event EmptyMethod OnGameOver;
		public StatusGame StatusGame { get; private set; }
		protected int score;
		protected Random random;

		public Game()
		{
			Field = new Field();
			random = new Random();
		}

		public void PreviewGame()
		{
			StatusGame = StatusGame.Preview;
			OnPreview();
		}

		public void NewGame()
		{
			StatusGame = StatusGame.Play;
			score = 0;
			OnNewGame();
		}

		public void Update( Controller controller )
		{
			switch( StatusGame )
			{
				case StatusGame.Play: OnUpdateGame( controller ); break;
				case StatusGame.Preview: OnUpdatePreview?.Invoke( controller ); break;
				case StatusGame.Over: OnUpdateOver?.Invoke( controller ); break;
			}
		}

		public void GameOver()
		{
			StatusGame = StatusGame.Over;
			OnGameOver();
		}

		public void DrawScore() => Field.DrawScore( score );

		public void WriteScore() => Field.InfoPanel = string.Format( "Score: {0}", score );

		public void BasePreview()
		{
			Field.InfoPanel = GetType().Name;
			Field.Clear( GameColor.Black );
		}
	}
}

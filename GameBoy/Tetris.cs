using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBoy
{
	class Tetris : Game
	{
		class TetrisFigure : Figure
		{
			private static List<TetrisFigure> stamps;

			public TetrisFigure()
			{
				if( stamps == null )
					InitStamps();
				Random random = new Random();
				body = stamps[random.Next( stamps.Count )].CopyBody();
				FillColor( ( GameColor ) random.Next( 7 ) );
				SetPosition( 14, 2 );
			}

			public TetrisFigure( List<GameBlock> body ) : base( body ) {}
			public TetrisFigure( Figure figure ) : base( figure ) {}
			public TetrisFigure( int[][] intBody, Vector vector = Vector.None, GameColor color = GameColor.Black ) : base( intBody, vector, color ) {}

			public void RotationRight(TetrisFigure passive)
			{
				TetrisFigure temp = new TetrisFigure( this );
				temp.RotationRight();
				if( !passive.Cross( temp ) && temp.X >= 0 && temp.body.Max( x => x.X ) < 10 && temp.body.Max( x => x.Y ) < 20 )
					RotationRight();
			}

			private void RotationRight()
			{
				int left = X;
				int top = Y;
				SetPosition( 0, 0 );
				foreach( var block in body )
				{
					int x = block.X, y = block.Y;
					block.X = -y;
					block.Y = x;
				}
				SetPosition( left, top );
			}

			public void TetrisMove(int dx, int dy, Figure passive)
			{
				TetrisFigure temp = new TetrisFigure( this );
				temp.Move( dx, dy );
				if( !passive.Cross( temp ) && !temp.body.Exists( x => x.X == -1 ) && !temp.body.Exists( x => x.X == 10 ) )
					Move( dx, dy );
			}
			
			private static void InitStamps()
			{
				stamps = new List<TetrisFigure>();
				stamps.Add( new TetrisFigure( new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, 2 }, new int[] { 0, 3 } }, Vector.Down ) );
				stamps.Add( new TetrisFigure( new int[][] { new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 2, 1 }, new int[] { 1, 0 } }, Vector.Down ) );
				stamps.Add( new TetrisFigure( new int[][] { new int[] { 0, 0 }, new int[] { 1, 1 }, new int[] { 0, 1 }, new int[] { 1, 0 } }, Vector.Down ) );
				stamps.Add( new TetrisFigure( new int[][] { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 0, 1 }, new int[] { 0, 2 } }, Vector.Down ) );
				stamps.Add( new TetrisFigure( new int[][] { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 1, 2 } }, Vector.Down ) );
				stamps.Add( new TetrisFigure( new int[][] { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 2, 1 } }, Vector.Down ) );
				stamps.Add( new TetrisFigure( new int[][] { new int[] { 2, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 0, 1 } }, Vector.Down ) );
			}
		}

		private TetrisFigure preview;
		private TetrisFigure active;
		private TetrisFigure passive;
		private TetrisFigure temp;
		private bool iteration;

		public Tetris() : base()
		{
			OnPreview += BasePreview;
			OnNewGame += Tetris_OnNewGame;
			OnUpdateGame += Tetris_OnUpdateGame;
			OnGameOver += DrawScore;
		}

		private void Tetris_OnNewGame()
		{
			iteration = false;
			passive = new TetrisFigure( new List<GameBlock>() );
			preview = new TetrisFigure();
			CreateFigure();
			DrawField();
		}

		private void Tetris_OnUpdateGame( Controller controller )
		{
			switch( controller.GameKey )
			{
				case GameKey.Up:
					active.RotationRight( passive );
					break;
				case GameKey.Down:
					Iteration();
					Iteration();
					break;
				case GameKey.Left:
					active.TetrisMove( -1, 0, passive );
					break;
				case GameKey.Right:
					active.TetrisMove( 1, 0, passive );
					break;
			}
			if (iteration = !iteration)
				Iteration();
			DrawField();
			if( passive.Cross( active ) )
				GameOver();
		}

		private void Iteration()
		{
			temp = new TetrisFigure( active );
			temp.Move();
			if( passive.Cross( temp ) || temp.body.Exists( x => x.Y == 20 ) )
			{
				passive.Join( active );
				CheckField();
				CreateFigure();
			}
			else
				active.Move();
		}

		private void CheckField()
		{
			int y = 19;
			while( y >= passive.body.Min( x => x.Y ) )
				if( passive.body.Count( x => x.Y == y ) != 10 )
					y--;
				else
				{
					passive.body.RemoveAll( x => x.Y == y );
					foreach( var block in passive.body.FindAll( x => x.Y < y ) )
						block.Y++;
					score++;
				}
		}

		private void CreateFigure()
		{
			active = preview;
			active.SetPosition( 4, 0 );
			preview = new TetrisFigure();
		}

		private void DrawField()
		{
			Field.Clear( GameColor.White );
			Field.DrawLine( GameColor.Green, 10, 0, 10, 19 );
			Field.DrawGameBlocks( passive.body );
			Field.DrawGameBlocks( active.body );
			Field.DrawGameBlocks( preview.body );
			WriteScore();
		}
	}
}

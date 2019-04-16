using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBoy
{
	class Snake : Game
	{
		private GameBlock head;
		private List<GameBlock> body;
		private GameBlock eat;

		public Snake() : base()
		{
			OnPreview += BasePreview;
			OnNewGame += Snake_OnNewGame;
			OnUpdateGame += Snake_OnUpdateGame;
			OnGameOver += DrawScore;
		}

		private void Snake_OnNewGame()
		{
			head = new GameBlock()	{ X = 10, Y = 10, Vector = Vector.Up, Color = GameColor.Green };
			body = new List<GameBlock>();
			body.Add( head );
			body.Add( new GameBlock() { X = 10, Y = 11, Vector = Vector.Up, Color = GameColor.Black	} );
			body.Add( new GameBlock() { X = 10, Y = 12, Vector = Vector.Up, Color = GameColor.Black	} );
			CreateEat();
			DrawField();
		}

		private void Snake_OnUpdateGame( Controller controller )
		{
			ControlMove( controller.GameKey );
			if( CheckGameOver() )
				GameOver();
			else
				SnakeMove();
		}

		private void ControlMove( GameKey key )
		{
			switch( key )
			{
				case GameKey.Left:  head.Vector  = head.Vector == Vector.Right ? Vector.Right : Vector.Left;  break;
				case GameKey.Right: head.Vector  = head.Vector == Vector.Left  ? Vector.Left  : Vector.Right; break;
				case GameKey.Up:    head.Vector  = head.Vector == Vector.Down  ? Vector.Down  : Vector.Up;    break;
				case GameKey.Down:  head.Vector  = head.Vector == Vector.Up    ? Vector.Up    : Vector.Down;  break;
				default: break;
			}
		}

		private bool CheckGameOver()
		{
			switch( head.Vector )
			{
				case Vector.Up: return !CheckEmptyBlock( head.X, head.Y - 1 );
				case Vector.Down: return !CheckEmptyBlock( head.X, head.Y + 1 );
				case Vector.Left: return !CheckEmptyBlock( head.X - 1, head.Y ); 
				case Vector.Right: return !CheckEmptyBlock( head.X + 1, head.Y );
				default: throw new NotImplementedException();
			}
		}

		private void SnakeMove()
		{
			var temp = body.Last().Copy();
			foreach( var block in body )
				block.Move();
			for( int i = body.Count - 1; i > 0; i-- )
				body[i].Vector = body[i - 1].Vector;
			if( head.Equals( eat ) )
			{
				score++;
				body.Add( temp );
				CreateEat();
			}
			DrawField();
		}

		private bool CheckEmptyBlock(int x, int y) => !( x < 0 || y < 0 || x == MainForm.FIELD_SIZE || y == MainForm.FIELD_SIZE ) && !body.Exists( a => a.Equals( new GameBlock() { X = x, Y = y } ) );

		private void CreateEat()
		{
			var emptyBlocks = new List<GameBlock>();
			for( int i = 0; i < MainForm.FIELD_SIZE; i++ )
				for( int j = 0; j < MainForm.FIELD_SIZE; j++ )
					if( CheckEmptyBlock( i, j ) )
						emptyBlocks.Add(new GameBlock() { X = i, Y = j, Color = GameColor.Red } );
			if (emptyBlocks.Count > 0)
				eat = emptyBlocks[random.Next( emptyBlocks.Count )];
		}

		private void DrawField()
		{
			Field.Clear( GameColor.White );
			Field.DrawGameBlock( eat );
			Field.DrawGameBlocks( body );
			WriteScore();
		}
	}
}

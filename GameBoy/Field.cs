using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBoy
{
	[Flags]
	public enum GameColor
	{
		Black = 0b000,
		White = 0b111,
		Red = 0b100,
		Green = 0b010,
		Blue = 0b001,
		Yellow = 0b110,
		Magenta = 0b101,
		Cyan = 0b011,
		Transparent = 0b1000
	}

	public class Figure
	{
		public int X
		{
			get { return body.Min( x => x.X ); }
			set
			{
				int dx = value - X;
				Move( dx, 0 );
			}
		}
		public int Y
		{
			get { return body.Min( x => x.Y ); }
			set
			{
				int dy = value - Y;
				Move( 0, dy );
			}
		}
		public List<GameBlock> body;

		public Figure() => body = new List<GameBlock>();
		public Figure( List<GameBlock> body ) => this.body = body;
		public Figure( Figure figure ) => body = figure.CopyBody();
		public Figure( int[][] intBody, Vector vector = Vector.None, GameColor color = GameColor.Black )
		{
			body = new List<GameBlock>();
			for( int i = 0; i < intBody.Length; i++ )
				body.Add( new GameBlock() { X = intBody[i][0], Y = intBody[i][1], Vector = vector, Color = color } );
		}

		public void Move()
		{
			foreach( var block in body )
				block.Move();
		}

		public void Move( int dx, int dy )
		{
			foreach( var block in body )
			{
				block.X += dx;
				block.Y += dy;
			}
		}

		public void FillColor(GameColor color)
		{
			foreach( var block in body )
				block.Color = color;
		}

		public List<GameBlock> CopyBody()
		{
			List<GameBlock> temp = new List<GameBlock>();
			foreach( var block in body )
				temp.Add( block.Copy() );
			return temp;
		}

		public void Join( Figure figure ) => body.AddRange( figure.body );

		public bool Cross( Figure figure )
		{
			foreach( var block in figure.body )
				if( body.Exists( x => x.Equals( block ) ) )
					return true;
			return false;
		}

		public void SetPosition(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	public class Field
	{
		public string InfoPanel;
		public GameColor[,] blocks;

		public Field()
		{
			blocks = new GameColor[MainForm.FIELD_SIZE, MainForm.FIELD_SIZE];
		}

		public void Clear( GameColor color )
		{
			for( int i = 0; i < MainForm.FIELD_SIZE; i++ )
				for( int j = 0; j < MainForm.FIELD_SIZE; j++ )
				{
					blocks[i, j] = color;
				}
		}

		public void DrawGameBlock( GameBlock block )
		{
			blocks[block.X, block.Y] = block.Color;
		}

		public void DrawGameBlocks( List<GameBlock> blocks )
		{
			foreach( var block in blocks )
				this.blocks[block.X, block.Y] = block.Color;
		}

		public void DrawScore( int score )
		{
			Clear( GameColor.Black );
			WriteString( "SCORE", GameColor.White, 1, 1 );
			WriteString( score.ToString(), GameColor.White, 1, 7 );
		}

		public void DrawLine( GameColor color, int x0, int y0, int x1, int y1 )
		{
			float dx = x1 - x0;
			float dy = y1 - y0;
			if ( Math.Abs(dx) > Math.Abs(dy))
			{
				dx /= Math.Abs( dx );
				dy /= Math.Abs( dx );
			}
			else
			{
				dx /= Math.Abs( dy );
				dy /= Math.Abs( dy );
			}
			float x = x0;
			float y = y0;
			blocks[x0, y0] = color;
			while( x != x1 || y != y1 )
			{
				x += dx;
				y += dy;
				blocks[( int ) Math.Round( x ), ( int ) Math.Round( y )] = color;
			}
		}
		
		public void WriteString( string text, GameColor color, int x, int y )
		{
			int dx = 0;
			foreach( var ch in text )
				dx += WriteChar( ch, color, x + dx, y ) + 1;
		}
		
		public int WriteChar( char ch, GameColor color, int x, int y )
		{
			switch( ch )
			{
				case '0':
					blocks[x, y] = color;
					blocks[x + 2, y] = color;
					blocks[x, y + 4] = color;
					blocks[x + 2, y + 4] = color;
					blocks[x + 1, y] = color;
					blocks[x, y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x, y + 3] = color;
					blocks[x + 2, y + 1] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x + 1, y + 4] = color;
					return 3;

				case '1':
					blocks[x, y] = color;
					blocks[x, y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x, y + 3] = color;
					blocks[x, y + 4] = color;
					return 1;

				case '2':
					blocks[x, y] = color;
					blocks[x + 1, y] = color;
					blocks[x + 2, y] = color;
					blocks[x + 2, y + 1] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x + 1, y + 2] = color;
					blocks[x, y + 2] = color;
					blocks[x, y + 3] = color;
					blocks[x, y + 4] = color;
					blocks[x + 1, y + 4] = color;
					blocks[x + 2, y + 4] = color;
					return 3;

				case '3':
					blocks[x, y] = color;
					blocks[x + 1, y] = color;
					blocks[x + 2, y] = color;
					blocks[x + 2, y + 1] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x, y + 2] = color;
					blocks[x + 1, y + 2] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x, y + 4] = color;
					blocks[x + 1, y + 4] = color;
					blocks[x + 2, y + 4] = color;
					return 3;

				case '4':
					blocks[x, y] = color;
					blocks[x, y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x + 1, y + 2] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x + 2, y + 1] = color;
					blocks[x + 2, y] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x + 2, y + 4] = color;
					return 3;

				case '5':
					blocks[x, y] = color;
					blocks[x + 1, y] = color;
					blocks[x + 2, y] = color;
					blocks[x, y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x + 1, y + 2] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x + 1, y + 4] = color;
					blocks[x, y + 4] = color;
					blocks[x + 2, y + 4] = color;
					return 3;

				case '6':
					blocks[x, y] = color;
					blocks[x + 1, y] = color;
					blocks[x + 2, y] = color;
					blocks[x , y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x, y + 3] = color;
					blocks[x, y + 4] = color;
					blocks[x + 1, y + 4] = color;
					blocks[x + 2, y + 4] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x + 1, y + 2] = color;
					return 3;

				case '7':
					blocks[x, y] = color;
					blocks[x + 1, y] = color;
					blocks[x + 2, y] = color;
					blocks[x + 2, y + 1] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x + 2, y + 4] = color;
					return 3;

				case '8':
					blocks[x, y] = color;
					blocks[x + 2, y] = color;
					blocks[x, y + 4] = color;
					blocks[x + 2, y + 4] = color;
					blocks[x + 1, y] = color;
					blocks[x, y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x, y + 3] = color;
					blocks[x + 2, y + 1] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x + 1, y + 4] = color;
					blocks[x + 1, y + 2] = color;
					return 3;

				case '9':
					blocks[x, y] = color;
					blocks[x + 2, y] = color;
					blocks[x, y + 4] = color;
					blocks[x + 2, y + 4] = color;
					blocks[x + 1, y] = color;
					blocks[x, y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x + 2, y + 1] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x + 1, y + 4] = color;
					blocks[x + 1, y + 2] = color;
					return 3;

				case 'C':
					blocks[x + 1, y] = color;
					blocks[x + 2, y] = color;
					blocks[x, y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x, y + 3] = color;
					blocks[x + 1, y + 4] = color;
					blocks[x + 2, y + 4] = color;
					return 3;

				case 'E':
					blocks[x, y] = color;
					blocks[x, y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x, y + 3] = color;
					blocks[x, y + 4] = color;
					blocks[x + 1, y] = color;
					blocks[x + 2, y] = color;
					blocks[x + 1, y + 2] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x + 1, y + 4] = color;
					blocks[x + 2, y + 4] = color;
					return 3;

				case 'O':
					blocks[x + 1, y] = color;
					blocks[x, y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x, y + 3] = color;
					blocks[x + 2, y + 1] = color;
					blocks[x + 2, y + 2] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x + 1, y + 4] = color;
					return 3;

				case 'R':
					blocks[x, y] = color;
					blocks[x, y + 1] = color;
					blocks[x, y + 2] = color;
					blocks[x, y + 3] = color;
					blocks[x, y + 4] = color;
					blocks[x + 1, y] = color;
					blocks[x + 2, y + 1] = color;
					blocks[x + 1, y + 2] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x + 2, y + 4] = color;
					return 3;

				case 'S':
					blocks[x + 1, y] = color;
					blocks[x + 2, y] = color;
					blocks[x, y + 1] = color;
					blocks[x + 1, y + 2] = color;
					blocks[x + 2, y + 3] = color;
					blocks[x, y + 4] = color;
					blocks[x + 1, y + 4] = color;
					return 3;
			}
			return 0;
		}
	}
}

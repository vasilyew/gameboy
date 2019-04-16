namespace GameBoy
{
	class Life : Game
	{
		private bool play;
		public GameColor[,] tempBlocks;

		public Life() : base()
		{
			OnPreview += BasePreview;
			OnNewGame += Life_OnNewGame;
			OnUpdateGame += Life_OnUpdateGame;
		}

		private void Life_OnNewGame()
		{
			play = false;
			Field.Clear( GameColor.White );
			tempBlocks = new GameColor[20, 20];
		}

		private void Life_OnUpdateGame( Controller controller )
		{
			if( !play )
			{
				if( controller.GameKey == GameKey.ButtonA )
					play = true;
				if( controller.GameKey == GameKey.ButtonB )
					NextStep();
				if( controller.GameClick != null )
					SetCell( controller.GameClick.X, controller.GameClick.Y );
			}
			else
			{
				if( controller.GameKey == GameKey.ButtonA )
					play = false;
				else
					NextStep();
			}
		}

		private void SetCell( int x, int y )
		{
			Field.blocks[x, y] = Field.blocks[x, y] == GameColor.Black ? GameColor.White : GameColor.Black;
		}

		private int Correct( int x )
		{
			if( x < 0 )
				return 19;
			else if( x == 20 )
				return 0;
			else
				return x;
		}

		private int CountNeighbour( int x, int y )
		{
			int sum = 0;
			if( Field.blocks[Correct( x ), Correct( y - 1 )] == GameColor.Black )
				sum++;
			if( Field.blocks[Correct( x + 1 ), Correct( y - 1 )] == GameColor.Black )
				sum++;
			if( Field.blocks[Correct( x + 1 ), Correct( y )] == GameColor.Black )
				sum++;
			if( Field.blocks[Correct( x + 1 ), Correct( y + 1 )] == GameColor.Black )
				sum++;
			if( Field.blocks[Correct( x ), Correct( y + 1 )] == GameColor.Black )
				sum++;
			if( Field.blocks[Correct( x - 1 ), Correct( y + 1 )] == GameColor.Black )
				sum++;
			if( Field.blocks[Correct( x - 1 ), Correct( y )] == GameColor.Black )
				sum++;
			if( Field.blocks[Correct( x - 1 ), Correct( y - 1 )] == GameColor.Black )
				sum++;
			return sum;
		}

		private void NextStep()
		{
			for( int i = 0; i < MainForm.FIELD_SIZE; i++ )
				for( int j = 0; j < MainForm.FIELD_SIZE; j++ )
					if( Field.blocks[i, j] == GameColor.White && CountNeighbour( i, j ) == 3 )
						tempBlocks[i, j] = GameColor.Black;
					else if( Field.blocks[i, j] == GameColor.Black && ( CountNeighbour( i, j ) < 2 || CountNeighbour( i, j ) > 3 ) )
						tempBlocks[i, j] = GameColor.White;
					else
						tempBlocks[i, j] = Field.blocks[i, j];

			for( int i = 0; i < MainForm.FIELD_SIZE; i++ )
				for( int j = 0; j < MainForm.FIELD_SIZE; j++ )
					Field.blocks[i, j] = tempBlocks[i, j];
		}
	}
}

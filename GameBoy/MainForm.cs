using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameBoy
{
	enum GameMode
	{
		SelectGame,
		PlayGame,
		EndGame
	}

	public partial class MainForm : Form
	{
		public const int FIELD_SIZE = 20;
		private int dX;
		private int dY;
		private int blockSize;
		private int heightInfoPanel;
		private BufferedGraphics bg;
		private StringFormat stringFormat;
		private Font font;
		private RectangleF rectangleInfoPanel;
		private List<Game> games;
		private Game game;
		private int selectGame;
		private Controller controller;
		private Controller controllerCash;
		private GameMode mode;

		public MainForm( List<Game> games )
		{
			this.games = games;
			controller = new Controller();
			controllerCash = new Controller();
			foreach( var g in games )
				g.OnGameOver += EndGame;
			selectGame = 0;
			game = games[selectGame];
			game.PreviewGame();
			stringFormat = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			InitializeComponent();
			InitBG();
			InitSize();
			DrawField( game.Field );
			timer.Start();
		}

		private void DrawField(Field field)
		{
			bg.Graphics.Clear( Color.Gray );
			bg.Graphics.FillRectangle( Brushes.Black, rectangleInfoPanel );
			bg.Graphics.DrawString( field.InfoPanel, font, Brushes.White, rectangleInfoPanel, stringFormat );
			for( int i = 0; i < FIELD_SIZE; i++ )
				for( int j = 0; j < FIELD_SIZE; j++ )
				{
					switch( field.blocks[i, j] )
					{
						case GameColor.Black:
							bg.Graphics.FillRectangle( Brushes.Black, dX + i * blockSize, dY + j * blockSize, blockSize, blockSize );
							break;

						case GameColor.White:
							bg.Graphics.FillRectangle( Brushes.White, dX + i * blockSize, dY + j * blockSize, blockSize, blockSize );
							break;

						case GameColor.Red:
							bg.Graphics.FillRectangle( Brushes.Red, dX + i * blockSize, dY + j * blockSize, blockSize, blockSize );
							break;

						case GameColor.Green:
							bg.Graphics.FillRectangle( Brushes.Green, dX + i * blockSize, dY + j * blockSize, blockSize, blockSize );
							break;

						case GameColor.Blue:
							bg.Graphics.FillRectangle( Brushes.Blue, dX + i * blockSize, dY + j * blockSize, blockSize, blockSize );
							break;

						case GameColor.Yellow:
							bg.Graphics.FillRectangle( Brushes.Yellow, dX + i * blockSize, dY + j * blockSize, blockSize, blockSize );
							break;

						case GameColor.Magenta:
							bg.Graphics.FillRectangle( Brushes.Magenta, dX + i * blockSize, dY + j * blockSize, blockSize, blockSize );
							break;

						case GameColor.Cyan:
							bg.Graphics.FillRectangle( Brushes.Cyan, dX + i * blockSize, dY + j * blockSize, blockSize, blockSize );
							break;
					}
				}
			for( int i = 0; i < FIELD_SIZE; i++ )
			{
				bg.Graphics.DrawLine( Pens.Gray, dX, dY + i * blockSize, dX + FIELD_SIZE * blockSize, dY + i * blockSize );
				bg.Graphics.DrawLine( Pens.Gray, dX + i * blockSize, dY, dX + i * blockSize, dY + FIELD_SIZE * blockSize );
			}
			bg.Render();
		}

		private void InitBG()
		{
			bg = BufferedGraphicsManager.Current.Allocate( CreateGraphics(), ClientRectangle );
			bg.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
		}

		private void InitSize()
		{
			heightInfoPanel = Height / (FIELD_SIZE + 1);
			blockSize = ClientSize.Height - heightInfoPanel > ClientSize.Width ? ClientSize.Width / FIELD_SIZE : (ClientSize.Height - heightInfoPanel) / FIELD_SIZE;
			dX = (ClientSize.Width - blockSize * FIELD_SIZE) / 2;
			dY = heightInfoPanel + (ClientSize.Height - heightInfoPanel - blockSize * FIELD_SIZE) / 2;
			font = new Font( "Arial", ( float )heightInfoPanel / 3 * 2, FontStyle.Regular, GraphicsUnit.Point );
			rectangleInfoPanel = new RectangleF( 0, 0, ClientSize.Width, heightInfoPanel );
		}

		private void MainForm_Paint( object sender, PaintEventArgs e )
		{
			bg.Render();
		}

		private void MainForm_Resize( object sender, EventArgs e )
		{
			InitBG();
			InitSize();
			DrawField( game.Field );
		}

		private void timer_Tick( object sender, EventArgs e )
		{
			game.Update( controller );
			controller.GameKey &= controllerCash.GameKey;
			controller.GameClick = null;
			DrawField( game.Field );
			if( game.StatusGame == StatusGame.Over )
				mode = GameMode.EndGame;
		}

		private void SelectNextGame()
		{
			selectGame++;
			if( selectGame == games.Count )
				selectGame = 0;
			game = games[selectGame];
			game.PreviewGame();
			DrawField( game.Field );
		}

		private void SelectBackGame()
		{
			selectGame--;
			if( selectGame == -1 )
				selectGame = games.Count - 1;
			game = games[selectGame];
			game.PreviewGame();
			DrawField( game.Field );
		}

		private void EndGame()
		{
			mode = GameMode.EndGame;
		}
			   
		private void MainForm_KeyDown( object sender, KeyEventArgs e )
		{
			 if ( mode == GameMode.SelectGame )
			{
				switch( e.KeyCode )
				{
					case Keys.Enter:
						mode = GameMode.PlayGame;
						game.NewGame();
						break;

					case Keys.Space:
						mode = GameMode.PlayGame;
						game.NewGame();
						break;

					case Keys.W:
						break;

					case Keys.Up:
						break;

					case Keys.Down:
						break;

					case Keys.S:
						break;

					case Keys.Left:
						SelectBackGame();
						break;

					case Keys.A:
						SelectBackGame();
						break;

					case Keys.Right:
						SelectNextGame();
						break;

					case Keys.D:
						SelectNextGame();
						break;
				}
			}
			else if ( mode == GameMode.PlayGame )
			{
				switch(e.KeyCode)
				{
					case Keys.F2:
						game.NewGame();
						timer.Start();
						break;

					case Keys.Escape:
						mode = GameMode.SelectGame;
						game.PreviewGame();
						timer.Start();
						break;

					case Keys.W:
						controller.GameKey |= GameKey.Up;
						controllerCash.GameKey |= GameKey.Up;
						break;

					case Keys.Up:
						controller.GameKey |= GameKey.Up;
						controllerCash.GameKey |= GameKey.Up;
						break;

					case Keys.Down:
						controller.GameKey |= GameKey.Down;
						controllerCash.GameKey |= GameKey.Down;
						break;

					case Keys.S:
						controller.GameKey |= GameKey.Down;
						controllerCash.GameKey |= GameKey.Down;
						break;

					case Keys.Left:
						controller.GameKey |= GameKey.Left;
						controllerCash.GameKey |= GameKey.Left;
						break;

					case Keys.A:
						controller.GameKey |= GameKey.Left;
						controllerCash.GameKey |= GameKey.Left;
						break;

					case Keys.Right:
						controller.GameKey |= GameKey.Right;
						controllerCash.GameKey |= GameKey.Right;
						break;

					case Keys.D:
						controller.GameKey |= GameKey.Right;
						controllerCash.GameKey |= GameKey.Right;
						break;

					case Keys.J:
						controller.GameKey |= GameKey.ButtonA;
						controllerCash.GameKey |= GameKey.ButtonA;
						break;

					case Keys.NumPad1:
						controller.GameKey |= GameKey.ButtonA;
						controllerCash.GameKey |= GameKey.ButtonA;
						break;

					case Keys.K:
						controller.GameKey |= GameKey.ButtonB;
						controllerCash.GameKey |= GameKey.ButtonB;
						break;

					case Keys.NumPad2:
						controller.GameKey |= GameKey.ButtonB;
						controllerCash.GameKey |= GameKey.ButtonB;
						break;

					case Keys.Enter:
						timer.Enabled = !timer.Enabled;
						break;

					case Keys.Space:
						timer.Enabled = !timer.Enabled;
						break;
				}
			}
			else if ( mode == GameMode.EndGame )
			{
				switch( e.KeyCode )
				{
					case Keys.Enter:
						mode = GameMode.PlayGame;
						game.NewGame();
						break;

					case Keys.Space:
						mode = GameMode.PlayGame;
						game.NewGame();
						break;

					case Keys.Escape:
						mode = GameMode.SelectGame;
						game.PreviewGame();
						break;
				}
			}
		}

		private void MainForm_KeyUp( object sender, KeyEventArgs e )
		{
			if( mode == GameMode.PlayGame )
			{
				switch( e.KeyCode )
				{
					case Keys.W:
						controllerCash.GameKey &= ~GameKey.Up;
						break;

					case Keys.Up:
						controllerCash.GameKey &= ~GameKey.Up;
						break;

					case Keys.Down:
						controllerCash.GameKey &= ~GameKey.Down;
						break;

					case Keys.S:
						controllerCash.GameKey &= ~GameKey.Down;
						break;

					case Keys.Left:
						controllerCash.GameKey &= ~GameKey.Left;
						break;

					case Keys.A:
						controllerCash.GameKey &= ~GameKey.Left;
						break;

					case Keys.Right:
						controllerCash.GameKey &= ~GameKey.Right;
						break;

					case Keys.D:
						controllerCash.GameKey &= ~GameKey.Right;
						break;

					case Keys.J:
						controllerCash.GameKey &= ~GameKey.ButtonA;
						break;

					case Keys.NumPad1:
						controllerCash.GameKey &= ~GameKey.ButtonA;
						break;

					case Keys.K:
						controllerCash.GameKey &= ~GameKey.ButtonB;
						break;

					case Keys.NumPad2:
						controllerCash.GameKey &= ~GameKey.ButtonB;
						break;
				}
			}
		}

		private void MainForm_MouseClick( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left && e.X - dX >= 0 && e.Y - dY >= 0 )
			{
				int x = (e.X - dX) / blockSize;
				int y = (e.Y - dY) / blockSize;
				if( x >= 0 && x < FIELD_SIZE && y >= 0 && y < FIELD_SIZE )
					controller.GameClick = new GameClick() { X = x, Y = y };
			}
		}
	}
}

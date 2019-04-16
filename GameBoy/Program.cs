using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GameBoy
{
	static class Program
	{
		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			List<Game> games = new List<Game>();
			games.Add( new Snake() );
			games.Add( new Tetris() );
			games.Add( new Life() );
			Application.Run( new MainForm( games ) );
		}
	}
}

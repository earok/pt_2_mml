using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_2_MML
{
	/// <summary>
	/// Data about a whole pattern
	/// </summary>
	public class PatternData
	{
		/// <summary>
		/// All notes for all 64 rows and all four channels
		/// </summary>
		public NoteData[,] NoteData = new NoteData[64, 4];

		/// <summary>
		/// The next position after this pattern
		/// </summary>
		public int NextPosition = -1;

		/// <summary>
		/// Is this the last pattern in the song?
		/// </summary>
		public bool SongEnd;
	}
}

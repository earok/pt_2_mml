using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_2_MML
{
	/// <summary>
	/// Data about each row
	/// </summary>
	public class RowData
	{
		public RowData(int channels)
		{
			NoteData = new NoteData[channels];
		}

		/// <summary>
		/// Four notes on each row
		/// </summary>
		public NoteData[] NoteData;

		/// <summary>
		/// Is this a loop row?
		/// </summary>
		public bool IsLoopRow;

		/// <summary>
		/// How many times does this loop repeat?
		/// </summary>
		public int LoopRepeats;

		/// <summary>
		/// How many ticks per row, if this setting needs to change?
		/// </summary>
		public int TicksPerRow;

		/// <summary>
		/// The tempo this note played back at
		/// </summary>
		public int Tempo;
	}
}

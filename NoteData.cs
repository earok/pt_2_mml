using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_2_MML
{
	/// <summary>
	/// Contains information about a specific node
	/// </summary>
	public class NoteData
	{
		/// <summary>
		/// The ID of the sample
		/// </summary>
		public int Sample;

		/// <summary>
		/// The period this note is played back at
		/// </summary>
		public int Period;

		/// <summary>
		/// The effect data
		/// </summary>
		public int Effect;

		/// <summary>
		/// Volume override, if any
		/// </summary>
		public int Volume = -1;

		/// <summary>
		/// The amount to slide the volume by (generates a macro)
		/// </summary>
		public int VolumeSlide = 0;

		/// <summary>
		/// Instant volume change up or down (EAx)
		/// </summary>
		public int VolumeAdjust;

		/// <summary>
		/// How many loops are owed? Will keep repeating until this is zero
		/// </summary>
		public int LoopsOwed;

		/// <summary>
		/// How many bytes into the sample do we play?
		/// </summary>
		public int Offset;

		/// <summary>
		/// This note should be repeated every X tick
		/// </summary>
		public int RepeatEveryTick;

		/// <summary>
		/// The type of effect
		/// </summary>
		public int EffectType => (Effect >> 8);

		/// <summary>
		/// The value of the effect
		/// </summary>
		public int EffectValue => Effect & 0xff;

		/// <summary>
		/// The index of the period
		/// </summary>
		public int PeriodIndex => Array.IndexOf(Program.PeriodTable, Period);

		/// <summary>
		/// The octave
		/// </summary>
		public int Octave => (PeriodIndex / 12) + 1;

		/// <summary>
		/// The string for the source node
		/// </summary>
		public string Note => Program.SourceNoteTable[PeriodIndex % 12];

		/// <summary>
		/// The string for the output note
		/// </summary>
		public string OutputNote => Program.OutputNoteTable[PeriodIndex % 12];

		/// <summary>
		/// Converts the note data to a string
		/// </summary>
		/// <returns>The output string</returns>
		public override string ToString()
		{
			var noteText = "...";
			var sampleText = "..";
			var effectText = "...";

			if (Sample > 0 && Period > 0)
			{
				sampleText = Sample.ToString("D2");
				noteText = Note + Octave.ToString();
			}

			if (Effect > 0)
			{
				effectText = string.Format("{0:X}{1:D2}", EffectType, EffectValue);
			}

			return string.Format("{0}{1}{2}", sampleText, noteText, effectText);
		}
	}
}

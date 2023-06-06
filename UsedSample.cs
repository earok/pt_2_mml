using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_2_MML
{
	/// <summary>
	/// Contains information about each sample that is actually used, and the period they're played back at
	/// </summary>
	public class UsedSample
	{
		/// <summary>
		/// The period for this sample
		/// </summary>
		public int Period;

		/// <summary>
		/// The source sample Id
		/// </summary>
		public int Sample;

		/// <summary>
		/// The source note
		/// </summary>
		public string Note;

		/// <summary>
		/// The source octave
		/// </summary>
		public int Octave;

		/// <summary>
		/// The used sample id, used to give every combination of sample and note a unique identifier
		/// </summary>
		public int Id;

		public UsedSample(int id, int octave, string note)
		{
			Id = id;
			Octave = octave;
			Note = note;
		}

		/// <summary>
		/// Returns the file name of the sample
		/// </summary>
		/// <param name="sample">The source sample data</param>
		/// <returns></returns>
		public string CorrectName(SampleData sample)
		{
			return sample.Name + "." + Note + Octave + ".wav";
		}
	}
}

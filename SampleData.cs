using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PT_2_MML
{
	/// <summary>
	/// Data about a protracker sample
	/// </summary>
	public class SampleData
	{
		/// <summary>
		/// The output name of the sample, with system friendly characters only
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				foreach (char c in Path.GetInvalidFileNameChars())
				{
					value = value.Replace(c, '_');
				}
				_name = value;
			}
		}
		
		/// <summary>
		/// The original name of the sample
		/// </summary>
		string _name = "";

		/// <summary>
		/// The length of the sample in words
		/// </summary>
		public int Length;

		/// <summary>
		/// The fine tune value
		/// </summary>
		public int FineTune;

		/// <summary>
		/// The default volume
		/// </summary>
		public int Volume;

		/// <summary>
		/// The repeat point of the sample
		/// </summary>
		public int RepeatPoint;

		/// <summary>
		/// How many words to repeat
		/// </summary>
		public int RepeatLength;

		/// <summary>
		/// The original ID of the sample
		/// </summary>
		public int Id;

		/// <summary>
		/// The starting point in the file
		/// </summary>
		public int Start;

		/// <summary>
		/// Is this a valid sample?
		/// </summary>
		/// <returns>Sample is valid or not</returns>
		public bool IsValid()
		{
			return Length > 0;
		}
	}
}

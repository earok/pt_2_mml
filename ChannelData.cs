using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_2_MML
{
	/// <summary>
	/// Contains information on one of the four protracker channels
	/// </summary>
	public class ChannelData
	{
		public ChannelData(int id)
		{
			Id = id;
		}

		/// <summary>
		/// Which ProTracker channel this relates to
		/// </summary>
		public int Id;

		/// <summary>
		/// Which MDS channel this relates to
		/// </summary>
		public string Map = "";

		/// <summary>
		/// What's the current volume of this channel?
		/// </summary>
		public int LastVolume;

		/// <summary>
		/// What was the last sample played on this channel?
		/// </summary>
		public SampleData? LastSampleData;

		/// <summary>
		/// How many rows does the last sample need to play for?
		/// </summary>
		public int LastSampleRows;

		/// <summary>
		/// Updates the minimum required rows of the last sample data
		/// </summary>
		public void SetSampleMinimumRows()
		{
			if (LastSampleData == null)
			{
				return;
			}

			LastSampleData.MinimumRows = Math.Max(LastSampleRows, LastSampleData.MinimumRows);
		}
	}
}

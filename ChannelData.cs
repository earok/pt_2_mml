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
		/// Are we repeating an instrument?
		/// </summary>
		public int RepeatInstrument = -1;
		internal int RepeatTicks;
	}
}

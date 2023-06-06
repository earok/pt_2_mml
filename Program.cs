﻿using NAudio.Wave;
using System.Text;

namespace PT_2_MML
{
	internal class Program
	{
		/// <summary>
		/// The raw bytes of the protracker file
		/// </summary>
		private static byte[] bytes = new byte[0];

		/// <summary>
		/// How far we've read into the raw bytes
		/// </summary>
		private static int offset = 0;

		/// <summary>
		/// This is the decibel output for all 64 (1-64) volume levels of Paula.
		/// </summary>
		internal static double[] AmigaVolumeTable = new double[] { 0, -0.1, -0.3, -0.4, -0.6, -0.7, -0.9, -1, -1.2, -1.3, -1.5, -1.6, -1.8, -2, -2.1, -2.3, -2.5, -2.7, -2.9, -3.1, -3.3, -3.5, -3.7, -3.9, -4.1, -4.3, -4.5, -4.8, -5, -5.2, -5.5, -5.8, -6, -6.3, -6.6, -6.9, -7.2, -7.5, -7.8, -8.2, -8.5, -8.9, -9.3, -9.7, -10.1, -10.5, -11, -11.5, -12, -12.6, -13.2, -13.8, -14.5, -15.3, -16.1, -17, -18.1, -19.2, -20.6, -22.1, -24.1, -26.6, -30.1, -36.1 };

		/// <summary>
		/// The valid protracker periods. Periods outside of this will not be supported.
		/// </summary>
		internal static int[] PeriodTable = new int[] { 856, 808, 762, 720, 678, 640, 604, 570, 538, 508, 480, 453, 428, 404, 381, 360, 339, 320, 302, 285, 269, 254, 240, 226, 214, 202, 190, 180, 170, 160, 151, 143, 135, 127, 120, 113 };

		/// <summary>
		/// The Protracker names for each note
		/// </summary>
		internal static string[] SourceNoteTable = new string[] { "C-", "C#", "D-", "D#", "E-", "F-", "F#", "G-", "G#", "A-", "A#", "B-" };

		/// <summary>
		/// The MDSDRV names for each note
		/// </summary>
		internal static string[] OutputNoteTable = new string[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };

		/// <summary>
		/// Operating system safe names for output notes
		/// </summary>
		internal static string[] FileNameTable = new string[] { "C_", "CS", "D_", "DS", "E_", "F_", "FS", "G_", "GS", "A_", "AS", "B_" };

		/// <summary>
		/// Contains information on each of the four protracker channels
		/// </summary>
		internal static ChannelData[] ChannelData = new ChannelData[] { new ChannelData(0), new ChannelData(1), new ChannelData(2), new ChannelData(3) };

		/// <summary>
		/// A list of how all fine tune settings for every sample maps to the actual playback frequency
		/// </summary>
		internal static int[][] FineTuneLookup = new int[16][];

		/// <summary>
		/// PCM samples are played back at multiples of this in MDSDRV
		/// </summary>
		internal static int OutHz = 2190;

		/// <summary>
		/// The maximum amount we can multiply
		/// </summary>
		const int MaxHzMultiplier = 4;

		/// <summary>
		/// The maximum WAV size that MDSDRV supported. We'll use a lower output frequency if a sample won't fit under this size limit
		/// </summary>
		const int MaxWavSize = 32 * 1024;

		/// <summary>
		/// A list of all of the samples that are actually needed, along with the required 
		/// </summary>
		internal static List<UsedSample> UsedSamples = new();

		/// <summary>
		/// A list of all Macros we need to dynamically generate to support ProTracker effects
		/// </summary>
		internal static List<Macro> Macros = new();

		static void Main(string[] args)
		{
			try
			{
				//Starts from fine tune of -8
				FineTuneLookup[8] = new int[] { 3911, 4144, 4390, 4655, 4926, 5231, 5542, 5872, 6223, 6593, 6982, 7389, 7830, 8287, 8779, 9309, 9852, 10463, 11084, 11745, 12445, 13185, 13964, 14779, 15694, 16574, 17559, 18668, 19705, 20864, 22168, 23489, 24803, 26273, 27928, 29557 };
				FineTuneLookup[9] = new int[] { 3941, 4173, 4423, 4685, 4961, 5255, 5577, 5902, 6256, 6630, 7024, 7436, 7882, 8346, 8845, 9359, 9935, 10525, 11154, 11823, 12489, 13235, 14019, 14903, 15764, 16731, 17734, 18767, 19815, 20988, 22308, 23646, 24978, 26469, 28150, 29806 };
				FineTuneLookup[10] = new int[] { 3967, 4202, 4456, 4717, 5003, 5294, 5612, 5941, 6300, 6667, 7066, 7483, 7935, 8405, 8912, 9433, 9991, 10588, 11224, 11902, 12578, 13334, 14131, 14966, 15905, 16810, 17824, 18866, 20039, 21239, 22449, 23805, 25155, 26668, 28375, 30058 };
				FineTuneLookup[11] = new int[] { 3999, 4233, 4484, 4755, 5038, 5334, 5648, 5991, 6345, 6718, 7122, 7547, 7989, 8465, 8979, 9509, 10076, 10683, 11296, 11983, 12667, 13435, 14245, 15093, 15977, 16971, 17914, 18967, 20153, 21367, 22592, 23966, 25335, 26870, 28375, 30058 };
				FineTuneLookup[12] = new int[] { 4026, 4263, 4518, 4787, 5074, 5374, 5693, 6032, 6391, 6769, 7180, 7595, 8043, 8526, 9048, 9586, 10134, 10748, 11368, 12064, 12759, 13538, 14360, 15223, 16122, 17052, 18096, 19172, 20268, 21496, 22737, 24129, 25517, 27076, 28837, 30315 };
				FineTuneLookup[13] = new int[] { 4054, 4294, 4553, 4819, 5111, 5415, 5730, 6073, 6437, 6821, 7224, 7661, 8116, 8588, 9095, 9638, 10222, 10814, 11479, 12147, 12851, 13642, 14477, 15288, 16196, 17218, 18189, 19277, 20384, 21627, 22883, 24294, 25702, 27284, 28837, 30577 };
				FineTuneLookup[14] = new int[] { 4086, 4325, 4583, 4859, 5148, 5448, 5777, 6115, 6484, 6874, 7283, 7711, 8173, 8651, 9165, 9718, 10281, 10914, 11553, 12231, 12945, 13748, 14536, 15421, 16345, 17302, 18378, 19382, 20621, 21760, 23032, 24461, 25890, 27495, 29073, 30843 };
				FineTuneLookup[15] = new int[] { 4115, 4357, 4618, 4892, 5186, 5491, 5815, 6169, 6532, 6914, 7328, 7761, 8229, 8715, 9237, 9771, 10371, 10981, 11629, 12316, 13040, 13855, 14657, 15557, 16421, 17472, 18473, 19596, 20742, 22030, 23335, 24631, 26080, 27710, 29313, 31113 };
				FineTuneLookup[0] = new int[] { 4144, 4390, 4655, 4926, 5231, 5542, 5872, 6223, 6593, 6982, 7389, 7830, 8287, 8779, 9309, 9852, 10463, 11084, 11745, 12445, 13185, 13964, 14779, 15694, 16574, 17559, 18668, 19705, 20864, 22168, 23489, 24803, 26273, 27928, 29557, 31388 };
				FineTuneLookup[1] = new int[] { 4173, 4423, 4685, 4961, 5262, 5568, 5902, 6256, 6630, 7024, 7436, 7882, 8346, 8845, 9359, 9935, 10525, 11154, 11823, 12489, 13235, 14019, 14841, 15764, 16652, 17646, 18767, 19815, 20988, 22308, 23646, 24978, 26469, 28150, 29806, 31388 };
				FineTuneLookup[2] = new int[] { 4202, 4456, 4717, 5003, 5294, 5612, 5941, 6300, 6667, 7066, 7483, 7935, 8405, 8912, 9433, 9991, 10588, 11224, 11902, 12578, 13334, 14131, 14966, 15834, 16810, 17824, 18866, 20039, 21239, 22449, 23805, 25155, 26668, 28375, 30058, 31669 };
				FineTuneLookup[3] = new int[] { 4233, 4484, 4755, 5038, 5334, 5648, 5991, 6345, 6718, 7122, 7547, 7989, 8465, 8979, 9509, 10076, 10683, 11296, 11983, 12667, 13435, 14245, 15093, 15977, 16971, 17914, 18967, 20153, 21367, 22592, 23966, 25335, 26870, 28375, 30058, 31954 };
				FineTuneLookup[4] = new int[] { 4263, 4518, 4787, 5074, 5374, 5693, 6032, 6391, 6769, 7165, 7595, 8043, 8526, 9048, 9586, 10134, 10748, 11368, 12064, 12759, 13538, 14360, 15223, 16122, 17052, 18096, 19172, 20268, 21496, 22737, 24129, 25517, 27076, 28604, 30315, 32245 };
				FineTuneLookup[5] = new int[] { 4294, 4553, 4819, 5111, 5415, 5730, 6073, 6437, 6821, 7224, 7661, 8116, 8588, 9095, 9638, 10222, 10814, 11479, 12147, 12851, 13642, 14477, 15288, 16196, 17218, 18189, 19277, 20384, 21627, 22883, 24294, 25702, 27284, 28837, 30577, 32540 };
				FineTuneLookup[6] = new int[] { 4325, 4583, 4859, 5148, 5448, 5777, 6115, 6484, 6874, 7283, 7711, 8173, 8651, 9165, 9718, 10281, 10914, 11553, 12231, 12945, 13748, 14536, 15421, 16345, 17302, 18378, 19382, 20621, 21760, 23032, 24461, 25890, 27495, 29073, 30843, 32540 };
				FineTuneLookup[7] = new int[] { 4357, 4618, 4892, 5186, 5491, 5815, 6169, 6532, 6914, 7328, 7761, 8229, 8715, 9237, 9771, 10371, 10981, 11629, 12316, 13040, 13855, 14657, 15557, 16421, 17387, 18473, 19596, 20742, 22030, 23335, 24631, 26080, 27710, 29313, 31113, 32842 };

				if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]) || File.Exists(args[0]) == false)
				{
					Console.WriteLine("Drag a MOD file on to this executable, or add the file path to the command line");
				}
				else
				{
					//Create the output folder
					if (Path.Exists("output") == false)
					{
						Directory.CreateDirectory("output");
					}

					//Load the file
					bytes = File.ReadAllBytes(args[0]);

					//Get the song title
					var title = ReadString(20);

					//Read each of the samples
					var samples = new Dictionary<int, SampleData>();
					for (var i = 0; i < 31; i++)
					{
						var sample = new SampleData
						{
							Id = i + 1,
							Name = ReadString(22),
							Length = ReadWord(),
							FineTune = ReadByte(),
							Volume = ReadByte(),
							RepeatPoint = ReadWord(),
							RepeatLength = ReadWord(),
						};

						if (sample.IsValid())
						{
							samples.Add(sample.Id, sample);
						}
					}

					var songLength = ReadByte();

					ReadByte(); //This should equal 127

					//Read each of the song positions
					var positions = new int[128];
					var lastPosition = 0;
					for (var i = 0; i < 128; i++)
					{
						positions[i] = ReadByte();
						lastPosition = Math.Max(lastPosition, positions[i]);
					}

					var patternCount = lastPosition + 1;
					var patterns = new PatternData[64];
					var tempoMode = false;
					var tempo = 125;
					var ticksPerRow = 6;

					var unsupportedEffects = new HashSet<string>();

					//Check that this is the M.K. style of protracker file
					Verify("M.K.");

					//Rip every pattern
					for (var i = 0; i < patternCount; i++)
					{
						var pattern = patterns[i] = new PatternData();

						//For every row...
						for (var rowId = 0; rowId < 64; rowId++)
						{
							//For every channel...
							for (var channelId = 0; channelId < 4; channelId++)
							{
								var b1 = ReadByte();
								var b2 = ReadByte();
								var b3 = ReadByte();
								var b4 = ReadByte();

								var noteData = pattern.NoteData[rowId, channelId] = new NoteData()
								{
									Effect = ((b3 & 0x0F) << 8) | b4,
									Period = ((b1 & 0x0F) << 8) | b2,
									Sample = (b1 & 0xF0) | (b3 >> 4),
								};

								var effectSupported = false;
								switch (noteData.EffectType)
								{
									case 0: //nothing!?
										effectSupported = true;
										break;

									case 0xb: //Position jump
										effectSupported = true;
										pattern.NextPosition = noteData.EffectValue;
										break;

									case 0xa: //volume slide
										effectSupported = true;
										if (noteData.EffectValue <= 0xf)
										{
											noteData.VolumeSlide = -noteData.EffectValue;
										}
										else
										{
											noteData.VolumeSlide = (noteData.EffectValue >> 4);
										}
										break;

									case 0xc: //Volume
										effectSupported = true;
										noteData.Volume = noteData.EffectValue;
										break;

									case 0xf: //Speed/tempo

										effectSupported = true;
										if (noteData.EffectValue == 0)
										{
											//End of the song
											pattern.SongEnd = true;
										}
										else if (noteData.EffectValue < 0x1f)
										{
											tempoMode = false;
											tempo = (50 * 60) / (noteData.EffectValue * 4);
											ticksPerRow = noteData.EffectValue;
										}
										else
										{
											tempoMode = true;
											tempo = noteData.EffectValue;
										}
										break;

								}

								if (effectSupported == false)
								{
									unsupportedEffects.Add(noteData.EffectType.ToString("X") + noteData.EffectValue.ToString("X2"));
								}

							}
						}
					}

					//Find the loop point
					var position = 0;
					var loopPosition = -1;
					var loopedPositions = new HashSet<int>();

					while (true)
					{
						loopedPositions.Add(position);
						var pattern = patterns[positions[position]];
						if (pattern.NextPosition == -1)
						{
							position += 1;
						}
						else
						{
							//This is the end of the song
							if (loopedPositions.Contains(pattern.NextPosition) || pattern.SongEnd)
							{
								if (pattern.SongEnd == false) //Infinitely looping song
								{
									loopPosition = pattern.NextPosition;
								}
								lastPosition = position;
								break;
							}
							position = pattern.NextPosition;

						}
					}

					//Serialize
					var sb = new StringBuilder();
					sb.AppendLine("#title " + title);
					sb.AppendLine("#date " + DateTime.Now.ToString("dd-MM-yyyy"));
					sb.AppendLine("#platform mdsdrv");
					sb.AppendLine(string.Empty);

					sb.AppendLine(";==== Positions ====");
					for (var i = 0; i <= lastPosition; i++)
					{
						sb.AppendLine(string.Format("; {0} = {1}", i, positions[i]));
					}
					sb.AppendLine(string.Empty);

					sb.AppendLine(";Patterns");
					sb.AppendLine(string.Empty);

					for (var i = 0; i < patternCount; i++)
					{
						sb.AppendLine(string.Format("; Pattern {0}", i));
						for (var j = 0; j < 64; j++)
						{
							sb.AppendLine(string.Format("; {0} {1} {2} {3}",
								patterns[i].NoteData[j, 0].ToString(),
								patterns[i].NoteData[j, 1].ToString(),
								patterns[i].NoteData[j, 2].ToString(),
								patterns[i].NoteData[j, 3].ToString()
								));
						}
						sb.AppendLine(string.Empty);
					}


					position = 0;
					loopedPositions.Clear();

					var mdChannels = new string[] { "F", "K", "L" };
					Console.WriteLine("Map Mega Drive / Genesis Channels to Protracker channel # (1-4, leave empty to ignore)");
					foreach (var mdChannel in mdChannels)
					{
						Console.Write("Channel " + mdChannel + ": ");
						if (int.TryParse(Console.ReadLine() ?? "", out int ptChannel))
						{
							ChannelData[Math.Clamp(ptChannel - 1, 0, 3)].Map = mdChannel;
						}
					}

					while (true)
					{
						loopedPositions.Add(position);
						var patternId = positions[position];

						var pattern = patterns[patternId];
						sb.AppendLine(string.Format("; Position {0} Pattern {1}", position, patternId));

						foreach (var channel in ChannelData)
						{
							if (string.IsNullOrWhiteSpace(channel.Map))
							{
								continue;
							}

							//If we're in tempo mode as opposed to tick mode
							if (patternId == 0 && tempoMode)
							{
								sb.Append("t" + tempo + " ");
								sb.Append(channel.Map + " l16 ");
							}

							//Set the loop position
							if (position == loopPosition)
							{
								sb.Append("L ");
							}

							sb.Append("V0 ");

							var row = 0;
							while (row < 64)
							{
								var note = pattern.NoteData[row, channel.Id];

								//Volume?
								if (note.Volume > -1)
								{
									channel.LastVolume = note.Volume;
									sb.Append("V" + FixVolume(note.Volume) + " ");
								}
								else if (note.Sample > 0)
								{
									var sample = samples[note.Sample];
									channel.LastVolume = sample.Volume;
									sb.Append("V" + FixVolume(sample.Volume) + " ");
								}

								//Macro
								if (note.VolumeSlide != 0)
								{
									var macroId = GetVolumeMacro(channel.LastVolume, note.VolumeSlide);
									sb.Append("P" + macroId + " ");
									channel.LastVolume = Math.Clamp(channel.LastVolume + note.VolumeSlide * ticksPerRow, 0, 64);
								}

								if (note.Sample > 0 && note.Period > 0)
								{
									var instrument = GetUsedSample(note.Period, note.Sample);

									sb.Append("@" + instrument);
									if (tempoMode)
									{
										sb.Append(note.OutputNote);
									}
									else
									{
										sb.Append(note.OutputNote + ":" + ticksPerRow + " ");
									}

									row += 1;

									if (channel.LastSampleData != null)
									{
										channel.SetSampleMinimumRows();
									}
									channel.LastSampleData = samples[note.Sample];
									channel.LastSampleRows = 0;
								}
								else
								{
									if (tempoMode)
									{
										sb.Append("^ ");
									}
									else
									{
										sb.Append("^:" + ticksPerRow + " ");
									}
									row += 1;
								}

								if (channel.LastSampleData != null)
								{
									channel.LastSampleRows++;
								}
							}

							sb.AppendLine(string.Empty);
						}

						sb.AppendLine(string.Empty);

						//Set the next position
						if (loopedPositions.Contains(pattern.NextPosition) || pattern.SongEnd)
						{
							break;
						}

						if (pattern.NextPosition == -1)
						{
							position += 1;
						}
						else
						{
							position = pattern.NextPosition;
						}
					}

					foreach (var channel in ChannelData)
					{
						channel.SetSampleMinimumRows();
					}

					sb.AppendLine(";==== Sample Section ====");

					//Get the sample offset
					foreach (var sample in samples.Values)
					{
						sample.Start = offset;

						//Convert from unsigned to signed
						for (var i = 0; i < sample.Length * 2; i++)
						{
							bytes[i + sample.Start] = (byte)(bytes[i + sample.Start] + 128);
						}

						offset += sample.Length * 2;
					}


					foreach (var usedSample in UsedSamples)
					{
						var sample = samples[usedSample.Sample];
						sb.AppendLine(string.Format("@{0} pcm \"{1}\" ;Length={2} FT={3} VOL={4} RP={5} RL={6} NOTE={7} ORIGINAL={8}", usedSample.Id, usedSample.CorrectName(sample), sample.Length, sample.FineTune, sample.Volume, sample.RepeatPoint, sample.RepeatLength, usedSample.Note, sample.Id));

						var lookup = Array.IndexOf(PeriodTable, usedSample.Period);

						var frequency = FineTuneLookup[sample.FineTune][lookup];

						//Make the SOURCE wav
						var waveFormat = new WaveFormat(frequency, 8, 1);

						//If repeating, make sure that the rip is at least as long as the sample is needed.
						int minSamples;
						if (sample.RepeatLength > 1)
						{
							minSamples = (frequency * sample.MinimumRows * ticksPerRow) / 50;
						}
						else
						{
							minSamples = sample.Length * 2;
						}

						//Is there a way to do this without writing a file?
						using (var writer = new WaveFileWriter("temp.wav", waveFormat))
						{
							writer.Write(bytes, sample.Start, sample.Length * 2);

							while (writer.Length < minSamples)
							{
								//Pad out the repeat length
								writer.Write(bytes, sample.Start + sample.RepeatPoint * 2, sample.RepeatLength * 2);
							}
						}

						using (var reader = new WaveFileReader("temp.wav"))
						{
							var targetMultiplier = Math.Min((MaxWavSize / reader.SampleCount) + 1, MaxHzMultiplier);


							var outFormat = new WaveFormat(OutHz * MaxHzMultiplier, 8, reader.WaveFormat.Channels);
							using (var resampler = new MediaFoundationResampler(reader, outFormat))
							{
								WaveFileWriter.CreateWaveFile("output/" + usedSample.CorrectName(sample), resampler);
							}
						}


					}

					sb.AppendLine(string.Empty);
					sb.AppendLine(";==== Autogen Macros ====");
					foreach (var macro in Macros)
					{
						//Insert a rest of 1 frame before we change the volume
						sb.Append(string.Format("*{0}", macro.ID));

						//Fix the volume for every frame
						var volume = macro.BaseVolume;

						while ((volume > 0 && macro.VolumeSlide < 0) || (volume < 64 && macro.VolumeSlide > 0))
						{
							volume = Math.Clamp(volume + macro.VolumeSlide, 0, 64);
							sb.Append(string.Format(" r:1 V{0}", FixVolume(volume)));
						}

						//				for (var i = 0; i < ticksPerRow; i++)
						//				{

						//				}

						sb.AppendLine(string.Empty);
					}

					/*
					sb.AppendLine(";==== A0Y Effects ====");
					sb.AppendLine("*100 ;A00 Do nothing!");
					for (var i = 1; i < 16; i++)
					{
						var speed = i;
						sb.AppendLine(string.Format("*{0} [r:1 V+{1}]{3}; {2}", i + 100, speed, "A0" + i.ToString("X"), ticksPerRow));
					}
					*/

					sb.AppendLine(string.Empty);
					sb.AppendLine(";==== Unsupported Effects ====");
					sb.Append(";");
					foreach (var effect in unsupportedEffects.Order())
					{
						sb.Append(effect + " ");
					}


					sb.AppendLine(string.Empty);

					File.WriteAllText("output/song.mml", sb.ToString());
					Console.WriteLine("Done! MML and WAV files are in the output folder");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Fatal error: " + ex.ToString());
			}

			if (File.Exists("temp.wav"))
			{
				File.Delete("temp.wav");
			}

			Console.WriteLine("Press any key to quit");
			Console.ReadKey();
		}

		private static int GetVolumeMacro(int baseVolume, int volumeSlide)
		{
			var index = Macros.FindIndex(p => p.VolumeSlide == volumeSlide && p.BaseVolume == baseVolume);
			if (index > -1)
			{
				return index + 100;
			}

			var id = Macros.Count + 100;
			Macros.Add(new Macro() { BaseVolume = baseVolume, VolumeSlide = volumeSlide, ID = id });
			return id;
		}

		private static int FixVolume(int volume)
		{
			//Hard coded silence setting
			if (volume == 0)
			{
				return 127;
			}

			var sourceVolume = AmigaVolumeTable[Math.Clamp(64 - volume, 0, AmigaVolumeTable.Length - 1)];
			return (int)Math.Round(sourceVolume / -.75);
		}

		private static int GetUsedSample(int period, int sample)
		{
			var note = "NONE";
			var octave = 1;

			if (sample > 0)
			{
				note = FileNameTable[(Array.IndexOf(PeriodTable, period) % FileNameTable.Length)];
				octave += (Array.IndexOf(PeriodTable, period)) / FileNameTable.Length;
			}

			var index = UsedSamples.FindIndex(p => p.Period == period && p.Sample == sample);
			if (index > -1)
			{
				return index + 1;
			}

			UsedSamples.Add(new UsedSample(UsedSamples.Count + 1, octave, note) { Period = period, Sample = sample });
			return UsedSamples.Count;
		}

		private static int ReadByte()
		{
			offset += 1;
			return bytes[offset - 1];
		}

		private static int ReadWord()
		{
			var result = bytes[offset] << 8 | bytes[offset + 1];
			offset += 2;
			return result;
		}

		private static string ReadString(int limit)
		{
			var result = "";
			for (var i = 0; i < limit; i++)
			{
				if (bytes[offset + i] == 0)
				{
					offset += limit;
					return result;
				}
				result += Encoding.ASCII.GetString(bytes, offset + i, 1);
			}
			offset += limit;
			return result;
		}

		private static void Verify(string verifyString)
		{
			var verifyBytes = Encoding.ASCII.GetBytes(verifyString);
			for (var i = 0; i < verifyBytes.Length; i++)
			{
				if (bytes[i + offset] != verifyBytes[i])
				{
					throw new Exception("Unable to verify file has M.K. header");
				}
			}
			offset += 4;
		}
	}
}
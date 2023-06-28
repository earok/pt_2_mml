using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Reflection.Metadata;
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
		/// The valid protracker periods. Periods outside of this will not be supported. Source: https://github.com/jfdelnero/HxCModPlayer/blob/master/hxcmod.c
		/// </summary>
		internal static int[] PeriodTable =
{
	// Finetune 0 (* 1.000000), Offset 0x0000
	27392, 25856, 24384, 23040, 21696, 20480, 19328, 18240, 17216, 16256, 15360, 14496,
	13696, 12928, 12192, 11520, 10848, 10240,  9664,  9120,  8606,  8128,  7680,  7248,
	 6848,  6464,  6096,  5760,  5424,  5120,  4832,  4560,  4304,  4064,  3840,  3624,
	 3424,  3232,  3048,  2880,  2712,  2560,  2416,  2280,  2152,  2032,  1920,  1812,
	 1712,  1616,  1524,  1440,  1356,  1280,  1208,  1140,  1076,  1016,   960,   906,
	  856,   808,   762,   720,   678,   640,   604,   570,   538,   508,   480,   453,
	  428,   404,   381,   360,   339,   320,   302,   285,   269,   254,   240,   226,
	  214,   202,   190,   180,   170,   160,   151,   143,   135,   127,   120,   113,
	  107,   101,    95,    90,    85,    80,    75,    71,    67,    63,    60,    56,
	   53,    50,    47,    45,    42,    40,    37,    35,    33,    31,    30,    28,
	   27,    25,    24,    22,    21,    20,    19,    18,    17,    16,    15,    14,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune 1 (* 0.992806), Offset 0x0120
	27195, 25670, 24209, 22874, 21540, 20333, 19189, 18109, 17092, 16139, 15249, 14392,
	13597, 12835, 12104, 11437, 10770, 10166,  9594,  9054,  8544,  8070,  7625,  7196,
	 6799,  6417,  6052,  5719,  5385,  5083,  4797,  4527,  4273,  4035,  3812,  3598,
	 3399,  3209,  3026,  2859,  2692,  2542,  2399,  2264,  2137,  2017,  1906,  1799,
	 1700,  1604,  1513,  1430,  1346,  1271,  1199,  1132,  1068,  1009,   953,   899,
	  850,   802,   757,   715,   673,   635,   600,   566,   534,   504,   477,   450,
	  425,   401,   378,   357,   337,   318,   300,   283,   267,   252,   238,   224,
	  212,   201,   189,   179,   169,   159,   150,   142,   134,   126,   119,   112,
	  106,   100,    94,    89,    84,    79,    74,    70,    67,    63,    60,    56,
	   53,    50,    47,    45,    42,    40,    37,    35,    33,    31,    30,    28,
	   27,    25,    24,    22,    21,    20,    19,    18,    17,    16,    15,    14,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune 2 (* 0.985663), Offset 0x0240
	26999, 25485, 24034, 22710, 21385, 20186, 19051, 17978, 16969, 16023, 15140, 14288,
	13500, 12743, 12017, 11355, 10692, 10093,  9525,  8989,  8483,  8011,  7570,  7144,
	 6750,  6371,  6009,  5677,  5346,  5047,  4763,  4495,  4242,  4006,  3785,  3572,
	 3375,  3186,  3004,  2839,  2673,  2523,  2381,  2247,  2121,  2003,  1892,  1786,
	 1687,  1593,  1502,  1419,  1337,  1262,  1191,  1124,  1061,  1001,   946,   893,
	  844,   796,   751,   710,   668,   631,   595,   562,   530,   501,   473,   447,
	  422,   398,   376,   355,   334,   315,   298,   281,   265,   250,   237,   223,
	  211,   199,   187,   177,   168,   158,   149,   141,   133,   125,   118,   111,
	  105,   100,    94,    89,    84,    79,    74,    70,    66,    62,    59,    55,
	   52,    49,    46,    44,    41,    39,    36,    34,    33,    31,    30,    28,
	   27,    25,    24,    22,    21,    20,    19,    18,    17,    16,    15,    14,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune 3 (* 0.978572), Offset 0x0360
	26805, 25302, 23862, 22546, 21231, 20041, 18914, 17849, 16847, 15908, 15031, 14185,
	13403, 12651, 11931, 11273, 10616, 10021,  9457,  8925,  8422,  7954,  7515,  7093,
	 6701,  6325,  5965,  5637,  5308,  5010,  4728,  4462,  4212,  3977,  3758,  3546,
	 3351,  3163,  2983,  2818,  2654,  2505,  2364,  2231,  2106,  1988,  1879,  1773,
	 1675,  1581,  1491,  1409,  1327,  1253,  1182,  1116,  1053,   994,   939,   887,
	  838,   791,   746,   705,   663,   626,   591,   558,   526,   497,   470,   443,
	  419,   395,   373,   352,   332,   313,   296,   279,   263,   249,   235,   221,
	  209,   198,   186,   176,   166,   157,   148,   140,   132,   124,   117,   111,
	  105,    99,    93,    88,    83,    78,    73,    69,    66,    62,    59,    55,
	   52,    49,    46,    44,    41,    39,    36,    34,    32,    30,    29,    27,
	   26,    24,    23,    22,    21,    20,    19,    18,    17,    16,    15,    14,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune 4 (* 0.971532), Offset 0x0480
	26612, 25120, 23690, 22384, 21078, 19897, 18778, 17721, 16726, 15793, 14923, 14083,
	13306, 12560, 11845, 11192, 10539,  9948,  9389,  8860,  8361,  7897,  7461,  7042,
	 6653,  6280,  5922,  5596,  5270,  4974,  4694,  4430,  4181,  3948,  3731,  3521,
	 3327,  3140,  2961,  2798,  2635,  2487,  2347,  2215,  2091,  1974,  1865,  1760,
	 1663,  1570,  1481,  1399,  1317,  1244,  1174,  1108,  1045,   987,   933,   880,
	  832,   785,   740,   700,   659,   622,   587,   554,   523,   494,   466,   440,
	  416,   392,   370,   350,   329,   311,   293,   277,   261,   247,   233,   220,
	  208,   196,   185,   175,   165,   155,   147,   139,   131,   123,   117,   110,
	  104,    98,    92,    87,    83,    78,    73,    69,    65,    61,    58,    54,
	   51,    49,    46,    44,    41,    39,    36,    34,    32,    30,    29,    27,
	   26,    24,    23,    21,    20,    19,    18,    17,    17,    16,    15,    14,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune 5 (* 0.964542), Offset 0x05a0
	26421, 24939, 23519, 22223, 20927, 19754, 18643, 17593, 16606, 15680, 14815, 13982,
	13210, 12470, 11760, 11112, 10463,  9877,  9321,  8797,  8301,  7840,  7408,  6991,
	 6605,  6235,  5880,  5556,  5232,  4938,  4661,  4398,  4151,  3920,  3704,  3496,
	 3303,  3117,  2940,  2778,  2616,  2469,  2330,  2199,  2076,  1960,  1852,  1748,
	 1651,  1559,  1470,  1389,  1308,  1235,  1165,  1100,  1038,   980,   926,   874,
	  826,   779,   735,   694,   654,   617,   583,   550,   519,   490,   463,   437,
	  413,   390,   367,   347,   327,   309,   291,   275,   259,   245,   231,   218,
	  206,   195,   183,   174,   164,   154,   146,   138,   130,   122,   116,   109,
	  103,    97,    92,    87,    82,    77,    72,    68,    65,    61,    58,    54,
	   51,    48,    45,    43,    41,    39,    36,    34,    32,    30,    29,    27,
	   26,    24,    23,    21,    20,    19,    18,    17,    16,    15,    14,    14,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune 6 (* 0.957603), Offset 0x06c0
	26231, 24760, 23350, 22063, 20776, 19612, 18509, 17467, 16486, 15567, 14709, 13881,
	13115, 12380, 11675, 11032, 10388,  9806,  9254,  8733,  8241,  7783,  7354,  6941,
	 6558,  6190,  5838,  5516,  5194,  4903,  4627,  4367,  4122,  3892,  3677,  3470,
	 3279,  3095,  2919,  2758,  2597,  2451,  2314,  2183,  2061,  1946,  1839,  1735,
	 1639,  1547,  1459,  1379,  1299,  1226,  1157,  1092,  1030,   973,   919,   868,
	  820,   774,   730,   689,   649,   613,   578,   546,   515,   486,   460,   434,
	  410,   387,   365,   345,   325,   306,   289,   273,   258,   243,   230,   216,
	  205,   193,   182,   172,   163,   153,   145,   137,   129,   122,   115,   108,
	  102,    97,    91,    86,    81,    77,    72,    68,    64,    60,    57,    54,
	   51,    48,    45,    43,    40,    38,    35,    34,    32,    30,    29,    27,
	   26,    24,    23,    21,    20,    19,    18,    17,    16,    15,    14,    13,
	   12,    12,    11,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune 7 (* 0.950714), Offset 0x07e0
	26042, 24582, 23182, 21904, 20627, 19471, 18375, 17341, 16367, 15455, 14603, 13782,
	13021, 12291, 11591, 10952, 10313,  9735,  9188,  8671,  8182,  7727,  7301,  6891,
	 6510,  6145,  5796,  5476,  5157,  4868,  4594,  4335,  4092,  3864,  3651,  3445,
	 3255,  3073,  2898,  2738,  2578,  2434,  2297,  2168,  2046,  1932,  1825,  1723,
	 1628,  1536,  1449,  1369,  1289,  1217,  1148,  1084,  1023,   966,   913,   861,
	  814,   768,   724,   685,   645,   608,   574,   542,   511,   483,   456,   431,
	  407,   384,   362,   342,   322,   304,   287,   271,   256,   241,   228,   215,
	  203,   192,   181,   171,   162,   152,   144,   136,   128,   121,   114,   107,
	  102,    96,    90,    86,    81,    76,    71,    68,    64,    60,    57,    53,
	   50,    48,    45,    43,    40,    38,    35,    33,    31,    29,    29,    27,
	   26,    24,    23,    21,    20,    19,    18,    17,    16,    15,    14,    13,
	   12,    12,    11,    10,    10,    10,     9,     9,     8,     8,     7,     7,

	// Finetune -8 (* 1.059463), Offset 0x0900
	29021, 27393, 25834, 24410, 22986, 21698, 20477, 19325, 18240, 17223, 16273, 15358,
	14510, 13697, 12917, 12205, 11493, 10849, 10239,  9662,  9118,  8611,  8137,  7679,
	 7255,  6848,  6458,  6103,  5747,  5424,  5119,  4831,  4560,  4306,  4068,  3839,
	 3628,  3424,  3229,  3051,  2873,  2712,  2560,  2416,  2280,  2153,  2034,  1920,
	 1814,  1712,  1615,  1526,  1437,  1356,  1280,  1208,  1140,  1076,  1017,   960,
	  907,   856,   807,   763,   718,   678,   640,   604,   570,   538,   509,   480,
	  453,   428,   404,   381,   359,   339,   320,   302,   285,   269,   254,   239,
	  227,   214,   201,   191,   180,   170,   160,   152,   143,   135,   127,   120,
	  113,   107,   101,    95,    90,    85,    79,    75,    71,    67,    64,    59,
	   56,    53,    50,    48,    44,    42,    39,    37,    35,    33,    32,    30,
	   29,    26,    25,    23,    22,    21,    20,    19,    18,    17,    16,    15,
	   14,    14,    13,    12,    12,    11,    10,    10,     8,     8,     7,     7,

	// Finetune -7 (* 1.051841), Offset 0x0a20
	28812, 27196, 25648, 24234, 22821, 21542, 20330, 19186, 18108, 17099, 16156, 15247,
	14406, 13598, 12824, 12117, 11410, 10771, 10165,  9593,  9052,  8549,  8078,  7624,
	 7203,  6799,  6412,  6059,  5705,  5385,  5082,  4796,  4527,  4275,  4039,  3812,
	 3602,  3400,  3206,  3029,  2853,  2693,  2541,  2398,  2264,  2137,  2020,  1906,
	 1801,  1700,  1603,  1515,  1426,  1346,  1271,  1199,  1132,  1069,  1010,   953,
	  900,   850,   802,   757,   713,   673,   635,   600,   566,   534,   505,   476,
	  450,   425,   401,   379,   357,   337,   318,   300,   283,   267,   252,   238,
	  225,   212,   200,   189,   179,   168,   159,   150,   142,   134,   126,   119,
	  113,   106,   100,    95,    89,    84,    79,    75,    70,    66,    63,    59,
	   56,    53,    49,    47,    44,    42,    39,    37,    35,    33,    32,    29,
	   28,    26,    25,    23,    22,    21,    20,    19,    18,    17,    16,    15,
	   14,    14,    13,    12,    12,    11,     9,     9,     8,     8,     7,     7,

	// Finetune -6 (* 1.044274), Offset 0x0b40
	28605, 27001, 25464, 24060, 22657, 21387, 20184, 19048, 17978, 16976, 16040, 15138,
	14302, 13500, 12732, 12030, 11328, 10693, 10092,  9524,  8987,  8488,  8020,  7569,
	 7151,  6750,  6366,  6015,  5664,  5347,  5046,  4762,  4495,  4244,  4010,  3784,
	 3576,  3375,  3183,  3008,  2832,  2673,  2523,  2381,  2247,  2122,  2005,  1892,
	 1788,  1688,  1591,  1504,  1416,  1337,  1261,  1190,  1124,  1061,  1003,   946,
	  894,   844,   796,   752,   708,   668,   631,   595,   562,   530,   501,   473,
	  447,   422,   398,   376,   354,   334,   315,   298,   281,   265,   251,   236,
	  223,   211,   198,   188,   178,   167,   158,   149,   141,   133,   125,   118,
	  112,   105,    99,    94,    89,    84,    78,    74,    70,    66,    63,    58,
	   55,    52,    49,    47,    44,    42,    39,    37,    34,    32,    31,    29,
	   28,    26,    25,    23,    22,    21,    20,    19,    18,    17,    16,    15,
	   14,    14,    13,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune -5 (* 1.036761), Offset 0x0c60
	28399, 26806, 25280, 23887, 22494, 21233, 20039, 18911, 17849, 16854, 15925, 15029,
	14199, 13403, 12640, 11943, 11247, 10616, 10019,  9455,  8922,  8427,  7962,  7514,
	 7100,  6702,  6320,  5972,  5623,  5308,  5010,  4728,  4462,  4213,  3981,  3757,
	 3550,  3351,  3160,  2986,  2812,  2654,  2505,  2364,  2231,  2107,  1991,  1879,
	 1775,  1675,  1580,  1493,  1406,  1327,  1252,  1182,  1116,  1053,   995,   939,
	  887,   838,   790,   746,   703,   664,   626,   591,   558,   527,   498,   470,
	  444,   419,   395,   373,   351,   332,   313,   295,   279,   263,   249,   234,
	  222,   209,   197,   187,   176,   166,   157,   148,   140,   132,   124,   117,
	  111,   105,    98,    93,    88,    83,    78,    74,    69,    65,    62,    58,
	   55,    52,    49,    47,    44,    41,    38,    36,    34,    32,    31,    29,
	   28,    26,    25,    23,    22,    21,    20,    19,    18,    17,    16,    15,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune -4 (* 1.029302), Offset 0x0d80
	28195, 26614, 25099, 23715, 22332, 21080, 19894, 18774, 17720, 16732, 15810, 14921,
	14097, 13307, 12549, 11858, 11166, 10540,  9947,  9387,  8858,  8366,  7905,  7460,
	 7049,  6653,  6275,  5929,  5583,  5270,  4974,  4694,  4430,  4183,  3953,  3730,
	 3524,  3327,  3137,  2964,  2791,  2635,  2487,  2347,  2215,  2092,  1976,  1865,
	 1762,  1663,  1569,  1482,  1396,  1318,  1243,  1173,  1108,  1046,   988,   933,
	  881,   832,   784,   741,   698,   659,   622,   587,   554,   523,   494,   466,
	  441,   416,   392,   371,   349,   329,   311,   293,   277,   261,   247,   233,
	  220,   208,   196,   185,   175,   165,   155,   147,   139,   131,   124,   116,
	  110,   104,    98,    93,    87,    82,    77,    73,    69,    65,    62,    58,
	   55,    51,    48,    46,    43,    41,    38,    36,    34,    32,    31,    29,
	   28,    26,    25,    23,    22,    21,    20,    19,    17,    16,    15,    14,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune -3 (* 1.021897), Offset 0x0ea0
	27992, 26422, 24918, 23545, 22171, 20928, 19751, 18639, 17593, 16612, 15696, 14813,
	13996, 13211, 12459, 11772, 11086, 10464,  9876,  9320,  8794,  8306,  7848,  7407,
	 6998,  6606,  6229,  5886,  5543,  5232,  4938,  4660,  4398,  4153,  3924,  3703,
	 3499,  3303,  3115,  2943,  2771,  2616,  2469,  2330,  2199,  2076,  1962,  1852,
	 1749,  1651,  1557,  1472,  1386,  1308,  1234,  1165,  1100,  1038,   981,   926,
	  875,   826,   779,   736,   693,   654,   617,   582,   550,   519,   491,   463,
	  437,   413,   389,   368,   346,   327,   309,   291,   275,   260,   245,   231,
	  219,   206,   194,   184,   174,   164,   154,   146,   138,   130,   123,   115,
	  109,   103,    97,    92,    87,    82,    77,    73,    68,    64,    61,    57,
	   54,    51,    48,    46,    43,    41,    38,    36,    34,    32,    31,    29,
	   28,    26,    25,    22,    21,    20,    19,    18,    17,    16,    15,    14,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune -2 (* 1.014545), Offset 0x0fc0
	27790, 26232, 24739, 23375, 22012, 20778, 19609, 18505, 17466, 16492, 15583, 14707,
	13895, 13116, 12369, 11688, 11006, 10389,  9805,  9253,  8731,  8246,  7792,  7353,
	 6948,  6558,  6185,  5844,  5503,  5194,  4902,  4626,  4367,  4123,  3896,  3677,
	 3474,  3279,  3092,  2922,  2751,  2597,  2451,  2313,  2183,  2062,  1948,  1838,
	 1737,  1640,  1546,  1461,  1376,  1299,  1226,  1157,  1092,  1031,   974,   919,
	  868,   820,   773,   730,   688,   649,   613,   578,   546,   515,   487,   460,
	  434,   410,   387,   365,   344,   325,   306,   289,   273,   258,   243,   229,
	  217,   205,   193,   183,   172,   162,   153,   145,   137,   129,   122,   115,
	  109,   102,    96,    91,    86,    81,    76,    72,    68,    64,    61,    57,
	   54,    51,    48,    46,    43,    41,    38,    36,    33,    31,    30,    28,
	   27,    25,    24,    22,    21,    20,    19,    18,    17,    16,    15,    14,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7,

	// Finetune -1 (* 1.007246), Offset 0x10e0
	27590, 26043, 24561, 23207, 21853, 20628, 19468, 18372, 17341, 16374, 15471, 14601,
	13795, 13022, 12280, 11603, 10927, 10314,  9734,  9186,  8668,  8187,  7736,  7301,
	 6898,  6511,  6140,  5802,  5463,  5157,  4867,  4593,  4335,  4093,  3868,  3650,
	 3449,  3255,  3070,  2901,  2732,  2579,  2434,  2297,  2168,  2047,  1934,  1825,
	 1724,  1628,  1535,  1450,  1366,  1289,  1217,  1148,  1084,  1023,   967,   913,
	  862,   814,   768,   725,   683,   645,   608,   574,   542,   512,   483,   456,
	  431,   407,   384,   363,   341,   322,   304,   287,   271,   256,   242,   228,
	  216,   203,   191,   181,   171,   161,   152,   144,   136,   128,   121,   114,
	  108,   102,    96,    91,    86,    81,    76,    72,    67,    63,    60,    56,
	   53,    50,    47,    45,    42,    40,    37,    35,    33,    31,    30,    28,
	   27,    25,    24,    22,    21,    20,    19,    18,    17,    16,    15,    14,
	   13,    13,    12,    11,    11,    10,     9,     9,     8,     8,     7,     7
};

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
		internal static List<ChannelData> ChannelData = new List<ChannelData>();

		/// <summary>
		/// PCM samples are played back at multiples of this in MDSDRV
		/// </summary>
		internal static int OutHz = 2190;

		/// <summary>
		/// The maximum amount we can multiply
		/// </summary>
		const int MaxHzMultiplier = 6; //8760

		/// <summary>
		/// The maximum WAV size that MDSDRV supported
		/// </summary>
		const int MaxWavSize = 32 * 1024; //32768

		/// <summary>
		/// A repeating sound should play for this many seconds
		/// </summary>
		const float MaxRepeatSeconds = 1;

		/// <summary>
		/// 50 ticks = pal, 60 ticks = NTSC
		/// </summary>
		public static int TicksPerFrame = 50;

		/// <summary>
		/// A list of all of the samples that are actually needed, along with the required 
		/// </summary>
		internal static List<UsedSample> UsedSamples = new();

		/// <summary>
		/// A list of all Macros we need to dynamically generate to support ProTracker effects
		/// </summary>
		internal static List<Macro> Macros = new();

		/// <summary>
		/// Number of channels in this mod
		/// </summary>
		internal static int Channels;

		static void Main(string[] args)
		{
			try
			{

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
							Name = (i + 1) + "_" + ReadString(22),
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

					var unsupportedEffects = new HashSet<string>();

					Verify();

					for (var i = 0; i < Channels; i++)
					{
						ChannelData.Add(new ChannelData(i));
					}

					//Rip every pattern
					for (var patternId = 0; patternId < patternCount; patternId++)
					{
						var pattern = patterns[patternId] = new PatternData();

						//For every row...
						for (var rowId = 0; rowId < 64; rowId++)
						{
							var row = patterns[patternId].RowData[rowId] = new RowData(Channels);

							//Set the defaults
							if (rowId == 0)
							{
								if (patternId == 0)
								{
									pattern.LastTicksPerRow = row.TicksPerRow = 6;
								}
								else
								{
									pattern.LastTicksPerRow = row.TicksPerRow = patterns[patternId - 1].LastTicksPerRow;
								}
							}


							//For every channel...
							for (var channelId = 0; channelId < Channels; channelId++)
							{
								var b1 = ReadByte();
								var b2 = ReadByte();
								var b3 = ReadByte();
								var b4 = ReadByte();

								var noteData = row.NoteData[channelId] = new NoteData()
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

									case 0x9: //Bytes offset from start
										noteData.Offset = 0x100 * noteData.EffectValue;
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

									case 0xe: //Various
										switch (noteData.EffectValue >> 4)
										{
											case 0x6: //Loop point
												var loops = noteData.EffectValue & 0xf;
												if (loops == 0)
												{
													row.IsLoopRow = true;
												}
												else
												{
													row.LoopRepeats = loops;
												}
												effectSupported = true;
												break;

											case 0xa: //Effect slide up
												noteData.VolumeAdjust = noteData.EffectValue & 0xf;
												effectSupported = true;
												break;

											case 0xb: //Effect slide down
												noteData.VolumeAdjust = -(noteData.EffectValue & 0xf);
												effectSupported = true;
												break;

											case 0x9: //Loop repeat
												noteData.RepeatEveryTick = noteData.EffectValue & 0xf;
												effectSupported = true;
												break;
										}
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
											pattern.LastTicksPerRow = row.TicksPerRow = noteData.EffectValue;
										}
										else
										{
											pattern.LastTempo = row.Tempo = noteData.EffectValue;
										}
										break;

								}

								if (effectSupported == false)
								{
									unsupportedEffects.Add(noteData.EffectType.ToString("X") + noteData.EffectValue.ToString("X2"));
								}

								//Is this a valid note?
								if (noteData.Period > 0 && Array.IndexOf(PeriodTable, noteData.Period) == -1)
								{
									throw new Exception(string.Format("Invalid period value: {0} in pattern {1} at row {2} and channel {3}", noteData.Period, patternId, rowId, channelId));
								}


							}
						}
					}

					//Second pass - make sure the loops owed are set on every channel
					foreach (var pattern in patterns)
					{
						if (pattern == null)
						{
							continue;
						}

						foreach (var row in pattern.RowData)
						{
							if (row.LoopRepeats > 0)
							{
								foreach (var note in row.NoteData)
								{
									note.LoopsOwed = row.LoopRepeats;
								}
							}
						}
					}

					//Find the loop point
					var position = 0;
					var loopPosition = 0;
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

						//We've come to the natural end of the song
						if (position >= songLength)
						{
							lastPosition = position;
							break;
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
							sb.Append("; ");
							foreach (var note in patterns[i].RowData[j].NoteData)
							{
								sb.Append(note.ToString() + " ");
							}
							sb.AppendLine(string.Empty);

						}
						sb.AppendLine(string.Empty);
					}


					position = 0;
					loopedPositions.Clear();


					var mdChannels = new string[] { "F", "K", "L" };
					Console.WriteLine("Map Mega Drive / Genesis Channels to Protracker channel # (1-" + Channels + ", leave empty to ignore)");
					foreach (var mdChannel in mdChannels)
					{
						Console.Write("Channel " + mdChannel + ": ");
						if (int.TryParse(Console.ReadLine() ?? "", out int ptChannel))
						{
							ChannelData[Math.Clamp(ptChannel - 1, 0, Channels - 1)].Map = mdChannel;
						}
					}

					Console.WriteLine("Do you want to adjust timings for NTSC? y/n");
					var isNTSC = Console.ReadLine();
					if (string.IsNullOrEmpty(isNTSC) == false && isNTSC.Trim().ToLower().StartsWith("y"))
					{
						TicksPerFrame = 60;
					}

					while (true)
					{
						loopedPositions.Add(position);
						var patternId = positions[position];

						var pattern = patterns[patternId];
						sb.AppendLine(string.Format("; Position {0} Pattern {1}", position, patternId));

						foreach (var channel in ChannelData)
						{
							var tempoMode = false;
							var ticksPerRow = pattern.RowData[0].TicksPerRow;

							if (string.IsNullOrWhiteSpace(channel.Map))
							{
								continue;
							}

							sb.Append(channel.Map + " ");

							//Set the loop position
							if (position == loopPosition)
							{
								sb.Append("L ");
							}

							var rowId = 0;
							var loopId = 0;
							while (rowId < 64)
							{
								var row = pattern.RowData[rowId];



								//Changed speed?
								if (row.TicksPerRow != 0)
								{
									ticksPerRow = (row.TicksPerRow * TicksPerFrame) / 50;
									tempoMode = false;
								}

								//If we're in tempo mode as opposed to tick mode
								if (row.Tempo != 0)
								{
									sb.Append("l16 t" + row.Tempo + " ");
									ticksPerRow = (int)Math.Ceiling(((TicksPerFrame * 60) / 4) / (decimal)row.Tempo);
									tempoMode = true;
								}

								//Is this a loop point?
								if (row.IsLoopRow)
								{
									loopId = rowId;
								}

								var note = row.NoteData[channel.Id];

								//Set to absolute volume
								if (note.Volume > -1)
								{
									channel.LastVolume = note.Volume;
									sb.Append("V" + FixVolume(note.Volume) + " ");
								}
								//Set to relative volume
								else if (note.VolumeAdjust != 0)
								{
									channel.LastVolume = Math.Clamp(channel.LastVolume + note.VolumeAdjust, 0, 64);
									sb.Append("V" + FixVolume(channel.LastVolume) + " ");
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
									var macroId = GetVolumeMacro(channel.LastVolume, note.VolumeSlide, ticksPerRow);
									sb.Append("P" + macroId + " ");
									channel.LastVolume = Math.Clamp(channel.LastVolume + note.VolumeSlide * ticksPerRow, 0, 64);
								}

								if (note.Sample > 0 && note.Period > 0)
								{
									//Are we repeating this in the same frame?
									if (note.RepeatEveryTick != 0)
									{
										var macroId = GetRepeatMacro(note.RepeatEveryTick, ticksPerRow);
										sb.Append("P" + macroId + " ");
									}

									var instrument = GetUsedSample(note.Period, note.Sample, note.Offset);

									sb.Append("@" + instrument * 2);
									if (tempoMode)
									{
										sb.Append(note.OutputNote + " ");
									}
									else
									{
										sb.Append(note.OutputNote + ":" + ticksPerRow + " ");
									}

									//Are we repeating an instrument?
									if (samples[note.Sample].RepeatLength > 1)
									{
										channel.RepeatInstrument = instrument * 2 + 1;
										channel.RepeatTicks = (int)Math.Ceiling(MaxRepeatSeconds * TicksPerFrame);
									}
									else
									{
										//Don't repeat
										channel.RepeatInstrument = -1;
									}
								}
								else if (channel.RepeatInstrument >= 0 && channel.RepeatTicks <= 0)
								{
									//We're repeating this instrument
									channel.RepeatTicks = (int)Math.Ceiling(MaxRepeatSeconds * TicksPerFrame);
									sb.Append("@" + channel.RepeatInstrument);
									if (tempoMode)
									{
										sb.Append("c ");
									}
									else
									{
										sb.Append("c:" + ticksPerRow + " ");
									}
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
								}

								channel.RepeatTicks -= ticksPerRow;


								//End of a loop
								if (note.LoopsOwed > 0)
								{
									note.LoopsOwed -= 1;
									rowId = loopId;
								}
								else
								{
									rowId++;
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

						//We've come to the natural end of the song
						if (position >= songLength)
						{
							break;
						}
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
						var frequency = GetFrequency(usedSample.Period, sample.FineTune);

						//Write our base sample
						WriteSamples(usedSample, sample, frequency, false);
						sb.AppendLine(string.Format("@{0} pcm \"{1}\" ;Length={2} FT={3} VOL={4} RP={5} RL={6} NOTE={7} ORIGINAL={8} OFFSET={9}", usedSample.Id * 2, usedSample.CorrectName(sample, false), sample.Length, sample.FineTune, sample.Volume, sample.RepeatPoint, sample.RepeatLength, usedSample.Note, sample.Id, usedSample.Offset));

						//Write our repeat sample
						if (sample.RepeatLength > 1)
						{
							WriteSamples(usedSample, sample, frequency, true);
							sb.AppendLine(string.Format("@{0} pcm \"{1}\" ;Length={2} FT={3} VOL={4} RP={5} RL={6} NOTE={7} ORIGINAL={8} (REPEAT) OFFSET={9}", usedSample.Id * 2 + 1, usedSample.CorrectName(sample, true), sample.Length, sample.FineTune, sample.Volume, sample.RepeatPoint, sample.RepeatLength, usedSample.Note, sample.Id, usedSample.Offset));
						}

					}

					sb.AppendLine(string.Empty);
					sb.AppendLine(";==== Autogen Macros ====");
					foreach (var macro in Macros)
					{
						sb.Append(string.Format("*{0} 'carry'", macro.ID));

						if (macro.VolumeSlide != 0)
						{
							//Fix the volume for every frame
							var volume = macro.BaseVolume;

							//Insert a rest of 1 before actually making the volume change
							for (var i = 0; i < macro.TicksPerRow; i++)
							{
								volume = Math.Clamp(volume + macro.VolumeSlide, 0, 64);
								sb.Append(string.Format(" r:1 V{0}", FixVolume(volume)));
							}

						}
						else if (macro.RepeatEveryTick != 0)
						{
							var tickOffset = macro.RepeatEveryTick;
							while (tickOffset < macro.TicksPerRow)
							{
								sb.Append(string.Format(" c:{0}", macro.RepeatEveryTick));
								tickOffset += macro.RepeatEveryTick;
							}
						}

						sb.AppendLine(string.Empty);
					}

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


		private static void WriteSamples(UsedSample usedSample, SampleData sample, int frequency, bool repeatMode)
		{
			//Make the SOURCE wav
			var waveFormat = new WaveFormat(frequency, 8, 1);

			var start = sample.Start + usedSample.Offset;
			var length = sample.Length * 2 - usedSample.Offset;


			//If repeating, make sure that the rip is at least as long as the sample is needed.
			int minSamples;
			if (sample.RepeatLength > 1)
			{
				minSamples = (int)Math.Ceiling(frequency * MaxRepeatSeconds * 50 / (double)TicksPerFrame);
			}
			else
			{
				minSamples = length;
			}

			//Is there a way to do this without writing a file?
			var totalLength = 0;
			using (var writer = new WaveFileWriter("temp.wav", waveFormat))
			{
				//Only need to write from the start if we're not writing the repeat samples
				if (repeatMode == false)
				{
					writer.Write(bytes, start, length);
				}

				while (writer.Length < minSamples)
				{
					//Pad out the repeat length
					writer.Write(bytes, sample.Start + sample.RepeatPoint * 2, sample.RepeatLength * 2);
				}

				totalLength = (int)writer.Length;
			}

			var timeInSeconds = totalLength / (double)frequency;

			using (var reader = new WaveFileReader("temp.wav"))
			{
				var multiplier = MaxHzMultiplier;

				//Get the highest quality conversion that doesn't exceed 32kb, go all of the way down to the minimum frequency if a sample is just too big
				while (multiplier > 2 && (OutHz * multiplier * timeInSeconds) > MaxWavSize)
				{
					multiplier--;
				}

				var outFormat = new WaveFormat(OutHz * multiplier, 8, reader.WaveFormat.Channels);
				using (var resampler = new MediaFoundationResampler(reader, outFormat))
				{
					WaveFileWriter.CreateWaveFile("output/" + usedSample.CorrectName(sample, repeatMode), resampler);
				}
			}
		}


		private static int GetRepeatMacro(int repeatEveryTick, int ticksPerRow)
		{
			var index = Macros.FindIndex(p => p.RepeatEveryTick == repeatEveryTick && p.TicksPerRow == ticksPerRow);
			if (index > -1)
			{
				return index + 100;
			}
			var id = Macros.Count + 100;
			Macros.Add(new Macro() { RepeatEveryTick = repeatEveryTick, TicksPerRow = ticksPerRow, ID = id });
			return id;
		}

		private static int GetVolumeMacro(int baseVolume, int volumeSlide, int ticksPerRow)
		{
			var index = Macros.FindIndex(p => p.VolumeSlide == volumeSlide && p.BaseVolume == baseVolume && p.TicksPerRow == ticksPerRow);
			if (index > -1)
			{
				return index + 100;
			}

			var id = Macros.Count + 100;
			Macros.Add(new Macro() { BaseVolume = baseVolume, VolumeSlide = volumeSlide, ID = id, TicksPerRow = ticksPerRow });
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

		private static int GetUsedSample(int period, int sample, int offset)
		{
			var note = "NONE";
			var octave = 1;

			if (sample > 0)
			{
				note = FileNameTable[(Array.IndexOf(PeriodTable, period) % FileNameTable.Length)];
				octave += (Array.IndexOf(PeriodTable, period)) / FileNameTable.Length;
			}

			var index = UsedSamples.FindIndex(p => p.Period == period && p.Sample == sample && p.Offset == offset);
			if (index > -1)
			{
				return index + 1;
			}

			UsedSamples.Add(new UsedSample(UsedSamples.Count + 1, octave, note, offset) { Period = period, Sample = sample });
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

		/// <summary>
		/// Verify there's a supported header and change settings accordingly
		/// </summary>
		private static void Verify()
		{
			var header = Encoding.ASCII.GetString(bytes[offset..(offset + 4)]);
			switch (header)
			{
				case "3CHN":
					Channels = 3;
					break;

				case "M.K.":
					Channels = 4;
					break;

				default:
					throw new Exception("Unsupported header: " + header);
			}
			offset += 4;
		}

		private static int GetFrequency(int period, int fineTune)
		{
			//Get the fine tuned period
			var lookup = Array.IndexOf(PeriodTable, period);
			var finalPeriod = PeriodTable[fineTune * (12 * 12) + lookup];
			return (int)Math.Round(7093789.2 / (finalPeriod * 2));
		}
	}
}
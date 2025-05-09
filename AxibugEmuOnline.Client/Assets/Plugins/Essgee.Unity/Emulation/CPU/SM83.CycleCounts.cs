﻿namespace Essgee.Emulation.CPU
{
    public partial class SM83
    {
        public static class CycleCounts
        {
            public const int AdditionalJumpCond8Taken = 4;
            public const int AdditionalRetCondTaken = 12;
            public const int AdditionalCallCondTaken = 12;

            // 32 cycles == dummy
            public static readonly int[] NoPrefix = new int[]
            {
                4,  12, 8,  8,  4,  4,  8,  4,      20, 8,  8,  8,  4,  4,  8,  4,  /* 0x00 - 0x0F */
				4,  12, 8,  8,  4,  4,  8,  4,      12, 8,  8,  8,  4,  4,  8,  4,  /* 0x10 - 0x1F */
				8,  12, 8,  8,  4,  4,  8,  4,      8,  8,  8,  8,  4,  4,  8,  4,  /* 0x20 - 0x2F */
				8,  12, 8,  8,  12, 12, 12, 4,      8,  8,  8,  8,  4,  4,  8,  4,  /* 0x30 - 0x3F */
				4,  4,  4,  4,  4,  4,  8,  4,      4,  4,  4,  4,  4,  4,  8,  4,  /* 0x40 - 0x4F */
				4,  4,  4,  4,  4,  4,  8,  4,      4,  4,  4,  4,  4,  4,  8,  4,  /* 0x50 - 0x5F */
				4,  4,  4,  4,  4,  4,  8,  4,      4,  4,  4,  4,  4,  4,  8,  4,  /* 0x60 - 0x6F */
				8,  8,  8,  8,  8,  8,  4,  8,      4,  4,  4,  4,  4,  4,  8,  4,  /* 0x70 - 0x7F */
				4,  4,  4,  4,  4,  4,  8,  4,      4,  4,  4,  4,  4,  4,  8,  4,  /* 0x80 - 0x8F */
				4,  4,  4,  4,  4,  4,  8,  4,      4,  4,  4,  4,  4,  4,  8,  4,  /* 0x90 - 0x9F */
				4,  4,  4,  4,  4,  4,  8,  4,      4,  4,  4,  4,  4,  4,  8,  4,  /* 0xA0 - 0xAF */
				4,  4,  4,  4,  4,  4,  8,  4,      4,  4,  4,  4,  4,  4,  8,  4,  /* 0xB0 - 0xBF */
				8,  12, 12, 16, 12, 16, 8,  16,     8,  16, 12, 32, 12, 24, 8,  16, /* 0xC0 - 0xCF */
				8,  12, 12, 32, 12, 16, 8,  16,     8,  16, 12, 32, 12, 32, 8,  16, /* 0xD0 - 0xDF */
				12, 12, 8,  32, 32, 16, 8,  16,     16, 4,  16, 32, 32, 32, 8,  16, /* 0xE0 - 0xEF */
				12, 12, 8,  4,  32, 16, 8,  16,     12, 8,  16, 4,  32, 32, 8,  16	/* 0xF0 - 0xFF */
			};

            public static readonly int[] PrefixCB = new int[]
            {
                8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0x00 - 0x0F */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0x10 - 0x1F */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0x20 - 0x2F */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0x30 - 0x3F */
				8,  8,  8,  8,  8,  8,  12, 8,      8,  8,  8,  8,  8,  8,  12, 8,  /* 0x40 - 0x4F */
				8,  8,  8,  8,  8,  8,  12, 8,      8,  8,  8,  8,  8,  8,  12, 8,  /* 0x50 - 0x5F */
				8,  8,  8,  8,  8,  8,  12, 8,      8,  8,  8,  8,  8,  8,  12, 8,  /* 0x60 - 0x6F */
				8,  8,  8,  8,  8,  8,  12, 8,      8,  8,  8,  8,  8,  8,  12, 8,  /* 0x70 - 0x7F */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0x80 - 0x8F */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0x90 - 0x9F */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0xA0 - 0xAF */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0xB0 - 0xBF */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0xC0 - 0xCF */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0xD0 - 0xDF */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8,  /* 0xE0 - 0xEF */
				8,  8,  8,  8,  8,  8,  16, 8,      8,  8,  8,  8,  8,  8,  16, 8   /* 0xF0 - 0xFF */
			};
        }
    }
}

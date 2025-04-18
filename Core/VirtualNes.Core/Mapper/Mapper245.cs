﻿//////////////////////////////////////////////////////////////
// Mapper245  Yong Zhe Dou E Long                                       //
//////////////////////////////////////////////////////////////////////////
using static VirtualNes.Core.CPU;
using static VirtualNes.MMU;
using BYTE = System.Byte;
using INT = System.Int32;

namespace VirtualNes.Core
{
    public class Mapper245 : Mapper
    {

        BYTE[] reg = new byte[8];
        BYTE prg0, prg1;
        BYTE chr01, chr23, chr4, chr5, chr6, chr7;
        BYTE we_sram;

        BYTE irq_type;
        BYTE irq_enable;
        BYTE irq_counter;
        BYTE irq_latch;
        BYTE irq_request;
        int MMC4prg, MMC4chr;

        public Mapper245(NES parent) : base(parent)
        {
        }


        //void Mapper245::Reset()
        public override void Reset()
        {
            for (INT i = 0; i < 8; i++)
            {
                reg[i] = 0x00;
            }

            prg0 = 0;
            prg1 = 1;

            SetPROM_32K_Bank(0, 1, PROM_8K_SIZE - 2, PROM_8K_SIZE - 1);

            if (VROM_1K_SIZE != 0)
            {
                SetVROM_8K_Bank(0);
            }

            we_sram = 0;    // Disable
            irq_enable = 0; // Disable
            irq_counter = 0;
            irq_latch = 0;
            irq_request = 0;

            nes.SetIrqType(NES.IRQMETHOD.IRQ_CLOCK);
        }

        //void Mapper245::Write(WORD addr, BYTE data)
        public override void Write(ushort addr, byte data)
        {
            switch (addr & 0xF7FF)
            {
                case 0x8000:
                    reg[0] = data;
                    break;
                case 0x8001:
                    reg[1] = data;
                    switch (reg[0])
                    {
                        case 0x00:
                            reg[3] = (byte)((data & 2) << 5);
                            SetPROM_8K_Bank(6, 0x3E | reg[3]);
                            SetPROM_8K_Bank(7, 0x3F | reg[3]);
                            break;
                        case 0x06:
                            prg0 = data;
                            break;
                        case 0x07:
                            prg1 = data;
                            break;
                    }
                    SetPROM_8K_Bank(4, prg0 | reg[3]);
                    SetPROM_8K_Bank(5, prg1 | reg[3]);
                    break;
                case 0xA000:
                    reg[2] = data;
                    if (!nes.rom.Is4SCREEN())
                    {
                        if ((data & 0x01) != 0) SetVRAM_Mirror(VRAM_HMIRROR);
                        else SetVRAM_Mirror(VRAM_VMIRROR);
                    }
                    break;
                case 0xA001:

                    break;
                case 0xC000:
                    reg[4] = data;
                    irq_counter = data;
                    irq_request = 0;
                    nes.cpu.ClrIRQ(IRQ_MAPPER);
                    break;
                case 0xC001:
                    reg[5] = data;
                    irq_latch = data;
                    irq_request = 0;
                    nes.cpu.ClrIRQ(IRQ_MAPPER);
                    break;
                case 0xE000:
                    reg[6] = data;
                    irq_enable = 0;
                    irq_request = 0;
                    nes.cpu.ClrIRQ(IRQ_MAPPER);
                    break;
                case 0xE001:
                    reg[7] = data;
                    irq_enable = 1;
                    irq_request = 0;
                    nes.cpu.ClrIRQ(IRQ_MAPPER);
                    break;
            }
        }

        //void Mapper245::Clock(INT cycles)
        public override void Clock(int cycles)
        {
            //	if( irq_request && (nes.GetIrqType() == NES::IRQ_CLOCK) ) {
            //		nes.cpu.IRQ_NotPending();
            //	}
        }

        //void Mapper245::HSync(INT scanline)
        public override void HSync(int scanline)
        {
            if ((scanline >= 0 && scanline <= 239))
            {
                if (nes.ppu.IsDispON())
                {
                    if (irq_enable != 0 && irq_request == 0)
                    {
                        if (scanline == 0)
                        {
                            if (irq_counter != 0)
                            {
                                irq_counter--;
                            }
                        }
                        if ((irq_counter--) == 0)
                        {
                            irq_request = 0xFF;
                            irq_counter = irq_latch;
                            nes.cpu.SetIRQ(IRQ_MAPPER);
                        }
                    }
                }
            }
            //	if( irq_request && (nes.GetIrqType() == NES::IRQ_HSYNC) ) {
            //		nes.cpu.IRQ_NotPending();
            //	}
        }

        void SetBank_CPU()
        {
            SetPROM_32K_Bank(prg0, prg1, PROM_8K_SIZE - 2, PROM_8K_SIZE - 1);
        }

        void SetBank_PPU()
        {
            if (VROM_1K_SIZE != 0)
            {
                if ((reg[0] & 0x80) != 0)
                {
                    SetVROM_8K_Bank(chr4, chr5, chr6, chr7,
                             chr23 + 1, chr23, chr01 + 1, chr01);
                }
                else
                {
                    SetVROM_8K_Bank(chr01, chr01 + 1, chr23, chr23 + 1,
                             chr4, chr5, chr6, chr7);
                }
            }
            else
            {
                if ((reg[0] & 0x80) != 0)
                {
                    SetCRAM_1K_Bank(4, (chr01 + 0) & 0x07);
                    SetCRAM_1K_Bank(5, (chr01 + 1) & 0x07);
                    SetCRAM_1K_Bank(6, (chr23 + 0) & 0x07);
                    SetCRAM_1K_Bank(7, (chr23 + 1) & 0x07);
                    SetCRAM_1K_Bank(0, chr4 & 0x07);
                    SetCRAM_1K_Bank(1, chr5 & 0x07);
                    SetCRAM_1K_Bank(2, chr6 & 0x07);
                    SetCRAM_1K_Bank(3, chr7 & 0x07);
                }
                else
                {
                    SetCRAM_1K_Bank(0, (chr01 + 0) & 0x07);
                    SetCRAM_1K_Bank(1, (chr01 + 1) & 0x07);
                    SetCRAM_1K_Bank(2, (chr23 + 0) & 0x07);
                    SetCRAM_1K_Bank(3, (chr23 + 1) & 0x07);
                    SetCRAM_1K_Bank(4, chr4 & 0x07);
                    SetCRAM_1K_Bank(5, chr5 & 0x07);
                    SetCRAM_1K_Bank(6, chr6 & 0x07);
                    SetCRAM_1K_Bank(7, chr7 & 0x07);
                }
            }
        }

        public override bool IsStateSave()
        {
            return true;
        }


        //void Mapper245::SaveState(LPBYTE p)
        public override void SaveState(byte[] p)
        {
            for (INT i = 0; i < 8; i++)
            {
                p[i] = reg[i];
            }
            p[8] = prg0;
            p[9] = prg1;
            p[10] = chr01;
            p[11] = chr23;
            p[12] = chr4;
            p[13] = chr5;
            p[14] = chr6;
            p[15] = chr7;
            p[16] = irq_enable;
            p[17] = (BYTE)irq_counter;
            p[18] = irq_latch;
            p[19] = irq_request;
        }

        //void Mapper245::LoadState(LPBYTE p)
        public override void LoadState(byte[] p)
        {
            for (INT i = 0; i < 8; i++)
            {
                reg[i] = p[i];
            }
            prg0 = p[8];
            prg1 = p[9];
            chr01 = p[10];
            chr23 = p[11];
            chr4 = p[12];
            chr5 = p[13];
            chr6 = p[14];
            chr7 = p[15];
            irq_enable = p[16];
            irq_counter = p[17];
            irq_latch = p[18];
            irq_request = p[19];
        }
    }
}

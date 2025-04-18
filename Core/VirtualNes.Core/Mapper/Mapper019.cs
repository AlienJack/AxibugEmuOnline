﻿//////////////////////////////////////////////////////////////////////////
// Mapper019  Namcot 106                                                //
//////////////////////////////////////////////////////////////////////////
using System;
using static VirtualNes.Core.CPU;
using static VirtualNes.MMU;
using BYTE = System.Byte;


namespace VirtualNes.Core
{
    public class Mapper019 : Mapper
    {
        BYTE patch;
        BYTE exsound_enable;

        BYTE[] reg = new byte[3];
        BYTE[] exram = new byte[128];

        BYTE irq_enable;
        ushort irq_counter;
        public Mapper019(NES parent) : base(parent)
        {
        }


        public override void Reset()
        {
            patch = 0;

            reg[0] = reg[1] = reg[2] = 0;

            MemoryUtility.ZEROMEMORY(exram, exram.Length);

            irq_enable = 0;
            irq_counter = 0;

            SetPROM_32K_Bank(0, 1, PROM_8K_SIZE - 2, PROM_8K_SIZE - 1);

            if (VROM_1K_SIZE >= 8)
            {
                SetVROM_8K_Bank(VROM_8K_SIZE - 1);
            }

            exsound_enable = 0xFF;

            uint crc = nes.rom.GetPROM_CRC();

            if (crc == 0xb62a7b71)
            {   // Family Circuit '91(J)
                patch = 1;
            }

            if (crc == 0x02738c68)
            {   // Wagan Land 2(J)
                patch = 3;
            }
            if (crc == 0x14942c06)
            {   // Wagan Land 3(J)
                patch = 2;
            }

            if (crc == 0x968dcf09)
            {   // Final Lap(J)
                nes.SetRenderMethod(EnumRenderMethod.PRE_ALL_RENDER);
            }
            if (crc == 0x3deac303)
            {   // Rolling Thunder(J)
                nes.SetRenderMethod(EnumRenderMethod.POST_ALL_RENDER);
            }

            if (crc == 0xb1b9e187)
            {   // For Kaijuu Monogatari(J)
                nes.SetRenderMethod(EnumRenderMethod.POST_ALL_RENDER);
            }

            if (crc == 0x6901346e)
            {   // For Sangokushi 2 - Haou no Tairiku(J)
                nes.SetRenderMethod(EnumRenderMethod.TILE_RENDER);
            }

            //	if( crc == 0xdd454208 ) {	// Hydlide 3(J)
            //		nes.SetRenderMethod( NES::PRE_ALL_RENDER );
            //	}

            if (crc == 0xaf15338f       // For Mindseeker(J)
             || crc == 0xb1b9e187       // For Kaijuu Monogatari(J)
             || crc == 0x96533999       // Dokuganryuu Masamune(J)
                                        //	 || crc == 0x2b825ce1		// Namco Classic(J)
                                        //	 || crc == 0x9a2b0641		// Namco Classic 2(J)
             || crc == 0x3296ff7a       // Battle Fleet(J)
             || crc == 0xdd454208)
            {   // Hydlide 3(J)
                exsound_enable = 0;
            }

            if (crc == 0x429fd177)
            {   // Famista '90(J)
                exsound_enable = 0;
            }

            if (exsound_enable != 0)
            {
                nes.apu.SelectExSound(0x10);
            }
        }

        //BYTE Mapper019::ReadLow(WORD addr)
        public override byte ReadLow(ushort addr)
        {
            BYTE data = 0;

            switch (addr & 0xF800)
            {
                case 0x4800:
                    if (addr == 0x4800)
                    {
                        if (exsound_enable != 0)
                        {
                            nes.apu.ExRead(addr);
                            data = exram[reg[2] & 0x7F];
                        }
                        else
                        {
                            data = WRAM[reg[2] & 0x7F];
                        }
                        if ((reg[2] & 0x80) != 0)
                            reg[2] = (byte)((reg[2] + 1) | 0x80);
                        return data;
                    }
                    break;
                case 0x5000:
                    return (byte)((BYTE)irq_counter & 0x00FF);
                case 0x5800:
                    return (BYTE)((irq_counter >> 8) & 0x7F);
                case 0x6000:
                case 0x6800:
                case 0x7000:
                case 0x7800:
                    return base.ReadLow(addr);
            }

            return (BYTE)(addr >> 8);
        }

        //void Mapper019::WriteLow(WORD addr, BYTE data)
        public override void WriteLow(ushort addr, byte data)
        {
            switch (addr & 0xF800)
            {
                case 0x4800:
                    if (addr == 0x4800)
                    {
                        if (exsound_enable != 0)
                        {
                            nes.apu.ExWrite(addr, data);
                            exram[reg[2] & 0x7F] = data;
                        }
                        else
                        {
                            WRAM[reg[2] & 0x7F] = data;
                        }
                        if ((reg[2] & 0x80) != 0)
                            reg[2] = (byte)((reg[2] + 1) | 0x80);
                    }
                    break;
                case 0x5000:
                    irq_counter = (byte)((irq_counter & 0xFF00) | (ushort)data);
                    //			if( irq_enable ) {
                    //				irq_counter++;
                    //			}
                    nes.cpu.ClrIRQ(IRQ_MAPPER);
                    break;
                case 0x5800:
                    irq_counter = (byte)((irq_counter & 0x00FF) | ((ushort)(data & 0x7F) << 8));
                    irq_enable = (byte)(data & 0x80);
                    //			if( irq_enable ) {
                    //				irq_counter++;
                    //			}
                    //			if( !irq_enable ) {
                    //				nes.cpu.ClrIRQ( IRQ_MAPPER );
                    //			}
                    nes.cpu.ClrIRQ(IRQ_MAPPER);
                    break;
                case 0x6000:
                case 0x6800:
                case 0x7000:
                case 0x7800:
                    base.WriteLow(addr, data);
                    break;
            }
        }

        //void Mapper019::Write(WORD addr, BYTE data)
        public override void Write(ushort addr, byte data)
        {
            //if( addr >= 0xC000 ) {
            //DEBUGOUT( "W %04X %02X L:%3d\n", addr, data, nes.GetScanline() );
            //}
            switch (addr & 0xF800)
            {
                case 0x8000:
                    if ((data < 0xE0) || (reg[0] != 0))
                    {
                        SetVROM_1K_Bank(0, data);
                    }
                    else
                    {
                        SetCRAM_1K_Bank(0, data & 0x1F);
                    }
                    break;
                case 0x8800:
                    if ((data < 0xE0) || (reg[0] != 0))
                    {
                        SetVROM_1K_Bank(1, data);
                    }
                    else
                    {
                        SetCRAM_1K_Bank(1, data & 0x1F);
                    }
                    break;
                case 0x9000:
                    if ((data < 0xE0) || (reg[0] != 0))
                    {
                        SetVROM_1K_Bank(2, data);
                    }
                    else
                    {
                        SetCRAM_1K_Bank(2, data & 0x1F);
                    }
                    break;
                case 0x9800:
                    if ((data < 0xE0) || (reg[0] != 0))
                    {
                        SetVROM_1K_Bank(3, data);
                    }
                    else
                    {
                        SetCRAM_1K_Bank(3, data & 0x1F);
                    }
                    break;
                case 0xA000:
                    if ((data < 0xE0) || (reg[1] != 0))
                    {
                        SetVROM_1K_Bank(4, data);
                    }
                    else
                    {
                        SetCRAM_1K_Bank(4, data & 0x1F);
                    }
                    break;
                case 0xA800:
                    if ((data < 0xE0) || (reg[1] != 0))
                    {
                        SetVROM_1K_Bank(5, data);
                    }
                    else
                    {
                        SetCRAM_1K_Bank(5, data & 0x1F);
                    }
                    break;
                case 0xB000:
                    if ((data < 0xE0) || (reg[1] != 0))
                    {
                        SetVROM_1K_Bank(6, data);
                    }
                    else
                    {
                        SetCRAM_1K_Bank(6, data & 0x1F);
                    }
                    break;
                case 0xB800:
                    if ((data < 0xE0) || (reg[1] != 0))
                    {
                        SetVROM_1K_Bank(7, data);
                    }
                    else
                    {
                        SetCRAM_1K_Bank(7, data & 0x1F);
                    }
                    break;
                case 0xC000:
                    if (patch == 0)
                    {
                        if (data <= 0xDF)
                        {
                            SetVROM_1K_Bank(8, data);
                        }
                        else
                        {
                            SetVRAM_1K_Bank(8, data & 0x01);
                        }
                    }
                    break;
                case 0xC800:
                    if (patch == 0)
                    {
                        if (data <= 0xDF)
                        {
                            SetVROM_1K_Bank(9, data);
                        }
                        else
                        {
                            SetVRAM_1K_Bank(9, data & 0x01);
                        }
                    }
                    break;
                case 0xD000:
                    if (patch == 0)
                    {
                        if (data <= 0xDF)
                        {
                            SetVROM_1K_Bank(10, data);
                        }
                        else
                        {
                            SetVRAM_1K_Bank(10, data & 0x01);
                        }
                    }
                    break;
                case 0xD800:
                    if (patch == 0)
                    {
                        if (data <= 0xDF)
                        {
                            SetVROM_1K_Bank(11, data);
                        }
                        else
                        {
                            SetVRAM_1K_Bank(11, data & 0x01);
                        }
                    }
                    break;
                case 0xE000:
                    SetPROM_8K_Bank(4, data & 0x3F);
                    if (patch == 2)
                    {
                        if ((data & 0x40) != 0) SetVRAM_Mirror(VRAM_VMIRROR);
                        else SetVRAM_Mirror(VRAM_MIRROR4L);
                    }
                    if (patch == 3)
                    {
                        if ((data & 0x80) != 0) SetVRAM_Mirror(VRAM_HMIRROR);
                        else SetVRAM_Mirror(VRAM_VMIRROR);
                    }
                    break;
                case 0xE800:
                    reg[0] = (byte)(data & 0x40);
                    reg[1] = (byte)(data & 0x80);
                    SetPROM_8K_Bank(5, data & 0x3F);
                    break;
                case 0xF000:
                    SetPROM_8K_Bank(6, data & 0x3F);
                    break;
                case 0xF800:
                    if (addr == 0xF800)
                    {
                        if (exsound_enable != 0)
                        {
                            nes.apu.ExWrite(addr, data);
                        }
                        reg[2] = data;
                    }
                    break;
            }
        }

        //void Mapper019::Clock(INT cycles)
        public override void Clock(int cycles)
        {
            if (irq_enable != 0)
            {
                irq_counter = (ushort)(irq_counter + cycles);
                if (irq_counter >= 0x7FFF)
                {
                    //			irq_counter = 0x7FFF;
                    //			nes.cpu.IRQ_NotPending();

                    irq_enable = 0;
                    //			irq_counter &= 0x7FFF;
                    irq_counter = 0x7FFF;
                    nes.cpu.SetIRQ(IRQ_MAPPER);
                }
            }
        }

        //void Mapper019::SaveState(LPBYTE p)
        public override void SaveState(byte[] p)
        {
            p[0] = reg[0];
            p[1] = reg[1];
            p[2] = reg[2];
            p[3] = irq_enable;
            //*(WORD*)&p[4] = irq_counter;
            BitConverter.GetBytes(irq_counter).CopyTo(p, 4);

            //::memcpy(&p[8], exram, sizeof(exram));
            Array.Copy(exram, p, exram.Length);

        }

        //void Mapper019::LoadState(LPBYTE p)
        public override void LoadState(byte[] p)
        {
            reg[0] = p[0];
            reg[1] = p[1];
            reg[2] = p[2];
            irq_enable = p[3];
            //irq_counter = *(WORD*)&p[4];
            irq_counter = BitConverter.ToUInt16(p, 4);
            //::memcpy(exram, &p[8], sizeof(exram));
            Array.Copy(p, exram, exram.Length);
        }


        public override bool IsStateSave()
        {
            return true;
        }
    }
}

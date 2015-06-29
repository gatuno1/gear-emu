/* --------------------------------------------------------------------------------
 * Gear: Parallax Inc. Propeller Debugger
 * Copyright 2007 - Robert Vandiver
 * --------------------------------------------------------------------------------
 * Cog.CS
 * Base class for a cog processor.  Abstract and must be extended.
 * --------------------------------------------------------------------------------
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * --------------------------------------------------------------------------------
 */


namespace Gear.EmulationCore
{
    public enum CogRunState
    {
        /// @brief Waiting for instruction to finish executing.
        STATE_EXECUTE,
        /// @brief %Cog is loading program memory.
        WAIT_LOAD_PROGRAM,
        /// @brief %Cog is executing an instruction, and waiting an alloted ammount of cycles
        WAIT_CYCLES,
        /// @brief Waits for an allotted number of cycles before changing to a new state
        WAIT_PREWAIT,

        /// @brief Interpreter is booting up
        BOOT_INTERPRETER,
        /// @brief Interpreter is executing an instruction
        WAIT_INTERPRETER,
        /// @brief Interpreter is fetching instruction
        EXEC_INTERPRETER,

        /// @brief Waits for pins to match
        WAIT_PEQ,
        /// @brief Waits for pins to NOT match
        WAIT_PNE,
        /// @brief Waits for count
        WAIT_CNT,
        /// @brief Waits for video
        WAIT_VID,

        /// @brief Waiting to read byte
        HUB_RDBYTE,
        /// @brief Waiting to read word
        HUB_RDWORD,
        /// @brief Waiting to read uint
        HUB_RDLONG,
        /// @brief Waiting to perform hub operation
        HUB_HUBOP
    }

    /// @brief %Cog RAM Special Purpose Registers.
    /// 
    /// @remark Source: Table 15 - %Cog RAM Special Purpose Registers, %Propeller 
    /// P8X32A Datasheet V1.4.0.
    /// @version v15.03.26 - added NONE to enum, to follow best practices and do not
    /// have an illegal value when is instantiated.
    public enum CogSpecialAddress : uint
    {
        COGID     = 0x1E9,    //!< Identification number of this cog.
        INITCOGID = 0x1EF,    //!< @todo Document enum value CogSpecialAddress.INITCOGID.
        PAR       = 0x1F0,    //!< Boot Parameter
        CNT       = 0x1F1,    //!< System Counter
        INA       = 0x1F2,    //!< Input States for P31 - P0.
        INB       = 0x1F3,    //!< Input States for P63 - P32.
        OUTA      = 0x1F4,    //!< Output States for P31 - P0.
        OUTB      = 0x1F5,    //!< Output States for P63 - P32.
        DIRA      = 0x1F6,    //!< Direction States for P31 - P0.
        DIRB      = 0x1F7,    //!< Direction States for P63 - P32.
        CNTA      = 0x1F8,    //!< Counter A Control.
        CNTB      = 0x1F9,    //!< Counter B Control.
        FRQA      = 0x1FA,    //!< Counter A Frequency.
        FRQB      = 0x1FB,    //!< Counter B Frequency.
        PHSA      = 0x1FC,    //!< Counter A Phase.
        PHSB      = 0x1FD,    //!< Counter B Phase.
        VCFG      = 0x1FE,    //!< Video Configuration.
        VSCL      = 0x1FF,    //!< Video Scale.
        NONE      = 0x000     //!< None
    }
    
    /// @todo Document enum Gear.EmulationCore.CogConditionCodes
    /// 
    public enum CogConditionCodes : uint
    {
        IF_NEVER        = 0x00, //!< Never execute
        IF_A            = 0x01, //!< if above (!C & !Z)
        IF_NC_AND_NZ    = 0x01, //!< if C clear and Z clear
        IF_NZ_AND_NC    = 0x01, //!< if Z clear and C clear
        IF_NC_AND_Z     = 0x02, //!< if C clear and Z set
        IF_Z_AND_NC     = 0x02, //!< if C set and Z clear
        IF_NC           = 0x03, //!< if C clear
        IF_AE           = 0x03, //!< if above/equal (!C)
        IF_NZ_AND_C     = 0x04, //!< if Z clear and C set
        IF_C_AND_NZ     = 0x04, //!< if C set and Z clear
        IF_NZ           = 0x05, //!< if Z clear
        IF_NE           = 0x05, //!< if not equal (!Z)
        IF_C_NE_Z       = 0x06, //!< if C not equal to Z
        IF_Z_NE_C       = 0x06, //!< if Z not equal to C
        IF_NC_OR_NZ     = 0x07, //!< if C clear or Z clear
        IF_NZ_OR_NC     = 0x07, //!< if Z clear or C clear
        IF_C_AND_Z      = 0x08, //!< if C set and Z set
        IF_Z_AND_C      = 0x08, //!< if Z set and C set
        IF_C_EQ_Z       = 0x09, //!< if C equal to Z
        IF_Z_EQ_C       = 0x09, //!< if Z equal to C
        IF_E            = 0x0A, //!< if equal (Z)
        IF_Z            = 0x0A, //!< if Z set
        IF_NC_OR_Z      = 0x0B, //!< if C clear or Z set
        IF_Z_OR_NC      = 0x0B, //!< if Z set or C clear
        IF_B            = 0x0C, //!< if below (C)
        IF_C            = 0x0C, //!< if C set
        IF_NZ_OR_C      = 0x0D, //!< if Z clear or C set
        IF_C_OR_NZ      = 0x0D, //!< if C set or Z clear
        IF_Z_OR_C       = 0x0E, //!< if Z set or C set
        IF_BE           = 0x0E, //!< if below/equal (C | Z)
        IF_C_OR_Z       = 0x0E, //!< if C set or Z set
        IF_ALWAYS       = 0x0F  //!< Always execute
    }

    /// @brief Base class for a %Cog emulator.
    abstract public partial class Cog
    {
        // Runtime variables
        protected uint[] Memory;            //!< %Cog Memory.

        protected PropellerCPU Hub;         //!< Host processor
        protected volatile uint PC;         //!< Program Cursor
        protected volatile int BreakPointCogCursor; //!< Breakpoint Address

        protected int StateCount;           //!< Argument for the current state
        protected CogRunState State;        //!< Current %Cog state
        protected CogRunState NextState;    //!< Next state %Cog state

        protected uint ProgramAddress;      //!< @todo Document member Cog.ProgramAddress
        protected uint ParamAddress;        //!< @todo Document member Cog.ParamAddress

        protected FreqGenerator FreqA;      //!< @todo Document member Cog.FreqA
        protected FreqGenerator FreqB;      //!< @todo Document member Cog.FreqB
        protected VideoGenerator Video;     //!< @todo Document member Cog.Video
        protected PLLGroup PhaseLockedLoop; //!< @todo Document member Cog.PhaseLockedLoop

        /// @brief Total %Cog memory implemented on P1 Chip.
        public const int TOTAL_COG_MEMORY = 0x200;  // 512 longs of memory

        /// @brief Default constructor.
        public Cog(PropellerCPU host, uint programAddress, uint param, uint frequency, PLLGroup pll)
        {
            Hub = host;

            Memory = new uint[TOTAL_COG_MEMORY];
            ProgramAddress = programAddress;
            ParamAddress = param;

            FreqA = new FreqGenerator(host, pll, true);
            FreqB = new FreqGenerator(host, pll, false);
            Video = new VideoGenerator(host);
            PhaseLockedLoop = pll;

            // Attach the video generator to PLLs
            PhaseLockedLoop.SetupPLL(Video);

            PC = 0;
            BreakPointCogCursor = -1;    // Breakpoint disabled initially

            // We are in boot time load
            Memory[(int)CogSpecialAddress.PAR] = param;
            State = CogRunState.WAIT_LOAD_PROGRAM;
            StateCount = 0;

            // Clear the special purpose registers
            for (int i = (int)CogSpecialAddress.CNT; i <= 0x1FF; i++)
            {
                this[i] = 0;
            }

            SetClock(frequency);
        }

        /// @brief Break Point position in the program running in this cog.
        public int BreakPoint
        {
            get { return BreakPointCogCursor; }
            set { BreakPointCogCursor = value; }
        }

        /// @brief Property to return complete OUT pins (P0..P63)
        /// Analyze all sources of pin changes in the cog: OUTA, OUTB, the two counters 
        /// and the video generator.
        public ulong OUT
        {
            get
            {
                return Memory[(int)CogSpecialAddress.OUTA] |
                    (Memory[(int)CogSpecialAddress.OUTB] << 32) |
                    FreqA.Output |
                    FreqB.Output |
                    Video.Output;
            }
        }

        /// @brief Property to return only OUTA pins.
        /// Analyze all sources of pin changes in the cog for OUTA pins (P31..P0): the two 
        /// counters and the video generator.
        public uint OUTA
        {
            get
            {
                return Memory[(int)CogSpecialAddress.OUTA] |
                    ((uint)FreqA.Output |
                    (uint)FreqB.Output |
                    (uint)Video.Output);
            }
        }

        /// @brief Property to return only OUTB pins.
        /// Analyze all sources of pin changes in the cog for OUTB pins (P63..P32): the 
        /// two counters and the video generator.
        public uint OUTB
        {
            get
            {
                return
                    (uint)((FreqA.Output |
                    FreqB.Output |
                    Video.Output) >> 32) |
                    Memory[(int)CogSpecialAddress.OUTB];
            }
        }

        /// @brief Property to return complete DIR pins.
        /// 
        public ulong DIR
        {
            get
            {
                return (Memory[(int)CogSpecialAddress.DIRB] << 32) |
                    Memory[(int)CogSpecialAddress.DIRA];
            }
        }

        /// @todo Document property Gear.EmulationCore.Cog.DIRA.
        /// 
        public uint DIRA
        {
            get
            {
                return Memory[(int)CogSpecialAddress.DIRA];
            }
        }

        /// @todo Document property Gear.EmulationCore.Cog.DIRB.
        /// 
        public uint DIRB
        {
            get
            {
                return Memory[(int)CogSpecialAddress.DIRB];
            }
        }

        /// @brief The actual position where the program is executing in this cog.
        public uint ProgramCursor
        {
            get { return PC; }
            set { PC = value; }
        }

        /// @brief Show a string with human readable state of a cog.
        public string CogState
        {
            get
            {
                switch (State)
                {
                    case CogRunState.HUB_HUBOP:
                    case CogRunState.HUB_RDBYTE:
                    case CogRunState.HUB_RDWORD:
                    case CogRunState.HUB_RDLONG:
                        return "Waiting for hub";
                    case CogRunState.BOOT_INTERPRETER:
                        return "Interpreter Boot";
                    case CogRunState.EXEC_INTERPRETER:
                        return "Interpreter Fetch";
                    case CogRunState.WAIT_INTERPRETER:
                        return "Interpreter Processing";
                    case CogRunState.STATE_EXECUTE:
                    case CogRunState.WAIT_PREWAIT:
                    case CogRunState.WAIT_CYCLES:
                        return "Running instruction";
                    case CogRunState.WAIT_LOAD_PROGRAM:
                        return "Loading Program";
                    case CogRunState.WAIT_CNT:
                        return "Waiting (CNT)";
                    case CogRunState.WAIT_PEQ:
                        return "Waiting (PEQ)";
                    case CogRunState.WAIT_PNE:
                        return "Waiting (PNE)";
                    case CogRunState.WAIT_VID:
                        return "Waiting (video)";
                    default:
                        return "ERROR";
                }
            }
        }

        /// @todo Document property Gear.EmulationCore.Cog.operator[].
        /// 
        public uint this[int i]
        {
            get
            {
                if (i >= TOTAL_COG_MEMORY)
                {
                    return 0x55;
                }
                else
                {
                    // show special registers, because their values are in
                    // variables in Cog object and not in memory array.
                    if (i >= (int)CogSpecialAddress.PAR)
                    {
                        return ReadLong((uint)i);
                    }
                    else
                    {
                        return Memory[i];
                    }
                }
            }

            set
            {
                if (i < TOTAL_COG_MEMORY)
                {
                    Memory[i] = value;
                }
            }
        }

        /// @todo Document method Gear.EmulationCore.Cog.ConditionCompare.
        /// 
        public static bool ConditionCompare(CogConditionCodes condition, bool a, bool b)
        {
            switch (condition)
            {
                case CogConditionCodes.IF_NEVER:
                    break;
                case CogConditionCodes.IF_NZ_AND_NC:
                    if (!a && !b) return false;
                    break;
                case CogConditionCodes.IF_NC_AND_Z:
                    if (a && !b) return false;
                    break;
                case CogConditionCodes.IF_NC:
                    if (!b) return false;
                    break;
                case CogConditionCodes.IF_C_AND_NZ:
                    if (!a && b) return false;
                    break;
                case CogConditionCodes.IF_NZ:
                    if (!a) return false;
                    break;
                case CogConditionCodes.IF_C_NE_Z:
                    if (a != b) return false;
                    break;
                case CogConditionCodes.IF_NC_OR_NZ:
                    if (!a || !b) return false;
                    break;
                case CogConditionCodes.IF_C_AND_Z:
                    if (a && b) return false;
                    break;
                case CogConditionCodes.IF_C_EQ_Z:
                    if (a == b) return false;
                    break;
                case CogConditionCodes.IF_Z:
                    if (a) return false;
                    break;
                case CogConditionCodes.IF_NC_OR_Z:
                    if (a || !b) return false;
                    break;
                case CogConditionCodes.IF_C:
                    if (b) return false;
                    break;
                case CogConditionCodes.IF_C_OR_NZ:
                    if (!a || b) return false;
                    break;
                case CogConditionCodes.IF_Z_OR_C:
                    if (a || b) return false;
                    break;
                case CogConditionCodes.IF_ALWAYS:
                    return false;
            }

            return true;
        }

        /// @todo Document method Gear.EmulationCore.Cog.HubAccessable().
        /// 
        public virtual void HubAccessable()
        {
            switch (State)
            {
                case CogRunState.WAIT_LOAD_PROGRAM:
                    Memory[StateCount++] = Hub.ReadLong(ProgramAddress);
                    ProgramAddress += 4;

                    if (StateCount == 0x1F0)
                    {
                        StateCount = 0;
                        Boot();
                    }
                    break;
            }
        }

        /// @todo Document method Gear.EmulationCore.Cog.DetachVideoHooks()
        /// 
        public void DetachVideoHooks()
        {
            // Detach the video hook
            PhaseLockedLoop.Destroy();
            // Detach the aural hook
            Video.DetachAural();
        }

        /// @brief Execute a Step in the cog.
        public bool Step()
        {
            bool result = DoInstruction();
            // Run our frequency counters
            FreqA.Tick(Hub.IN);
            FreqB.Tick(Hub.IN);
            return result;    // false - we hit a breakpoint
        }

        /// @todo Document method Gear.EmulationCore.Cog.SetClock().
        /// 
        public void SetClock(uint freq)
        {
            FreqA.SetClock(freq);
            FreqB.SetClock(freq);
        }

        /// @todo Document method Gear.EmulationCore.Cog.StepInstruction().
        public void StepInstruction()
        {
            int i = 0x2000;    // Maximum of 8k clocks (covers load instruction)
            do
            {
                Hub.Step();
            }
            while ( (State != CogRunState.EXEC_INTERPRETER) && 
                (State != CogRunState.STATE_EXECUTE && --i > 0) );
        }

        /// @todo Document method Gear.EmulationCore.Cog.ReadLong().
        /// 
        public uint ReadLong(uint address)
        {
            // values using CogSpecialAddress enum, instead of direct hex values
            switch ((CogSpecialAddress)(address & 0x1FF))
            {
                case CogSpecialAddress.CNT:
                    return Hub.Counter;
                case CogSpecialAddress.INA:
                    return Hub.INA;
                case CogSpecialAddress.INB:
                    return Hub.INB;
                case CogSpecialAddress.CNTA:
                    return FreqA.CTR;
                case CogSpecialAddress.CNTB:
                    return FreqB.CTR;
                case CogSpecialAddress.FRQA:
                    return FreqA.FRQ;
                case CogSpecialAddress.FRQB:
                    return FreqB.FRQ;
                case CogSpecialAddress.PHSA:
                    return FreqA.PHS;
                case CogSpecialAddress.PHSB:
                    return FreqB.PHS;
                case CogSpecialAddress.VCFG:
                    return Video.CFG;
                case CogSpecialAddress.VSCL:
                    return Video.SCL;
                default:
                    return Memory[address & 0x1FF];
            }
        }

        /// @brief Write cog RAM with a specified value
        /// @details This method take care of special cog address that in this class aren't 
        /// write in memory array Cog.Memory.
        /// @param[in] address Address to write.
        /// @param[in] data Data to write in address.
        /// @note PAR address is a special case, because unless Propeller Manual V1.2 
        /// specifications says it is a read-only register, there are claims that in reality it 
        /// is writable as explains 
        /// <a href="http://forums.parallax.com/showthread.php/115909-PASM-simulator-debugger)">
        /// Forum thread "PASM simulator / debugger?</a>.
        /// They claims that some parallax video drivers in PASM changes the PAR register, 
        /// and GEAR didn't emulate that.
        protected void WriteLong(uint address, uint data)
        {
            // values using CogSpecialAddress enum, instead of direct hex values
            switch ((CogSpecialAddress)(address & 0x1FF))
            {
                // Read only registers
                // case CogSpecialAddress.PAR: // PAR register changed to writable
                case CogSpecialAddress.CNT:
                case CogSpecialAddress.INA:
                case CogSpecialAddress.INB:
                    return;
                case CogSpecialAddress.CNTA:
                    FreqA.CTR = data;
                    break;
                case CogSpecialAddress.CNTB:
                    FreqB.CTR = data;
                    break;
                case CogSpecialAddress.FRQA:
                    FreqA.FRQ = data;
                    break;
                case CogSpecialAddress.FRQB:
                    FreqB.FRQ = data;
                    break;
                case CogSpecialAddress.PHSA:
                    FreqA.PHS = data;
                    break;
                case CogSpecialAddress.PHSB:
                    FreqB.PHS = data;
                    break;
                case CogSpecialAddress.VCFG:
                    Video.CFG = data;
                    break;
                case CogSpecialAddress.VSCL:
                    Video.SCL = data;
                    break;
                default:
                    Memory[address & 0x1FF] = data;
                    return;
            }
        }

        /// @brief Execute a instruction in this cog.
        /// @returns TRUE if it is time to trigger a breakpoint, or FALSE if not.
        abstract public bool DoInstruction();

        /// @todo Document method Gear.EmulationCore.Cog.Boot()
        abstract public void Boot();
    }
}

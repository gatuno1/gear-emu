using System;
using System.Collections.Generic;
using Gear.EmulationCore;
using Gear.PluginSupport;

namespace Gear.GUI
{
    public partial class MemoryWatch : Gear.PluginSupport.PluginBase
    {
        /// @brief List of watches
        List<MemWatcher> Watches;

        /// @brief Default Constructor
        public MemoryWatch(PropellerCPU chip)
            : base(chip)
        {
            Watches = new List<MemWatcher>();
            InitializeComponent();
            OnReset();
        }

        /// @brief Title to be shown in tab.
        public override string Title
        {
            get { return "Watches"; }
        }

        /// @brief Set the reference to the emulated chip. Occurs once the plugin is loaded.
        public override void PresentChip()
        {
            NotifyOnClock();
        }

        /// @brief Called every clock tick, if called Chip.NotifyOnClock() in method
        /// PresentChip() above.
        public override void OnClock(double time, uint sysCounter)
        {
            foreach (MemWatcher watch in Watches)
            {
                if (watch.haveToUpdate && (watch.HaveToStop(sysCounter)))
                    BreakPoint();
            }
        }

        /// @brief When repaint event is called, is the opportunity to detect changes
        public override void Repaint(bool force)
        {
            //update each watch to detect changes from this moment
            foreach (MemWatcher watch in Watches)
            {
                watch.UpdateValue();
                watch.UpdateFrame(Chip.Counter);
            }
        }

        /// @brief Called when you press the Reset button, to reset the plugin to a
        ///initial state.
        public override void OnReset()
        {

        }

        public class MemWatcher
        {
            public enum StopMode
            {
                none = 0,       //don't stop on value changes
                anyChange,      //stop when a value change is detected
                equalValue      //stop when a specific value is detected
            }

            private PropellerCPU _chip; //to access main memory
            private Cog _cog;           //to access cog memory

            public bool IsCogBased { get; set; }

            public string Name { get; set; }    //name to show
            private uint _addr;
            public uint Addr
            {
                get { return _addr; }
                set
                {
                    if (IsCogBased)
                    {
                        if (ValidCogAddr(value))
                            _addr = value;
                        else
                            throw new Exception(string.Format(
                                "Value '{0:N}'(0x{1:X}) is not valid for Cog Address.",
                                value, value));
                    }
                    else
                    {
                        if (ValidHubAddr(value))
                            _addr = value;
                        else
                            throw new Exception(string.Format(
                                "Value '{0:N}'(0x{1:X}) is not valid for Main memory Address.",
                                value, value));
                    }
                }
            }

            public uint Value { get; private set; }
            private uint ComparedValue;
            private uint LastCounter;

            private StopMode _mode;
            public StopMode Mode
            {
                get { return _mode; }
                set
                {
                    _mode = value;
                    switch (value)
                    {
                        case StopMode.none:
                            haveToUpdate = false;
                            break;
                        case StopMode.anyChange:
                        case StopMode.equalValue:
                            haveToUpdate = true;
                            break;
                    }
                }
            }

            public bool isStoppedBy { get; private set; }

            public void SetComparedValue(uint val)
            {
                ComparedValue = val;
            }

            private uint UpdatedValue()
            {
                return Value = (IsCogBased) ?
                    _cog.ReadLong(_addr) :
                    _chip.ReadLong(_addr);
            }

            public void UpdateValue()
            {
                UpdatedValue();
            }

            public bool UpdateFrame(uint counter)
            {
                if (counter != LastCounter)
                {
                    LastCounter = counter;
                    return true;
                }
                else
                    return false;
            }

            private bool _mustUpdate;
            public bool haveToUpdate
            {
                get
                {
                    if (_mustUpdate)
                        UpdatedValue();
                    return _mustUpdate;
                }
                private set { _mustUpdate = value; }
            }

            public bool HaveToStop(uint counter)
            {
                bool retVal = false;
                if (counter != LastCounter)
                {
                    switch (Mode)
                    {
                        case StopMode.none:
                            break;
                        case StopMode.anyChange:
                            if (retVal = (UpdatedValue() != ComparedValue))
                                isStoppedBy = true;
                            break;
                        case StopMode.equalValue:
                            if (retVal = (UpdatedValue() == ComparedValue))
                                isStoppedBy = true;
                            break;
                    }
                }
                return retVal;
            }

            /// @brief Create a new Watch for a %Cog memory location.
            /// @param cog Reference to %Cog to inspect its memory.
            /// @param name Name to asociate the Watch.
            /// @param address Memory address to inspect.
            public MemWatcher(Cog cog, string name, uint address)
            {
                IsCogBased = true;
                _chip = null;
                _cog = cog;
                Addr = address; //this assigment validate the value or throw an exception
                //CogSpecialAddress tmp;
                //if (Enum.IsDefined(typeof(CogSpecialAddress), Addr) &&
                //    Enum.TryParse(Addr, out tmp))
                //{
                //    specialAddr = tmp;
                //    Name = specialAddr.ToString();
                //}
                Name = name;
                if (string.IsNullOrEmpty(Name))
                    Name = name;
                Value = cog.ReadLong(Addr);
                ComparedValue = 0;
                isStoppedBy = false;
                Mode = StopMode.none;
            }

            /// @brief Create a new Watch for a Main memory location.
            /// @param chip Reference to %PropellerCPU to inspect its memory.
            /// @param name Name to asociate the Watch.
            /// @param address Memory address to inspect.
            public MemWatcher(PropellerCPU chip, string name, uint address)
            {
                IsCogBased = false;
                _chip = chip;
                _cog = null;
                Addr = address; //this assigment validate the value or throw an exception
                Name = name;
                Value = chip.ReadLong(Addr);
                ComparedValue = 0;
                isStoppedBy = false;
                Mode = StopMode.none;
            }

            private bool ValidCogAddr(uint address)
            {
                return (address < Cog.TOTAL_COG_MEMORY);
            }

            private bool ValidHubAddr(uint address)
            {
                return (address < PropellerCPU.TOTAL_RAM);
            }
        }

    }
}

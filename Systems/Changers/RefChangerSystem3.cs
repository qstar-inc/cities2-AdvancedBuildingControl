using AdvancedBuildingControl.Interface;
using StarQ.Shared.Extensions;

namespace AdvancedBuildingControl.Systems.Changers
{
    public partial class RefChangerSystem
    {
        public void Add2New_CheckNApply<T>(
            out bool skip,
            ref T abc_comp,
            ref int currVal,
            int ecd,
            bool ecf
        )
            where T : struct, IABC_Component_Int
        {
            skip = false;
            int capacityValue = 0;

            if (abc_comp.IsDefault())
                abc_comp.Original = currVal;

            if (ecd == currVal)
            {
                skip = true;
                LogHelper.SendLog($"Same value {ecd}", LogLevel.DEV);
                return;
            }

            if (abc_comp.IsDefault())
            {
                if (ecf)
                    capacityValue = ecd;
                else
                    skip = true;
            }
            else
                capacityValue = abc_comp.Modified;

            if (!skip)
            {
                currVal = capacityValue;
                abc_comp.Modified = capacityValue;
                abc_comp.Enabled = true;
            }
        }

        public void Add2New_CheckNApply<T>(
            out bool skip,
            ref T abc_comp,
            ref float currVal,
            float ecd,
            bool ecf
        )
            where T : struct, IABC_Component_Float
        {
            skip = false;
            float capacityValue = 0;
            if (ecd == currVal)
            {
                skip = true;
                LogHelper.SendLog($"Same value {ecd}", LogLevel.DEV);
                return;
            }

            if (abc_comp.IsDefault())
            {
                if (ecf)
                    capacityValue = ecd;
                else
                    skip = true;
            }
            else
                capacityValue = abc_comp.Modified;

            if (!skip)
            {
                if (abc_comp.IsDefault())
                    abc_comp.Original = currVal;
                currVal = capacityValue;
                abc_comp.Modified = capacityValue;
                abc_comp.Enabled = true;
            }
        }
    }
}

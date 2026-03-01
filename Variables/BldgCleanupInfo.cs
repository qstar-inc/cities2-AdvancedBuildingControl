namespace AdvancedBuildingControl.Variables
{
    public class BldgCleanupInfo
    {
        public bool Enabled = false;
        public BldgCleanupTypeInfo[] Array = new BldgCleanupTypeInfo[0];
    }

    public class BldgCleanupTypeInfo
    {
        public bool Enabled = false;
        public BldgCleanupType CleanupType = BldgCleanupType._None;
        public float CurrentValueNumber = 0;
    }

    public enum BldgCleanupType
    {
        _None,
        Garbage,
        Crime,
        OutgoingMail,
        PhysicalDamage,
        FireDamage,
        WaterDamage,
        _All,
    }
}

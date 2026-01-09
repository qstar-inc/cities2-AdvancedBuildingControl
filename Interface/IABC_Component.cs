namespace AdvancedBuildingControl.Interface
{
    public interface IABC_Component_Int
    {
        bool IsDefault();
        int Modified { get; set; }
        int Original { get; set; }
        bool Enabled { get; set; }
    }

    public interface IABC_Component_Float
    {
        bool IsDefault();
        float Modified { get; set; }
        float Original { get; set; }
        bool Enabled { get; set; }
    }
}

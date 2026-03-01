from pathlib import Path

INPUT_FILE = Path(__file__).parent / "comp.txt"

CURRENT_FOLDER = Path(__file__).parent

VARIABLES_FOLDER = CURRENT_FOLDER.parent.parent / "Variables"
VARIABLES_FOLDER.mkdir(exist_ok=True)

SYSTEMS_FOLDER = CURRENT_FOLDER.parent.parent / "Systems"
SYSTEMS_FOLDER.mkdir(exist_ok=True)

EXTENSIONS_FOLDER = CURRENT_FOLDER.parent.parent / "Extensions"
EXTENSIONS_FOLDER.mkdir(exist_ok=True)

UI_WRITER_FOLDER = CURRENT_FOLDER.parent / "src" / "types"
UI_WRITER_FOLDER.mkdir(exist_ok=True)

UpdateValueType_FILE = VARIABLES_FOLDER / "UpdateValueType.cs"
ComponentName_FILE = VARIABLES_FOLDER / "ComponentName.cs"
ComponentName_Get_FILE = VARIABLES_FOLDER / "ComponentName_Get.cs"
Helper_TryLimit_FILE = VARIABLES_FOLDER / "UVTHelper_TryLimit.cs"
ValueFormat_Get_FILE = VARIABLES_FOLDER / "ValueFormat_Get.cs"
BldgComponentInfo_FILE = VARIABLES_FOLDER / "BldgComponentInfo.cs"
TryApply_FILE = SYSTEMS_FOLDER / "SelectedPrefabModifierSystem_TryApply.cs"
SIP_abc2_FILE = SYSTEMS_FOLDER / "SIP_ABC2.cs"
Writer_FILE = EXTENSIONS_FOLDER / "BldgComponentInfoWriterExtensions.cs"
UI_Writer_FILE = UI_WRITER_FOLDER / "BldgComponentInfo.tsx"
UI_UVT_Type_FILE = UI_WRITER_FOLDER / "UpdateValueType.tsx"


def field_short(field_name: str) -> str:
    return field_name.replace("m_", "", 1)


def comp_lower(comp_name: str) -> str:
    if not comp_name:
        return ""
    return comp_name[0].lower() + comp_name[1:]


def utv_conv(field_type: str) -> str:
    return {
        "float": "UVTHelper.ConvertToFloat(modifiedValue)",
        "int": "UVTHelper.ConvertToInt(modifiedValue)",
        "int2": "UVTHelper.ConvertToInt2(modifiedValue)",
        "bool": "UVTHelper.ConvertToBool(modifiedValue)",
        "short": "UVTHelper.ConvertToShort(modifiedValue)",
        "sbyte": "UVTHelper.ConvertToSByte(modifiedValue)",
        "byte": "UVTHelper.ConvertToByte(modifiedValue)",
    }.get(field_type, f"({field_type})modifiedValue")


def valueFormatCS(field_type: str) -> str:
    return {
        "float": "Float",
        "int": "Int",
        "int2": "Int2",
        "bool": "Bool",
        "short": "Short",
        "sbyte": "SByte",
        "byte": "Byte",
        "Resource": "Ulong",
    }.get(field_type, f"Int")


def valueFormatJS(field_type: str) -> str:
    return {
        "float": "number",
        "int": "number",
        "int2": "Number2",
        "bool": "boolean",
        "short": "number",
        "sbyte": "number",
        "byte": "number",
        "CoverageService": "number",
        "MaintenanceType": "number",
        "RoadTypes": "number",
        "EnergyTypes": "number",
        "SizeClass": "number",
        "PolicePurpose": "number",
        "TransportType": "number",
        "AllowedWaterTypes": "number",
        "WorkplaceComplexity": "number",
        "Resource": "string",
    }.get(field_type, f"Unknown")


def defsBackend(field_type: str) -> str:
    return {
        "bool": "false",
    }.get(field_type, f"0")


def group_fields_by_type(components: dict) -> dict:
    grouped = {}

    for _, comp_info in components.items():
        for field in comp_info["fields"]:
            field_type = valueFormatCS(field["type"])
            comp = {
                "f_type": field["type"],
                "full_name": field["full"],
                "min": field["min"],
                "max": field["max"],
            }

            grouped.setdefault(field_type, []).append(comp)
    return grouped


def get_apl_bool(apl) -> bool:
    try:
        if apl is None or str(apl).strip() == "":
            return False
        return int(apl) == 1
    except (ValueError, TypeError):
        return False


def parse_file(path: Path) -> dict:
    components = {}

    with path.open(encoding="utf-8") as f:
        for line_no, line in enumerate(f, 1):
            line = line.rstrip("\n")
            if not line:
                continue

            parts = line.split("\t")
            if len(parts) < 6:
                raise ValueError(
                    f"Line {line_no}: Expected 6 columns, got {len(parts)}"
                )

            (
                comp_name,
                field_name,
                field_type,
                enum_base,
                enum_flag,
                id_,
                apl,
                min,
                max,
                x,
            ) = parts

            if x.strip().lower() == "x":
                continue

            if comp_name not in components:
                components[comp_name] = {
                    "name": comp_name,
                    "lower": comp_lower(comp_name),
                    "fields": [],
                }

            short = field_short(field_name)

            components[comp_name]["fields"].append(
                {
                    "name": field_name,
                    "short": short,
                    "full": f"{comp_name}_{short}",
                    "type": field_type,
                    "id": id_,
                    "apl": get_apl_bool(apl),
                    "min": min,
                    "max": max,
                    "enum_base": enum_base,
                    "enum_flag": enum_flag,
                }
            )
    components = {name: comp for name, comp in components.items() if comp["fields"]}

    return components


if __name__ == "__main__":
    data = parse_file(INPUT_FILE)
    grouped_by_type = group_fields_by_type(data)

    text1 = """
namespace AdvancedBuildingControl.Variables
{
    public enum UpdateValueType : ushort
    {
        //start
        _None = 0,
        _All = 1,
"""
    text2 = """
namespace AdvancedBuildingControl.Variables
{
    public enum ComponentName
    {
        _None,

        //start
"""
    text3 = """
namespace AdvancedBuildingControl.Variables
    {
        public partial class UVTHelper
        {
            public static ComponentName GetComponentName(UpdateValueType updateValueType)
            {
                switch (updateValueType)
                {
                    case UpdateValueType._None:
                    case UpdateValueType._All:
                        break;

                    // start
"""
    text4 = """using System;

namespace AdvancedBuildingControl.Variables
{
    public partial class UVTHelper
    {
"""
    text5 = """
namespace AdvancedBuildingControl.Variables
    {
        public partial class UVTHelper
        {
            public static ValueFormat GetValueFormat(UpdateValueType updateValueType)
            {
                switch (updateValueType)
                {
                    case UpdateValueType._None:
                    case UpdateValueType._All:
                        break;

                    // start
                    """
    text6 = """
using Game.Economy;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Vehicles;
using Unity.Mathematics;

namespace AdvancedBuildingControl.Variables
{
    public class BldgComponentInfo
    {
"""
    text7_1 = """
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Game;
using Game.Economy;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Vehicles;
using StarQ.Shared.Extensions;
using Unity.Entities;
using Unity.Mathematics;

namespace AdvancedBuildingControl.Systems
{
    public partial class SelectedPrefabModifierSystem : GameSystemBase
    {
        public bool TryApply(
            Entity selectedPrefab,
            UpdateValueType valueType,
            long modifiedValue,
            out long originalValue
        )
        {
            originalValue = -1L;
            LogHelper.SendLog($"TryApply: {valueType}, {modifiedValue}", LogLevel.DEVD);
            ComponentName componentName = UVTHelper.GetComponentName(valueType);
"""
    text7_2 = ""
    text7_3 = """
//end

switch (componentName)
{
    case ComponentName._None:
        return false;

    //start
"""
    text7_4 = ""
    text7_5 = """
    //end
    default:
        break;
}

switch (valueType)
{
    case UpdateValueType._None:
        return true;
    case UpdateValueType._All:
        return true;
    //start
"""
    text7_6 = ""
    text8 = """
using AdvancedBuildingControl.Extensions;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Game.Prefabs;

namespace AdvancedBuildingControl.Systems
{
    public partial class SIP_ABC : ExtendedInfoSectionBase
    {
        public void CheckComponents()
        {
            BldgComponentInfo c = new();
            //start
"""
    text9 = """
using AdvancedBuildingControl.Variables;
using Colossal.UI.Binding;

namespace AdvancedBuildingControl.Extensions
{
    public static class BldgComponentInfoWriterExtensions
    {
        public static void Write(this IJsonWriter writer, BldgComponentInfo value)
        {
            writer.TypeBegin(value.GetType().FullName);

            //start
"""
    text10 = """
import { Number2 } from "cs2/bindings";

export interface BldgComponentInfo {
"""
    text11 = """
export enum UpdateValueType {
  _None = 0,
  _All = 1,
"""

    for comp, info in data.items():
        lower = info["lower"]
        text2 += f"{comp},"
        text3_1 = ""
        text6 += f"public bool {comp} {{ get; set; }} = false;"
        text7_2 += f"{comp} {lower} = default;"
        text7_4 += f"case ComponentName.{comp}: if (!EntityManager.TryGetComponent(selectedPrefab, out {lower})) return false; break;"
        xpl = ""
        text9 += (
            f"writer.PropertyName(nameof(value.{comp}));writer.Write(value.{comp});"
        )
        text10 += f"{comp}: boolean;"

        # print(comp)
        for field in info["fields"]:
            id = field["id"]
            full = field["full"]
            f_type = field["type"]
            name = field["name"]
            min = field["min"]
            max = field["max"]
            text1 += f"{full} = {id},"
            text3_1 += f"case UpdateValueType.{full}:"
            text6 += f"public {f_type} {full} {{ get; set; }} = {defsBackend(f_type)};"
            apl = (
                "AdditionalPostApply(selectedPrefab, modifiedValue, valueType);"
                if field["apl"]
                else ""
            )
            enum_base = ""
            writer_write = f"value.{full}"
            if field["enum_base"] != "":
                if field["enum_base"] != "ulong" and f_type != "Resource":
                    enum_base = "(int)"
                    writer_write = f"(int){writer_write}"
                else:
                    enum_base = "(ulong)"
                    writer_write = f"((ulong){writer_write}).ToString()"
            text7_6 += f"""
                case UpdateValueType.{full}:
                    {f_type} orig_{full} = {lower}.{name};
                    originalValue = UVTHelper.ConvertToLong({enum_base}orig_{full});                    
                    {info["lower"]}.{name} = {utv_conv(f_type)};
                    utils.SetOrAdd(selectedPrefab, {lower});{apl}
                    return true;
"""
            xpl += f"c.{full} = {lower}.{name};"
            text9 += f"writer.PropertyName(nameof(value.{full}));writer.Write({writer_write});"
            text10 += f"{full}: {valueFormatJS(f_type)};"
            text11 += f"{full} = {id},"
            # print(field)
        text3 += f"{text3_1} return ComponentName.{comp};"
        text8 += f"""if (EntityManager.TryGetComponent(selectedPrefab, out {comp} {lower}))
{{
    c.{comp} = true;
    {xpl}
}}"""

    for f_type, item in grouped_by_type.items():
        text5_1 = ""
        text4_1 = ""
        ftypeX = item[0]["f_type"]
        text4_1a = f"""
        public static void TryLimit(UpdateValueType updateValueType, ref {ftypeX} value)
        {{
            bool clamp;
            {ftypeX} min = {ftypeX}.MinValue;
            {ftypeX} max = {ftypeX}.MaxValue;
            switch (updateValueType)
            {{
"""
        text4_1b = """
            
                default:
                    return;
            }
            if (clamp)
                Math.Clamp(value, min, max);
        }
"""

        for field in item:
            min = field["min"]
            max = field["max"]
            text5 += f"case UpdateValueType.{field["full_name"]}:"

            if len(min) > 0 or len(max) > 0:
                if text4_1 == "":
                    text4_1 = text4_1a
                text4_1 += f"case UpdateValueType.{field["full_name"]}: clamp = true;"
                if len(min) > 0:
                    text4_1 += f"min = {min};"
                if len(max) > 0:
                    text4_1 += f"max = {max};"
                text4_1 += "break;"
        if text4_1 != "":
            text4 += text4_1 + text4_1b
        text5 += f"{text5_1} return ValueFormat.{f_type};"

    text1 += """
        //end
    }
}
"""
    text2 += """
        //end
    }
}
"""
    text3 += """
                //end
            }

            return ComponentName._None;
        }
    }
}
"""
    text4 += """
    }
}
"""
    text5 += """
                //end
            }

            return ValueFormat.Unknown;
        }
    }
}
"""
    text6 += """
        //end
    }
}
"""
    text7_7 = """
                //end
                default:
                    return false;
            }
        }
    }
}
"""
    text7 = text7_1 + text7_2 + text7_3 + text7_4 + text7_5 + text7_6 + text7_7
    text8 += """
            //end
            bldgComponentInfo = c;
        }
    }
}"""
    text9 += """
            //end
            writer.TypeEnd();
        }
    }
}
"""
    text10 += "}"
    text11 += "}"

    with UpdateValueType_FILE.open("w", encoding="utf-8") as f:
        f.write(text1)
    print(f"{UpdateValueType_FILE} created")
    with ComponentName_FILE.open("w", encoding="utf-8") as f:
        f.write(text2)
    print(f"{ComponentName_FILE} created")
    with ComponentName_Get_FILE.open("w", encoding="utf-8") as f:
        f.write(text3)
    print(f"{ComponentName_Get_FILE} created")
    with Helper_TryLimit_FILE.open("w", encoding="utf-8") as f:
        f.write(text4)
    print(f"{Helper_TryLimit_FILE} created")
    with ValueFormat_Get_FILE.open("w", encoding="utf-8") as f:
        f.write(text5)
    print(f"{ValueFormat_Get_FILE} created")
    with BldgComponentInfo_FILE.open("w", encoding="utf-8") as f:
        f.write(text6)
    print(f"{BldgComponentInfo_FILE} created")
    with TryApply_FILE.open("w", encoding="utf-8") as f:
        f.write(text7)
    print(f"{TryApply_FILE} created")
    with SIP_abc2_FILE.open("w", encoding="utf-8") as f:
        f.write(text8)
    print(f"{SIP_abc2_FILE} created")
    with Writer_FILE.open("w", encoding="utf-8") as f:
        f.write(text9)
    print(f"{Writer_FILE} created")
    with UI_Writer_FILE.open("w", encoding="utf-8") as f:
        f.write(text10)
    print(f"{UI_Writer_FILE} created")
    with UI_UVT_Type_FILE.open("w", encoding="utf-8") as f:
        f.write(text11)
    print(f"{UI_UVT_Type_FILE} created")

using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace FNO.Models
{
    public class Name : BaseModel
    {
        [StorageTarget]
        public string First { get; set; }
        [StorageTarget]
        public string Second { get; set; }

        public string Chu2Name => CodeMasterServce.GetValue<NameMaster>(First);
        public string Job => CodeMasterServce.GetValue<JobMaster>(Second);
        public string Attribute => CodeMasterServce.GetMaster<NameMaster>(First).Attribute;
        public string AttributeName => CodeMasterServce.GetValue<AttributeMaster>(Attribute);
        public ATTRIBUTE_TYPE AttributeType => CodeMasterServce.GetMaster<AttributeMaster>(Attribute).Type;
        public int DistanceFromCenter => CodeMasterServce.GetMaster<AttributeMaster>(Attribute).DistanceFromCenter;
        public List<string> StrongAttackTarget => CodeMasterServce.GetMaster<AttributeMaster>(Attribute).StrongAttackTarget;
        public List<string> NeighborAttribute => CodeMasterServce.GetMaster<AttributeMaster>(Attribute).NeighborAttribute;

        public ImageSource CharactorImage => ImageSource.FromResource("FNO.Resouces.Chars." + CodeMasterServce.GetValue<ImageMaster>(int.Parse(Second).ToString()));
        public ImageSource CharactorCutInImage => ImageSource.FromResource("FNO.Resouces.CutIn." + CodeMasterServce.GetValue<ImageMaster>(int.Parse(Second).ToString()));
        public Color CharactorColor => Color.FromHex(CodeMasterServce.GetValue<ColorMaster>(CodeMasterServce.GetMaster<NameMaster>(First).Attribute));

        public int HP => CodeMasterServce.GetMaster<JobMaster>(Second).HP;
        public int DEX => CodeMasterServce.GetMaster<JobMaster>(Second).DEX;


        public Name()
        {

        }

        public Name(NameMaster name, JobMaster job)
        {
            First = name.Code;
            Second = job.Code;
        }

        public override bool Equals(object obj)
        {
            var name = obj as Name;
            if (name == null) return false;
            if (First == name.First && Second == name.Second) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return int.Parse(First + "00" + Second);
        }
    }
}

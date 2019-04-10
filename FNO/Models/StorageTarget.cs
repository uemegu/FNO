using System;
namespace FNO.Models
{
    public class StorageTarget : Attribute
    {
        public bool CanMarge { get; set; }
        public StorageTarget(bool margeFlg = false)
        {
            CanMarge = margeFlg;
        }
    }
}

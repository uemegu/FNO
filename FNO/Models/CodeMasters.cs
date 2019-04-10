using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using FNO.Utils;

namespace FNO.Models
{
    public class CodeMasterServce
    {
        public static List<string> DefaultNames { get; private set; }
        public static List<string> DefaultWinComment { get; private set; }
        public static List<string> DefaultLoseComment { get; private set; }
        public static List<string> DefaultBattleComment { get; private set; }

        private static List<NameMaster> _nameMastersForGet = new List<NameMaster>();
        private static Dictionary<Type, IList<BaseCodeMaster>> _masters = new Dictionary<Type, IList<BaseCodeMaster>>();

        public static void Load()
        {
            LoadLocal();

            var masters = GetMasters<NameMaster>();
            foreach (var m in masters)
            {
                var type = GetMaster<AttributeMaster>(m.Attribute).Type;
                if (type == ATTRIBUTE_TYPE.NORMAL)
                {
                    // ノーマル二つ名の倍率をあげる
                    for (var i = 0; i < Const.GetRareName_Rate; i++)
                    {
                        _nameMastersForGet.Add(m);
                    }
                }
                else if (type == ATTRIBUTE_TYPE.RARE)
                {
                    _nameMastersForGet.Add(m);
                }
            }
        }

        public static string GetValue<T>(string code) where T : BaseCodeMaster
        {
            return (from m in _masters[typeof(T)] where m.Code == code select m.Value).FirstOrDefault();
        }

        public static T GetMaster<T>(string code) where T : BaseCodeMaster
        {
            var master = (T)(from m in _masters[typeof(T)] where m.Code == code select m).FirstOrDefault();
            if (master == null)
            {
                throw new Exception("Unknown Code:" + code);
            }
            return master;
        }

        public static IList<T> GetMasters<T>() where T : BaseCodeMaster
        {
            var masters = _masters[typeof(T)];
            var result = new List<T>();
            foreach (T master in masters) result.Add(master);
            return result;
        }

        public static int GetCount<T>() where T : BaseCodeMaster
        {
            return _masters[typeof(T)].Count;
        }

        public static NameMaster GetNameRandom()
        {
            return _nameMastersForGet[MyRandom.GetRandom(_nameMastersForGet.Count)];
        }

        public static NameMaster GetNameRandomByAttribute(string attribute)
        {
            var masters = GetMasters<NameMaster>().Where((arg) => arg.Attribute == attribute).ToList();
            return masters[MyRandom.GetRandom(masters.Count())];
        }

        private static void LoadLocal()
        {
            Masters data;
            var assembly = typeof(CodeMasterServce).GetTypeInfo().Assembly;
            var resourceName = "FNO.Models.DummyJson.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                data = JsonConvert.DeserializeObject<Masters>(reader.ReadToEnd());
            }

            _masters.Add(data.ImageMaster[0].GetType(), (IList<BaseCodeMaster>)data.ImageMaster);
            _masters.Add(data.NameMaster[0].GetType(), (IList<BaseCodeMaster>)data.NameMaster);
            _masters.Add(data.JobMaster[0].GetType(), (IList<BaseCodeMaster>)data.JobMaster);
            _masters.Add(data.ColorMaster[0].GetType(), (IList<BaseCodeMaster>)data.ColorMaster);
            _masters.Add(data.AttributeMaster[0].GetType(), (IList<BaseCodeMaster>)data.AttributeMaster);
            DefaultNames = data.DefaultNames;
            DefaultWinComment = data.DefaultWinComment;
            DefaultLoseComment = data.DefaultLoseComment;
            DefaultBattleComment = data.DefaultBattleComment;
        }
    }

    public class Masters
    {
        public List<ImageMaster> ImageMaster { get; set; }
        public List<NameMaster> NameMaster { get; set; }
        public List<JobMaster> JobMaster { get; set; }
        public List<ColorMaster> ColorMaster { get; set; }
        public List<AttributeMaster> AttributeMaster { get; set; }
        public List<string> DefaultNames { get; set; }
        public List<string> DefaultWinComment { get; set; }
        public List<string> DefaultLoseComment { get; set; }
        public List<string> DefaultBattleComment { get; set; }
    }

    public class BaseCodeMaster
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class ImageMaster : BaseCodeMaster { }
    public class NameMaster : BaseCodeMaster
    {
        public string Attribute { get; set; }
    }
    public class JobMaster : BaseCodeMaster
    {
        public int HP { get; set; }
        public int DEX { get; set; }
    }
    public class ColorMaster : BaseCodeMaster { }
    public class AttributeMaster : BaseCodeMaster
    {
        public ATTRIBUTE_TYPE Type { get; set; }
        public int DistanceFromCenter { get; set; }
        public List<string> StrongAttackTarget { get; set; }
        public List<string> NeighborAttribute { get; set; }
    }
    public enum ATTRIBUTE_TYPE
    {
        NORMAL = 0,
        RARE = 1,
        ACHIEVEMENT = 2
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoGenerator.Model
{
    class GlobalSettings
    {
        private static readonly Lazy<GlobalSettings> lazyInstance = new Lazy<GlobalSettings>(() => new GlobalSettings());

        public static GlobalSettings Instance
        {
            get => lazyInstance.Value;
        }

        private GlobalSettings() {
            businessInfos = BusinessInfo.defaults();
        }

        internal List<BusinessInfo> businessInfos;
    }
}

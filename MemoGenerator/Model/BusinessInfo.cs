using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoGenerator.Model
{
#nullable enable

    class BusinessInfo
    {
        internal BusinessInfo(string name, string? memoText)
        {
            this.name = name;
            this.memoText = memoText;
        }

        string name;
        internal string? memoText;

        internal static List<BusinessInfo> defaults()
        {
            return new List<BusinessInfo>(new BusinessInfo[] {
                new BusinessInfo("미지정", null),
                new BusinessInfo("일이삼사", null),
                new BusinessInfo("대미기프트", "대미"),
            });
        }
    }

#nullable disable
}

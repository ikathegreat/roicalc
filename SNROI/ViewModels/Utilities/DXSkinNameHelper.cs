using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Xpf.Core;

namespace SNROI.ViewModels.Utilities
{
    public static class DXSkinNameHelper
    {
        public static UserLookAndFeel GetUserLookAndFeelFromApplicationTheme()
        {
            var lookAndFeel = new UserLookAndFeel(null) { UseDefaultLookAndFeel = false };
            var skins = SkinManager.Default.Skins;

            //i hate this
            foreach (SkinContainer skin in skins)
            {
                if (skin.SkinName.Replace(" ", "") != ApplicationThemeHelper.ApplicationThemeName)
                    continue;
                lookAndFeel.SetSkinStyle(skin.SkinName);
                break;
            }

            return lookAndFeel;
        }
    }
}

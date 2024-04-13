using MacGruber;
using noone77521;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Eosin
{
    public partial class VRRenderer
    {
        /// <summary>
        /// 创建标题UI
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected UIDynamicTextInfo CreateTitleUI(string text, bool rightSide = false)
        {
            return CreateTitleUINoLang(Lang.Get(text), rightSide);
        }

        /// <summary>
        /// 创建标题UI（不进行语言处理）
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        protected UIDynamicTextInfo CreateTitleUINoLang(string text, bool rightSide = false)
        {
            var title = Utils.SetupInfoOneLine(this, text, rightSide);
            title.text.alignment = UnityEngine.TextAnchor.MiddleCenter;
            title.text.fontStyle = UnityEngine.FontStyle.Bold;

            return title;
        }

        /// <summary>
        /// 设置Toggle，自动将参数转换为翻译后的显示样式
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        protected JSONStorableBool SetupToggle(string paramName, bool defaultValue, bool rightSide)
        {
            string label = Lang.Get(paramName);

            return Utils.SetupToggle(this, paramName, label, defaultValue, rightSide);
        }

        protected JSONStorableFloat SetupSliderFloat(string paramName, float defaultValue, float minValue, float maxValue, bool rightSide)
        {
            var label = Lang.Get(paramName);

            return Utils.SetupSliderFloat(this, paramName, label, defaultValue, minValue, maxValue, rightSide);
        }

        protected UIDynamicButton SetupButton(string paramName, UnityAction callback, bool rightSide)
        {
            var label = Lang.Get(paramName);

            return Utils.SetupButton(this, paramName, label, callback, rightSide);
        }

        protected JSONStorableStringChooser SetupStringChooser(string paramName, List<string> entries, int defaultIndex, bool rightSide)
        {
            var label = Lang.Get(paramName);
            var displayEntries = entries.Select(e => Lang.Get(e)).ToList();

            return Utils.SetupStringChooser(this, paramName, label, entries, displayEntries, defaultIndex, rightSide);
        }

        protected JSONStorableStringChooser SetupStringChooserNoLang(string paramName, List<string> entries, int defaultIndex, bool rightSide)
        {
            var label = Lang.Get(paramName);
            return Utils.SetupStringChooser(this, paramName, label, entries, entries, defaultIndex, rightSide);
        }

        protected JSONStorableFloat SetupSliderInt(string paramName, int defaultValue, int minValue, int maxValue, bool rightSide)
        {
            var label = Lang.Get(paramName);
            return Utils.SetupSliderInt(this, paramName, label, defaultValue, minValue, maxValue, rightSide);
        }

        protected JSONStorableUrl SetupTexture2DChooser(string paramName, string defaultValue, bool rightSide, TextureSettings settings, TextureSetCallback callback, bool infoText = true)
        {
            var label = Lang.Get(paramName);
            return Utils.SetupTexture2DChooser(this, paramName, label, defaultValue, rightSide, settings, callback, infoText);
        }

        protected JSONStorableColor SetupColor(string paramName, Color color, bool rightSide)
        {
            var label = Lang.Get(paramName);

            return Utils.SetupColor(this, paramName, label, color, rightSide);
        }

        protected JSONStorableFloat SetupSliderFloatWithRange(string paramName, float defaultValue, float minValue, float maxValue, bool rightSide)
        {
            var label = Lang.Get(paramName);
            return Utils.SetupSliderFloatWithRange(this, paramName, label, defaultValue, minValue, maxValue, rightSide);
        }

        protected JSONStorableFloat SetupSliderIntWithRange(string paramName, int defaultValue, int minValue, int maxValue, bool rightSide)
        {
            var label = Lang.Get(paramName);
            return Utils.SetupSliderIntWithRange(this, paramName, label, defaultValue, minValue, maxValue, rightSide);
        }

    }
}

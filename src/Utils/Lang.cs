using MVR.FileManagementSecure;
using System;
using UnityEngine;

namespace noone77521
{
    /// <summary>
    /// 语言处理类
    /// </summary>
    internal class Lang : MSJSONClass
    {
        private static string _PluginPath = "";

        /// <summary>
        /// 初始化语言处理组件，使插件可以读取语言配置文件
        /// </summary>
        /// <param name="pluginPath"></param>
        public static void Init(string pluginPath)
        {
            _PluginPath = pluginPath;
        }

        /// <summary>
        /// 获取指定字符串的翻译
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            return GetInstance().GetValue(key);
        }

        /// <summary>
        /// 获取指定翻译的原始字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string From(string value)
        {
            return GetInstance().FromKey(value);
        }

        /// <summary>
        /// 生成语言配置文件
        /// </summary>
        public static void GenerateProfile()
        {
            GetInstance().GenerateLangProfile();
        }

        /// <summary>
        /// 获取语言参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetValue(string key)
        {
            if (this.HasKey(key))
            {
                return this[key];
            }
            else
            {
                this[key] = key;

                return key;
            }
        }

        /// <summary>
        /// 获取原始语言
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string FromKey(string value)
        {
            foreach (var key in this.Keys)
            {
                var v = this[key].Value;

                if (value == v)
                {
                    return key;
                }
            }

            return value;
        }

        /// <summary>
        /// 语言处理对象
        /// </summary>
        private Lang()
        {

        }

        /// <summary>
        /// 生成语言配置文件
        /// </summary>
        private void GenerateLangProfile()
        {
            this.Save(LangFilePath);

            // 保存完成后打开保存目录
            SuperController.singleton.OpenFolderInExplorer(Config.saveDataPath);
        }

        /// <summary>
        /// 加载语言配置文件
        /// </summary>
        private void LoadProfile()
        {
            // 如果文件存在，则加载语言配置文件
            if (FileManagerSecure.FileExists(LangFilePath))
            {
                this.Load(LangFilePath);
            }
        }

        /// <summary>
        /// 根据系统语言加载语言字典
        /// </summary>
        private void LoadByLanguage()
        {
            var path = Config.saveDataPath;

            if (!string.IsNullOrEmpty(_PluginPath))
            {
                path = _PluginPath;
            }

            var fileName = $"{Application.systemLanguage}";

            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    fileName = "Chinese";
                    break;
                default:
                    break;
            }

            var langFile = path + $"/Lang/{fileName}.json";

            if (!FileManagerSecure.FileExists(langFile))
            {
                langFile = path + $"/{fileName}.json";
            }

            if (FileManagerSecure.FileExists(langFile))
            {
                try
                {
                    this.Load(langFile);
                }
                catch (Exception ex)
                {
                    LogUtil.LogError(ex);
                }
            }
            else if (fileName.Equals("Chinese"))
            {
                LoadChinese();
            }
        }

        private static string LangFilePath
        {
            get
            {
                return Config.saveDataPath + "\\lang.json";
            }
        }
        private static Lang _instance;
        private static object _lock = new object();

        /// <summary>
        /// 语言处理类的单例
        /// </summary>
        private static Lang GetInstance()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new Lang();

                    _instance.LoadByLanguage();

                    _instance.LoadProfile();
                }

                if (string.IsNullOrEmpty(_PluginPath))
                {
                    LogUtil.LogError($"The language module is not initialized and the multi-language support feature does not work.");
                }

                return _instance;
            }
        }

        protected override void BeforeToString()
        {

        }

        /// <summary>
        /// 加载中文
        /// </summary>
        void LoadChinese()
        {
            // 渲染相关的语言设置
            this["Preview"] = "预览";
            this["Preview Stays Open"] = "预览保持打开";
            this["Preview Audio From Cam Pos"] = "从镜头位置预览音频";
            this["Start Playback Unfreezes Motion"] = "开始回放解除动作冻结";
            this["Crosshair"] = "十字准线";
            this["Mute All Sound"] = "将所有声音静音";
            this["Ignore VRAM Warning"] = "忽略VRAM警告";
            this["Preserve Transparency (PNG Only)"] = "保留透明度(仅限PNG)";
            this["Empty and Target Stay Visible"] = "保持目标可见";
            this["Use Post-Processing Effects"] = "使用后期处理效果";
            this["Use Command Buffer Effects"] = "使用命令缓冲区效果";
            this["Preview Size (%)"] = "预览尺寸 (%)";
            this["Play / Pause Video"] = "播放/暂停视频";
            this["Play / Pause Video With Animation"] = "播放/暂停带有动画的视频";
            this["Video Time"] = "视频时间";
            this["Filename"] = "文件名";
            this["Output Resolution"] = "输出分辨率";
            this["PNG (Lossless, Big & Slow)\nTransparency Support"] = "PNG(无损、大而慢)\n透明度支持";
            this["JPEG (Lossy, Small & Fast)\nNo Transparency"] = "JPEG(有损、小而快)／无透明度";
            this["JPEG Quality (%)"] = "JPEG 质量 (%)";
            this["Audio Sample Range (Prevent Popping)"] = "音频采样范围(防止爆音)";
            this["Clear Background"] = "清除背景";
            this["Load Background Image"] = "加载背景图像";
            this["BG Stereo Layout"] = "背景立体声布局";
            this["Mono"] = "单声道";
            this["Left Right"] = "左 右";
            this["Right Left"] = "右 左";
            this["Bottom Top"] = "底部 顶部";
            this["Top Bottom"] = "顶部 底部";
            this["Loop Video"] = "循环视频";
            this["Unpause Video On Render"] = "渲染时取消视频暂停";
            this["Render Background To Output"] = "将背景渲染到输出";
            this["Background Color"] = "背景颜色";
            this["Hide Background Color On Preview Only"] = "仅在预览时隐藏背景颜色";
            this["Take Single Screenshot (F9)"] = "拍摄单张屏幕截图(F9)";
            this["Start Playback and Record Video (F10)"] = "开始播放和录制视频 (F10)";
            this["Record Video (F11) (Escape To Cancel)"] = "录制视频(F11)(Esc取消)";
            this["Seek To Beginning (F12)"] = "跳转到起始位置(F12)";
            this["Record Audio"] = "录制音频";
            this["Resume Last Recording"] = "恢复上次录制";
            this["DAZ BVH Scale (cm units)"] = "DAZ BVH缩放(厘米单位)";
            this["BVH Starts At Origin"] = "BVH从起点开始";
            this["BVH Unoriented Rest Pose"] = "BVH无定向重置动作";
            this["Render Mode"] = "渲染模式";
            this["Flat"] = "平面";
            this["VR180 Stereo"] = "VR180立体";
            this["VR360 Stereo"] = "VR360立体";
            this["VR180 Mono"] = "VR180单幅";
            this["VR360 Mono"] = "VR360单幅";
            this["BVH Animations"] = "BVH动画";
            this["Stereo Mode"] = "立体模式";
            this["Static (Fast, Front View Accurate, Transparency)"] = "静态(快速、前视图准确、可透明)";
            this["Panoramic (Very Slow, All Directions Accurate, Transparency)"] = "全景(非常慢、所有方向准确、可透明)";
            this["120° Triangle Split (Fast, 3 Seams, Most Directions Accurate)"] = "120°三角分割(快速、3个接缝、大多数方向准确)";
            this["90° Square Split (Fast, 4 Seams, Most Directions Accurate)"] = "90°方形分割(快速、4个接缝、方向准确)";
            this["Interpupillary Distance (mm)"] = "瞳孔间距(毫米)";
            this["Cubemap/Panoramic Side Resolution"] = "立方贴图/全景侧面分辨率";
            this["VR & Flat MSAA"] = "VR和平面多重采样去锯齿";
            this["Seconds To Record"] = "录制秒数";
            this["Frame Rate"] = "帧率";
            this["Render Every nth Frame"] = "每 n 帧渲染一次";
            this["Smooth Stitching Overlap (%)"] = "平滑接缝重叠率(%)";
            this["Camera Target"] = "镜头目标";
            this["Flat Horizontal FOV"] = "平面水平FOV";
            this["Flat Supersampling Multiplier"] = "平面超级采样乘法器";
            this["Flat Kernel Mode"] = "平面内核模式";
            this["Linear (Blurring)"] = "线性(模糊)";
            this["Lanczos 3 (Sharpening)"] = "三对角矩阵(锐化)";
            this["Hide Vertical Extremes Size (Degrees)"] = "隐藏垂直端点大小(度)";
            this["Eye Pivot Distance (mm) (Stereo)"] = "眼轴距离(mm)(立体)";
            this["Front FOV (Triangle Stereo)"] = "前FOV(三角立体)";
            this["Hide Seams Size (Triangle/Square)"] = "隐藏接缝大小(三角/方形)";
            this["Hide Seams Parallax (Triangle/Square)"] = "隐藏接缝视差(三角/方形)";
            this["Clear Seam Texture"] = "清除接缝纹理";
            this["Load Seam Texture"] = "加载接缝纹理";
            this["Seam Tint"] = "接缝着色";
            this["Load Background Video (No Autoplay)"] = "加载背景视频（无自动播放）";
            this["Estimated VRAM Usage"] = "估计VRAM使用量";
            this["Effective Frame Rate"] = "有效帧率";
            this["Video Frames Size"] = "视频帧大小";
            this["VR Camera Position Mode"] = "VR镜头位置模式";
            this["Refresh Player"] = "刷新播放器插件";
            this["Refresh Records"] = "刷新渲染记录";
            this["Player Plugin"] = "播放器插件";
            this["Render for MMDShow"] = "MMDShow 渲染设定";
            this["Enable Control Player"] = "允许控制播放器";
            this["Sync FOV"] = "同步FOV";
            this["NoSet"] = "无设置";
            this["Sit"] = "坐姿";
            this["Stand"] = "站姿";
            this["Enabled Camera Motion for VR"] = "VR渲染时启用镜头动作";
            this["This plugin is an improvement based on the VRRenderer plugin created by Eosin. It is licensed under CC BY-SA. The original plugin can be found at https://hub.virtamate.com/resources/11994/. \n\nBuilding upon VRRenderer, this plugin adds support for vertical screen rendering. It also incorporates rendering process control for mmd2timeline Player (MMDShow) and offers support for breakpoint recording and setting initial camera positions for VR recording.\n\nThe VRRenderer plugin utilizes code from MacGruber's Essentials (licensed under CC BY-SA) and Élie Michel's LilyRender360 (licensed under MIT). \n\nSee https://github.com/jamexuxu/Eosin_VRRenderer for further information."] = "此插件是基于Eosin制作的VRRenderer插件进行改进的。它采用了CC BY-SA授权。原始插件可以在https://hub.virtamate.com/resources/11994/找到。\n\n在VRRenderer的基础上，该插件增加了对竖屏渲染的支持。它还针对mmd2timeline Player（MMDShow）做了渲染过程控制，并提供了对断点录制和VR录制时初始相机位置设置的支持。\n\nVRRenderer插件使用了MacGruber的Essentials（采用CC BY-SA授权）和Élie Michel的LilyRender360（采用MIT授权）的代码。\n\n 更多信息请参见：\nhttps://github.com/jamexuxu/Eosin_VRRenderer";

            this["Begin Frame"] = "起始帧";
            this["Capture Records"] = "采集记录";
            this["Capture Record Info"] = "采集记录信息";
            this["Start Capture"] = "开始采集";
            this["Recapture"] = "重新采集";
            this["Continue Capture"] = "继续采集";
            this["Render FPS"] = "渲染FPS";
            this["Aspect Ratio"] = "宽高比";
            this["Resolution Output"] = "输出分辨率";
            this["Portait Mode (Flip AspectRatio)"] = "竖屏模式（翻转纵横比）";
            this["Resolution Multiplier"] = "分辨率倍数";
            this["Downscale Method"] = "缩小比例方法";
            this["MSAA Override"] = "多重采样抗锯齿覆盖";
            this["Image Format"] = "图像格式";
            this["Enable Multi-Threaded Encoding"] = "开启多线程渲染";
            this["Encoder Thread Count"] = "渲染线程数";
            this["Multi-Threaded Encoding Settings"] = "多线程渲染设定";

            this["Stream Push Settings"] = "网络推送设置";
            this["Stream Mode"] = "推送模式";
            this["Host"] = "服务器";
            this["Port"] = "端口";
            this["Don't Stream"] = "不推送（保存本地）";
            this["Stream"] = "推送到服务器";
            this["Stream + Images"] = "推送到服务器+保存本地";
        }
    }
}

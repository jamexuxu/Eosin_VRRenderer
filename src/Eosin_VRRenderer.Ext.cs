using MacGruber;
using noone77521;
using MVR.FileManagementSecure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Experimental.PlayerLoop;

namespace Eosin
{
    struct PlayerItem
    {
        public Atom Atom;
        public string PlayerStoreId;
        public string SettingsStoreId;

        public string Id
        {
            get
            {
                return this.Atom?.uid + " > " + PlayerStoreable?.storeId;
            }
        }
        public JSONStorable PlayerStoreable
        {
            get
            {
                return this.Atom?.GetStorableByID(this.PlayerStoreId);
            }
        }
        public JSONStorable SettingsStoreable
        {
            get
            {
                return this.Atom?.GetStorableByID(this.SettingsStoreId);
            }
        }

    }
    public partial class VRRenderer
    {
        JSONStorableBool _enableControlPlayerJSON;

        JSONStorableStringChooser _playerChooserJSON;
        JSONStorableBool _syncFovJSON;
        JSONStorableBool _enableCameraMotionInVRJSON;

        List<PlayerItem> _playerItems = new List<PlayerItem>();

        List<string> _CaptureRecordList = new List<string> { "New" };

        JSONStorableStringChooser _UICaptureRecordChooser;

        //JSONStorableString _UICaptureRecordInfo;
        JSONStorableFloat _UICaptureFrame;

        /// <summary>
        /// 是否允许从Player同步Fov
        /// </summary>
        bool EnableSyncFovFromPlayer
        {
            get
            {
                if (EnablePlayerRender)
                {
                    return _syncFovJSON.val;
                }

                return false;
            }
        }

        JSONStorable PlayerPlugin
        {
            get
            {
                var playerItem = _playerItems.FirstOrDefault(p => p.Id == _playerChooserJSON.val);

                return playerItem.PlayerStoreable;
            }
        }

        JSONStorable SettingsPlugin
        {
            get
            {
                var playerItem = _playerItems.FirstOrDefault(p => p.Id == _playerChooserJSON.val);

                return playerItem.SettingsStoreable;
            }
        }

        float PlayerFov
        {
            get
            {
                var currentFovJSON = this.PlayerPlugin?.GetFloatJSONParam("Current FOV");

                if (currentFovJSON != null)
                {
                    return currentFovJSON.val;
                }

                return 40f;
            }
        }

        bool ReadyToRendering
        {
            get
            {
                var readyToRenderingJSON = this.PlayerPlugin?.GetBoolJSONParam("Ready To Render");

                if (readyToRenderingJSON != null)
                {
                    return readyToRenderingJSON.val;
                }

                return false;
            }
        }

        float MaxProgressValue
        {
            get
            {
                var maxProgressValueJSON = this.PlayerPlugin?.GetFloatJSONParam("Max Progress Value");

                if (maxProgressValueJSON != null)
                {
                    return maxProgressValueJSON.val;
                }

                return 10f;
            }
        }

        string MMDTitle
        {
            get
            {
                var currentTitleJSON = this.PlayerPlugin?.GetStringJSONParam("Current Title");

                if (currentTitleJSON != null)
                {
                    return currentTitleJSON.val;
                }

                return null;
            }
        }

        string PlayerAudioPath
        {
            get
            {
                // 刷新音频地址
                PlayerPlugin?.CallAction("Refresh Audio Path");
                var currentAudioPahtJSON = this.PlayerPlugin?.GetStringJSONParam("Current Audio");

                if (currentAudioPahtJSON != null)
                {
                    return currentAudioPahtJSON.val;
                }

                return "audio.wav";
            }
        }

        /// <summary>
        /// 数据文件路径
        /// </summary>
        string InfoFilePath
        {
            get
            {
                return SaveDirectory + "record.json";
            }
        }

        string _saveDirectory;

        string SaveDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_saveDirectory))
                    InitSaveDirectory();

                return _saveDirectory;
            }
        }

        /// <summary>
        /// 主目录
        /// </summary>
        string MainPath
        {
            get
            {
                return $"{SCREENSHOT_DIRECTORY}{MMDTitle}/";
            }
        }

        /// <summary>
        /// 是否允许播放器渲染
        /// </summary>
        bool EnablePlayerRender
        {
            get
            {
                return _enableControlPlayerJSON.val && _playerChooserJSON.val != "None";
            }
        }

        string _currentTitle = null;

        void FixedUpdate()
        {
            // 如果没有播放器插件
            if (PlayerPlugin == null)
            {
                RefreshPlayerPluginList();

                return;
            }

            // 检查MMD是否发生了变化
            CheckMMDChanged();
        }

        /// <summary>
        /// 检查MMD是否发生了变化
        /// </summary>
        void CheckMMDChanged()
        {
            var isChanged = (_currentTitle != MMDTitle);

            if (isChanged)
            {
                _currentTitle = MMDTitle;

                GetCaptureRecords();
            }
        }

        void InitSaveDirectory()
        {
            var recordName = _UICaptureRecordChooser.val == "New" ? null : _UICaptureRecordChooser.val;

            if (string.IsNullOrEmpty(recordName))
            {
                recordName = GetFilename();
                var newList = _UICaptureRecordChooser.choices.ToList();
                newList.Add(recordName);
                _UICaptureRecordChooser.choices = newList;
                _UICaptureRecordChooser.displayChoices = newList;
                _UICaptureRecordChooser.val = recordName;
            }

            var title = MMDTitle;

            if (title != null)
            {
                _saveDirectory = $"{SCREENSHOT_DIRECTORY}{title}/{recordName}/";
            }
            else
            {
                _saveDirectory = $"{SCREENSHOT_DIRECTORY}/{recordName}/";
            }

            FileManagerSecure.CreateDirectory(_saveDirectory);
        }

        string oldCameraControlValue;
        string oldCameraAtomValue;
        string oldSyncModeValue;
        bool waitingToReady;

        bool? oldNonVRCameraEnabled;
        bool? oldVRCameraEnabled;

        IEnumerator ReadyToPlayerRender()
        {
            if (!EnablePlayerRender) yield break;

            waitingToReady = true;

            if (SettingsPlugin != null)
            {
                // 非平面模式
                if (renderModeIdx != 0 && renderModeIdx != 5)
                {
                    // 不启用VR镜头
                    if (!_enableCameraMotionInVRJSON.val)
                    {
                        var cameraNonVREnabledJSON = SettingsPlugin.GetBoolJSONParam("Camera Enabled Non-VR");
                        if (cameraNonVREnabledJSON != null)
                        {
                            oldNonVRCameraEnabled = cameraNonVREnabledJSON.val;
                            cameraNonVREnabledJSON.val = false;
                        }
                        var cameraVREnabledJSON = SettingsPlugin.GetBoolJSONParam("Camera Enabled in VR");
                        if (cameraVREnabledJSON != null)
                        {
                            oldVRCameraEnabled = cameraVREnabledJSON.val;
                            cameraVREnabledJSON.val = false;
                        }
                    }
                }

                var cameraControl = SettingsPlugin.GetStringChooserJSONParam("Camera Control");

                if (cameraControl != null)
                {
                    oldCameraControlValue = cameraControl.val;
                    cameraControl.val = CameraControlModes.GetName(CameraControlModes.Atom);
                }

                var cameraAtom = SettingsPlugin.GetStringChooserJSONParam("Camera Atom");

                if (cameraAtom != null)
                {
                    oldCameraAtomValue = cameraAtom.val;
                    cameraAtom.val = containingAtom.uid;
                }

                var syncMode = SettingsPlugin.GetStringChooserJSONParam("Sync Mode");

                if (syncMode != null)
                {
                    oldSyncModeValue = syncMode.val;
                    syncMode.val = syncMode.choices.Last();
                }
            }

            if (PlayerPlugin != null)
            {
                _secondsToRecordChooser.val = MaxProgressValue;

                //InitSaveDirectory();

                var startProgressJSON = PlayerPlugin.GetFloatJSONParam("Start Progress Value");

                if (startProgressJSON != null)
                {
                    startProgressJSON.val = _UICaptureFrame.val / frameRateInt;
                }

                PlayerPlugin.CallAction("Preparing For Rendering");
            }

            SaveToJSON();

            SaveComposeBatFile(MMDTitle, SaveDirectory, PlayerAudioPath);
        }

        void StartPlayerRender()
        {
            waitingToReady = false;

            PlayerPlugin.CallAction("Play");

            BeginRender();
        }

        void EndPlayerRender()
        {
            if (!EnablePlayerRender) return;

            PlayerPlugin.CallAction("Finish Rendering");

            SuperController.singleton.OpenFolderInExplorer(SaveDirectory);

            if (SettingsPlugin != null)
            {
                if (oldNonVRCameraEnabled.HasValue)
                {
                    var cameraNonVREnabledJSON = SettingsPlugin.GetBoolJSONParam("Camera Enabled Non-VR");
                    if (cameraNonVREnabledJSON != null)
                    {
                        cameraNonVREnabledJSON.val = oldNonVRCameraEnabled.Value;
                    }
                }

                if (oldVRCameraEnabled.HasValue)
                {
                    var cameraVREnabledJSON = SettingsPlugin.GetBoolJSONParam("Camera Enabled in VR");
                    if (cameraVREnabledJSON != null)
                    {
                        cameraVREnabledJSON.val = oldVRCameraEnabled.Value;
                    }
                }

                if (!string.IsNullOrEmpty(oldCameraControlValue))
                {
                    var cameraControl = SettingsPlugin.GetStringChooserJSONParam("Camera Control");

                    if (cameraControl != null)
                    {
                        cameraControl.val = oldCameraControlValue;
                    }
                }
                if (!string.IsNullOrEmpty(oldCameraAtomValue))
                {
                    var cameraAtom = SettingsPlugin.GetStringChooserJSONParam("Camera Atom");

                    if (cameraAtom != null)
                    {
                        cameraAtom.val = oldCameraAtomValue;
                    }
                }
                if (!string.IsNullOrEmpty(oldSyncModeValue))
                {
                    var syncMode = SettingsPlugin.GetStringChooserJSONParam("Sync Mode");

                    if (syncMode != null)
                    {
                        syncMode.val = oldSyncModeValue;
                    }
                }
            }
        }
        void BuildExtUI()
        {
            CreateTitleUI("Render for MMDShow", true);

            _enableControlPlayerJSON = SetupToggle("Enable Control Player", true, true);

            _syncFovJSON = SetupToggle("Sync FOV", true, true);
            RegisterBool(_syncFovJSON);

            _enableCameraMotionInVRJSON = SetupToggle("Enabled Camera Motion for VR", false, true);
            RegisterBool(_enableCameraMotionInVRJSON);

            _playerChooserJSON = SetupStringChooser("Player Plugin", new List<string>() { "None" }, 0, true);

            _playerChooserJSON.isStorable = false;
            _playerChooserJSON.isRestorable = false;

            //SetupButton("Refresh Player Plugins", RefreshPlayerPluginList, true);

            // TODO 创建VR录制时的默认镜头位置选项

            // 录制记录
            _UICaptureRecordChooser = SetupStringChooserNoLang("Capture Records", _CaptureRecordList, 0, true);
            _UICaptureRecordChooser.isStorable = false;
            _UICaptureRecordChooser.isRestorable = false;

            //_UICaptureRecordInfo = new JSONStorableString(Lang.Get("Capture Record Info"), "");
            //_UICaptureRecordInfo.isStorable = false;
            //_UICaptureRecordInfo.isRestorable = false;

            //// 抓取记录提示
            //Utils.SetupInfoTextNoScroll(this, _UICaptureRecordInfo,
            //    38.0f, true);

            _UICaptureFrame = SetupSliderInt("Begin Frame", 0, 0, 0, true);
            _UICaptureFrame.isRestorable = false;
            _UICaptureFrame.isStorable = false;

            Utils.SetupTwinButton(this, Lang.Get($"Refresh Player"), RefreshPlayerPluginList, Lang.Get($"Refresh Records"), () => GetCaptureRecords(), true);

            CreateTitleUI("Multi-Threaded Encoding Settings", true);

            enableThreadsToggle = SetupToggle("Enable Multi-Threaded Encoding", true, true);
            numThreadsSlider = SetupSliderIntWithRange("Encoder Thread Count", 4, 1, MAX_ENC_THREADS, true);

            Utils.SetupSpacer(this, 10f, true);

            _UICaptureRecordChooser.setCallbackFunction += s =>
             {
                 if (string.IsNullOrEmpty(s) || s == "New")
                 {
                     _UICaptureFrame.val = 0;
                     //_UICaptureRecordInfo.val = "";
                     // 重置保存目录
                     _saveDirectory = null;
                 }
                 else
                 {
                     LoadCaptureInfo(s);
                 }
             };

            RefreshPlayerPluginList();

            GetCaptureRecords();
        }

        /// <summary>
        /// 获取并更新截取记录
        /// </summary>
        void GetCaptureRecords(string choice = null)
        {
            var list = new List<string> { "New" };

            if (!string.IsNullOrEmpty(MMDTitle) && FileManagerSecure.DirectoryExists(MainPath))
            {
                // 获取主目录下的截取记录目录列表
                var paths = FileManagerSecure.GetDirectories(MainPath);

                foreach (var path in paths)
                {
                    var name = FileManagerSecure.GetFileName(path);

                    list.Add(name);
                }
            }

            _UICaptureRecordChooser.choices = list;
            _UICaptureRecordChooser.displayChoices = list;
            _UICaptureRecordChooser.valNoCallback = list.FirstOrDefault();
            if (string.IsNullOrEmpty(choice))
            {
                _UICaptureRecordChooser.val = _UICaptureRecordChooser.defaultVal;
            }
            else
            {
                _UICaptureRecordChooser.val = choice;
            }
        }

        /// <summary>
        /// 加载截取信息
        /// </summary>
        /// <param name="subdir"></param>
        /// <returns></returns>
        private void LoadCaptureInfo(string subdir)
        {
            try
            {
                // 加载之前的配置
                RestorSettingsFromJSON();

                var files = FileManagerSecure.GetFiles(SaveDirectory, $"*{FileExtName}");

                var currentFrame = 0;

                foreach (var file in files)
                {
                    var fileName = FileManagerSecure.GetFileName(file).Trim('0');
                    var index = fileName.Substring(0, fileName.Length - 4);
                    int fileIndex;

                    if (int.TryParse(index, out fileIndex))
                    {
                        currentFrame = Math.Max(currentFrame, fileIndex);
                    }
                }

                // 计算帧数
                var totalFrames = (int)MaxProgressValue * frameRateInt;
                //_UICaptureRecordInfo.val = $"{Lang.Get("Capture Progress:")}{currentFrame}/{totalFrames}.";
                _UICaptureFrame.max = totalFrames;
                _UICaptureFrame.val = currentFrame;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ex, $"Capturer::LoadCaptureInfo:");
            }
        }

        string FileExtName
        {
            get
            {
                return (myFileFormat == FORMAT_JPG) ? ".jpg" : ".png";
            }
        }

        /// <summary>
        /// 保存合成BAT文件
        /// </summary>
        private void SaveComposeBatFile(string title, string dir, string audio = "audio.wav", string bat = "compose.bat", string outfile = "output.mp4")
        {
            FileManagerSecure.CreateDirectory(dir);

            var batFileName = $"{dir}{bat}";

            var str =
                $"chcp 65001" +
                $"\r\n" +
                $"setlocal enabledelayedexpansion" +
                $"\r\n" +
                $"ffmpeg ^\r\n\t-r {frameRateInt} ^\r\n\t-f image2 ^\r\n\t-i \"%%06d{FileExtName}\" ^\r\n\t-i \"{audio}\" ^\r\n\t-c:v libx265 -crf 18 {outfile}\r\n" +
                //$"ffmpeg -r {fps} -f image2 -i %d{ext} -i \"{_CurrentMMD.AudioSetting.AudioPath}\" -c:v libx265 {outfile}" +
                $"\r\n" +
                $"pause" +
                $"\r\n";

            FileManagerSecure.WriteAllText(batFileName, str);

            LogUtil.Log($"[{title}] {Lang.Get("Rendering Started.")} {Lang.Get("Execute")} \"{batFileName}\" {Lang.Get("to create your video file when the rendering is complete.")}");
        }

        /// <summary>
        /// 保存到JSON数据文件
        /// </summary>
        void SaveToJSON()
        {
            this.SaveJSON(this.GetJSON(), InfoFilePath);
        }

        /// <summary>
        /// 从JSON数据文件恢复
        /// </summary>
        void RestorSettingsFromJSON()
        {
            if (!string.IsNullOrEmpty(InfoFilePath))
            {
                if (FileManagerSecure.FileExists(InfoFilePath))
                {
                    var jc = LoadJSON(InfoFilePath).AsObject;

                    if (jc != null)
                    {
                        this.RestoreFromJSON(jc);
                    }
                }
            }
        }

        void RefreshPlayerPluginList()
        {
            _playerItems.Clear();
            foreach (var atom in GetSceneAtoms())
            {
                string playerStoreId = null;
                string settingsStoreId = null;

                foreach (var storable in atom.GetStorableIDs())
                {
                    if (storable.StartsWith("plugin#"))
                    {
                        if (storable.IndexOf("mmd2timeline.Player") > 7)
                        {
                            playerStoreId = storable;
                        }
                        else if (storable.IndexOf("mmd2timeline.Settings") > 7)
                        {
                            settingsStoreId = storable;
                        }
                    }
                }

                if (playerStoreId != null && settingsStoreId != null)
                {
                    var playerStorable = atom.GetStorableByID(playerStoreId);

                    // 获取渲染支持标识，如果找到标识才会将其加入插件列表
                    var renderFlag = playerStorable?.GetBoolJSONParam("Ready To Render");
                    if (renderFlag != null)
                    {
                        _playerItems.Add(new PlayerItem { Atom = atom, PlayerStoreId = playerStoreId, SettingsStoreId = settingsStoreId });
                    }
                }
            }

            var idList = _playerItems.Select(p => p.Id).ToList();

            idList.Insert(0, "None");

            _playerChooserJSON.choices = idList;
            _playerChooserJSON.displayChoices = idList;
            _playerChooserJSON.val = idList.Last();
        }
    }
}

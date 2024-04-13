using System.Collections.Generic;

namespace noone77521
{
    /// <summary>
    /// 枚举类型处理类
    /// </summary>
    /// <remarks>用于统一进行假枚举类型的处理</remarks>
    internal class EnumClass
    {
        private List<string> dict = new List<string>();

        private EnumClass() { }

        /// <summary>
        /// 初始化枚举类
        /// </summary>
        /// <param name="items"></param>
        public EnumClass(params string[] items)
        {
            foreach (var item in items)
            {
                dict.Add(item);
            }
        }

        /// <summary>
        /// 获取名称列表
        /// </summary>
        public List<string> Names
        {
            get
            {
                return dict.ToArray().ToList();
            }
        }

        /// <summary>
        /// 根据名称获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetValue(string name)
        {
            return dict.IndexOf(name);
        }

        /// <summary>
        /// 根据值获取名称
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetName(int value)
        {
            if (value < 0 || value >= dict.Count)
            {
                value = 0;
            }
            return dict[value];
        }
    }

    /// <summary>
    /// VR位置模式
    /// </summary>
    internal class VRPositionModes
    {
        public const int NoSet = 0;
        public const int Stand = 1;
        public const int Sit = 2;

        /// <summary>
        /// 初始化枚举类
        /// </summary>
        private static EnumClass enums = new EnumClass("NoSet", "Stand", "Sit");

        /// <summary>
        /// 获取名称列表
        /// </summary>
        public static List<string> Names
        {
            get
            {
                return enums.Names;
            }
        }

        /// <summary>
        /// 根据名称获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetValue(string name)
        {
            return enums.GetValue(name);
        }

        /// <summary>
        /// 根据值获取名称
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(int value)
        {
            return enums.GetName(value);
        }
    }

    /// <summary>
    /// 相机控制模式
    /// </summary>
    internal class CameraControlModes
    {
        public const int Original = 0;
        public const int Custom = 1;
        public const int WindowCamera = 2;
        public const int Atom = 3;
        //public const int Evaluation1 = 4;
        //public const int Evaluation2 = 5;

        /// <summary>
        /// 初始化枚举类
        /// </summary>
        private static EnumClass enums = new EnumClass("Original", "Custom", "WindowCamera", "Atom"/*, "Evaluation1", "Evaluation2"*/);

        /// <summary>
        /// 获取名称列表
        /// </summary>
        public static List<string> Names
        {
            get
            {
                return enums.Names;
            }
        }

        /// <summary>
        /// 根据名称获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetValue(string name)
        {
            return enums.GetValue(name);
        }

        /// <summary>
        /// 根据值获取名称
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(int value)
        {
            return enums.GetName(value);
        }
    }
}

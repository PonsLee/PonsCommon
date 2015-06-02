using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pons.Model
{
    /// <summary>
    /// 池内部子项对象
    /// 包一层，添加了用于标记子项对象的生命周期的相关属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectInPool<T> : IDisposable
    {
       private bool _isUsing = false;
        /// <summary>
        /// 是否正在被使用
        /// </summary>
        public bool IsUsing
        {
            get { return _isUsing; }
        }

        private DateTime _lastUseTime;
        /// <summary>
        /// 最后一次使用时间
        /// </summary>
        public DateTime LastUseTime
        {
            get { return _lastUseTime; }
        }

        /// <summary>
        /// 池中的对象实体
        /// </summary>
        public T PoolObject { get; set; }

        public ObjectInPool()
        {
            _isUsing = true;
            _lastUseTime = DateTime.Now;
        }

        /// <summary>
        /// 开始使用
        /// </summary>
        public void Open()
          {
            _isUsing = true;
            _lastUseTime = DateTime.Now;
        }

        /// <summary>
        /// 关闭使用
        /// </summary>
        public void Close()
        {
            if (!_isUsingEnduring)
            {
                _isUsing = false;
            }
            _lastUseTime = DateTime.Now;
        }

        /// <summary>
        /// 扩展数据对象
        /// </summary>
        public object DataContext
        {
            get;
            set;
        }

        private bool _isUsingEnduring = false;
        /// <summary>
        /// 是否长时间使用
        /// </summary>
        public bool IsUsingEnduring
        {
            get { return _isUsingEnduring; }
            set
            {
                _isUsingEnduring = value;
                if (value)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}

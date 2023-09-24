using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGenesis.Core.Classes.Datas
{
    /// <summary>
    /// 内存使用情况
    /// </summary>
    public record MemoryMetrics
    {
        /// <summary>
        /// 总计内存（MB）
        /// </summary>
        public double Total { get; set; }

        /// <summary>
        /// 已使用内存（MB）
        /// </summary>
        public double Used { get; set; }

        /// <summary>
        /// 空闲内存（MB）
        /// </summary>
        public double Free { get; set; }
    }
}

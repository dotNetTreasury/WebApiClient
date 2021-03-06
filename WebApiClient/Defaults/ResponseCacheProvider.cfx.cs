﻿#if !NET45 && !NET46

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示Api响应结果缓存提供者的接口
    /// </summary>
    public class ResponseCacheProvider : Disposable, IResponseCacheProvider
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static readonly ResponseCacheProvider Instance = new ResponseCacheProvider();

        /// <summary>
        /// 内存缓存
        /// </summary>
        private readonly MemoryCache cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        /// <summary>
        /// 获取提供者的友好名称
        /// </summary>
        public string Name { get; } = nameof(MemoryCache);

        /// <summary>
        /// 从缓存中获取响应实体
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public Task<ResponseCacheResult> GetAsync(string key)
        {
            if (this.cache.TryGetValue(key, out var value) == false)
            {
                var result = ResponseCacheResult.NoValue;
                return Task.FromResult(result);
            }
            else
            {
                var val = value as ResponseCacheEntry;
                var result = new ResponseCacheResult(val, true);
                return Task.FromResult(result);
            }
        }

        /// <summary>
        /// 设置响应实体到缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="entry">缓存实体</param>
        /// <param name="expiration">有效时间</param>
        /// <returns></returns>
        public Task SetAsync(string key, ResponseCacheEntry entry, TimeSpan expiration)
        {
            this.cache.Set(key, entry, DateTimeOffset.Now.Add(expiration));
            return ApiTask.CompletedTask;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected override void Dispose(bool disposing)
        {
            this.cache.Dispose();
        }
    }
}
#endif
﻿namespace Foundation.BlazorExtensions
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.JSInterop;
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public abstract class StorageBase
    {
        private readonly string _fullTypeName;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        protected internal StorageBase()
        {   
            _fullTypeName = GetType().FullName.Replace('.', '_');
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public Task ClearAsync(IJSRuntime jsRuntime) => jsRuntime.InvokeAsync<object>($"{_fullTypeName}.Clear");


        
        public Task<string> GetItemAsync(string key,IJSRuntime jsRuntime)
        {
            return jsRuntime.InvokeAsync<string>($"{_fullTypeName}.GetItem", key);
        }

        public async Task<T> GetItemAsync<T>(string key,IJSRuntime jsRuntime)
        {
            var json = await GetItemAsync(key,jsRuntime);
            return string.IsNullOrWhiteSpace(json) ? default(T) : JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
        }

        
        public Task<string> KeyAsync(int index,IJSRuntime jsRuntime)
        {
            return jsRuntime.InvokeAsync<string>($"{_fullTypeName}.Key", index);
        }

        public Task RemoveItemAsync(string key,IJSRuntime jsRuntime)
        {
            return jsRuntime.InvokeAsync<object>($"{_fullTypeName}.RemoveItem", key);
        }

        [Obsolete()]
        public void RemoveItem(string key,IJSRuntime jsRuntime)
        {
            ((IJSInProcessRuntime)jsRuntime).Invoke<object>($"{_fullTypeName}.RemoveItem", key);
        }

        public Task SetItemAsync(string key, string data,IJSRuntime jsRuntime)
        {
            return jsRuntime.InvokeAsync<object>($"{_fullTypeName}.SetItem", key, data);
        }
        

        public Task SetItemAsync<T>(string key, T data,IJSRuntime jsRuntime)
        {
            return SetItemAsync(key, JsonSerializer.Serialize<T>(data, _jsonSerializerOptions),jsRuntime);
        }

       
    }

    public sealed class LocalStorage : StorageBase
    {

    }

    public sealed class SessionStorage : StorageBase
    {

    }

   
}

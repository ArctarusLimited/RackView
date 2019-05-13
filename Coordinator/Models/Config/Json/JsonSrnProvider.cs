using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Coordinator.Helpers;
using Coordinator.Models.Config.Json;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Coordinator.Models.Config.Json
{
    /// <summary>
    /// SRN provider that uses JSON flatfiles as a backing store.
    /// </summary>
    [SrnProvider("json", true)]
    public class JsonSrnProvider : ISrnProvider
    {
        private readonly string _servicePath;
        private readonly IDictionary<string, JsonSrnDictionary> _cache = new Dictionary<string, JsonSrnDictionary>();

        private readonly FileSystemWatcher _watcher;

        public JsonSrnProvider(IHostingEnvironment env)
        {
            _servicePath = Path.Combine(env.ContentRootPath, "Data/srn_service");
            Directory.CreateDirectory(_servicePath);

            _watcher = new FileSystemWatcher(_servicePath) {EnableRaisingEvents = true};

            // We want to remove changed / deleted files from the cache
            // To me in the morning: fucking hell it's 02:40am why am I still doing this pls question this code, it's not very efficient lol
            _watcher.Changed += (sender, args) => { PurgeFromCache(args.Name); };
            _watcher.Deleted += (sender, args) => { PurgeFromCache(args.Name); };
        }

        private async Task<JsonSrnDictionary> GetCacheDictAsync(string nameSpace, bool createIfNotExist = false)
        {
            if (_cache.ContainsKey(nameSpace)) return _cache[nameSpace];

            // Store not in the cache, we need to load it from the filesystem.
            var path = BuildPath(nameSpace);
            if (File.Exists(path))
            {
                _cache[nameSpace] = JsonConvert.DeserializeObject<JsonSrnDictionary>(
                    await File.ReadAllTextAsync(BuildPath(nameSpace)), new DynamicDictConverter<JsonSrnDictionary>());
            }
            else if (createIfNotExist)
            {
                _cache[nameSpace] = new JsonSrnDictionary();
            }
            else
            {
                return null;
            }

            return _cache[nameSpace];
        }

        public async Task<dynamic> GetAsync(Srn srn)
        {
            if (!srn.HasNamespace())
            {
                // Retrieve all namespaces
                return Directory.GetFiles(_servicePath).Select(Path.GetFileNameWithoutExtension);
            }

            var obj = await GetCacheDictAsync(srn.Namespace);
            if (!srn.HasKey()) return obj;

            return obj?[srn.Key];
        }

        public async Task SetAsync(Srn srn, dynamic value)
        {
            if (!srn.HasNamespace() || !srn.HasKey()) throw new Exception("Setting namespaces directly is not supported.");

            var obj = await GetCacheDictAsync(srn.Namespace, true);
            if (obj == null) return;

            obj[srn.Key] = value;

            // Save the JSON file.
            _watcher.EnableRaisingEvents = false;
            await File.WriteAllTextAsync(BuildPath(srn.Namespace), JsonConvert.SerializeObject(obj));
            _watcher.EnableRaisingEvents = true;
        }

        public async Task DeleteAsync(Srn srn)
        {
            if (!srn.HasKey())
            {
                // we're deleting the namespace itself
                _cache.Remove(srn.Namespace);
                _watcher.EnableRaisingEvents = false;
                File.Delete(BuildPath(srn.Namespace));
                _watcher.EnableRaisingEvents = true;

                return;
            }

            await SetAsync(srn, null);
        }

        private string BuildPath(string nameSpace)
        {
            return Path.Combine(_servicePath, $"{nameSpace}.json");
        }

        private void PurgeFromCache(string filePath)
        {
            var name = Path.GetFileNameWithoutExtension(filePath);
            if (_cache.ContainsKey(name)) _cache.Remove(name);
        }
    }
}

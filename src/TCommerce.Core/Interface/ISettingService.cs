using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Configuration;

namespace TCommerce.Core.Interface
{
    public interface ISettingService
    {
        Task<T> LoadSettingAsync<T>() where T : ISettings, new();
        Task<ISettings> LoadSettingAsync(Type type);
        Task SaveSettingAsync<T>(T settings) where T : ISettings;
    }
}

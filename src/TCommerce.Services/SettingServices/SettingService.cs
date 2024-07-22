using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Configuration;
using TCommerce.Data;
using TCommerce.Services.DbManageServices;
using TCommerce.Services.IRepositoryServices;

namespace TCommerce.Services.SettingServices
{
    public class SettingService : ISettingService
    {
        private readonly IRepository<Setting> _settingRepository;
        private readonly IServiceProvider _serviceProvider;
        public SettingService(IRepository<Setting> settingRepository, IServiceProvider serviceProvider)
        {
            _settingRepository = settingRepository;
            _serviceProvider = serviceProvider;
        }

        public async Task<T> LoadSettingAsync<T>() where T : ISettings, new()
        {
            var settings = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var setting = await _settingRepository.Table.FirstOrDefaultAsync(s => s.Name == property.Name);
                if (setting != null)
                {
                    // Deserialize JSON string back to the property type
                    var value = JsonConvert.DeserializeObject(setting.Value, property.PropertyType);
                    property.SetValue(settings, value);
                }
            }

            return settings;
        }

        public async Task<ISettings> LoadSettingAsync(Type type)
        {
            var settings = Activator.CreateInstance(type);

            if (!DatabaseManager.IsDatabaseInstalled())
                return settings as ISettings;

            foreach (var prop in type.GetProperties())
            {
                var setting = await _settingRepository.Table.FirstOrDefaultAsync(s => s.Name == prop.Name);
                if (setting != null)
                {
                    // Deserialize JSON string back to the property type
                    var value = JsonConvert.DeserializeObject(setting.Value, prop.PropertyType);
                    prop.SetValue(settings, value);
                }
            }

            return settings as ISettings;   
        }

        public async Task SaveSettingAsync<T>(T settings) where T : ISettings
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                // Serialize property value to JSON string
                var value = JsonConvert.SerializeObject(property.GetValue(settings));
                var setting = await _settingRepository.Table.FirstOrDefaultAsync(s => s.Name == property.Name);

                if (setting == null)
                {
                    setting = new Setting { Name = property.Name, Value = value };
                    await _settingRepository.CreateAsync(setting);
                }
                else
                {
                    setting.Value = value;
                    await _settingRepository.UpdateAsync(setting);
                }
            }
        }
    }
}

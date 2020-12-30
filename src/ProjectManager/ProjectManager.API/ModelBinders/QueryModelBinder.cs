using ProjectManager.Core.Helpers;
using ProjectManager.Queries;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ProjectManager.API.ModelBinders
{
    public class QueryModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            bindingContext.ThrowIfNull(nameof(bindingContext));

            object model;

            if (bindingContext.HttpContext.Request.QueryString.HasValue)
            {
                var queryStringParameters = bindingContext.HttpContext
                                                    .Request
                                                    .Query
                                                    .ToDictionary(pair => pair.Key, pair => pair.Value.ToString());

                var jsonString = JsonConvert.SerializeObject(queryStringParameters);

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CustomResolver()
                };

                model = JsonConvert.DeserializeObject(jsonString, bindingContext.ModelType, settings);
            }
            else
            {
                model = Activator.CreateInstance(bindingContext.ModelType);
            }

            BindRouteDataToModel(bindingContext, model);

            bindingContext.Model = model;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);

            return Task.CompletedTask;
        }

        private static void BindRouteDataToModel(ModelBindingContext bindingContext, object model)
        {
            foreach (var (key, value) in bindingContext.ActionContext.RouteData.Values)
            {
                if (key.EqualsAnyIgnoreCase("action", "controller"))
                {
                    continue;
                }

                ReflectionHelper.SetProperty(model, key.FirstCharToUpper(), value);
            }
        }
    }

    public class QueryModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var @interface = context.Metadata
                                    .ModelType
                                    .GetInterfaces()
                                    .FirstOrDefault(i => i.Name.StartsWith(nameof(IQuery<object>)));
            return @interface != null ? new QueryModelBinder() : null;
        }
    }

    internal class CustomResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);

            contract.Converter = new ComplexTypeConverter();

            return contract;
        }
    }

    internal class ComplexTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object rootObject = Activator.CreateInstance(objectType);

            foreach (var token in JToken.ReadFrom(reader))
            {
                var propertyInfo = rootObject.GetType()
                   .GetProperty(token.Path, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    var tk = token as JProperty;
                    if (propertyInfo.PropertyType.IsArray || propertyInfo.PropertyType.IsInterface)
                    {
                        var complexObject = tk.Value != null
                            ? JsonConvert.DeserializeObject(tk.Value.ToString(), propertyInfo.PropertyType)
                            : null;

                        propertyInfo.SetValue(rootObject, complexObject);
                    }
                    else
                    {
                        propertyInfo.SetValue(rootObject, Convert.ChangeType
                          (tk.Value, propertyInfo.PropertyType.UnderlyingSystemType), null);
                    }
                }
            }

            return rootObject;
        }

        public override bool CanConvert(Type objectType) => true;

        public override bool CanWrite => true;
    }
}
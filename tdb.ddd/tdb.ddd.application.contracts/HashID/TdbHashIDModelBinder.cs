using Microsoft.AspNetCore.Mvc.ModelBinding;
using tdb.ddd.contracts;

namespace tdb.ddd.application.contracts
{
    /// <summary>
    /// 自动解密ID并绑定模型
    /// </summary>
    public class TdbHashIDModelBinder : IModelBinder
    {
        /// <summary>
        /// 绑定模型
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            var str = valueProviderResult.FirstValue;
            bindingContext.Result = ModelBindingResult.Success(TdbHashID.DecodeSingleLong(str));

            return Task.CompletedTask;
        }
    }
}

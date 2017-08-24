using Orchard;
using Orchard.Alias;
using Orchard.Conditions.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Tags.Models;
using System;
using System.Linq;

namespace RW.Orchard.Conditions.Providers
{
    [OrchardFeature("RW.Orchard.Conditions.ContentId")]
    public sealed class ContentIdCondition : IConditionProvider
    {
        private readonly IAliasService _aliasService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        public ContentIdCondition(IAliasService aliasService, IWorkContextAccessor workContextAccessor, IContentManager contentManager)
        {
            _aliasService = aliasService;
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
        }
        public void Evaluate(ConditionEvaluationContext evaluationContext)
        {
            if (!string.Equals(evaluationContext.FunctionName, "contentid", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var contentId = -1;

            var parsingSuccess = int.TryParse(Convert.ToString(evaluationContext.Arguments[0]), out contentId);
            
            var _currentContent = GetContentItemForCurrentRequest();

            if (parsingSuccess)
            {
                evaluationContext.Result = _currentContent.Id == contentId;
            }
            else
            {
                evaluationContext.Result = true;
            }
        }

        private IContent GetContentItemForCurrentRequest()
        {
            var requestUrl = _workContextAccessor.GetContext().HttpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(1).Trim('/');
            var requestItemRoute = _aliasService.Get(requestUrl);

            return requestItemRoute == null ? _contentManager.New("Dummy") : _contentManager.Get(Convert.ToInt32(requestItemRoute["Id"]));
        }
    }
}
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
    [OrchardFeature("RW.Orchard.Conditions.Tag")]
    public sealed class TagCondition : IConditionProvider
    {
        private readonly IAliasService _aliasService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        public TagCondition(IAliasService aliasService, IWorkContextAccessor workContextAccessor, IContentManager contentManager)
        {
            _aliasService = aliasService;
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
        }
        public void Evaluate(ConditionEvaluationContext evaluationContext)
        {
            if (!string.Equals(evaluationContext.FunctionName, "tag", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var tagName = Convert.ToString(evaluationContext.Arguments[0]);

            var _currentContent = GetContentItemForCurrentRequest();

            if (_currentContent.Has<TagsPart>())
            {
                var tagPart = _currentContent.Get<TagsPart>();
                evaluationContext.Result = tagPart.CurrentTags != null && tagPart.CurrentTags.Any(tag => tag.Equals(tagName, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                evaluationContext.Result = false;
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
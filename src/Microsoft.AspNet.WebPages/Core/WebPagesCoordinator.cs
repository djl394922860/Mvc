﻿using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModel;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.WebPages.Core
{
    // This class acts as a controller, but is named differently so it only gets exposed through
    // the WebPagesActionDescriptorProvider.
    [WebPagesDefaultActionConvention]
    public class WebPagesCoordinator
    {
        private static readonly char[] PathSeparators = new [] { '/', '\\' };

        [Activate]
        public ViewDataDictionary ViewData { get; set; }

        [Activate]
        public ICompositeViewEngine ViewEngine { get; set; }

        [Activate]
        public ActionContext ActionContext { get; set; }

        [Activate]
        public IOptionsAccessor<WebPagesOptions> OptionsAccessor { get; set; }

        [WebPagesDefaultActionConvention]
        public IActionResult WebPagesView(string viewPath)
        {
            viewPath = OptionsAccessor.Options.PagesFolderPath.TrimEnd(PathSeparators) 
                + "/"
                + viewPath 
                + ".cshtml";

            var result = ViewEngine.FindView(ActionContext, viewPath);

            if (result.Success)
            {
                return new WebPagesViewResult(result.View, ViewData);
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        private class WebPagesDefaultActionConvention : Attribute, IActionModelConvention
        {
            public void Apply([NotNull]ActionModel model)
            {
                model.ApiExplorerIsVisible = false;
            }
        }

        public class RouteTemplate : IRouteTemplateProvider
        {
            public readonly string _viewAttributeRouteFormatString = "{0}/{{*viewPath}}";

            private string _urlPrefix;

            public RouteTemplate([NotNull] string urlPrefix)
            {
                _urlPrefix = urlPrefix;
            }

            public string Name { get { return "__WebPages__"; } }

            public int? Order { get { return null; } }

            public string Template
            {
                get
                {
                    return string.Format(_viewAttributeRouteFormatString, _urlPrefix.Trim(PathSeparators));
                }
            }
        }
    }
}

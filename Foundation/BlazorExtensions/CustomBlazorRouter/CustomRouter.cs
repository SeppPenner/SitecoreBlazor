﻿using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.RenderTree;
using Microsoft.AspNetCore.Blazor.Services;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Foundation.BlazorExtensions.CustomBlazorRouter
{
    /// <summary>
    /// A component that displays whichever other component corresponds to the
    /// current navigation location.
    /// </summary>
    public class CustomRouter : IComponent, IDisposable
    {
        private static readonly char[] _queryOrHashStartChar = new char[2]
        {
          '?',
          '#'
        };
        private RenderHandle _renderHandle;
        private string _baseUri;
        private string _locationAbsolute;

        [Inject]
        private IUriHelper UriHelper { get; set; }

        [Parameter] private RouterDataRoot RouteValues { get; set; }
        /// <summary>
        /// Gets or sets the assembly that should be searched, along with its referenced
        /// assemblies, for components matching the URI.
        /// </summary>
        [Parameter] private Assembly AppAssembly { get; set; }

        private RouteTable Routes { get; set; }

        public void Init(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
            _baseUri = UriHelper.GetBaseUri();
            _locationAbsolute = UriHelper.GetAbsoluteUri();
            UriHelper.OnLocationChanged += OnLocationChanged;
        }

        public void SetParameters(ParameterCollection parameters)
        {
            parameters.AssignToProperties(this);
            //var types = ComponentResolver.ResolveComponents(AppAssembly);
            //Routes = RouteTable.Create(types);
            Routes = RouteTable.CreateNew(RouteValues);
            Refresh();
        }

       
        /// <inheritdoc />
        public void Dispose()
        {
            UriHelper.OnLocationChanged -= OnLocationChanged;
        }

        private string StringUntilAny(string str, char[] chars)
        {
            var firstIndex = str.IndexOfAny(chars);
            return firstIndex < 0
                ? str
                : str.Substring(0, firstIndex);
        }

        protected virtual void Render(RenderTreeBuilder builder, Type handler, IDictionary<string, object> parameters)
        {
            builder.OpenComponent(0, typeof(LayoutDisplay));
            builder.AddAttribute(1, LayoutDisplay.NameOfPage, handler);
            builder.AddAttribute(2, LayoutDisplay.NameOfPageParameters, parameters);
            builder.CloseComponent();
        }

     
        private void Refresh()
        {
            var locationPath = UriHelper.ToBaseRelativePath(_baseUri, _locationAbsolute);
            locationPath = StringUntilAny(locationPath, _queryOrHashStartChar);
            var context = new RouteContext(locationPath);
            Routes.Route(context);

            //Not valid route
            if (context.Handler == null)
                context = SetErrorContext();
           
            if (!typeof(IComponent).IsAssignableFrom(context.Handler))
            {
                throw new InvalidOperationException($"The type {context.Handler.FullName} " +
                                                    $"does not implement {typeof(IComponent).FullName}.");
            }

            _renderHandle.Render(builder => Render(builder, context.Handler, context.Parameters));
        }

        private RouteContext SetErrorContext()
        {
            var errContext = new RouteContext("error");
            Routes.Route(errContext);
            RouteContext context = errContext;
            return context;
        }

        private void OnLocationChanged(object sender, string newAbsoluteUri)
        {
            _locationAbsolute = newAbsoluteUri;
            if (_renderHandle.IsInitialized)
            {
                Refresh();
            }
        }
    }
}

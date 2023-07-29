// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable UnusedMember.Global
namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a URL, optionally adding parameters to it.
    /// </summary>
    [CreateAssetMenu(fileName = "Url", menuName = "Doozy/Bindy/Transformer/Url", order = -950)]
    public class UrlTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a URL, optionally adding parameters to it.\n\n" +
            "This UrlFormatter supports adding URL parameters to a URL. It accepts a list of parameters, where each parameter consists of a name and a value.\n\n" +
            "The transformer adds the parameters to the URL as query string parameters. " +
            "If the URL already contains a query string, the parameters are added to the end of the query string, separated by an ampersand (&). " +
            "Otherwise, the parameters are added to the URL as the first query string parameter, separated by a question mark (?).";

        protected override Type[] fromTypes => new[] { typeof(string) };
        protected override Type[] toTypes => new[] { typeof(string) };

        [SerializeField] private List<Parameter> Parameters = new();
        /// <summary>
        /// List of parameters to add to the URL.
        /// </summary>
        public List<Parameter> parameters => Parameters;

        /// <summary>
        /// Transforms a URL before it is displayed in a UI component.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            if (!(source is string url)) return source;

            var listOfParameters =
                Parameters
                    .Select(parameter => new KeyValuePair<string, string>(parameter.Name, parameter.Value))
                    .ToList();

            if (listOfParameters.Count == 0) return source;

            url = AddParametersToUrl(url, listOfParameters);
            return url;
        }

        /// <summary>
        /// Adds parameters to a URL.
        /// </summary>
        /// <param name="url"> The URL to add parameters to. </param>
        /// <param name="parameters"> The parameters to add to the URL. </param>
        /// <returns> The URL with the parameters added. </returns>
        private static string AddParametersToUrl(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            string separator = url.Contains("?") ? "&" : "?";
            return url + separator + string.Join("&", parameters.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
        }

        /// <summary>
        /// Parameter for the URL transformer.
        /// </summary>
        [Serializable]
        public class Parameter
        {
            /// <summary>
            /// Name of the parameter.
            /// </summary>
            public string Name;

            /// <summary>
            /// Parameter value.
            /// </summary>
            public string Value;
        }
    }
}

// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Bindy.Transformers;
using UnityEditor;

namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(ObjectTransformer), true)]
    public class ObjectTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
    }
}

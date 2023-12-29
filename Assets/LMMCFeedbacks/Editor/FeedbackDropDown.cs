using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Rendering.Universal;

namespace LMMCFeedbacks.Editor
{
    public class FeedbackDropDown : AdvancedDropdown
    {
        private readonly List<string> _feedbackList = new();
        public event Action<AdvancedDropdownItem> OnSelect;
        
        public FeedbackDropDown(AdvancedDropdownState state) : base(state)
        {
            var types = ReflectionUtility.FindClassesImplementing<IFeedback>();
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                if (instance is IFeedback custom) _feedbackList.Add(custom.Name);
            }
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Root");

            for( int i = 0; i < _feedbackList.Count; i++ )
            {
                var path = _feedbackList[i].Split('/');
                var parent = root;

                foreach (var name in path)
                {
                    var name1 = name;
                    var child = parent.children.FirstOrDefault(x => x.name == name1);

                    if( child == null )
                    {
                        child = new AdvancedDropdownItem(name)
                        {
                            id = i
                        };
                        parent.AddChild(child);
                    }

                    parent = child;
                }
            }

            return root;
        }
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            OnSelect?.Invoke(item);
        }
    }
}
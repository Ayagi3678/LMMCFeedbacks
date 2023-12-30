using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace LMMCFeedbacks.Editor
{
    public class FeedbackDropDown : AdvancedDropdown
    {
        private readonly List<IFeedback> _feedbacks = new();
        public event Action<Type> OnSelect;
        
        public FeedbackDropDown(AdvancedDropdownState state) : base(state)
        {
            var types = ReflectionUtility.FindClassesImplementing<IFeedback>();
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                if (instance is IFeedback custom) _feedbacks.Add(custom);
            }
            _feedbacks= _feedbacks.OrderByDescending(x => x.Name.Split('/').Length).ToList();
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Root");

            for( int i = 0; i < _feedbacks.Count; i++ )
            {
                var path = _feedbacks[i].Name.Split('/');
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
            OnSelect?.Invoke(_feedbacks[item.id].GetType());
        }
    }
}
using System;
using UnityEngine;

namespace Utilities.DisableIf
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DisableIfAttribute : PropertyAttribute {
        
        public string FieldName { get; private set; }
        public Condition Cond { get; private set; }

        public DisableIfAttribute(string fieldName, Condition cond) {
            FieldName = fieldName;
            Cond = cond;
        }
    
        public enum Condition {
            IS_TRUE,
            IS_FALSE
        }
    }
}

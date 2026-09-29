using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DIP.Attributes
{

    /// <summary>
    /// Validates that a property is required only when specific conditions are NOT met (AND logic)
    /// Usage: [ConditionalRequired(ErrorMessage = "License Type is required")]
    /// Or use with trigger values to skip requirement
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConditionalRequiredCustomAttribute : ValidationAttribute
    {
        private readonly List<(string PropertyName, string[] ExcludedValues)> _conditions = new();

        public ConditionalRequiredCustomAttribute()
        {
        }

        /// <summary>
        /// Skip requirement when <paramref name="skipWhenProperty"/> equals any of <paramref name="skipWhenValues"/>.
        /// </summary>
        public ConditionalRequiredCustomAttribute(string skipWhenProperty, params string[] skipWhenValues)
        {
            _conditions.Add((skipWhenProperty, skipWhenValues));
        }

        /// <summary>
        /// Skip requirement when either condition property matches its excluded value.
        /// </summary>
        public ConditionalRequiredCustomAttribute(
            string skipWhenProperty1, string skipWhenValue1,
            string skipWhenProperty2, string skipWhenValue2)
        {
            _conditions.Add((skipWhenProperty1, new[] { skipWhenValue1 }));
            _conditions.Add((skipWhenProperty2, new[] { skipWhenValue2 }));
        }

        /// <summary>
        /// Add a condition: property must NOT equal any of these values for requirement to apply
        /// </summary>
        public void When(string propertyName, params string[] excludedValues)
        {
            _conditions.Add((propertyName, excludedValues));
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            // If value is already provided, it's valid
            if (!string.IsNullOrWhiteSpace(value?.ToString()))
                return ValidationResult.Success;

            // Check all conditions - ALL conditions must be met for validation to apply
            bool shouldValidate = true;

            foreach (var (propertyName, excludedValues) in _conditions)
            {
                var conditionProp = context.ObjectType.GetProperty(propertyName);
                if (conditionProp == null)
                {
                    return new ValidationResult($"Unknown property: {propertyName}");
                }

                var propValue = conditionProp.GetValue(context.ObjectInstance)?.ToString() ?? string.Empty;

                // If property matches excluded values, skip the requirement
                if (excludedValues.Contains(propValue))
                {
                    shouldValidate = false;
                    break;
                }
            }

            // If validation should apply and value is empty
            if (shouldValidate)
            {
                return new ValidationResult(
                    FormatErrorMessage(context.MemberName ?? ""),
                    new[] { context.MemberName ?? "" }
                );
            }

            return ValidationResult.Success;
        }
    }
}
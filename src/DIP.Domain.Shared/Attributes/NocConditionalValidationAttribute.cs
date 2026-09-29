using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace DIP.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class NocConditionalValidationAttribute : ValidationAttribute
{
    private readonly string _conditionProperty;
    private readonly HashSet<string> _triggerValues;
    private readonly List<Type> _innerTypes;

    public NocConditionalValidationAttribute(
        string conditionProperty,
        string[] triggerValues,
        params Type[] innerAttributeTypes)
    {
        _conditionProperty = conditionProperty;
        _triggerValues = new HashSet<string>(triggerValues);
        _innerTypes = innerAttributeTypes.ToList();
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var conditionProp = context.ObjectType.GetProperty(_conditionProperty);
        if (conditionProp == null)
            return new ValidationResult($"Unknown property: {_conditionProperty}");

        var conditionValue = conditionProp.GetValue(context.ObjectInstance)?.ToString();

        // If condition matches → apply inner validations
        if (_triggerValues.Contains(conditionValue ?? string.Empty))
        {
            foreach (var type in _innerTypes)
            {
                var innerAttr = (ValidationAttribute)Activator.CreateInstance(type)!;

                // If attribute has a property called OtherProperty, set it.
                var otherProp = type.GetProperty("OtherProperty");
                if (otherProp != null)
                {
                    otherProp.SetValue(innerAttr, "BasicRent");
                }

                var result = innerAttr.GetValidationResult(value, context);
                if (result != ValidationResult.Success)
                    return result;
            }
        }

        return ValidationResult.Success;
    }
}




public class LessThanAttribute : ValidationAttribute
{
    public string? OtherProperty { get; set; } = "BasicRent";

    public LessThanAttribute() { }

    public LessThanAttribute(string otherProperty)
    {
        OtherProperty = otherProperty ?? "BasicRent";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (OtherProperty == null)
            return new ValidationResult("OtherProperty is required.");

        var otherPropInfo = validationContext.ObjectType.GetProperty(OtherProperty);
        if (otherPropInfo == null)
            return new ValidationResult($"Unknown property: {OtherProperty}");

        var currentValue = ConvertToNullableDecimal(value);
        var otherValue = ConvertToNullableDecimal(otherPropInfo.GetValue(validationContext.ObjectInstance));

        if (currentValue.HasValue && otherValue.HasValue)
        {
            if (currentValue >= otherValue)
            {
                return new ValidationResult(
                    ErrorMessage ?? $"Utility Charge must be less than Basic Rent.",
                    [validationContext.MemberName!]
                );
            }
        }

        return ValidationResult.Success;
    }

    private decimal? ConvertToNullableDecimal(object? value)
    {
        if (value == null) return null;

        try
        {
            return Convert.ToDecimal(value);
        }
        catch
        {
            return null;
        }
    }
}

// Fromdate and To date Validation check
public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateGreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var currentValue = (DateTime?)value;

        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
        var comparisonValue = (DateTime?)property.GetValue(validationContext.ObjectInstance);

        if (currentValue.HasValue && comparisonValue.HasValue)
        {
            if (currentValue <= comparisonValue)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.MemberName} should be greater than {_comparisonProperty}", [validationContext.MemberName!]);
            }
        }

        return ValidationResult.Success;
    }
}


[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ConditionalRangeForNocAttribute : ValidationAttribute
{
    private readonly string _conditionProperty;
    private readonly object _min;
    private readonly object _max;

    public ConditionalRangeForNocAttribute(string conditionProperty, object min, object max)
    {
        _conditionProperty = conditionProperty;
        _min = min;
        _max = max;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var conditionProp = validationContext.ObjectType.GetProperty(_conditionProperty);
        if (conditionProp == null)
            return new ValidationResult($"Unknown property: {_conditionProperty}");

        var conditionValue = conditionProp.GetValue(validationContext.ObjectInstance)?.ToString();

        if (conditionValue != "1")
            return ValidationResult.Success;

        if (value == null)
            return ValidationResult.Success;

        var intValue = Convert.ToInt32(value);
        var minValue = Convert.ToInt32(_min);
        var maxValue = Convert.ToInt32(_max);

        if (intValue < minValue || intValue > maxValue)
        {
            // Use ErrorMessageResourceType if set

            return new ValidationResult(
                ErrorMessage ?? $"{validationContext.MemberName} must be greater than zero.",
                [validationContext.MemberName!]
            );
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// When IsSisterCompany is 2 (Yes) and PropertyBillable is false,
/// security deposit may be zero; otherwise it must be greater than zero.
/// If PropertyBillable is not present on the model, deposit must be greater than zero (same as before).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SecurityDepositConditionalRangeAttribute : ValidationAttribute
{
    private const string PropertyBillableName = "PropertyBillable";
    private const string IsSisterCompanyName = "IsSisterCompany";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var instance = validationContext.ObjectInstance;
        var type = validationContext.ObjectType;

        var isSisterProp = type.GetProperty(IsSisterCompanyName);
        var billableProp = type.GetProperty(PropertyBillableName);

        int isSisterCompany = 0;
        if (isSisterProp != null)
            isSisterCompany = Convert.ToInt32(isSisterProp.GetValue(instance) ?? 0);

        bool propertyBillable = true;
        if (billableProp != null && billableProp.PropertyType == typeof(bool))
            propertyBillable = (bool)(billableProp.GetValue(instance) ?? true);

        var allowZero = isSisterCompany == 2 && !propertyBillable;
        var minInclusive = allowZero ? 0d : 1d;
        const double maxInclusive = int.MaxValue;

        double amount;
        try
        {
            amount = Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
        catch (InvalidCastException)
        {
            return new ValidationResult(
                ErrorMessage ?? "Security Deposit must be a valid number.",
                [validationContext.MemberName!]);
        }
        catch (FormatException)
        {
            return new ValidationResult(
                ErrorMessage ?? "Security Deposit must be a valid number.",
                [validationContext.MemberName!]);
        }
        catch (OverflowException)
        {
            return new ValidationResult(
                ErrorMessage ?? "Security Deposit must be a valid number.",
                [validationContext.MemberName!]);
        }

        if (amount < minInclusive || amount > maxInclusive)
        {
            string message;
            if (!allowZero)
                message = ErrorMessage ?? "Security Deposit must be greater than zero.";
            else if (amount < 0)
                message = "Security Deposit cannot be negative.";
            else
                message = "Security Deposit exceeds the maximum allowed value.";
            return new ValidationResult(message, [validationContext.MemberName!]);
        }

        return ValidationResult.Success;
    }
}


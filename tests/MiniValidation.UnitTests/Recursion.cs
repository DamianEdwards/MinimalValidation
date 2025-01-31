﻿using Microsoft.Extensions.DependencyInjection;

namespace MiniValidation.UnitTests;

public class Recursion
{
    [Fact]
    public void Recurses_When_Top_Level_Is_Invalid()
    {
        var thingToValidate = new TestType { RequiredName = null, Child = new TestChildType { RequiredCategory = null, MinLengthFive = "123" } };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Equal(3, errors.Count);
        Assert.Collection(errors,
            entry => Assert.Equal($"{nameof(TestType.RequiredName)}", entry.Key),
            entry => Assert.Equal($"{nameof(TestType.Child)}.{nameof(TestChildType.RequiredCategory)}", entry.Key),
            entry => Assert.Equal($"{nameof(TestType.Child)}.{nameof(TestChildType.MinLengthFive)}", entry.Key)
        );
    }

    [Fact]
    public void Invalid_When_Child_Invalid_And_Recurse_True()
    {
        var thingToValidate = new TestType { Child = new TestChildType { RequiredCategory = null, MinLengthFive = "123" } };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Equal(2, errors.Count);
    }

    [Fact]
    public void Invalid_When_Child_Invalid_And_Recurse_Default()
    {
        var thingToValidate = new TestType { Child = new TestChildType { RequiredCategory = null } };

        var result = MiniValidator.TryValidate(thingToValidate, out var errors);

        Assert.False(result);
        Assert.Single(errors);
    }

    [Fact]
    public void Valid_When_Child_Invalid_And_Recurse_False()
    {
        var thingToValidate = new TestType { Child = new TestChildType { RequiredCategory = null, MinLengthFive = "123" } };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: false, out var errors);

        Assert.True(result);
        Assert.Empty(errors);
    }

    [Fact]
    public void Valid_When_Child_Invalid_And_Property_Decorated_With_SkipRecursion()
    {
        var thingToValidate = new TestType { SkippedChild = new TestChildType { RequiredCategory = null, MinLengthFive = "123" } };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: false, out var errors);

        Assert.True(result);
        Assert.Empty(errors);
    }

    [Fact]
    public void Invalid_When_Enumerable_Item_Invalid_When_Recurse_Default()
    {
        var thingToValidate = new List<TestType> { new() { Child = new TestChildType { RequiredCategory = null, MinLengthFive = "123" } } };

        var result = MiniValidator.TryValidate(thingToValidate, out var errors);

        Assert.False(result);
        Assert.Equal(2, errors.Count);
    }

    [Fact]
    public void Invalid_When_Enumerable_Item_Invalid_When_Recurse_True()
    {
        var thingToValidate = new List<TestType> { new() { Child = new TestChildType { RequiredCategory = null, MinLengthFive = "123" } } };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Equal(2, errors.Count);
    }

    [Fact]
    public void Valid_When_Enumerable_Item_Invalid_When_Recurse_False()
    {
        var thingToValidate = new List<TestType> { new() { Child = new TestChildType { RequiredCategory = null, MinLengthFive = "123" } } };
        
        var result = MiniValidator.TryValidate(thingToValidate, recurse: false, out _);

        Assert.True(result);
    }

    [Fact]
    public void Valid_When_Enumerable_Item_Has_Invalid_Descendant_But_Property_Decorated_With_SkipRecursion()
    {
        var thingToValidate = new List<TestType> { new() { SkippedChild = new() { RequiredCategory = null } } };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out _);

        Assert.True(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(11)]
    public void Error_Message_Keys_For_Descendants_Are_Formatted_Correctly(int maxDepth)
    {
        var thingToValidate = new TestType { Child = new() };

        TestChildType.AddDescendents(thingToValidate.Child, maxDepth);

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Single(errors);

        var messagePrefix = string.Concat(Enumerable.Repeat($"{nameof(TestType.Child)}.", maxDepth + 1));
        var entry = Assert.Single(errors);
        Assert.Equal($"{messagePrefix}{nameof(TestChildType.RequiredCategory)}", entry.Key);
    }

    [Fact]
    public void Error_Message_Keys_For_Root_Enumerable_Are_Formatted_Correctly()
    {
        var thingToValidate = new List<TestType>
        {
            new() ,
            new() { RequiredName = null, TenOrMore = 5 },
        };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Equal(2, errors.Count);
        Assert.Collection(errors,
            entry => Assert.Equal($"[1].{nameof(TestType.RequiredName)}", entry.Key),
            entry => Assert.Equal($"[1].{nameof(TestType.TenOrMore)}", entry.Key));
    }

    [Fact]
    public void Error_Message_Keys_For_Descendant_Enumerable_Are_Formatted_Correctly()
    {
        var thingToValidate = new TestType();
        thingToValidate.Children.Add(new() { });
        thingToValidate.Children.Add(new() { RequiredCategory = null });

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        var entry = Assert.Single(errors);
        Assert.Equal($"{nameof(TestType.Children)}[1].{nameof(TestChildType.RequiredCategory)}", entry.Key);
    }

    [Fact]
    public void First_Error_In_Root_Enumerable_Returns_Immediately()
    {
        var thingToValidate = new List<TestType>
        {
            new() { RequiredName = null },
            new() { RequiredName = null },
        };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        var entry = Assert.Single(errors);
        Assert.Equal($"[0].{nameof(TestType.RequiredName)}", entry.Key);
    }

    [Fact]
    public void First_Error_In_Descendant_Enumerable_Returns_Immediately()
    {
        var thingToValidate = new TestType();
        thingToValidate.Children.Add(new() { MinLengthFive = "123" });
        thingToValidate.Children.Add(new() { RequiredCategory = null });

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        var entry = Assert.Single(errors);
        Assert.Equal($"{nameof(TestType.Children)}[0].{nameof(TestChildType.MinLengthFive)}", entry.Key);
    }

    [Fact]
    public void All_Errors_From_Invalid_Item_In_Descendant_Enumerable_Reported()
    {
        var thingToValidate = new TestType();
        thingToValidate.Children.Add(new());
        thingToValidate.Children.Add(new() { RequiredCategory = null, MinLengthFive = "123" });

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Equal(2, errors.Count);
        Assert.Collection(errors,
            entry => Assert.Equal($"{nameof(TestType.Children)}[1].{nameof(TestChildType.RequiredCategory)}", entry.Key),
            entry => Assert.Equal($"{nameof(TestType.Children)}[1].{nameof(TestChildType.MinLengthFive)}", entry.Key));
    }

    [Fact]
    public void Valid_When_Descendant_Invalid_And_Property_Decorated_With_SkipRecursion()
    {
        var thingToValidate = new TestType();
        thingToValidate.Children.Add(new());
        thingToValidate.Children.Add(new() { SkippedChild = new() { RequiredCategory = null } });

        var result = MiniValidator.TryValidate(thingToValidate, recurse: false, out var errors);

        Assert.True(result);
        Assert.Empty(errors);
    }

    [Fact]
    public void Invalid_When_Descendant_Invalid_And_Property_Is_Required_And_Decorated_With_SkipRecursion()
    {
        var thingToValidate = new TestSkippedChildType();

        var result = MiniValidator.TryValidate(thingToValidate, recurse: false, out var errors);

        Assert.False(result);
        Assert.Single(errors);
    }

    [Fact]
    public void Invalid_When_Derived_Type_Has_Invalid_Inherited_Property()
    {
        var thingToValidate = new TestType { Child = new TestChildTypeDerivative { RequiredCategory = null } };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Single(errors);
    }

    [Fact]
    public void Invalid_When_Derived_Type_Has_Invalid_Own_Property()
    {
        var thingToValidate = new TestType { Child = new TestChildTypeDerivative { DerivedMinLengthTen = "123" } };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, out var errors);

        Assert.False(result);
        Assert.Single(errors);
    }

    [Fact]
    public void Valid_When_Derived_Type_Has_Invalid_Own_Property_With_Recurse_False()
    {
        var thingToValidate = new TestType { Child = new TestChildTypeDerivative { DerivedMinLengthTen = "123" } };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: false, out var errors);

        Assert.True(result);
        Assert.Empty(errors);
    }

    [Fact]
    public void Invalid_When_ValidatableObject_Child_Validate_Is_Invalid()
    {
        var thingToValidate = new TestValidatableType
        {
            ValidatableChild = new TestValidatableChildType { TwentyOrMore = 12 }
        };

        var result = MiniValidator.TryValidate(thingToValidate, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        Assert.Equal($"{nameof(TestValidatableType.ValidatableChild)}.{nameof(TestValidatableType.TwentyOrMore)}", errors.Keys.First());
    }

    [Fact]
    public void Invalid_When_Derived_ValidatableObject_Child_Validate_Is_Invalid()
    {
        var thingToValidate = new TestValidatableType
        {
            Child = new TestValidatableChildType { TwentyOrMore = 19 }
        };

        var result = MiniValidator.TryValidate(thingToValidate, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        Assert.Equal($"{nameof(TestValidatableType.Child)}.{nameof(TestValidatableType.TwentyOrMore)}", errors.Keys.First());
    }

    [Fact]
    public void Invalid_When_Derived_Polymorphic_Child_Validate_Is_Invalid()
    {
        var thingToValidate = new TestValidatableType
        {
            Child = new TestValidatableChildType { MinLengthFive = "123" }
        };

        var result = MiniValidator.TryValidate(thingToValidate, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        Assert.Equal($"{nameof(TestValidatableType.Child)}.{nameof(TestValidatableChildType.MinLengthFive)}", errors.Keys.First());
    }

    [Fact]
    public void Child_ValidatableObject_Is_Validated_When_Parent_Is_Invalid()
    {
        var thingToValidate = new TestValidatableType
        {
            TenOrMore = 9,
            ValidatableChild = new() { TwentyOrMore = 12 }
        };

        var result = MiniValidator.TryValidate(thingToValidate, out var errors);

        Assert.False(result);
        Assert.Equal(2, errors.Count);
        Assert.Equal($"{nameof(TestValidatableType.TenOrMore)}", errors.Keys.First());
    }

    [Fact]
    public void Invalid_When_Derived_ValidatableOnlyChild_Is_Invalid()
    {
        var thingToValidate = new TestValidatableType
        {
            ValidatableOnlyChild = new TestValidatableOnlyType { TwentyOrMore = 12 }
        };

        var result = MiniValidator.TryValidate(thingToValidate, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        Assert.Equal($"{nameof(TestValidatableType.ValidatableOnlyChild)}.{nameof(TestValidatableOnlyType.TwentyOrMore)}", errors.Keys.First());
    }

    [Fact]
    public void Invalid_When_Polymorphic_ValidatableOnlyChild_Is_Invalid()
    {
        var thingToValidate = new TestValidatableType
        {
            PocoChild = new TestValidatableOnlyType { TwentyOrMore = 12 }
        };

        var result = MiniValidator.TryValidate(thingToValidate, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        Assert.Equal($"{nameof(TestValidatableType.PocoChild)}.{nameof(TestValidatableOnlyType.TwentyOrMore)}", errors.Keys.First());
    }

    [Fact]
    public void Invalid_When_Polymorphic_Child_With_Validation_Attributes_Is_Invalid()
    {
        var thingToValidate = new TestValidatableType
        {
            PocoChild = new TestChildType { MinLengthFive = "123" }
        };

        var result = MiniValidator.TryValidate(thingToValidate, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        Assert.Equal($"{nameof(TestValidatableType.PocoChild)}.{nameof(TestChildType.MinLengthFive)}", errors.Keys.First());
    }

    [Fact]
    public void Throws_InvalidOperationException_When_AsyncValidatableOnlyChild_Is_Invalid_Without_Allowing_SyncOverAsync()
    {
        var thingToValidate = new TestTypeWithAsyncChild
        {
            NeedsAsync = new() { TwentyOrMore = 12 }
        };

        Assert.Throws<ArgumentException>(() =>
        {
            var result = MiniValidator.TryValidate(thingToValidate, out var errors);
        });
    }

    [Fact]
    public async Task Invalid_When_AsyncValidatableOnlyChild_Is_Invalid()
    {
        var thingToValidate = new TestTypeWithAsyncChild
        {
            NeedsAsync = new() { TwentyOrMore = 12 }
        };

        var (isValid, errors) = await MiniValidator.TryValidateAsync(thingToValidate);

        Assert.False(isValid);
        Assert.Single(errors);
        Assert.Equal($"{nameof(TestTypeWithAsyncChild.NeedsAsync)}.{nameof(TestAsyncValidatableChildType.TwentyOrMore)}", errors.Keys.First());
    }

    [Fact]
    public void Invalid_When_AsyncValidatableOnlyChild_Is_Invalid_Allowing_SyncOverAsync()
    {
        var thingToValidate = new TestTypeWithAsyncChild
        {
            NeedsAsync = new() { TwentyOrMore = 12 }
        };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, allowAsync: true, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        Assert.Equal($"{nameof(TestTypeWithAsyncChild.NeedsAsync)}.{nameof(TestAsyncValidatableChildType.TwentyOrMore)}", errors.Keys.First());
    }

    [Fact]
    public void Throws_InvalidOperationException_When_Polymorphic_AsyncValidatableOnlyChild_Is_Invalid_Without_Allowing_SyncOverAsync()
    {
        var thingToValidate = new TestValidatableType
        {
            PocoChild = new TestAsyncValidatableChildType { TwentyOrMore = 12 }
        };

        Assert.Throws<InvalidOperationException>(() =>
        {
            var tcs = new TaskCompletionSource<object?>();
            var serviceProvider = new ServiceCollection().AddSingleton<Task>(tcs.Task).BuildServiceProvider();

            try
            {
                var result = MiniValidator.TryValidate(thingToValidate, serviceProvider, out var errors);
            }
            finally
            {
                tcs.SetResult(null);
            }
        });
    }

    [Fact]
    public void Invalid_When_Polymorphic_AsyncValidatableOnlyChild_Is_Invalid_Allowing_SyncOverAsync()
    {
        var thingToValidate = new TestValidatableType
        {
            PocoChild = new TestAsyncValidatableChildType { TwentyOrMore = 12 }
        };

        var result = MiniValidator.TryValidate(thingToValidate, recurse: true, allowAsync: true, out var errors);

        Assert.False(result);
        Assert.Single(errors);
        Assert.Equal($"{nameof(TestValidatableType.PocoChild)}.{nameof(TestValidatableOnlyType.TwentyOrMore)}", errors.Keys.First());
    }

    [Fact]
    public async Task Invalid_When_Polymorphic_AsyncValidatableOnlyChild_Is_Invalid()
    {
        var thingToValidate = new TestValidatableType
        {
            PocoChild = new TestAsyncValidatableChildType { TwentyOrMore = 12 }
        };

        var (isValid, errors) = await MiniValidator.TryValidateAsync(thingToValidate);

        Assert.False(isValid);
        Assert.Single(errors);
        Assert.Equal($"{nameof(TestValidatableType.PocoChild)}.{nameof(TestAsyncValidatableChildType.TwentyOrMore)}", errors.Keys.First());
    }
    
    [Fact]
    public async Task Throws_When_Validates_With_Recurse_And_Object_Has_Not_Implemented_Property()
    {
        var thingToValidate = new TestTypeWithNotImplementedProperty
        {
            PropertyToBeRequired = "test1"
        };


        await Assert.ThrowsAsync<Exception>(async () => await MiniValidator.TryValidateAsync(thingToValidate, recurse: true));
    }
    
    [Fact]
    public async Task DoesntThrow_When_Validates_Without_Recurse_And_Object_Has_Not_Implemented_Property()
    {
        var thingToValidate = new TestTypeWithNotImplementedProperty
        {
            PropertyToBeRequired = "test1"
        };

        var (isValid, _) = await MiniValidator.TryValidateAsync(thingToValidate, recurse: false);

        Assert.True(isValid);
    }
}
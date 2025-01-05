using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMiniValidator();
builder.Services.AddClassMiniValidator<WidgetValidator>();

var app = builder.Build();

app.MapSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World");

app.MapGet("/widgets", () =>
    new[] {
        new Widget { Name = "Shinerizer" },
        new Widget { Name = "Sparklizer" }
    });

app.MapGet("/widgets/{name}", (string name) =>
    new Widget { Name = name });

app.MapPost("/widgets", Results<ValidationProblem, Created<Widget>> (Widget widget, IMiniValidator validator) =>
{
    if (!validator.TryValidate(widget, out var errors))
    {
        return TypedResults.ValidationProblem(errors);
    }

    return TypedResults.Created($"/widgets/{widget.Name}", widget);
});

app.MapPost("/widgets/class-validator", async Task<Results<ValidationProblem, Created<WidgetWithClassValidator>>> (WidgetWithClassValidator widget, IMiniValidator<WidgetWithClassValidator> validator) =>
{
    var (isValid, errors) = await validator.TryValidateAsync(widget);
    if (!isValid)
    {
        return TypedResults.ValidationProblem(errors);
    }

    return TypedResults.Created($"/widgets/{widget.Name}", widget);
});

app.MapPost("/widgets/custom-validation", Results<ValidationProblem, Created<WidgetWithCustomValidation>> (WidgetWithCustomValidation widget, IMiniValidator<WidgetWithCustomValidation> validator) =>
{
    if (!validator.TryValidate(widget, out var errors))
    {
        return TypedResults.ValidationProblem(errors);
    }

    return TypedResults.Created($"/widgets/{widget.Name}", widget);
});

app.Run();

class Widget
{
    [Required, MinLength(3), Display(Name = "Widget name")]
    public string? Name { get; set; }

    public override string? ToString() => Name;
}

class WidgetWithClassValidator : Widget
{
    [Required, MinLength(3), Display(Name = "Widget name")]
    public string? Name { get; set; }

    public override string? ToString() => Name;
}

class WidgetWithCustomValidation : Widget, IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.Equals(Name, "Widget", StringComparison.OrdinalIgnoreCase))
        {
            yield return new($"Cannot name a widget '{Name}'.", [nameof(Name)]);
        }
    }
}

class WidgetValidator : IValidate<WidgetWithClassValidator>
{
    public IEnumerable<ValidationResult> Validate(WidgetWithClassValidator instance, ValidationContext validationContext)
    {
        if (string.Equals(instance.Name, "Widget", StringComparison.OrdinalIgnoreCase))
        {
            return [new($"Cannot name a widget '{instance.Name}'.", [nameof(instance.Name)])];
        }

        return [];
    }
}

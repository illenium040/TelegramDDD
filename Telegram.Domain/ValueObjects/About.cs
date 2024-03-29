﻿using Newtonsoft.Json;
using Telegram.Domain.Primitivies;
using Telegram.Domain.Shared;

namespace Telegram.Domain.ValueObjects;

public class About : ValueObject<string>
{
    public static readonly int MaxLength = 150;

    [JsonConstructor]
    private About(string value) : base(value) { }

    public static Result<About> Create(string? about) => about?.Length > MaxLength
            ? Result.Failure<About>(new Error(
                "About.Create",
                $"Length of about is more than {MaxLength}"
                ))
            : Result.Success<About>(new(about ?? ""));

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}

// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Reporting;

/// <summary>
/// Represents a severity level for the library.
/// Even though the severity instances have a name, their identities are defined by their priotiries only.
/// </summary>
public readonly struct Severity : IEquatable<Severity>, IComparable<Severity>
{
    /// <summary>
    /// A simple note to the user.
    /// </summary>
    public static readonly Severity Note = new("note", 0);

    /// <summary>
    /// Help text for the user.
    /// </summary>
    public static readonly Severity Help = new("help", 1);

    /// <summary>
    /// A warning for potential mistake.
    /// </summary>
    public static readonly Severity Warning = new("warning", 2);

    /// <summary>
    /// Error.
    /// </summary>
    public static readonly Severity Error = new("error", 3);

    /// <summary>
    /// An internal error, signaling an internal bug or ICE.
    /// </summary>
    public static readonly Severity InternalError = new("internal error", 4);

    /// <summary>
    /// The name of the severity level.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The priority of the severity level.
    /// </summary>
    public readonly int Priority;

    /// <summary>
    /// Initializes a new instance of the <see cref="Severity"/> struct.
    /// </summary>
    /// <param name="name">The name of the severity level.</param>
    /// <param name="priority">The priority of the severity level.</param>
    public Severity(string name, int priority)
    {
        this.Name = name;
        this.Priority = priority;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Severity s && this.Equals(s);

    /// <inheritdoc/>
    public bool Equals(Severity other) => this.Priority == other.Priority;

    /// <inheritdoc/>
    public override int GetHashCode() => this.Priority.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(Severity other) => this.Priority - other.Priority;

    /// <summary>
    /// Compares two <see cref="Severity"/>s for equality.
    /// </summary>
    /// <param name="left">The first <see cref="Severity"/> to compare.</param>
    /// <param name="right">The second <see cref="Severity"/> to compare.</param>
    /// <returns>True, if <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
    public static bool operator ==(Severity left, Severity right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="Severity"/>s for inequality.
    /// </summary>
    /// <param name="left">The first <see cref="Severity"/> to compare.</param>
    /// <param name="right">The second <see cref="Severity"/> to compare.</param>
    /// <returns>True, if <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
    public static bool operator !=(Severity left, Severity right) => !left.Equals(right);

    /// <summary>
    /// Less-than compares two <see cref="Severity"/>s.
    /// </summary>
    /// <param name="left">The first <see cref="Severity"/> to compare.</param>
    /// <param name="right">The second <see cref="Severity"/> to compare.</param>
    /// <returns>True, if <paramref name="left"/> is less severe than <paramref name="right"/>.</returns>
    public static bool operator <(Severity left, Severity right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Less-than or equals compares two <see cref="Severity"/>s.
    /// </summary>
    /// <param name="left">The first <see cref="Severity"/> to compare.</param>
    /// <param name="right">The second <see cref="Severity"/> to compare.</param>
    /// <returns>True, if <paramref name="left"/> is less severe than <paramref name="right"/>
    /// or they are equal.</returns>
    public static bool operator <=(Severity left, Severity right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Greater-than compares two <see cref="Severity"/>s.
    /// </summary>
    /// <param name="left">The first <see cref="Severity"/> to compare.</param>
    /// <param name="right">The second <see cref="Severity"/> to compare.</param>
    /// <returns>True, if <paramref name="left"/> is more severe than <paramref name="right"/>.</returns>
    public static bool operator >(Severity left, Severity right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Greater-than or equals compares two <see cref="Severity"/>s.
    /// </summary>
    /// <param name="left">The first <see cref="Severity"/> to compare.</param>
    /// <param name="right">The second <see cref="Severity"/> to compare.</param>
    /// <returns>True, if <paramref name="left"/> is more severe than <paramref name="right"/>
    /// or they are equal.</returns>
    public static bool operator >=(Severity left, Severity right) => left.CompareTo(right) >= 0;
}

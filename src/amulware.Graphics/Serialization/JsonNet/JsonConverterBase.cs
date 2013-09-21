﻿// This code file is an almost direct copy of NodaConverterBase.cs
// from the Noda Time library. Original copyright notice:

// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0

// We thank the authors of Noda Time for sharing this code!

using System;
using Newtonsoft.Json;
using System.IO;

namespace amulware.Graphics.Serialization.JsonNet
{
    /// <summary>
    /// Base class for all the Json.NET converters.
    /// This deals handles all the boilerplate code dealing with nullity.
    /// </summary>
    /// <typeparam name="T">The type to convert to/from JSON.</typeparam>
    public abstract class JsonConverterBase<T> : JsonConverter
    {
        private static readonly Type NullableT = typeof(T).IsValueType ? typeof(Nullable<>).MakeGenericType(typeof(T)) : typeof(T);

        /// <summary>
        /// Returns whether or not this converter supports the given type.
        /// </summary>
        /// <param name="objectType">The type to check for compatibility.</param>
        /// <returns>True if the given type is supported by this converter (including the nullable form for
        /// value types); false otherwise.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType) || objectType == NullableT;
        }

        /// <summary>
        /// Converts the JSON stored in a reader into the relevant Noda Time type.
        /// </summary>
        /// <param name="reader">The Json.NET reader to read data from.</param>
        /// <param name="objectType">The type to convert the JSON to.</param>
        /// <param name="existingValue">An existing value; ignored by this converter.</param>
        /// <param name="serializer">A serializer to use for any embedded deserialization.</param>
        /// <exception cref="InvalidDataException">The JSON was invalid for this converter.</exception>
        /// <returns>The deserialized value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType != NullableT)
                {
                    throw new InvalidDataException(string.Format("Cannot convert null value to {0}.", objectType));
                }
                return null;
            }

            // Handle empty strings automatically
            if (reader.TokenType == JsonToken.String)
            {
                string value = (string)reader.Value;
                if (value == "")
                {
                    if (objectType != NullableT)
                    {
                        throw new InvalidDataException(string.Format("Cannot convert null value to {0}.", objectType));
                    }
                    return null;
                }
            }

            // Delegate to the concrete subclass. At this point we know that we don't want to return null, so we
            // can ask the subclass to return a T, which we will box. That will be valid even if objectType is
            // T? because the boxed form of a non-null T? value is just the boxed value itself.

            // Note that we don't currently pass existingValue down; we could change this if we ever found a use for it.
            return readJsonImpl(reader, serializer);
        }

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final conversion from a non-null JSON value to
        /// a value of type T.
        /// </summary>
        /// <param name="reader">The JSON reader to pull data from</param>
        /// <param name="serializer">The serializer to use for nested serialization</param>
        protected abstract T readJsonImpl(JsonReader reader, JsonSerializer serializer);

        /// <summary>
        /// Writes the given value to a Json.NET writer.
        /// </summary>
        /// <param name="writer">The writer to write the JSON to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="serializer">The serializer to use for any embedded serialization.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Json.NET should prevent this happening, but let's validate...
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // Note: don't need to worry about value is T? due to the way boxing works.
            // Again, Json.NET probably prevents us from needing to check this, really.
            if (!(value is T))
            {
                throw new ArgumentException(string.Format("Unexpected value when converting. Expected {0}, got {1}.", typeof(T).FullName, value.GetType().FullName));
            }
            writeJsonImpl(writer, (T)value, serializer);
        }

        /// <summary>
        /// Implemented by concrete subclasses, this performs the final write operation for a non-null value of type T
        /// to JSON.
        /// </summary>
        /// <param name="writer">The writer to write JSON data to</param>
        /// <param name="value">The value to serializer</param>
        /// <param name="serializer">The serializer to use for nested serialization</param>
        protected abstract void writeJsonImpl(JsonWriter writer, T value, JsonSerializer serializer);
    }
}

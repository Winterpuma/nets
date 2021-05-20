using System;
using DataClassLibrary;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SolveTask.Server
{
    public class ResultFigPosConverter : JsonConverter<ResultFigPos>
    {
        private readonly JsonEncodedText xVal = JsonEncodedText.Encode("xCenter");
        private readonly JsonEncodedText yVal = JsonEncodedText.Encode("yCenter");
        private readonly JsonEncodedText angle = JsonEncodedText.Encode("angle");
        private readonly JsonConverter<int> _intConverter;

        public ResultFigPosConverter(JsonSerializerOptions options)
        {
            if (options?.GetConverter(typeof(int)) is JsonConverter<int> intConverter)
            {
                _intConverter = intConverter;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override ResultFigPos Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            };

            int x = 0;
            bool xSet = false;

            int y = 0;
            bool ySet = false;

            int a = 0;
            bool aSet = false;

            // Get the first property.
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            if (reader.ValueTextEquals(xVal.EncodedUtf8Bytes))
            {
                x = ReadProperty(ref reader, options);
                xSet = true;
            }
            else if (reader.ValueTextEquals(yVal.EncodedUtf8Bytes))
            {
                y = ReadProperty(ref reader, options);
                ySet = true;
            }
            else if (reader.ValueTextEquals(angle.EncodedUtf8Bytes))
            {
                a = ReadProperty(ref reader, options);
                aSet = true;
            }
            else
            {
                throw new JsonException();
            }

            // Get the second property.
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            if (reader.ValueTextEquals(xVal.EncodedUtf8Bytes))
            {
                x = ReadProperty(ref reader, options);
                xSet = true;
            }
            else if (reader.ValueTextEquals(yVal.EncodedUtf8Bytes))
            {
                y = ReadProperty(ref reader, options);
                ySet = true;
            }
            else if (reader.ValueTextEquals(angle.EncodedUtf8Bytes))
            {
                a = ReadProperty(ref reader, options);
                aSet = true;
            }
            else
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            if (reader.ValueTextEquals(xVal.EncodedUtf8Bytes))
            {
                x = ReadProperty(ref reader, options);
                xSet = true;
            }
            else if (reader.ValueTextEquals(yVal.EncodedUtf8Bytes))
            {
                y = ReadProperty(ref reader, options);
                ySet = true;
            }
            else if (reader.ValueTextEquals(angle.EncodedUtf8Bytes))
            {
                a = ReadProperty(ref reader, options);
                aSet = true;
            }
            else
            {
                throw new JsonException();
            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return new ResultFigPos("", x, y, a);
        }

        private int ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            //Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);

            reader.Read();
            return _intConverter.Read(ref reader, typeof(int), options);
        }

        public override void Write(Utf8JsonWriter writer, ResultFigPos value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

}

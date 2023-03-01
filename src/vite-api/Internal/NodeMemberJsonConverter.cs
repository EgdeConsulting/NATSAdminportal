using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace vite_api.Internal;

public sealed class NodeMemberJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(NodeMember<>);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var typeValue = typeToConvert.GetGenericArguments()[0];

        var converter = (JsonConverter)Activator.CreateInstance(
            typeof(NodeMemberJsonConverterInner<>).MakeGenericType(typeValue),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[] { options },
            culture: null)!;

        return converter;
    }

    private sealed class NodeMemberJsonConverterInner<T> : JsonConverter<NodeMember<T>>
    {
        private readonly JsonConverter<T> _valueConverter;

        public NodeMemberJsonConverterInner(JsonSerializerOptions options)
        {
            _valueConverter = (JsonConverter<T>)options.GetConverter(typeof(T));
        }

        public override NodeMember<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var root = new NodeMember<T>();
            ReadIntoNodeMember(ref reader, typeToConvert, options, root);
            while (reader.Read()) { /* reading to the end */ }
            return root;
        }

        private void ReadIntoNodeMember(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options,
            NodeMember<T> parent)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Unexpected token type");

            reader.Read();

            // We want those in this order for simplicity, even if it make serialization
            // performance worse. Shouldn't be a concern for this, so favouring simplicity.
            if (!TryLocateProperty(reader, "name", out var nameReader))
                throw new JsonException();

            T? value = default;
            if (TryLocateProperty(reader, "value", out var valueReader))
                _valueConverter.Read(ref valueReader, typeToConvert, options);

            var name = nameReader.GetString();
            if (string.IsNullOrEmpty(name))
                throw new JsonException();

            var self = parent.AddChild(name, value);
            if (TryLocateProperty(reader, "subSubjects", out var subSubjectsReader))
                ReadIntoNodeMember(ref subSubjectsReader, typeToConvert, options, self);
        }

        public override void Write(Utf8JsonWriter writer, NodeMember<T> value, JsonSerializerOptions options)
        {
            // It's a little ugly, but we're using empty name to indicate it is the root node.
            if (string.IsNullOrEmpty(value.Name))
            {
                WriteRoot(writer, value, options);
            }
            else
            {
                WriteMember(writer, value, options);
            }
        }

        private void WriteRoot(Utf8JsonWriter writer, NodeMember<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var child in value.Children)
                WriteMember(writer, child, options);
            writer.WriteEndArray();
        }

        private void WriteMember(Utf8JsonWriter writer, NodeMember<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("name");
            writer.WriteStringValue(value.Name);

            if (options.DefaultIgnoreCondition > JsonIgnoreCondition.WhenWritingNull || value.Value != null)
            {
                if (value.Value == null)
                {
                    writer.WriteNull("value");
                }
                else
                {
                    writer.WritePropertyName("value");
                    _valueConverter.Write(writer, value.Value, options);
                }
            }

            if (options.DefaultIgnoreCondition > JsonIgnoreCondition.WhenWritingDefault || value.Children.Any())
            {
                writer.WriteStartArray("subSubjects");

                foreach (var child in value.Children)
                    Write(writer, child, options);

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

    }
    private static bool TryLocateProperty(Utf8JsonReader reader, string propertyName, out Utf8JsonReader result)
    {
        result = default;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var myDepth = reader.CurrentDepth + 1;
        while (reader.Read())
        {
            if (reader.CurrentDepth > myDepth)
                reader.Read();

            if (reader.CurrentDepth < myDepth)
                return false;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var prop = reader.GetString();
            reader.Read();
            if (prop != propertyName)
                continue;

            result = reader;
            return true;
        }

        return false;
    }
}

using System;

namespace BeatSaberMarkupLanguage.Parser
{
    public abstract class BSMLValue
    {
        public abstract string MemberName { get; }

        public bool FromUIValue { get; internal set; }

        public abstract void SetValue(object value);

        public abstract object GetValue();

        public override string ToString()
        {
            return $"{nameof(BSMLValue)}[{nameof(MemberName)}={MemberName}, {nameof(FromUIValue)}={FromUIValue}]";
        }

        internal T GetValueAs<T>()
        {
            object value = GetValue();

            if (value is not T)
            {
                throw new InvalidCastException($"Expected value '{MemberName}' to be of type '{typeof(T)}'");
            }

            return (T)value;
        }
    }
}

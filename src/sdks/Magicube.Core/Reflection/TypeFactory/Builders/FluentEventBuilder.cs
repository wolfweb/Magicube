using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Builders {
    internal class FluentEventBuilder : IEventBuilder {
        private readonly string _eventName;

        private readonly Type _eventType;

        private readonly Func<string, EventAttributes, Type, EventBuilder> _define;

        public FluentEventBuilder(
            string eventName,
            Type eventType,
            Func<string, EventAttributes, Type, EventBuilder> define) {
            _eventName = eventName;
            _eventType = eventType;
            _define = define;
            EventAttributes = EventAttributes.None;
        }

        public EventAttributes EventAttributes { get; private set; }

        public IEventBuilder SpecialName() {
            EventAttributes |= EventAttributes.SpecialName;
            return this;
        }

        public IEventBuilder RTSpecialName() {
            EventAttributes |= EventAttributes.RTSpecialName;
            return this;
        }

        public EventBuilder Define() {
            return _define(
                _eventName,
                EventAttributes,
                _eventType);
        }
    }
}
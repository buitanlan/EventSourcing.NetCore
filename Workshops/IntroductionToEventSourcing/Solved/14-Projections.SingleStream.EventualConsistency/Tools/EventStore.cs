namespace IntroductionToEventSourcing.GettingStateFromEvents.Tools;

public class EventStore
{
    private readonly Dictionary<Type, List<Action<EventEnvelope>>> handlers = new();

    private readonly Random random = new();

    public void Register<TEvent>(Action<EventEnvelope<TEvent>> handler) where TEvent : notnull
    {
        var eventType = typeof(TEvent);

        void WrappedHandler(object @event) => handler((EventEnvelope<TEvent>)@event);

        if (handlers.ContainsKey(eventType))
            handlers[eventType].Add(WrappedHandler);
        else
            handlers.Add(eventType, new List<Action<EventEnvelope>> { WrappedHandler });
    }

    public void Append(EventEnvelope eventEnvelope)
    {
        if (!handlers.TryGetValue(eventEnvelope.Data.GetType(), out var eventHandlers)) return;

        foreach (var handle in eventHandlers)
        {
            var numberOfRepeatedPublish = random.Next(1, 5);

            do
            {
                handle(eventEnvelope);
            } while (--numberOfRepeatedPublish > 0);
        }
    }

    public void Append(params EventEnvelope[] eventEnvelopes)
    {
        foreach (var @event in eventEnvelopes)
        {
            Append(@event);
        }
    }
}
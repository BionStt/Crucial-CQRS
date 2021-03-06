﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crucial.Framework.DesignPatterns.CQRS.Events;
using Crucial.Framework.DesignPatterns.CQRS.Messaging;
using Crucial.Framework.DesignPatterns.CQRS.Utils;
using System.Threading.Tasks;

namespace Crucial.Framework.DesignPatterns.CQRS.Messaging
{
    public class EventBus:IEventBus
    {
        private IEventHandlerFactory _eventHandlerFactory;

        public EventBus(IEventHandlerFactory eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
        }
        
        public async Task Publish<T>(T @event) where T : Event
        {
            var handlers = _eventHandlerFactory.GetHandlers<T>();

            var eventHandlers = new List<Task>();

            foreach (var eventHandler in handlers)
            {
                eventHandlers.Add(eventHandler.Handle(@event));
            }

            await Task.WhenAll(eventHandlers);
        }

        public async Task Replay(IEnumerable<Event> eventList)
        {
            foreach (dynamic ev in eventList)
            {
                var handlers = _eventHandlerFactory.GetHandlers(ev);

                List<Task> tasks = new List<Task>();

                foreach (var eventHandler in handlers)
                {
                    tasks.Add(eventHandler.Handle(ev));
                }

                await Task.WhenAll(tasks);
            }
        }
    }
}

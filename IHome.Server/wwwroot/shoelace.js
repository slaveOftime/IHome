Blazor.registerCustomEventType('sl-change', {
    browserEventName: 'sl-change',
    createEventArgs: event => {
        return {
            value: event.target.value
        };
    }
});
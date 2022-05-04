mergeInto(LibraryManager.library, {
    OnServerObjectChange: function (str) {
        var json = UTF8ToString(str);
        window.reduxStore.dispatch({ type: 'SET_SO_STATE', json: json })
    },
});
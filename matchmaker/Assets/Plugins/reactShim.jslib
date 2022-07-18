mergeInto(LibraryManager.library, {
    OnServerObjectChange: function (str) {
        var json = UTF8ToString(str);
        window.gpfReact.onSOSync(json)
    },
    onQuerySuccess: function (jsonStr, queryIdStr) {
        var json = UTF8ToString(jsonStr);
        var queryId = UTF8ToString(queryIdStr);
        window.gpfReact.onQuerySuccess(json, queryId)
    },
    onQueryFailure: function (reasonStr, queryIdStr) {
        var reason = UTF8ToString(reasonStr);
        var queryId = UTF8ToString(queryIdStr);
        window.gpfReact.onQueryFailure(reason, queryId)
    },
});
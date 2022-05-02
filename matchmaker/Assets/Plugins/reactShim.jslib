mergeInto(LibraryManager.library, {
    OnServerObjectChange: function (str) {
        var json = UTF8ToString(str);
        window.alert(json);
    },
});
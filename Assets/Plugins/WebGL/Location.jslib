// FILE: Assets/Plugins/WebGL/Location.jslib
var Location = {
    href: function () {
        return allocate(intArrayFromString(location.href), 'i8', ALLOC_NORMAL);
    },
}
mergeInto(LibraryManager.library, Location);
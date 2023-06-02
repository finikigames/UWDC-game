mergeInto(LibraryManager.library, {
  registerVisibilityChangeEvent: function () {
    document.addEventListener("visibilitychange", function () {
      SendMessage("MyObject", "OnVisibilityChange", document.visibilityState);
    });
    if (document.visibilityState != "visible")
      SendMessage("MyObject", "OnVisibilityChange", document.visibilityState);
  },
});
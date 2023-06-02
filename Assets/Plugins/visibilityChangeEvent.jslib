mergeInto(LibraryManager.library, {
  registerVisibilityChangeEvent: function () {
    document.addEventListener("visibilitychange", function () {
      SendMessage("ProjectContext", "OnVisibilityChange", document.visibilityState);
    });
    if (document.visibilityState != "visible")
      SendMessage("ProjectContext", "OnVisibilityChange", document.visibilityState);
  },
});
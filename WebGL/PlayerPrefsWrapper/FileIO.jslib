var FileIO = {

  LocalStorageIsActive : function() {
    if(navigator.cookieEnabled&& typeof window.localStorage !== "undefined"){
        return 1;
    }
    else{
        return 0;
    }
  },

  SaveToLocalStorage : function(key, data) {
    if(navigator.cookieEnabled&& typeof window.localStorage !== "undefined"){
        window.localStorage.setItem(UTF8ToString(key), UTF8ToString(data));
    }
  },

  LoadFromLocalStorage : function(key) {
    var returnStr;
    returnStr = window.localStorage.getItem(UTF8ToString(key));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  RemoveFromLocalStorage : function(key) {
    if(navigator.cookieEnabled&& typeof window.localStorage !== "undefined"){
        window.localStorage.removeItem(UTF8ToString(key));
    }
  },
  ClearLocalStorage : function() {
    if(navigator.cookieEnabled&& typeof window.localStorage !== "undefined"){
        window.localStorage.clear();
    }
  },

  HasKeyInLocalStorage : function(key) {
    if(navigator.cookieEnabled&& typeof window.localStorage !== "undefined"){
        if (window.localStorage.getItem(UTF8ToString(key))) {
            return 1;
        }
        else {
            return 0;
        }
    }
    else{
        return 0;
    }
  }
};

mergeInto(LibraryManager.library, FileIO);;

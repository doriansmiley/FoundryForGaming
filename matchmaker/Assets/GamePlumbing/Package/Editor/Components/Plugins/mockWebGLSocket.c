    int WebSocketConnect(int instanceId){
    }
    int WebSocketClose(int instanceId, int code, const char * reason){
    }
    int WebSocketSend(int instanceId, const char * dataPtr, int dataLength){
    }
    int WebSocketSendText(int instanceId, const char * message){
    }
    int WebSocketGetState(int instanceId){
    }
    int WebSocketAllocate(const char * url){
    }
    void WebSocketFree(int instanceId){
    }
    void WebSocketSetOnOpen(void (*callback)(int)){
    }
    void WebSocketSetOnMessage(void (*callback)(int,void*,int)){
    }
    void WebSocketSetOnError(void (*callback)(int,void*)){
    }
    void WebSocketSetOnClose(void (*callback)(int, int)){
    }
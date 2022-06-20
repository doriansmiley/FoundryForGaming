mod utils;

use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use web_sys::{ErrorEvent, MessageEvent, WebSocket};
use js_sys::*;
use std::cell::RefCell;

// When the `wee_alloc` feature is enabled, use `wee_alloc` as the global
// allocator.
#[cfg(feature = "wee_alloc")]
#[global_allocator]
static ALLOC: wee_alloc::WeeAlloc = wee_alloc::WeeAlloc::INIT;
thread_local!(static GLOBAL_WS: RefCell<WebSocket> = RefCell::new(WebSocket::new("wss://echo.websocket.events").unwrap()));


#[wasm_bindgen]
extern {
    fn alert(s: &str);
}

#[wasm_bindgen]
pub fn greet() {
    alert("Hello, hello-world!");
}

macro_rules! console_log {
    ($($t:tt)*) => (log(&format_args!($($t)*).to_string()))
}

#[wasm_bindgen]
extern "C" {
    #[wasm_bindgen(js_namespace = console)]
    fn log(s: &str);
}

#[wasm_bindgen]
pub fn send_message(val: &str) {
    GLOBAL_WS.with(|ws| {
       let value = JSON::parse(val).unwrap();
        console_log!("send_message unwrapped: {:?}", value);
        let abuf = val.as_bytes();
        let cloned_ws = ws.borrow().clone();
        match cloned_ws.send_with_u8_array(abuf) {
            Ok(_) => console_log!("message successfully sent: {:?}", abuf),
            Err(err) => console_log!("error sending message: {:?}", err),
        }
    });
}

#[wasm_bindgen]
pub fn subscribe(callback: js_sys::Function) {
    GLOBAL_WS.with(|ws| {
        let WS = ws.borrow();
        let this = JsValue::null();
        // create callback
    let cloned_ws = WS.clone();
    let onmessage_callback = Closure::wrap(Box::new(move |e: MessageEvent| {
        // Handle difference Text/Binary,...
        if let Ok(abuf) = e.data().dyn_into::<js_sys::ArrayBuffer>() {
            console_log!("message event, received arraybuffer: {:?}", abuf);
            let array = js_sys::Uint8Array::new(&abuf);
            let len = array.byte_length() as usize;
            console_log!("Arraybuffer received {}bytes: {:?}", len, array.to_vec());
            let _ = callback.call1(&this, &array);
            //let _ = callback.call1(&this, &abuf);
        } else if let Ok(blob) = e.data().dyn_into::<web_sys::Blob>() {
            console_log!("message event, received blob: {:?}", blob);
            // better alternative to juggling with FileReader is to use https://crates.io/crates/gloo-file
            let fr = web_sys::FileReader::new().unwrap();
            let fr_c = fr.clone();
            // create onLoadEnd callback
            let onloadend_cb = Closure::wrap(Box::new(move |_e: web_sys::ProgressEvent| {
                let array = js_sys::Uint8Array::new(&fr_c.result().unwrap());
                let len = array.byte_length() as usize;
                console_log!("Blob received {}bytes: {:?}", len, array.to_vec());
                // here you can for example use the received image/png data
            })
                as Box<dyn FnMut(web_sys::ProgressEvent)>);
            fr.set_onloadend(Some(onloadend_cb.as_ref().unchecked_ref()));
            fr.read_as_array_buffer(&blob).expect("blob not readable");
            onloadend_cb.forget();
        } else if let Ok(txt) = e.data().dyn_into::<js_sys::JsString>() {
            console_log!("message event, received Text: {:?}", txt);
            let _ = callback.call1(&this, &txt);
        } else {
            console_log!("message event, received Unknown: {:?}", e.data());
        }
    }) as Box<dyn FnMut(MessageEvent)>);
    // set message event handler on WebSocket
    WS.set_onmessage(Some(onmessage_callback.as_ref().unchecked_ref()));
    // forget the callback to keep it alive
    onmessage_callback.forget();
    });
}

#[wasm_bindgen]
pub fn get_status() -> u16 {
    let mut t = 0;
    GLOBAL_WS.with(|ws| {
        t = ws.borrow().ready_state();
    });
    return t;
}

#[wasm_bindgen]
pub fn start_websocket(callback: js_sys::Function) -> Result<(), JsValue> {
    GLOBAL_WS.with(|ws| {
    let WS = ws.borrow();
    let this = JsValue::null();

    // For small binary messages, like CBOR, Arraybuffer is more efficient than Blob handling
    WS.set_binary_type(web_sys::BinaryType::Arraybuffer);

    let onerror_callback = Closure::wrap(Box::new(move |e: ErrorEvent| {
        console_log!("error event: {:?}", e);
    }) as Box<dyn FnMut(ErrorEvent)>);
    WS.set_onerror(Some(onerror_callback.as_ref().unchecked_ref()));
    onerror_callback.forget();

    let onopen_callback = Closure::wrap(Box::new(move |_| {
        console_log!("socket opened");
        let _ = callback.call1(&this, &js_sys::JsString::from("socket opened"));
    }) as Box<dyn FnMut(JsValue)>);
    WS.set_onopen(Some(onopen_callback.as_ref().unchecked_ref()));
    onopen_callback.forget();
    });

    Ok(())
}

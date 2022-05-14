import * as types from "./appActionTypes"

const initialState = {
  sos: {},
  tested: false,
}

// Use the initialState as a default value
export default function appReducer(state = initialState, action) {
  // The reducer normally looks at the action type field to decide what happens
  switch (action.type) {
    // Do something here based on the different types of actions
    case types.SET_SO_STATE: {
      let so = action.so
      let newSos = { ...state.sos }
      newSos[so.ID.id] = so
      return { ...state, sos: newSos }
    }
    case types.TEST: {
      return { ...state, tested: true }
    }
    default:
      // If this reducer doesn't recognize the action type, or doesn't
      // care about this specific action, return the existing state unchanged
      return state
  }
}

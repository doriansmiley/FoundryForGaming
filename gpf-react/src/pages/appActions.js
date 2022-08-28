import * as types from "./appActionTypes"

export const SetSO = json => {
  return {
    type: types.SET_SO_STATE,
    payload: json,
  }
}

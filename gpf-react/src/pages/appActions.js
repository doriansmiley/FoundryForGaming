import * as types from "./appActionTypes"

export const SetSO = so => {
  return {
    type: types.SET_SO_STATE,
    payload: so,
  }
}
